using GLMS.Web.Models;
using GLMS.Web.Observers.Contracts;

namespace GLMS.Web.Observers
{
    public class AuditLogger : IContractObserver
    {
        private readonly ILogger<AuditLogger> _logger;

        public AuditLogger(ILogger<AuditLogger> logger)
        {
            _logger = logger;
        }

        public void OnStatusChanged(Contract contract)
        {
            // Log the status change with timestamp for audit trail
            _logger.LogInformation(
                "[AUDIT] Contract ID {ContractId} status changed to {Status} at {Timestamp}",
                contract.Id,
                contract.Status.ToString(),
                DateTime.UtcNow
            );
        }
    }
}