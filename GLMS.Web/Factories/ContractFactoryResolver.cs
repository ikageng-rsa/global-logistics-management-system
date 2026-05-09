using GLMS.Web.Enums;
using GLMS.Web.Factories.Interfaces;
using GLMS.Web.Models;

namespace GLMS.Web.Factories
{
    public class ContractFactoryResolver
    {
        private readonly StandardContractFactory _standard;
        private readonly PremiumContractFactory _premium;
        private readonly ExpressContractFactory _express;

        public ContractFactoryResolver()
        {
            _standard = new StandardContractFactory();
            _premium = new PremiumContractFactory();
            _express = new ExpressContractFactory();
        }

        public IContractFactory Resolve(ServiceLevel serviceLevel)
        {
            return serviceLevel switch
            {
                ServiceLevel.Standard => _standard,
                ServiceLevel.Premium => _premium,
                ServiceLevel.Express => _express,
                _ => throw new ArgumentException($"No factory registered for service level: {serviceLevel}")
            };
        }
    }
}