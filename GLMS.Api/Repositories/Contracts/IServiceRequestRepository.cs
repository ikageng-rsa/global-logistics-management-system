
using GLMS.Api.Models;

namespace GLMS.Api.Repositories.Contracts
{
    public interface IServiceRequestRepository
    {
        IEnumerable<ServiceRequest> GetAll();
        IEnumerable<ServiceRequest> GetByContractId(int contractId);
        ServiceRequest? GetById(int id);
        void Add(ServiceRequest serviceRequest);
        void Update(ServiceRequest serviceRequest);
        void Delete(int id);
        void Save();
    }
}
