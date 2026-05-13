namespace GLMS.Web.Services.Contracts
{
    public interface IFileService
    {
        Task<string> SaveAgreementAsync(IFormFile file);
        void DeleteAgreement(string filePath);
        bool IsValidPdf(IFormFile file);
    }
}