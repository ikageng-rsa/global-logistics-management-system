using GLMS.Web.Data;
using GLMS.Web.Models;
using GLMS.Web.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace GLMS.Web.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Client> GetAll()
        {
            return _context.Clients
                .AsNoTracking()
                .Include(c => c.Contracts)
                .ToList();
        }

        public Client? GetById(int id)
        {
            return _context.Clients
                .AsNoTracking()
                .Include(c => c.Contracts)
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(Client client)
        {
            _context.Clients.Add(client);
        }

        public void Update(Client client)
        {
            _context.Clients.Update(client);
        }

        public void Delete(int id)
        {
            var client = _context.Clients.Find(id);
            if (client != null)
                _context.Clients.Remove(client);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}