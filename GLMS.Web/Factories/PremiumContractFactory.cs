using GLMS.Web.Enums;
using GLMS.Web.Factories.Interfaces;
using GLMS.Web.Models;

namespace GLMS.Web.Factories
{
    public class PremiumContractFactory : IContractFactory
    {
        public Contract CreateContract(int clientId, DateTime startDate, DateTime endDate)
        {
            ApplyPricing(startDate, endDate);

            return new Contract
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = ContractStatus.Draft,
                ServiceLevel = ServiceLevel.Premium
            };
        }

        private void ApplyPricing(DateTime startDate, DateTime endDate)
        {
            // Premium contracts have a minimum duration of 90 days
            if ((endDate - startDate).TotalDays < 90)
                throw new ArgumentException("Premium contracts require a minimum duration of 90 days.");
        }
    }
}