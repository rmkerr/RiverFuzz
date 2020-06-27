using Dapper;
using Dapper.Contrib.Extensions;
using Database.Entities;
using HttpTokenize;
using HttpTokenize.Substitutions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class DatabaseCore
    {
        internal virtual IDbConnection GetConnection()
        {
            throw new NotImplementedException("Derived DatabaseCore classed must implement GetConnection");
        }

        public async Task<List<KnownEndpointEntity>> GetAllEndpoints()
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<KnownEndpointEntity>(@"SELECT * FROM endpoints ORDER BY id ASC");
                return result.ToList();
            }
        }

        public async Task<List<RequestEntity>> GetAllExecutedRequests()
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestEntity>(@"SELECT * FROM requests ORDER BY id ASC");
                return result.ToList();
            }
        }

        public async Task<List<RequestSequenceEntity>> GetAllRequestSequences()
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM sequences ORDER BY id ASC";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceEntity>(sQuery);
                return result.ToList();
            }
        }

        public async Task<RequestSequenceEntity> GetRequestSequenceById(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = @"select  *,
                                (SELECT MIN(id) FROM public.sequences sub WHERE sub.run_id = main.run_id AND sub.id > main.id) as next_id,
                                (SELECT MAX(id) FROM public.sequences sub WHERE sub.run_id = main.run_id AND sub.id<main.id) as prev_id
                                from public.sequences as main
                                WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<RequestSequenceEntity>> GetRequestSequencesByRunId(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = @"select  *,
                                (SELECT MIN(id) FROM public.sequences sub WHERE sub.run_id = main.run_id AND sub.id > main.id) as next_id,
                                (SELECT MAX(id) FROM public.sequences sub WHERE sub.run_id = main.run_id AND sub.id<main.id) as prev_id
                                from public.sequences as main
                                WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceEntity>(sQuery, new { ID = id });
                return result.ToList();
            }
        }

        public async Task<KnownEndpointEntity> GetEndpointById(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM endpoints WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<KnownEndpointEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<RequestEntity> GetExecutedRequestById(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM requests WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<RequestEntity>> GetExecutedRequestsBySequence(int seqId)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM requests WHERE sequence_id = @seqId ORDER BY id ASC";
                conn.Open();
                var result = await conn.QueryAsync<RequestEntity>(sQuery, new { seqId = seqId });
                return result.ToList();
            }
        }

        public async Task<SubstitutionEntity> GetSubstitutionById(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM substitutions WHERE id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<SubstitutionEntity>(sQuery, new { ID = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<SubstitutionEntity>> GetAllSubstitutions()
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM substitutions ORDER BY id ASC";
                conn.Open();
                var result = await conn.QueryAsync<SubstitutionEntity>(sQuery);
                return result.ToList();
            }
        }

        public async Task<List<SubstitutionEntity>> GetSubstitutionsBySequence(int seqId)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM substitutions WHERE sequence_id = @seqId ORDER BY id ASC";
                conn.Open();
                var result = await conn.QueryAsync<SubstitutionEntity>(sQuery, new { seqId = seqId });
                return result.ToList();
            }
        }

        public async Task<ResponseEntity> GetResponseById(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM responses WHERE id = @id";
                conn.Open();
                var result = await conn.QueryAsync<ResponseEntity>(sQuery, new { id = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<ResponseEntity>> GetResponsesBySequence(int seqId)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM responses WHERE sequence_id = @seqId ORDER BY id ASC";
                conn.Open();
                var result = await conn.QueryAsync<ResponseEntity>(sQuery, new { seqId = seqId });
                return result.ToList();
            }
        }

        public async Task<List<ResponseEntity>> GetAllResponses()
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<ResponseEntity>(@"SELECT * FROM responses ORDER BY id ASC");
                return result.ToList();
            }
        }

        public async Task<RequestSequenceLabelEntity> GetRequestSequenceLabelById(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM sequence_labels WHERE id = @id";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceLabelEntity>(sQuery, new { id = id });
                return result.FirstOrDefault();
            }
        }

        public async Task<List<RequestSequenceLabelEntity>> GetAllRequestSequenceLabels()
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<RequestSequenceLabelEntity>(@"SELECT * FROM sequence_labels ORDER BY id ASC");
                return result.ToList();
            }
        }

        public async Task<List<RequestSequenceLabelEntity>> GetRequestSequenceLabelsBySequence(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM sequence_labels WHERE sequence_id = @seqId ORDER BY id ASC";
                conn.Open();
                var result = await conn.QueryAsync<RequestSequenceLabelEntity>(sQuery, new { seqId = id });
                return result.ToList();
            }
        }

        public async Task<List<SequenceSummaryEntity>> GetAllSequenceSummaries()
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = @"SELECT last_req.sequence_id as sequence_id,
                                        last_req.url as url,
                                        last_req.method as method,
                                        responses.status as status
                                FROM
                                    (select a.*
                                        FROM requests a
                                        LEFT OUTER JOIN requests b ON 
                                        a.sequence_id = b.sequence_id
                                        AND a.sequence_position < b.sequence_position
                                        where b.id IS NULL) last_req
                                JOIN responses ON (responses.sequence_id = last_req.sequence_id
                                                    and responses.sequence_position = last_req.sequence_position)
                                ORDER BY sequence_id ASC;";
                conn.Open();
                var result = await conn.QueryAsync<SequenceSummaryEntity>(sQuery);
                return result.ToList();
            }
        }
        public async Task<List<SequenceSummaryEntity>> GetSequenceSummariesByRunId(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = @"SELECT last_req.sequence_id as sequence_id,
                                       last_req.url as url,
                                       last_req.method as method,
                                       responses.status as status
                                FROM
                                    (select a.*
                                     FROM requests a
                                     LEFT OUTER JOIN requests b ON 
                                     a.sequence_id = b.sequence_id
                                     AND a.sequence_position < b.sequence_position
                                     where b.id IS NULL) last_req
                                JOIN responses ON (responses.sequence_id = last_req.sequence_id
                                                   and responses.sequence_position = last_req.sequence_position)
                                JOIN sequences ON sequences.id = responses.sequence_id
                                WHERE sequences.run_id = @id
                                ORDER BY sequence_id ASC";
                conn.Open();
                var result = await conn.QueryAsync<SequenceSummaryEntity>(sQuery, new { id = id });
                return result.ToList();
            }
        }

        public async Task AddRequestSequenceLabel(RequestSequenceLabelEntity label)
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                label.id = (await conn.QueryAsync<int>(
                    @"INSERT INTO sequence_labels
                    ( sequence_id, name ) VALUES 
                    ( @sequence_id, @name )
                    RETURNING id;", label)).First();
            }
        }

        public async Task DeleteRequestSequenceLabel(int id)
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Open();
                await conn.ExecuteAsync(@"DELETE FROM sequence_labels WHERE id = @labelId", new { labelId = id });
            }
        }

        public async Task<List<FuzzerRunEntity>> GetAllFuzzerRuns()
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<FuzzerRunEntity>(@"SELECT * FROM fuzzer_run ORDER BY id ASC");
                return result.ToList();
            }
        }

        public async Task<List<FuzzerGenerationEntity>> GetFuzzerGenerationByRun(int id)
        {
            using (IDbConnection connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<FuzzerGenerationEntity>(@"SELECT * FROM fuzzer_generation WHERE run_id = @runId ORDER BY id ASC", new { runId = id });
                return result.ToList();
            }
        }

        public void AddEndpoint(KnownEndpointEntity endpoint)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                endpoint.id = connection.Query<int>(
                    @"INSERT INTO endpoints 
                    ( url, method, headers, content, friendly_name ) VALUES 
                    ( @url, @method, @headers, @content, @friendly_name )
                    RETURNING id;", endpoint).First();
            }
        }

        public async Task<List<KnownEndpointEntity>> AllEndpoints()
        {
            //

            using (var connection = GetConnection())
            {
                connection.Open();
                var result = await connection.QueryAsync<KnownEndpointEntity>(@"SELECT * FROM endpoints ORDER BY id ASC");
                return result.ToList();
            }
        }

        public void AddExecutedRequest(RequestEntity request)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                request.id = connection.Query<int>(
                    @"INSERT INTO requests 
                    ( url, method, headers, content, sequence_id, sequence_position ) VALUES 
                    ( @url, @method, @headers, @content, @sequence_id, @sequence_position )
                    RETURNING id;", request).First();
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

        public void AddFuzzerRun(FuzzerRunEntity model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                model.id = connection.Query<int>(
                    @"INSERT INTO fuzzer_run
                    ( start_time, end_time, name ) VALUES 
                    ( @start_time, @end_time, @name )
                    RETURNING id;", model).First();
            }
        }

        public void UpdateFuzzerRunEndTime(FuzzerRunEntity model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                connection.Execute(
                    @"UPDATE fuzzer_run
                      SET end_time = @end_time 
                      WHERE id = @id;", model);
            }
        }

        public void AddFuzzerGeneration(FuzzerGenerationEntity model)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                model.id = connection.Query<int>(
                    @"INSERT INTO fuzzer_generation
                    ( run_id, run_position, population_size, execution_time, executed_requests ) VALUES 
                    ( @run_id, @run_position, @population_size, @execution_time, @executed_requests )
                    RETURNING id;", model).First();
            }
        }

        public void AddDictionary(string name, List<string> contents)
        {
            using (var connection = GetConnection())
            {
                DictionaryNameEntity nameEntity = new DictionaryNameEntity();
                nameEntity.name = name;

                connection.Open();
                nameEntity.id = connection.Query<int>(
                    @"INSERT INTO dictionary_name
                    ( name ) VALUES 
                    ( @name )
                    RETURNING id;", nameEntity).First();

                foreach(string value in contents)
                {
                    DictionaryEntryEntity entry = new DictionaryEntryEntity();
                    entry.dictionary_id = nameEntity.id.Value;
                    entry.content = value;

                    connection.Execute(@"INSERT INTO dictionary_entry
                    ( content, dictionary_id ) VALUES
                    ( @content, @dictionary_id );", entry);
                }
            }
        }

        public async Task<List<string>> GetAllDictionaryEntries()
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT content FROM dictionary_entry";
                conn.Open();
                var result = await conn.QueryAsync<string>(sQuery);
                return result.ToList();
            }
        }

        public async Task<List<SequenceMetadataEntity>> GetSequenceMetadata(int sequence_id)
        {
            using (IDbConnection conn = GetConnection())
            {
                string sQuery = "SELECT * FROM sequence_metadata WHERE sequence_id = @ID";
                conn.Open();
                var result = await conn.QueryAsync<SequenceMetadataEntity>(sQuery, new { ID = sequence_id });
                return result.ToList();
            }
        }

        public async Task AddRequestSequence(RequestSequence sequence, FuzzerRunEntity run)
        {
            RequestSequenceEntity model = new RequestSequenceEntity();
            model.request_count = sequence.StageCount();
            model.substitution_count = sequence.SubstitutionCount();
            model.run_id = run.id.GetValueOrDefault(0);

            using (var connection = GetConnection())
            {
                connection.Open();

                model.id = connection.Query<int>(
                    @"INSERT INTO sequences
                    ( request_count, substitution_count, run_id ) VALUES 
                    ( @request_count, @substitution_count, @run_id )
                    RETURNING id;", model).First();

                foreach (SequenceMetadata meta in sequence.GetDebugMetadata())
                {
                    SequenceMetadataEntity metadata_entity =
                        new SequenceMetadataEntity { sequence_id = model.id, content = meta.Content, type = meta.Type };

                    connection.Execute(@"INSERT INTO sequence_metadata 
                                         ( sequence_id, type, content ) VALUES
                                         ( @sequence_id, @type, @content );", metadata_entity);
                }
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
                    await AddRequestSequenceLabel(labelEntity);
                }
            }
            else
            {
                Console.WriteLine("Warning: Truncated request sequence.");
            }
        }

        public bool DatabaseInitialized()
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                return connection.ExecuteScalar<bool>(@"SELECT EXISTS (
                   SELECT FROM information_schema.tables
                   WHERE  table_name = 'db_version'
                );");
            }
        }

        public void InitializeDatabase()
        {
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

                // Database version.
                connection.Execute(
                    @"CREATE TABLE db_version (
                        version             TEXT      PRIMARY KEY
                    );");

                connection.Execute(
                    @"INSERT INTO db_version VALUES (@Version);",
                        new { Version = GetType().Assembly.GetName().Version.ToString() });
            }
        }
    }
}
