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

namespace Database
{
    public class DatabaseHelper
    {
        public DatabaseHelper(string dbFile)
        {
            DbFile = dbFile;
        }

        public void AddEndpoint(RequestEntity endpoint)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO endpoints 
                    ( url, method, headers, content ) VALUES 
                    ( @url, @method, @headers, @content );
                    select last_insert_rowid()", endpoint).First();
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
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO requests 
                    ( url, method, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @url, @method, @headers, @content, @sequence_id, @sequence_position );
                    select last_insert_rowid()", endpoint).First();
            }
        }

        public void AddResponse(ResponseEntity response)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                response.id = connection.Query<int>(
                    @"INSERT INTO responses
                    ( status, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @status, @headers, @content, @sequence_id, @sequence_position );
                    select last_insert_rowid()", response).First();
            }
        }

        public void AddSubstitution(SubstitutionEntity model)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                model.id = connection.Query<int>(
                    @"INSERT INTO substitutions
                    ( type, summary, sequence_id, sequence_position ) VALUES 
                    ( @type, @summary, @sequence_id, @sequence_position );
                    select last_insert_rowid()", model).First();
            }
        }

        public void AddRequestSequenceLabel(RequestSequenceLabelEntity model)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                model.id = connection.Query<int>(
                    @"INSERT INTO sequence_labels
                    ( sequence_id, name ) VALUES 
                    ( @sequence_id, @name );
                    select last_insert_rowid()", model).First();
            }
        }

        public void AddRequestSequence(RequestSequence sequence)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            RequestSequenceEntity model = new RequestSequenceEntity();
            model.request_count = sequence.StageCount();
            model.substitution_count = sequence.SubstitutionCount();

            using (var connection = GetConnection())
            {
                connection.Open();

                model.id = connection.Query<int>(
                    @"INSERT INTO sequences
                    ( request_count, substitution_count ) VALUES 
                    ( @request_count, @substitution_count );
                    select last_insert_rowid()", model).First();
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

        private SQLiteConnection GetConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }

        private void CreateDatabase()
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                // Executed sequences
                connection.Execute(
                    @"CREATE TABLE sequences (
                        id                  INTEGER     PRIMARY KEY AUTOINCREMENT,
                        request_count       INTEGER     NOT NULL,
                        substitution_count  INTEGER     NOT_NULL
                    );");

                // All executed requests.
                connection.Execute(
                    @"CREATE TABLE requests (
                        id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        url     TEXT    NOT NULL,
                        method  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT,
                        sequence_position INTEGER,
                        sequence_id INTEGER,
                        FOREIGN KEY(sequence_id) REFERENCES executed_sequences(id)
                    );");

                // Substitutions.
                connection.Execute(
                    @"CREATE TABLE substitutions (
                        id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                        type                TEXT    NOT NULL,
                        summary             TEXT    NOT NULL,
                        sequence_position   INTEGER,
                        sequence_id         INTEGER,
                        FOREIGN KEY(sequence_id) REFERENCES executed_sequences(id)
                    );");

                // Known endpoints.
                connection.Execute(
                    @"CREATE TABLE endpoints (
                        id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        url     TEXT    NOT NULL,
                        method  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT
                    );");

                // Response test table.
                connection.Execute(
                    @"CREATE TABLE responses (
                        id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        status  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT,
                        sequence_position INTEGER,
                        sequence_id INTEGER,
                        FOREIGN KEY(sequence_id) REFERENCES executed_sequences(id)
                    );");

                // Sequence tag metadata
                connection.Execute(
                    @"CREATE TABLE sequence_labels (
                        id                  INTEGER     PRIMARY KEY AUTOINCREMENT,
                        sequence_id         INTEGER     NOT NULL,
                        name                TEXT        NOT_NULL
                    );");
            }
        }

        public void DeleteDatabase()
        {
            File.Delete(DbFile);
        }

        private string DbFile;
    }
}
