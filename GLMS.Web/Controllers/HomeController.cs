using GLMS.Web.Enums;
using GLMS.Web.Models;
using GLMS.Web.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public HomeController(
            IClientRepository clientRepository,
            IContractRepository contractRepository,
            IServiceRequestRepository serviceRequestRepository)
        {
            _clientRepository = clientRepository;
            _contractRepository = contractRepository;
            _serviceRequestRepository = serviceRequestRepository;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var contracts = _contractRepository.GetAll().ToList();
            var requests = _serviceRequestRepository.GetAll().ToList();

            ViewBag.TotalClients = _clientRepository.GetAll().Count();
            ViewBag.TotalContracts = contracts.Count;
            ViewBag.ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active);
            ViewBag.ExpiredContracts = contracts.Count(c => c.Status == ContractStatus.Expired);
            ViewBag.OnHoldContracts = contracts.Count(c => c.Status == ContractStatus.OnHold);
            ViewBag.TotalRequests = requests.Count;
            ViewBag.PendingRequests = requests.Count(r => r.Status == RequestStatus.Pending);
            ViewBag.TotalValueUSD = requests.Sum(r => r.CostUSD);
            ViewBag.TotalValueZAR = requests.Sum(r => r.CostZAR);

            return View();
        }
    }
}