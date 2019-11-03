using Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IEndpointRepository
    {
        Task<RequestEntity> GetByID(int id);
        Task<List<RequestEntity>> GetAll();

    }
}
