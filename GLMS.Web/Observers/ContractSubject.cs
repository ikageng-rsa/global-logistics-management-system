using GLMS.Web.Models;
using GLMS.Web.Observers.Contracts;

namespace GLMS.Web.Observers
{
    public class ContractSubject
    {
        private readonly List<IContractObserver> _observers = new();

        public void Attach(IContractObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IContractObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify(Contract contract)
        {
            foreach (var observer in _observers)
            {
                observer.OnStatusChanged(contract);
            }
        }
    }
}