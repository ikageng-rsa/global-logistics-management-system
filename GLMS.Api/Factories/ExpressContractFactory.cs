using GLMS.Api.Enums;
using GLMS.Api.Factories.Interfaces;
using GLMS.Api.Models;

namespace GLMS.Api.Factories
{
    public class ExpressContractFactory : IContractFactory
    {
        public Contract CreateContract(int clientId, DateTime startDate, DateTime endDate)
        {
            ValidateDuration(startDate, endDate);

            return new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = ContractStatus.Draft,
                ServiceLevel = ServiceLevel.Express
            };
        }

        private void ValidateDuration(DateTime startDate, DateTime endDate)
        {
            // Express contracts are short-term — capped at 30 days
            if ((endDate - startDate).TotalDays > 30)
                throw new ArgumentException("Express contracts cannot exceed 30 days.");
        }
    }
}