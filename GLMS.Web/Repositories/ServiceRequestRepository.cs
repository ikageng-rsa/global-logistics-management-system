using Microsoft.EntityFrameworkCore;
using GLMS.Web.Data;
using GLMS.Web.Models;
using GLMS.Web.Repositories.Contracts;

namespace GLMS.Web.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly AppDbContext _context;

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ServiceRequest> GetAll()
        {
            return _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ToList();
        }

        public IEnumerable<ServiceRequest> GetByContractId(int contractId)
        {
            return _context.ServiceRequests
                .Where(sr => sr.ContractId == contractId)
                .ToList();
        }

        public ServiceRequest? GetById(int id)
        {
            return _context.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefault(sr => sr.Id == id);
        }

        public void Add(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Add(serviceRequest);
        }

        public void Update(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
        }

        public void Delete(int id)
        {
            var sr = _context.ServiceRequests.Find(id);
            if (sr != null)
                _context.ServiceRequests.Remove(sr);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
