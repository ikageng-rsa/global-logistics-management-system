using GLMS.Api.Models;

namespace GLMS.Api.Repositories.Contracts
{
    public interface IContractRepository
    {
        IEnumerable<Contract> GetAll();
        Contract? GetById(int id);
        void Add(Contract contract);
        void Update(Contract contract);
        void Delete(int id);
        void Save();
    }
}
