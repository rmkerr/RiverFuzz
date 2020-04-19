using Database.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IFuzzerRepository
    {
        // Endpoints
        Task<KnownEndpointEntity> GetEndpointById(int id);
        Task<List<KnownEndpointEntity>> GetAllEndpoints();
        void AddEndpoint(KnownEndpointEntity endpoint);

        // Executed Requests
        Task<RequestEntity> GetExecutedRequestById(int id);
        Task<List<RequestEntity>> GetExecutedRequestsBySequence(int id);
        Task<List<RequestEntity>> GetAllExecutedRequests();

        // Responses
        Task<ResponseEntity> GetResponseById(int id);
        Task<List<ResponseEntity>> GetResponsesBySequence(int id);
        Task<List<ResponseEntity>> GetAllResponses();

        // Request Sequences
        Task<RequestSequenceEntity> GetRequestSequenceById(int id);
        Task<List<RequestSequenceEntity>> GetRequestSequencesByRunId(int id);
        Task<List<RequestSequenceEntity>> GetAllRequestSequences();

        // Request Sequence Metadata
        Task<List<SequenceMetadataEntity>> GetSequenceMetadata(int id);

        // Request Sequence Summaries
        Task<List<SequenceSummaryEntity>> GetAllSequenceSummaries();
        Task<List<SequenceSummaryEntity>> GetSequenceSummariesByRunId(int id);

        // Substitutions
        Task<SubstitutionEntity> GetSubstitutionById(int id);
        Task<List<SubstitutionEntity>> GetAllSubstitutions();
        Task<List<SubstitutionEntity>> GetSubstitutionsBySequence(int id);

        // Sequence Labels
        Task<RequestSequenceLabelEntity> GetRequestSequenceLabelById(int id);
        Task<List<RequestSequenceLabelEntity>> GetAllRequestSequenceLabels();
        Task<List<RequestSequenceLabelEntity>> GetRequestSequenceLabelsBySequence(int id);
        Task AddRequestSequenceLabel(RequestSequenceLabelEntity label);
        Task DeleteRequestSequenceLabel(int id);

        // Fuzzer Runs
        Task<List<FuzzerRunEntity>> GetAllFuzzerRuns();
        Task<List<FuzzerGenerationEntity>> GetFuzzerGenerationByRun(int id);

        // Dictionaries
        void AddDictionary(string name, List<string> contents);
    }
}
