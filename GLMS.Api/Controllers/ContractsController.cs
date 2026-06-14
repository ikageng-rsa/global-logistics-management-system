using GLMS.Api.DTOs;
using GLMS.Api.Enums;
using GLMS.Api.Factories;
using GLMS.Api.Models;
using GLMS.Api.Observers;
using GLMS.Api.Repositories.Contracts;
using GLMS.Api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly ContractFactoryResolver _factoryResolver;
        private readonly ContractSubject _contractSubject;
        private readonly IFileService _fileService;

        public ContractsController(
            IContractRepository contractRepository,
            ContractFactoryResolver factoryResolver,
            ContractSubject contractSubject,
            IFileService fileService)
        {
            _contractRepository = contractRepository;
            _factoryResolver = factoryResolver;
            _contractSubject = contractSubject;
            _fileService = fileService;
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
        [Authorize(Roles = "Admin")]
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var contract = _contractRepository.GetById(id);

            if (contract == null)
                return NotFound();

            _contractRepository.Delete(id);
            _contractRepository.Save();

            return NoContent();
        }

        [HttpPost("{id}/agreement")]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile agreementFile)
        {
            var contract = _contractRepository.GetById(id);
            if (contract == null)
                return NotFound();

            if (agreementFile == null || agreementFile.Length == 0)
                return BadRequest(new { error = "Please select a PDF file to upload." });

            if (!_fileService.IsValidPdf(agreementFile))
                return BadRequest(new { error = "Invalid file. Only PDF files under 10MB are accepted." });

            // Delete old agreement if one exists
            if (!string.IsNullOrEmpty(contract.AgreementFilePath))
                _fileService.DeleteAgreement(contract.AgreementFilePath);

            contract.AgreementFilePath = await _fileService.SaveAgreementAsync(agreementFile);

            _contractRepository.Update(contract);
            _contractRepository.Save();

            return Ok(new { filePath = contract.AgreementFilePath });
        }
    }
}