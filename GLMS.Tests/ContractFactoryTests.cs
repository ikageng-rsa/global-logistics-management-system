using GLMS.Web.Enums;
using GLMS.Web.Factories;
using GLMS.Web.Models;

namespace GLMS.Tests
{
    public class ContractFactoryTests
    {
        private readonly DateTime _today = DateTime.Today;

        [Fact]
        public void StandardFactory_ValidDates_CreatesStandardContract()
        {
            var factory = new StandardContractFactory();
            var start = _today;
            var end = _today.AddDays(60);

            var contract = factory.CreateContract(1, start, end);

            // Assert
            Assert.Equal(ServiceLevel.Standard, contract.ServiceLevel);
            Assert.Equal(ContractStatus.Draft, contract.Status);
        }

        [Fact]
        public void StandardFactory_DurationUnder30Days_ThrowsArgumentException()
        {
            // 20 days is below the 30 day minimum
            var factory = new StandardContractFactory();
            var start = _today;
            var end = _today.AddDays(20);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                factory.CreateContract(1, start, end));
        }

        [Fact]
        public void PremiumFactory_DurationUnder90Days_ThrowsArgumentException()
        {
            // 60 days is below the 90 day minimum for premium
            var factory = new PremiumContractFactory();
            var start = _today;
            var end = _today.AddDays(60);

            Assert.Throws<ArgumentException>(() =>
                factory.CreateContract(1, start, end));
        }
        [Fact]
        public void ExpressFactory_DurationOver30Days_ThrowsArgumentException()
        {
            var factory = new ExpressContractFactory();
            var start = _today;
            var end = _today.AddDays(45);

            Assert.Throws<ArgumentException>(() =>
                factory.CreateContract(1, start, end));
        }

    }
}