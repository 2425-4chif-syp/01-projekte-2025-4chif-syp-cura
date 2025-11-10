namespace WebApi.DTOs
{
    public class DailyStatusDto
    {
        public DateTime Date { get; set; }
        public int Scheduled { get; set; }
        public int Taken { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
