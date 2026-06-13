using GLMS.Api.DTOs;
using GLMS.Api.Enums;
using GLMS.Api.Factories;
using GLMS.Api.Models;
using GLMS.Api.Observers;
using GLMS.Api.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly ContractFactoryResolver _factoryResolver;
        private readonly ContractSubject _contractSubject;

        public ContractsController(
            IContractRepository contractRepository,
            ContractFactoryResolver factoryResolver,
            ContractSubject contractSubject)
        {
            _contractRepository = contractRepository;
            _factoryResolver = factoryResolver;
            _contractSubject = contractSubject;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Contract>> GetAll(
            [FromQuery] ContractStatus? status,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var contracts = _contractRepository.GetAll().AsQueryable();

            if (status.HasValue)
                contracts = contracts.Where(c => c.Status == status.Value);

            if (startDate.HasValue)
                contracts = contracts.Where(c => c.StartDate >= startDate.Value);

            if (endDate.HasValue)
                contracts = contracts.Where(c => c.EndDate <= endDate.Value);

            return Ok(contracts.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Contract> GetById(int id)
        {
            var contract = _contractRepository.GetById(id);

            if (contract == null)
                return NotFound();

            return Ok(contract);
        }

        [HttpPost]
        public ActionResult<Contract> Create([FromBody] CreateContractRequest request)
        {
            try
            {
                var factory = _factoryResolver.Resolve(request.ServiceLevel);
                var contract = factory.CreateContract(request.ClientId, request.StartDate, request.EndDate);

                _contractRepository.Add(contract);
                _contractRepository.Save();

                return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var contract = _contractRepository.GetById(id);

            if (contract == null)
                return NotFound();

            contract.Status = request.Status;

            // Observer pattern — notify all observers of the status change
            _contractSubject.Notify(contract);

            _contractRepository.Update(contract);
            _contractRepository.Save();

            return Ok(contract);
        }
    }
}