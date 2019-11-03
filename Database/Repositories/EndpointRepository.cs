using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Database.Entities;
using Microsoft.Extensions.Configuration;

namespace Database.Repositories
{
    public class EndpointRepository : IEndpointRepository
    {
        private readonly IConfiguration config_;

        public EndpointRepository(IConfiguration config)
        {
            config_ = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SQLiteConnection(config_.GetConnectionString("MyConnectionString"));
            }
        }

        public async Task<List<RequestEntity>> GetAll()
        {
            using (IDbConnection connection = Connection)
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestEntity>(@"SELECT * FROM endpoints");
                return result.ToList();
            }
        }

        public async Task<RequestEntity> GetByID(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM endpoints WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }
    }
}
