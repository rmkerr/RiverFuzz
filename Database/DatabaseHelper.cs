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

namespace Database
{
    /**
     * Used by the command line interface.
     * */
    public class DatabaseHelper : DatabaseCore
    {
        private string DbName;
        private bool Production;

        public DatabaseHelper(string dbname, bool production)
        {
            DbName = dbname;
            Production = production;
        }

        internal IDbConnection GetConnection(string DbName)
        {
            if (Production)
            {
                return new NpgsqlConnection($"Server=riverfuzz-postgres.postgres.database.azure.com;Database={DbName};Port=5432;User Id=postgres@riverfuzz-postgres;Password='3r!T8*Qb8YNFlG8Eb8u';Ssl Mode=Require;");
            }
            else
            {
                return new NpgsqlConnection($"Server=localhost;Database={DbName};Port=5432;User Id=postgres;Password='3r!T8*Qb8YNFlG8Eb8u';");
            }
        }

        internal override IDbConnection GetConnection()
        {
            return GetConnection(DbName);
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
                        content TEXT,
                        friendly_name TEXT
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
