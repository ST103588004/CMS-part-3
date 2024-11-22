using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace CMS.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClaimsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Claims
        public async Task<IActionResult> Index()
        {
            return View(await _context.Claim.ToListAsync());
        }

        // GET: Claims/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim
                .FirstOrDefaultAsync(m => m.ClaimId == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }

        // GET: Claims/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Claims/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClaimId,LecturerName,HoursWorked,HourlyRate,AdditionalNotes,SubmittedDate,SupportingDocumentPath,Status")] Claim claim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(claim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // GET: Claims/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim.FindAsync(id);
            if (claim == null)
            {
                return NotFound();
            }
            return View(claim);
        }

        // POST: Claims/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClaimId,LecturerName,HoursWorked,HourlyRate,AdditionalNotes,SubmittedDate,SupportingDocumentPath,Status")] Claim claim)
        {
            if (id != claim.ClaimId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(claim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClaimExists(claim.ClaimId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(claim);
        }

        // GET: Claims/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var claim = await _context.Claim
                .FirstOrDefaultAsync(m => m.ClaimId == id);
            if (claim == null)
            {
                return NotFound();
            }

            return View(claim);
        }
     
        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim model, IFormFile supportingDocument)
        {
            if (!ModelState.IsValid)
            {
                var claim = new Claim
                {
                    LecturerName = model.LecturerName, // Capturing the lecturer's name
                    HoursWorked = model.HoursWorked,
                    HourlyRate = model.HourlyRate,
                    AdditionalNotes = model.AdditionalNotes,
                    SubmittedDate = DateTime.Now,
                    Status = "Pending"
                };

                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(supportingDocument.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("SupportingDocument", "File type not allowed.");
                        return View(model);
                    }

                    if (supportingDocument.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("SupportingDocument", "File size exceeds 5 MB.");
                        return View(model);
                    }

                    var path = Path.Combine("wwwroot/uploads", supportingDocument.FileName);
                    

                    claim.SupportingDocumentPath = path;
                }

                _context.Claim.Add(claim);
                _context.SaveChanges();

                return RedirectToAction("ClaimsCoordinator");
            }

            return View(model);
        }
        [Authorize(Roles ="Coordinator")]
        public async Task<IActionResult> ClaimsCoordinator()
        {
            // Get only pending claims for the Programme Coordinator
            var pendingClaims = await _context.Claim
                .Where(c => c.Status == "Rejected" || c.Status == "Pending")
                .ToListAsync();

            return View(pendingClaims);
        }

       
        public async Task<IActionResult> ClaimsManager()
        {
            // Get only approved claims for the Academic Manager
            var approvedClaims = await _context.Claim
                .Where(c => c.Status == "Approved")
                .ToListAsync();

            return View(approvedClaims);
        }
        [Authorize]
        public async Task<IActionResult> TrackClaims()
        {
            // Get only approved claims for the Academic Manager
            var approvedClaims = await _context.Claim
                .ToListAsync();

            return View(approvedClaims);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            // Find the claim
            var claim = await _context.Claim.FindAsync(claimId);
            if (claim != null)
            {
                // Update the status to "Approved"
                claim.Status = "Approved";
                _context.Update(claim);
                await _context.SaveChangesAsync();
            }


            return RedirectToAction("TrackClaims");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            // Find the claim
            var claim = await _context.Claim.FindAsync(claimId);
            if (claim != null)
            {
                // Optionally, update the status to "Rejected" or keep it in "Pending"
                claim.Status = "Rejected";
                _context.Update(claim);
                await _context.SaveChangesAsync();
            }

            // Redirect back to the ClaimsCoordinator view after rejecting the claim
            return RedirectToAction("ClaimsCoordinator");
        }




        // POST: Claims/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var claim = await _context.Claim.FindAsync(id);
            if (claim != null)
            {
                _context.Claim.Remove(claim);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClaimExists(int id)
        {
            return _context.Claim.Any(e => e.ClaimId == id);
        }
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> LecturerSummary()
        {
            var lecturerClaims = await _context.Claim
                .GroupBy(c => c.LecturerName)
                .Select(group => new LecturerSummaryViewModel
                {
                    LecturerName = group.Key,
                    TotalOwed = group.Sum(c => c.HoursWorked * c.HourlyRate),
                    HoursWorked = group.Sum(c => c.HoursWorked),
                    HourlyRate = group.Average(c => c.HourlyRate),
                    AdditionalNotes = string.Join(", ", group.Select(c => c.AdditionalNotes)), // Combine notes
                    SubmittedDate = group.Max(c => c.SubmittedDate), // Get the latest submitted date
                })
                .ToListAsync();

            return View(lecturerClaims);
        }
        // 

        public async Task<IActionResult> DownloadLecturerReport(string lecturerName)
        {
            // Get lecturer claims from the database
            var lecturerClaims = await _context.Claim
                .Where(c => c.LecturerName == lecturerName)
                .ToListAsync();

            if (lecturerClaims == null || !lecturerClaims.Any())
            {
                return NotFound();
            }

            // Create CSV content
            var csvContent = new StringBuilder();
            csvContent.AppendLine("Lecturer Name,Hours Worked,Hourly Rate,Total Owed,Additional Notes,Submitted Date,Status");

            foreach (var claim in lecturerClaims)
            {
                csvContent.AppendLine($"{claim.LecturerName},{claim.HoursWorked},{claim.HourlyRate},{claim.HoursWorked * claim.HourlyRate},\"{claim.AdditionalNotes}\",{claim.SubmittedDate.ToShortDateString()},{claim.Status}");
            }

            // Return the CSV file as a download
            var fileName = $"{lecturerName}_ClaimsReport_{DateTime.Now:yyyyMMdd}.csv";
            return File(Encoding.UTF8.GetBytes(csvContent.ToString()), "text/csv", fileName);
        }


    }


}
