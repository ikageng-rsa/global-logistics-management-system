using GLMS.Web.Models;

namespace GLMS.Web.Observers.Contracts
{
    public interface IContractObserver
    {
        void OnStatusChanged(Contract contract);
    }
}