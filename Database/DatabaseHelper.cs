using Dapper;
using Database.Models;
using DatabaseModels.Models;
using System;
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
                    ( status, headers, content ) VALUES 
                    ( @status, @headers, @content );
                    select last_insert_rowid()", response).First();
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
                        content TEXT
                    );");
            }
        }

        private string DbFile;
    }
}
