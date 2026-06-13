using Microsoft.AspNetCore.Mvc;
using GLMS.Api.Models;
using GLMS.Api.Enums;
using GLMS.Api.Repositories.Contracts;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;

        public ContractsController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
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
    }
}