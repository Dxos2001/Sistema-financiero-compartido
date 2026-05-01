using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;

namespace FinancieraBackend.Domain.Interfaces
{
    public interface IConsensusRequestService
    {
        Task<ConsensusRequests> CreateRequestAsync(CreateConsensusRequestDTO dto);
        Task<ConsensusRequests> GetRequestAsync(int id);
        Task<List<ConsensusRequests>> GetRequestsByTransactionAsync(int transactionId);
        Task<List<ConsensusRequests>> GetPendingRequestsForApproverAsync(int approverId);
        Task<bool> UpdateRequestStatusAsync(int id, UpdateConsensusRequestStatusDTO dto);
    }
}
