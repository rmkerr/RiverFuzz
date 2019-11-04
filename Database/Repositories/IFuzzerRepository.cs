using Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IFuzzerRepository
    {
        // Endpoints
        Task<RequestEntity> GetEndpointById(int id);
        Task<List<RequestEntity>> GetAllEndpoints();

        // Executed Requests
        Task<RequestEntity> GetExecutedRequestById(int id);
        Task<RequestEntity> GetExecutedRequestBySequence(int id);
        Task<List<RequestEntity>> GetAllExecutedRequest();
    }
}
