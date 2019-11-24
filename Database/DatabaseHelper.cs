using HttpTokenize;
using HttpTokenize.Substitutions;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

using Dapper;
using Database.Entities;
using System.Threading.Tasks;
using Npgsql;

namespace Database
{
    public class DatabaseHelper
    {
        public DatabaseHelper(string dbname)
        {
            DbName = dbname;
        }

        public void AddEndpoint(RequestEntity endpoint)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO endpoints 
                    ( url, method, headers, content ) VALUES 
                    ( @url, @method, @headers, @content )
                    RETURNING id;", endpoint).First();
            }
        }

        public async Task<List<RequestEntity>> AllEndpoints()
        {
          //

            using (var connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestEntity>(@"SELECT * FROM endpoints");
                return result.ToList();
            }
        }

        public void AddExecutedRequest(RequestEntity endpoint)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO requests 
                    ( url, method, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @url, @method, @headers, @content, @sequence_id, @sequence_position )
                    RETURNING id;", endpoint).First();
            }
        }

        public void AddResponse(ResponseEntity response)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                response.id = connection.Query<int>(
                    @"INSERT INTO responses
                    ( status, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @status, @headers, @content, @sequence_id, @sequence_position )
                    RETURNING id;", response).First();
            }
        }

        public void AddSubstitution(SubstitutionEntity model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                model.id = connection.Query<int>(
                    @"INSERT INTO substitutions
                    ( type, summary, sequence_id, sequence_position ) VALUES 
                    ( @type, @summary, @sequence_id, @sequence_position )
                    RETURNING id;", model).First();
            }
        }

        public void AddRequestSequenceLabel(RequestSequenceLabelEntity model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                model.id = connection.Query<int>(
                    @"INSERT INTO sequence_labels
                    ( sequence_id, name ) VALUES 
                    ( @sequence_id, @name )
                    RETURNING id;", model).First();
            }
        }

        public void AddRequestSequence(RequestSequence sequence)
        {
            RequestSequenceEntity model = new RequestSequenceEntity();
            model.request_count = sequence.StageCount();
            model.substitution_count = sequence.SubstitutionCount();

            using (var connection = GetConnection())
            {
                connection.Open();

                model.id = connection.Query<int>(
                    @"INSERT INTO sequences
                    ( request_count, substitution_count ) VALUES 
                    ( @request_count, @substitution_count )
                    RETURNING id;", model).First();
            }

            sequence.Id = model.id;

            List<Response>? results = sequence.GetResponses();
            if (results != null && results.Count == sequence.StageCount())
            {
                for (int i = 0; i < sequence.StageCount(); ++i)
                {
                    Request request = sequence.Get(i).Request;
                    RequestEntity requestModel = RequestEntity.FromRequest(request);
                    requestModel.sequence_id = model.id;
                    requestModel.sequence_position = i;
                    AddExecutedRequest(requestModel);
                    request.Id = requestModel.id;

                    Response response = results[i];
                    ResponseEntity responseModel = ResponseEntity.FromResponse(response);
                    responseModel.sequence_id = model.id;
                    responseModel.sequence_position = i;
                    AddResponse(responseModel);
                    response.Id = responseModel.id;

                    foreach (ISubstitution sub in sequence.Get(i).Substitutions)
                    {
                        SubstitutionEntity subModel = SubstitutionEntity.FromSubstitution(sub);
                        subModel.sequence_id = model.id;
                        subModel.sequence_position = i;
                        AddSubstitution(subModel);
                    }
                }

                if (sequence.GetLastResponse() != null)
                {
                    int statusCode = (int)sequence.GetLastResponse().Status;
                    RequestSequenceLabelEntity labelEntity = new RequestSequenceLabelEntity();
                    labelEntity.sequence_id = model.id.Value;
                    if (statusCode >= 100 && statusCode < 200)
                    {
                        labelEntity.name = "Informational";
                    }
                    else if (statusCode >= 200 && statusCode < 300)
                    {
                        labelEntity.name = "Success";
                    }
                    else if (statusCode >= 300 && statusCode < 400)
                    {
                        labelEntity.name = "Redirection";
                    }
                    else if (statusCode >= 400 && statusCode < 500)
                    {
                        labelEntity.name = "Client Error";
                    }
                    else if (statusCode >= 500 && statusCode < 600)
                    {
                        labelEntity.name = "Server Error";
                    }
                    else
                    {
                        labelEntity.name = "Unknown Status";
                    }
                    AddRequestSequenceLabel(labelEntity);
                }
            }
            else
            {
                Console.WriteLine("Warning: Truncated request sequence.");
            }
        }

        private NpgsqlConnection GetConnection(string DbName)
        {
            return new NpgsqlConnection($"Server=riverfuzz-testing.postgres.database.azure.com;Database={DbName};Port=5432;User Id=riverfuzz@riverfuzz-testing;Password='3r!T8*Qb8YNFlG8Eb8u';Ssl Mode=Require;");
        }

        private NpgsqlConnection GetConnection()
        {
            return GetConnection(DbName);
        }

        public void CreateDatabase()
        {

            using (var connection = GetConnection("postgres"))
            {
                //CREATE DATABASE riverfuzz;
                connection.Execute($"CREATE DATABASE {DbName};", null,null, 1000);
            }

            using (var connection = GetConnection())
            {
                connection.Open();

                // Executed sequences
                connection.Execute(
                    @"CREATE TABLE sequences (
                        id                  SERIAL      PRIMARY KEY,
                        request_count       INTEGER     NOT NULL,
                        substitution_count  INTEGER     NOT NULL
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
            }
        }

        public void DeleteDatabase()
        {
            using (var connection = GetConnection("postgres"))
            {
                connection.Open();

                // Executed sequences
                connection.Execute(
                    @"DROP DATABASE IF EXISTS riverfuzz;");
            }
        }

        private string DbName;
    }
}
