using GLMS.Api.Enums;
using GLMS.Api.Models;
using GLMS.Api.Observers.Contracts;
using GLMS.Api.Repositories.Contracts;

namespace GLMS.Api.Observers
{
    public class ServiceRequestBlocker : IContractObserver
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public ServiceRequestBlocker(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public void OnStatusChanged(Contract contract)
        {
            // Block all pending service requests if contract is Expired or OnHold
            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                var pendingRequests = _serviceRequestRepository
                    .GetByContractId(contract.Id)
                    .Where(sr => sr.Status == RequestStatus.Pending)
                    .ToList();

                foreach (var request in pendingRequests)
                {
                    request.Status = RequestStatus.Rejected;
                    _serviceRequestRepository.Update(request);
                }

                _serviceRequestRepository.Save();
            }
        }
    }
}