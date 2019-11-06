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
    public class FuzzerRepository : IFuzzerRepository
    {
        private readonly IConfiguration config_;

        public FuzzerRepository(IConfiguration config)
        {
            config_ = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SQLiteConnection(config_.GetConnectionString("DefaultConnection"));
            }
        }

        public async Task<List<RequestEntity>> GetAllEndpoints()
        {
            using (IDbConnection connection = Connection)
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestEntity>(@"SELECT * FROM endpoints");
                return result.ToList();
            }
        }

        public async Task<List<RequestEntity>> GetAllExecutedRequest()
        {
            using (IDbConnection connection = Connection)
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestEntity>(@"SELECT * FROM requests");
                return result.ToList();
            }
        }

        public async Task<List<RequestSequenceEntity>> GetAllRequestSequences()
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM sequences";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceEntity>(sQuery);
                return result.ToList();
            }
        }

        public async Task<RequestSequenceEntity> GetRequestSequenceById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM sequences WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<RequestEntity> GetEndpointById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM endpoints WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<RequestEntity> GetExecutedRequestById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM requests WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<RequestEntity> GetExecutedRequestBySequence(int seqId)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM requests WHERE sequenceid = @seqId";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { seqId = seqId });
                return result.FirstOrDefault();
            }
        }
    }
}
