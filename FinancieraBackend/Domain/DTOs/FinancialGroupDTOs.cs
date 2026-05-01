using System.ComponentModel.DataAnnotations;

namespace FinancieraBackend.Domain.DTOs
{
    public class CreateFinancialGroupDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int CreatedBy { get; set; }
    }

    public class UpdateFinancialGroupDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
