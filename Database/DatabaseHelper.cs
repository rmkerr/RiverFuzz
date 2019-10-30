using Dapper;
using Database.Models;
using DatabaseModels.Models;
using HttpTokenize;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace Database
{
    public class DatabaseHelper
    {
        public DatabaseHelper(string dbFile)
        {
            DbFile = dbFile;
        }   

        public void AddEndpoint(RequestModel endpoint)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO known_endpoints 
                    ( url, method, headers, content ) VALUES 
                    ( @url, @method, @headers, @content );
                    select last_insert_rowid()", endpoint).First();
            }
        }

        public void AddExecutedRequest(RequestModel endpoint)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO executed_requests 
                    ( url, method, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @url, @method, @headers, @content, @sequence_id, @sequence_position );
                    select last_insert_rowid()", endpoint).First();
            }
        }

        public void AddResponse(ResponseModel response)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            using (var connection = GetConnection())
            {
                connection.Open();
                response.id = connection.Query<int>(
                    @"INSERT INTO response_test
                    ( status, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @status, @headers, @content, @sequence_id, @sequence_position );
                    select last_insert_rowid()", response).First();
            }
        }

        public void AddRequestSequence(RequestSequence sequence)
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            RequestSequenceModel model = new RequestSequenceModel();
            model.request_count = sequence.StageCount();
            model.substitution_count = sequence.SubstitutionCount();

            using (var connection = GetConnection())
            {
                connection.Open();

                model.id = connection.Query<int>(
                    @"INSERT INTO executed_sequences
                    ( request_count, substitution_count ) VALUES 
                    ( @request_count, @substitution_count );
                    select last_insert_rowid()", model).First();
            }

            List<Response>? results = sequence.GetResponses();
            if (results != null && results.Count == sequence.StageCount())
            {
                for (int i = 0; i < sequence.StageCount(); ++i)
                {
                    Request request = sequence.Get(i).Request;
                    RequestModel requestModel = RequestModel.FromRequest(request);
                    requestModel.sequence_id = model.id;
                    requestModel.sequence_position = i;
                    AddExecutedRequest(requestModel);

                    Response response = results[i];
                    ResponseModel responseModel = ResponseModel.FromResponse(response);
                    requestModel.sequence_id = model.id;
                    requestModel.sequence_position = i;
                    AddResponse(responseModel);
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
                    @"CREATE TABLE executed_sequences (
                        id                  INTEGER     PRIMARY KEY AUTOINCREMENT,
                        request_count       INTEGER     NOT NULL,
                        substitution_count  INTEGER     NOT_NULL
                    );");

                // All executed requests.
                connection.Execute(
                    @"CREATE TABLE executed_requests (
                        id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        url     TEXT    NOT NULL,
                        method  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT,
                        sequence_position INTEGER,
                        sequence_id INTEGER,
                        FOREIGN KEY(sequence_id) REFERENCES executed_sequences(id)
                    );");

                // Known endpoints.
                connection.Execute(
                    @"CREATE TABLE known_endpoints (
                        id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        url     TEXT    NOT NULL,
                        method  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT
                    );");

                // Response test table.
                connection.Execute(
                    @"CREATE TABLE response_test (
                        id      INTEGER PRIMARY KEY AUTOINCREMENT,
                        status  TEXT    NOT NULL,
                        headers TEXT,
                        content TEXT,
                        sequence_position INTEGER,
                        sequence_id INTEGER,
                        FOREIGN KEY(sequence_id) REFERENCES executed_sequences(id)
                    );");
            }
        }

        private string DbFile;
    }
}
