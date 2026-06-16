using GLMS.Web.Enums;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IClientApiService _clientApiService;
        private readonly IContractApiService _contractApiService;
        private readonly IServiceRequestApiService _serviceRequestApiService;

        public HomeController(
            IClientApiService clientApiService,
            IContractApiService contractApiService,
            IServiceRequestApiService serviceRequestApiService)
        {
            _clientApiService = clientApiService;
            _contractApiService = contractApiService;
            _serviceRequestApiService = serviceRequestApiService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var clients = await _clientApiService.GetAllAsync();
            var contracts = (await _contractApiService.GetAllAsync()).ToList();
            var requests = (await _serviceRequestApiService.GetAllAsync()).ToList();

            ViewBag.TotalClients = clients.Count();
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