using GLMS.Api.Models;

namespace GLMS.Api.Observers.Contracts
{
    public interface IContractObserver
    {
        void OnStatusChanged(Contract contract);
    }
}