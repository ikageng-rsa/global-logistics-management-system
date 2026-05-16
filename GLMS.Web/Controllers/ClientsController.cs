using GLMS.Web.Models;
using GLMS.Web.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLMS.Web.Controllers
{
    [Authorize]

    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        [HttpGet("clients")]
        public IActionResult Index()
        {
            var clients = _clientRepository.GetAll();
            return View(clients);
        }

        [HttpGet("clients/details/{id}")]
        public IActionResult Details(int id)
        {
            var client = _clientRepository.GetById(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpGet("clients/create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("clients/create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                _clientRepository.Add(client);
                _clientRepository.Save();

                TempData["Success"] = "Client added successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        [HttpGet("clients/edit/{id}")]
        public IActionResult Edit(int id)
        {
            var client = _clientRepository.GetById(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost("clients/edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Client client)
        {
            if (id != client.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _clientRepository.Update(client);
                _clientRepository.Save();

                TempData["Success"] = "Client updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        [HttpGet("clients/delete/{id}")]
        public IActionResult Delete(int id)
        {
            var client = _clientRepository.GetById(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost("clients/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _clientRepository.Delete(id);
            _clientRepository.Save();

            TempData["Success"] = "Client deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}