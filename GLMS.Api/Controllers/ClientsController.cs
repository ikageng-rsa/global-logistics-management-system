using Microsoft.AspNetCore.Mvc;
using GLMS.Api.Models;
using GLMS.Api.Repositories.Contracts;

namespace GLMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET: api/clients
        [HttpGet]
        public ActionResult<IEnumerable<Client>> GetAll()
        {
            var clients = _clientRepository.GetAll();
            return Ok(clients);
        }

        // GET: api/clients/5
        [HttpGet("{id}")]
        public ActionResult<Client> GetById(int id)
        {
            var client = _clientRepository.GetById(id);

            if (client == null)
                return NotFound();

            return Ok(client);
        }
    }
}