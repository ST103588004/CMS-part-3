namespace CMS.Models
{
    public class LecturerSummaryViewModel
    {
        public string LecturerName { get; set; }
        public decimal TotalOwed { get; set; }
        public decimal HoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string AdditionalNotes { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}
