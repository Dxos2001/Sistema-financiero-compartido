using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.Models;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Infrastructure;

namespace FinancieraBackend.Application.Services
{
    public class ConsensusRequestService : IConsensusRequestService
    {
        private readonly DBConnection _context;

        public ConsensusRequestService(DBConnection context)
        {
            _context = context;
        }

        public Task<ConsensusRequests> CreateRequestAsync(CreateConsensusRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<ConsensusRequests>> GetPendingRequestsForApproverAsync(int approverId)
        {
            throw new NotImplementedException();
        }

        public Task<ConsensusRequests> GetRequestAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ConsensusRequests>> GetRequestsByTransactionAsync(int transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateRequestStatusAsync(int id, UpdateConsensusRequestStatusDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
