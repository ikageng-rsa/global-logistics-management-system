using GLMS.Api.Enums;

namespace GLMS.Api.DTOs
{
    public class CreateContractRequest
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ServiceLevel ServiceLevel { get; set; }
    }
}