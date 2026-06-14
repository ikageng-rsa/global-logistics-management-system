using GLMS.Api.Models;
using GLMS.Api.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Client>> GetAll()
        {
            var clients = _clientRepository.GetAll();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public ActionResult<Client> GetById(int id)
        {
            var client = _clientRepository.GetById(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public ActionResult<Client> Create([FromBody] Client client)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _clientRepository.Add(client);
            _clientRepository.Save();

            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Client client)
        {
            if (id != client.Id)
                return BadRequest("ID in route does not match ID in body.");

            var existing = _clientRepository.GetById(id);
            if (existing == null)
                return NotFound();

            _clientRepository.Update(client);
            _clientRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _clientRepository.GetById(id);
            if (existing == null)
                return NotFound();

            _clientRepository.Delete(id);
            _clientRepository.Save();

            return NoContent();
        }
    }
}