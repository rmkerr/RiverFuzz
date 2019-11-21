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
using WebView.Models;

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
            RequestSequenceViewModel requestSequence = await GetFullSequence(id);

            return View(requestSequence);
        }

        [HttpGet]
        [Route("Results/Summary")]
        public async Task<IActionResult> Summary()
        {
            List<RequestSequenceEntity> sequences = await _endpointRepository.GetAllRequestSequences();

            // TODO: This makes many DB calls. Should just make one. Maybe add SummaryViewModel?
            List<RequestSequenceViewModel> sequenceViewModels = new List<RequestSequenceViewModel>();
            foreach (RequestSequenceEntity entity in sequences)
            {
                sequenceViewModels.Add(await GetFullSequence(entity.id.GetValueOrDefault()));
            }

            return View(sequenceViewModels);
        }

        private async Task<RequestSequenceViewModel> GetFullSequence(int id)
        {
            RequestSequenceEntity sequence = await _endpointRepository.GetRequestSequenceById(id);
            List<RequestEntity> requests = await _endpointRepository.GetExecutedRequestsBySequence(id);
            List<ResponseEntity> responses = await _endpointRepository.GetResponsesBySequence(id);
            List<SubstitutionEntity> substitutions = await _endpointRepository.GetSubstitutionsBySequence(id);
            List<RequestSequenceLabelEntity> labels = await _endpointRepository.GetRequestSequenceLabelsBySequence(id);

            RequestSequenceViewModel sequenceViewModel = new RequestSequenceViewModel(sequence);

            foreach (RequestEntity entity in requests)
            {
                var model = new RequestViewModel(entity);
                model.Sequence = sequenceViewModel;
                sequenceViewModel.Requests.Add(model);
            }

            foreach (ResponseEntity entity in responses)
            {
                var model = new ResponseViewModel(entity);
                model.Sequence = sequenceViewModel;
                sequenceViewModel.Responses.Add(model);
            }

            // Add labels
            foreach (RequestSequenceLabelEntity label in labels)
            {
                sequenceViewModel.Labels.Add(label.name);
            }

            // Split substitutions.
            List<List<SubstitutionViewModel>> substitutionsGrouped = new List<List<SubstitutionViewModel>>();
            for (int i = 0; i < sequence.request_count; ++i)
            {
                List<SubstitutionViewModel> stage = new List<SubstitutionViewModel>();
                foreach (SubstitutionEntity entity in substitutions)
                {
                    if (entity.sequence_position == i)
                    {
                        var model = new SubstitutionViewModel(entity);
                        model.Sequence = sequenceViewModel;
                        stage.Add(model);
                    }
                }
                sequenceViewModel.Substitutions.Add(stage);
            }

            return sequenceViewModel;
        }
    }
}
