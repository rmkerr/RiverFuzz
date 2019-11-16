using Database;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebView.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;

        public ResultsController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
        }

        [HttpGet]
        [Route("Results/API/Requests/ById/{id}")]
        public async Task<ActionResult<RequestEntity>> GetExecutedRequestById(int id)
        {
            return await _endpointRepository.GetExecutedRequestById(id);
        }

        [HttpGet]
        [Route("Results/API/Requests/BySequence/{id}")]
        public async Task<ActionResult<List<RequestEntity>>> GetExecutedRequestsBySequence(int id)
        {
            return await _endpointRepository.GetExecutedRequestsBySequence(id);
        }

        [HttpGet]
        [Route("Results/API/Requests/All")]
        public async Task<ActionResult<List<RequestEntity>>> GetAllRequests()
        {
            return await _endpointRepository.GetAllExecutedRequests();
        }

        [HttpGet]
        [Route("Results/API/Substitutions/ById/{id}")]
        public async Task<ActionResult<SubstitutionEntity>> GetSubstitutionsById(int id)
        {
            return await _endpointRepository.GetSubstitutionById(id);
        }

        [HttpGet]
        [Route("Results/API/Substitutions/BySequence/{id}")]
        public async Task<ActionResult<List<SubstitutionEntity>>> GetSubstitutionsBySequence(int id)
        {
            return await _endpointRepository.GetSubstitutionsBySequence(id);
        }

        [HttpGet]
        [Route("Results/API/Substitutions/All")]
        public async Task<ActionResult<List<SubstitutionEntity>>> GetAllSubstitutions()
        {
            return await _endpointRepository.GetAllSubstitutions();
        }

        [HttpGet]
        [Route("Results/API/Sequences/All")]
        public async Task<ActionResult<List<RequestSequenceEntity>>> GetAllRequestSequences()
        {
            return await _endpointRepository.GetAllRequestSequences();
        }

        [HttpGet]
        [Route("Results/API/Sequences/ById/{id}")]
        public async Task<ActionResult<RequestSequenceEntity>> GetRequestSequenceById(int id)
        {
            return await _endpointRepository.GetRequestSequenceById(id);
        }

        [HttpGet]
        [Route("Results/Sequence/{id}")]
        public async Task<IActionResult> Sequence(int id)
        {
            RequestSequenceEntity sequence = await _endpointRepository.GetRequestSequenceById(id);
            List<RequestEntity> requests = await _endpointRepository.GetExecutedRequestsBySequence(id);
            List<ResponseEntity> responses = await _endpointRepository.GetResponsesBySequence(id);
            List<SubstitutionEntity> substitutions = await _endpointRepository.GetSubstitutionsBySequence(id);

            // Split substitutions.
            List<List<SubstitutionEntity>> substitutionsGrouped = new List<List<SubstitutionEntity>>();
            for (int i = 0; i < sequence.request_count; ++i)
            {
                substitutionsGrouped.Add(substitutions.Where(x => x.sequence_position == i).ToList());
            }

            ViewData["Requests"] = requests;
            ViewData["Substitutions"] = substitutionsGrouped;
            ViewData["Responses"] = responses;
            ViewData["Sequence"] = sequence;
            return View();
        }

        [HttpGet]
        [Route("Results/Summary")]
        public async Task<IActionResult> Summary()
        {
            List<RequestSequenceEntity> sequences = await _endpointRepository.GetAllRequestSequences();

            ViewData["Sequences"] = sequences;
            return View();
        }
    }
}
