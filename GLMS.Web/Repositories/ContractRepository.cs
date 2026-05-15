using Microsoft.EntityFrameworkCore;
using GLMS.Web.Models;
using GLMS.Web.Data;
using GLMS.Web.Repositories.Contracts;

namespace GLMS.Web.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly AppDbContext _context;

        public ContractRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Contract> GetAll()
        {
            return _context.Contracts
                .AsNoTracking()
                .Include(c => c.Client)
                .ToList();
        }

        public Contract? GetById(int id)
        {
            return _context.Contracts
                .AsNoTracking()
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefault(c => c.Id == id);
        }

        public void Add(Contract contract)
        {
            _context.Contracts.Add(contract);
        }

        public void Update(Contract contract)
        {
            _context.Contracts.Update(contract);
        }

        public void Delete(int id)
        {
            var contract = _context.Contracts.Find(id);
            if (contract != null)
                _context.Contracts.Remove(contract);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
