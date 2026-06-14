using GLMS.Api.Enums;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Contracts;
using GLMS.Api.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository,
            ICurrencyService currencyService)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
            _currencyService = currencyService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ServiceRequest>> GetAll()
        {
            var requests = _serviceRequestRepository.GetAll();
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public ActionResult<ServiceRequest> GetById(int id)
        {
            var request = _serviceRequestRepository.GetById(id);

            if (request == null)
                return NotFound();

            return Ok(request);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> Create([FromBody] ServiceRequest serviceRequest)
        {
            // Workflow validation - block creation on Expired or OnHold contracts
            var contract = _contractRepository.GetById(serviceRequest.ContractId);

            if (contract == null)
                return BadRequest(new { error = "Selected contract does not exist." });

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                return BadRequest(new { error = $"Service requests cannot be created for a contract with status: {contract.Status}." });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Currency conversion - USD to ZAR via external API
            serviceRequest.CostZAR = await _currencyService.ConvertUsdToZarAsync(serviceRequest.CostUSD);

            _serviceRequestRepository.Add(serviceRequest);
            _serviceRequestRepository.Save();

            return CreatedAtAction(nameof(GetById), new { id = serviceRequest.Id }, serviceRequest);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
                return BadRequest("ID in route does not match ID in body.");

            var existing = _serviceRequestRepository.GetById(id);
            if (existing == null)
                return NotFound();

            // Block edits if the contract is Expired or OnHold
            var contract = _contractRepository.GetById(serviceRequest.ContractId);

            if (contract == null)
                return BadRequest(new { error = "Associated contract not found." });

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                return BadRequest(new { error = $"This service request cannot be edited because the contract is {contract.Status}." });
            }

            // Recalculate ZAR if USD cost changed
            serviceRequest.CostZAR = await _currencyService.ConvertUsdToZarAsync(serviceRequest.CostUSD);

            _serviceRequestRepository.Update(serviceRequest);
            _serviceRequestRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _serviceRequestRepository.GetById(id);
            if (existing == null)
                return NotFound();

            _serviceRequestRepository.Delete(id);
            _serviceRequestRepository.Save();

            return NoContent();
        }
    }
}