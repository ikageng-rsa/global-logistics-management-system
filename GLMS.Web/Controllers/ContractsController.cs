using GLMS.Web.Enums;
using GLMS.Web.Models;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS.Web.Controllers
{
    [Authorize]

    public class ContractsController : Controller
    {
        private readonly IContractApiService _contractApiService;
        private readonly IClientApiService _clientApiService;

        public ContractsController(
            IContractApiService contractApiService,
            IClientApiService clientApiService)
        {
            _contractApiService = contractApiService;
            _clientApiService = clientApiService;
        }

        [HttpGet("contracts")]
        public async Task<IActionResult> Index()
        {
            var contracts = await _contractApiService.GetAllAsync();
            return View(contracts);
        }

        [HttpGet("contracts/details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractApiService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpGet("contracts/create")]
        public async Task<IActionResult> Create()
        {
            await PopulateClientDropdown();
            return View();
        }

        [HttpPost("contracts/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clientId, DateTime startDate, DateTime endDate, ServiceLevel serviceLevel)
        {
            var (success, error, _) = await _contractApiService.CreateAsync(clientId, startDate, endDate, serviceLevel);

            if (success)
            {
                TempData["Success"] = "Contract created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, error ?? "Failed to create contract.");
            await PopulateClientDropdown();
            return View();
        }

        [HttpGet("contracts/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var contract = await _contractApiService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost("contracts/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.Id) return BadRequest();

            var success = await _contractApiService.UpdateStatusAsync(id, contract.Status);

            if (success)
            {
                TempData["Success"] = "Contract updated.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "Failed to update contract.");
            return View(contract);
        }

        [HttpGet("contracts/delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractApiService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost("contracts/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _contractApiService.DeleteAsync(id);
            TempData["Success"] = "Contract deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("contracts/upload-agreement/{id}")]
        public async Task<IActionResult> UploadAgreement(int id)
        {
            var contract = await _contractApiService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost("contracts/upload-agreement/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile agreementFile)
        {
            if (agreementFile == null || agreementFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a PDF file to upload.");
                var contract = await _contractApiService.GetByIdAsync(id);
                return View(contract);
            }

            var (success, _) = await _contractApiService.UploadAgreementAsync(id, agreementFile);

            if (success)
            {
                TempData["Success"] = "Agreement uploaded successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ModelState.AddModelError(string.Empty, "Invalid file. Only PDF files under 10MB are accepted.");
            var failedContract = await _contractApiService.GetByIdAsync(id);
            return View(failedContract);
        }

        private async Task PopulateClientDropdown()
        {
            var clients = await _clientApiService.GetAllAsync();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
        }
    }
}