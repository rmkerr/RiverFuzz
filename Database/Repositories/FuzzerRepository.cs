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
using Npgsql;

namespace Database.Repositories
{
    public class FuzzerRepository : DatabaseCore, IFuzzerRepository
    {
        private readonly IConfiguration config_;

        public FuzzerRepository(IConfiguration config)
        {
            config_ = config;
        }

        internal override IDbConnection GetConnection()
        {
            return new NpgsqlConnection(config_.GetConnectionString("DefaultConnection"));
        }
    }
}
