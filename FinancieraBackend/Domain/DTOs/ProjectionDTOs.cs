namespace FinancieraBackend.Domain.DTOs
{
    public class ProjectionResponseDTO
    {
        public int GroupId { get; set; }
        public string MarkdownReport { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
