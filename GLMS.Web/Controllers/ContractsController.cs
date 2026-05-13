using GLMS.Web.Enums;
using GLMS.Web.Factories;
using GLMS.Web.Models;
using GLMS.Web.Observers;
using GLMS.Web.Repositories.Contracts;
using GLMS.Web.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Web.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractRepository _contractRepository;
        private readonly IClientRepository _clientRepository;
        private readonly ContractFactoryResolver _factoryResolver;
        private readonly ContractSubject _contractSubject;
        private readonly IFileService _fileService;

        public ContractsController(
            IContractRepository contractRepository,
            IClientRepository clientRepository,
            ContractFactoryResolver factoryResolver,
            ContractSubject contractSubject,
            IFileService fileService)
        {
            _contractRepository = contractRepository;
            _clientRepository = clientRepository;
            _factoryResolver = factoryResolver;
            _contractSubject = contractSubject;
            _fileService = fileService;
        }

        [HttpGet("contracts")]
        public IActionResult Index()
        {
            var contracts = _contractRepository.GetAll();

            ViewBag.Clients = new SelectList(
                _contractRepository.GetAll(),
                "Id",
                "Name"
            );
            return View(contracts);
        }

        [HttpGet("contracts/details/{id}")]
        public IActionResult Details(int id)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpGet("contracts/create")]
        public IActionResult Create()
        {
            PopulateClientDropdown();
            return View();
        }

        [HttpPost("contracts/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(int clientId, DateTime startDate, DateTime endDate, ServiceLevel serviceLevel)
        {
            try
            {
                // Factory Method pattern — correct factory selected by service level
                var factory = _factoryResolver.Resolve(serviceLevel);
                var contract = factory.CreateContract(clientId, startDate, endDate);

                _contractRepository.Add(contract);
                _contractRepository.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                PopulateClientDropdown();
                return View();
            }
        }

        [HttpGet("contracts/edit/{id}")]
        public IActionResult Edit(int id)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null) return NotFound();
            PopulateClientDropdown();
            return View(contract);
        }

        [HttpPost("contracts/edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contract contract)
        {
            if (id != contract.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                // Observer pattern — notify all observers of status change
                _contractSubject.Notify(contract);

                _contractRepository.Update(contract);
                _contractRepository.Save();

                return RedirectToAction(nameof(Index));
            }

            PopulateClientDropdown();
            return View(contract);
        }

        [HttpGet("contracts/delete/{id}")]
        public IActionResult Delete(int id)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        [HttpPost("contracts/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _contractRepository.Delete(id);
            _contractRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Contracts/UploadAgreement/5
        public IActionResult UploadAgreement(int id)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null) return NotFound();
            return View(contract);
        }

        // POST: /Contracts/UploadAgreement/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile agreementFile)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null) return NotFound();

            if (agreementFile == null || agreementFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a PDF file to upload.");
                return View(contract);
            }

            if (!_fileService.IsValidPdf(agreementFile))
            {
                ModelState.AddModelError(string.Empty,
                    "Invalid file. Only PDF files under 10MB are accepted.");
                return View(contract);
            }

            // Delete old agreement if one exists
            if (!string.IsNullOrEmpty(contract.AgreementFilePath))
                _fileService.DeleteAgreement(contract.AgreementFilePath);

            // Save new file and store the path
            contract.AgreementFilePath = await _fileService.SaveAgreementAsync(agreementFile);

            _contractRepository.Update(contract);
            _contractRepository.Save();

            TempData["Success"] = "Agreement uploaded successfully.";
            return RedirectToAction(nameof(Details), new { id = contract.Id });
        }

        private void PopulateClientDropdown()
        {
            var clients = _clientRepository.GetAll();
            ViewBag.Clients = new SelectList(clients, "Id", "Name");
        }
    }
}