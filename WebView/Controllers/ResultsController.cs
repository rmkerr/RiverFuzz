using Database;
using Database.Entities;
using Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebView.Models;
using WebView.Models.Results;

namespace WebView.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ILogger<EndpointsController> _logger;
        private readonly IFuzzerRepository _endpointRepository;
        private readonly IConfiguration _configuration;

        public ResultsController(ILogger<EndpointsController> logger, IFuzzerRepository endpointRepo, IConfiguration configuration)
        {
            _logger = logger;
            _endpointRepository = endpointRepo;
            _configuration = configuration;
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

        public async Task<IActionResult> AddRequestSequenceLabel(RequestSequenceLabelEntity entity) // TODO: Remove db entity, change to binding model.
        {
            await _endpointRepository.AddRequestSequenceLabel(entity);
            return RedirectToAction("Sequence",  new { id = entity.sequence_id });
        }

        public async Task<IActionResult> DeleteRequestSequenceLabel(RequestSequenceLabelEntity entity) // TODO: Remove db entity, change to binding model.
        {
            await _endpointRepository.DeleteRequestSequenceLabel(entity.id.Value);
            return RedirectToAction("Sequence", new { id = entity.sequence_id });
        }

        [HttpGet]
        [Route("Results/Sequence/{id}")]
        public async Task<IActionResult> Sequence(int id)
        {
            RequestSequenceViewModel requestSequence = await GetFullSequence(id);

            return View(requestSequence);
        }

        [HttpGet]
        public async Task<IActionResult> Summary(int? id)
        {
            List<SequenceSummaryEntity> sequences;
            if (id == null)
            {
                sequences = await _endpointRepository.GetAllSequenceSummaries();
            }
            else
            {
                sequences = await _endpointRepository.GetSequenceSummariesByRunId(id.Value);
            }

            List<SequenceSummaryViewModel> sequenceViewModels = new List<SequenceSummaryViewModel>();
            foreach (SequenceSummaryEntity entity in sequences)
            {
                SequenceSummaryViewModel model = new SequenceSummaryViewModel(entity);
                List<RequestSequenceLabelEntity> labels = await _endpointRepository.GetRequestSequenceLabelsBySequence(entity.sequence_id.Value);

                foreach (RequestSequenceLabelEntity label in labels)
                {
                    model.Labels.Add(new RequestSequenceLabelViewModel(label));
                }
                sequenceViewModels.Add(model);
            }

            return View(sequenceViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Statistics()
        {
            return View(await GetRunSummaries());
        }

        [HttpGet]
        [Route("Results/API/Statistics")]
        public async Task<List<FuzzerRunViewModel>> APIStatistics()
        {
            return await GetRunSummaries();
        }

        private async Task<List<FuzzerRunViewModel>> GetRunSummaries()
        {
            List<FuzzerRunViewModel> runModels = new List<FuzzerRunViewModel>();
            List<FuzzerRunEntity> runEntities = await _endpointRepository.GetAllFuzzerRuns();

            foreach (FuzzerRunEntity runEntity in runEntities)
            {
                List<FuzzerGenerationEntity> generationEntities = await _endpointRepository.GetFuzzerGenerationByRun(runEntity.id.Value);

                FuzzerRunViewModel runModel = new FuzzerRunViewModel(runEntity);
                foreach (FuzzerGenerationEntity generationEntity in generationEntities)
                {
                    runModel.Generations.Add(new FuzzerGenerationViewModel(generationEntity));
                }

                runModels.Add(runModel);
            }

            return runModels;
        }

        private async Task<RequestSequenceViewModel> GetFullSequence(int id)
        {
            RequestSequenceEntity sequence = await _endpointRepository.GetRequestSequenceById(id);
            List<RequestEntity> requests = await _endpointRepository.GetExecutedRequestsBySequence(id);
            List<ResponseEntity> responses = await _endpointRepository.GetResponsesBySequence(id);
            List<SubstitutionEntity> substitutions = await _endpointRepository.GetSubstitutionsBySequence(id);
            List<RequestSequenceLabelEntity> labels = await _endpointRepository.GetRequestSequenceLabelsBySequence(id);

            requests.Sort(delegate (RequestEntity left, RequestEntity right)
            {
                return left.sequence_position - right.sequence_position ?? 0;
            });

            responses.Sort(delegate (ResponseEntity left, ResponseEntity right)
            {
                return left.sequence_position - right.sequence_position ?? 0;
            });

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
                sequenceViewModel.Labels.Add(new RequestSequenceLabelViewModel(label));
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

            if (_configuration.GetValue<bool>("ShowDebugMetadata", false))
            {
                List<SequenceMetadataEntity> metadataEntities = await _endpointRepository.GetSequenceMetadata(id);
                foreach (SequenceMetadataEntity entity in metadataEntities)
                {
                    sequenceViewModel.Metadata.Add(new SequenceMetadataViewModel(entity));
                }
                sequenceViewModel.ShowMetadata = true;
            }

            return sequenceViewModel;
        }
    }
}
