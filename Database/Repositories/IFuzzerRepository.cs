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
        Task<RequestEntity> GetEndpointById(int id);
        Task<List<RequestEntity>> GetAllEndpoints();

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
        Task<List<RequestSequenceEntity>> GetAllRequestSequences();

        // Substitutions
        Task<SubstitutionEntity> GetSubstitutionById(int id);
        Task<List<SubstitutionEntity>> GetAllSubstitutions();
        Task<List<SubstitutionEntity>> GetSubstitutionsBySequence(int id);

        // Sequence Labels
        Task<RequestSequenceLabelEntity> GetRequestSequenceLabelById(int id);
        Task<List<RequestSequenceLabelEntity>> GetAllRequestSequenceLabels();
        Task<List<RequestSequenceLabelEntity>> GetRequestSequenceLabelsBySequence(int id);
        Task AddRequestSequenceLabel(RequestSequenceLabelEntity label);
    }
}
