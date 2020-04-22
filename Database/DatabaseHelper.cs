using HttpTokenize;
using HttpTokenize.Substitutions;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Database.Entities;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Database
{
    /**
     * Used by the command line interface.
     * */
    public class DatabaseHelper : DatabaseCore, IDatabaseHelper
    {
        private string DbName;
        private bool IsProduction;
        private string ConnectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetConnectionString("DefaultConnection");
            var csBuilder = new NpgsqlConnectionStringBuilder(ConnectionString);
            if(csBuilder.Database == null)
            {
                throw new ArgumentNullException("No name in database field of connection string");
            }
            this.DbName = csBuilder.Database;
        }

        //public DatabaseHelper(string dbname, bool production)
        //{
        //    DbName = dbname;
        //    IsProduction = production;
        //}

        //TODO: we should look at putting these in a config class or something
        internal IDbConnection GetConnection(string DbName)
        {
            //return IsProduction
            //    ? new NpgsqlConnection($"Server=riverfuzz-postgres.postgres.database.azure.com;Database={DbName};Port=5432;User Id=postgres@riverfuzz-postgres;Password='3r!T8*Qb8YNFlG8Eb8u';Ssl Mode=Require;")
            //    : new NpgsqlConnection($"Server=db;Database={DbName};Port=5432;User Id=postgres;Password='3r!T8*Qb8YNFlG8Eb8u';");
            var builder = new NpgsqlConnectionStringBuilder(this.ConnectionString);
            builder.Database = DbName;

            return new NpgsqlConnection(builder.ConnectionString);
        }

        internal override IDbConnection GetConnection()
        {
            return GetConnection(DbName);
        }
        
        public void CreateIfNotExists()
        {
            bool exists = false;
            int tableCount = 0;

            using(var connection = GetConnection("postgres"))
            {
                string dbExistsQuery = $@"SELECT exists(
                                    SELECT datname FROM pg_database
                                    WHERE datname = '{this.DbName}')";

                string tableCountQuery = @"SELECT count(*)
                                    FROM
	                                    pg_catalog.pg_tables
                                    WHERE
	                                    schemaname != 'pg_catalog'
                                    AND schemaname != 'information_schema'";

                exists = connection.ExecuteScalar<bool>(dbExistsQuery);
                tableCount = connection.ExecuteScalar<int>(tableCountQuery);
            }

            if(!exists || tableCount == 0 )
            {
                this.DeleteDatabase();
                this.CreateDatabase();
            }
        }

        public void CreateDatabase()
        {

            using (var connection = GetConnection("postgres"))
            {
                //CREATE DATABASE riverfuzz;
                connection.Execute($"CREATE DATABASE {DbName};", null, null, 1000);
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                // Executed sequences
                connection.Execute(
                    @"CREATE TABLE sequences (
                        id                  SERIAL      PRIMARY KEY,
                        request_count       INTEGER     NOT NULL,
                        substitution_count  INTEGER     NOT NULL,
                        run_id              INTEGER     NOT NULL
                    );");

                // Sequence metadata
                connection.Execute(
                    @"CREATE TABLE sequence_metadata (
                        id                  SERIAL      PRIMARY KEY,
                        sequence_id         INTEGER     NOT NULL,
                        type                TEXT        NOT NULL,
                        content             TEXT        NOT NULL
                    );");

                // All executed requests.
                connection.Execute(
                    @"CREATE TABLE requests (
                        id      SERIAL  PRIMARY KEY,
                        url     TEXT    NOT NULL,
                        method  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT,
                        sequence_position INTEGER,
                        sequence_id INTEGER
                    );");

                // Substitutions.
                connection.Execute(
                    @"CREATE TABLE substitutions (
                        id                  SERIAL PRIMARY KEY,
                        type                TEXT    NOT NULL,
                        summary             TEXT    NOT NULL,
                        sequence_position   INTEGER,
                        sequence_id         INTEGER
                    );");

                // Known endpoints.
                connection.Execute(
                    @"CREATE TABLE endpoints (
                        id      SERIAL  PRIMARY KEY,
                        url     TEXT    NOT NULL,
                        method  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT
                    );");

                // Response test table.
                connection.Execute(
                    @"CREATE TABLE responses (
                        id      SERIAL  PRIMARY KEY,
                        status  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT,
                        sequence_position INTEGER,
                        sequence_id INTEGER
                    );");

                // Sequence tag metadata
                connection.Execute(
                    @"CREATE TABLE sequence_labels (
                        id                  SERIAL     PRIMARY KEY,
                        sequence_id         INTEGER     NOT NULL,
                        name                TEXT        NOT NULL
                    );");

                // Fuzzer run metadata.
                connection.Execute(
                    @"CREATE TABLE fuzzer_run (
                        id                  SERIAL      PRIMARY KEY,
                        name                TEXT        NOT NULL,
                        start_time          TIMESTAMP   NOT NULL,
                        end_time            TIMESTAMP   NOT NULL
                    );");

                // Fuzzer generation metadata.
                connection.Execute(
                    @"CREATE TABLE fuzzer_generation (
                        id                  SERIAL      PRIMARY KEY,
                        run_id              INTEGER     NOT NULL,
                        run_position        INTEGER     NOT NULL,
                        population_size     INTEGER     NOT NULL,
                        execution_time      TIME        NOT NULL,
                        executed_requests   INTEGER     NOT NULL
                    );");

                // Dictionary metadata.
                connection.Execute(
                    @"CREATE TABLE dictionary_name (
                        id                  SERIAL      PRIMARY KEY,
                        name                TEXT        NOT NULL
                    );");

                // Dictionary entries.
                connection.Execute(
                    @"CREATE TABLE dictionary_entry (
                        id                  SERIAL      PRIMARY KEY,
                        dictionary_id       INTEGER     NOT NULL,
                        content             TEXT        NOT NULL
                    );");
            }
        }

        public void DeleteDatabase()
        {
            using (var connection = GetConnection("postgres"))
            {
                connection.Open();

                // Executed sequences
                connection.Execute(
                    @"DROP DATABASE IF EXISTS riverfuzz;", null, null, 1000);
            }
        }
    }
}
