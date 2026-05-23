using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IProjectionService
    {
        Task<ProjectionResponseDTO> GenerateSixMonthProjectionAsync(int groupId);
    }
}
