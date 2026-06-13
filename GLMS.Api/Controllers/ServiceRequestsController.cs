using Microsoft.AspNetCore.Mvc;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Contracts;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;

        public ServiceRequestsController(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
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
    }
}