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

        public async Task<List<RequestEntity>> GetAllExecutedRequests()
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

        public async Task<List<RequestEntity>> GetExecutedRequestsBySequence(int seqId)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM requests WHERE sequence_id = @seqId";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { seqId = seqId });
                return result.ToList();
            }
        }

        public async Task<SubstitutionEntity> GetSubstitutionById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM substitutions WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<SubstitutionEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<SubstitutionEntity>> GetAllSubstitutions()
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM substitutions";
                conn.Open();
                var result = await conn.QueryAsync<SubstitutionEntity>(sQuery);
                return result.ToList();
            }
        }

        public async Task<List<SubstitutionEntity>> GetSubstitutionsBySequence(int seqId)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM substitutions WHERE sequence_id = @seqId";
                conn.Open();
                var result = await conn.QueryAsync<SubstitutionEntity>(sQuery, new { seqId = seqId });
                return result.ToList();
            }
        }

        public async Task<ResponseEntity> GetResponseById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM responses WHERE id = @id";
                conn.Open();
                var result = await conn.QueryAsync<ResponseEntity>(sQuery, new { id = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<ResponseEntity>> GetResponsesBySequence(int seqId)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM responses WHERE sequence_id = @seqId";
                conn.Open();
                var result = await conn.QueryAsync<ResponseEntity>(sQuery, new { seqId = seqId });
                return result.ToList();
            }
        }

        public async Task<List<ResponseEntity>> GetAllResponses()
        {
            using (IDbConnection connection = Connection)
            {
                connection.Open();
                var result = await connection.QueryAsync<ResponseEntity>(@"SELECT * FROM responses");
                return result.ToList();
            }
        }

        public async Task<RequestSequenceLabelEntity> GetRequestSequenceLabelById(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM sequence_labels WHERE id = @id";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceLabelEntity>(sQuery, new { id = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<RequestSequenceLabelEntity>> GetAllRequestSequenceLabels()
        {
            using (IDbConnection connection = Connection)
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestSequenceLabelEntity>(@"SELECT * FROM sequence_labels");
                return result.ToList();
            }
        }

        public async Task<List<RequestSequenceLabelEntity>> GetRequestSequenceLabelsBySequence(int id)
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = "SELECT * FROM sequence_labels WHERE sequence_id = @seqId";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceLabelEntity>(sQuery, new { seqId = id });
                return result.ToList();
            }
        }
    }
}
