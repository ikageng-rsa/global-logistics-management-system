using GLMS.Api.Enums;
using GLMS.Api.Factories.Interfaces;
using GLMS.Api.Models;

namespace GLMS.Api.Factories
{
    public class StandardContractFactory : IContractFactory
    {
        public Contract CreateContract(int clientId, DateTime startDate, DateTime endDate)
        {
            ValidateSLA(startDate, endDate);

            return new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = ContractStatus.Draft,
                ServiceLevel = ServiceLevel.Standard
            };
        }

        private void ValidateSLA(DateTime startDate, DateTime endDate)
        {
            // Standard contracts haveminimum duration of 30 days
            if ((endDate - startDate).TotalDays < 30)
                throw new ArgumentException("Standard contracts require a minimum duration of 30 days.");
        }
    }
}