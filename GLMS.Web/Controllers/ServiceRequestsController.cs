using GLMS.Web.Enums;
using GLMS.Web.Models;
using GLMS.Web.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    public class ServiceRequestsController : Controller
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

        [HttpGet("service-requests")]
        public IActionResult Index()
        {
            var requests = _serviceRequestRepository.GetAll();
            return View(requests);
        }

        [HttpGet("service-requests/details/{id}")]
        public IActionResult Details(int id)
        {
            var request = _serviceRequestRepository.GetById(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpGet("service-requests/create")]
        public IActionResult Create()
        {
            PopulateContractDropdown();
            return View();
        }

        [HttpPost("service-requests/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceRequest serviceRequest)
        {
            // Workflow validation — block creation on Expired or OnHold contracts
            var contract = _contractRepository.GetById(serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError(string.Empty, "Selected contract does not exist.");
                PopulateContractDropdown();
                return View(serviceRequest);
            }

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                ModelState.AddModelError(string.Empty,
                    $"Service requests cannot be created for a contract with status: {contract.Status}.");
                PopulateContractDropdown();
                return View(serviceRequest);
            }

            if (ModelState.IsValid)
            {
                _serviceRequestRepository.Add(serviceRequest);
                _serviceRequestRepository.Save();
                return RedirectToAction(nameof(Index));
            }

            PopulateContractDropdown();
            return View(serviceRequest);
        }

        [HttpGet("service-requests/edit/{id}")]
        public IActionResult Edit(int id)
        {
            var request = _serviceRequestRepository.GetById(id);
            if (request == null) return NotFound();
            PopulateContractDropdown();
            return View(request);
        }

        [HttpPost("service-requests/edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _serviceRequestRepository.Update(serviceRequest);
                _serviceRequestRepository.Save();
                return RedirectToAction(nameof(Index));
            }

            PopulateContractDropdown();
            return View(serviceRequest);
        }

        [HttpGet("service-requests/delete/{id}")]
        public IActionResult Delete(int id)
        {
            var request = _serviceRequestRepository.GetById(id);
            if (request == null) return NotFound();
            return View(request);
        }

        [HttpPost("service-requests/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _serviceRequestRepository.Delete(id);
            _serviceRequestRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateContractDropdown()
        {
            // Only show Active contracts in the dropdown
            var activeContracts = _contractRepository.GetAll()
                .Where(c => c.Status == ContractStatus.Active)
                .ToList();

            ViewBag.Contracts = new SelectList(activeContracts, "Id", "Id");
        }
    }
}