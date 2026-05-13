using GLMS.Web.Services.Contracts;

namespace GLMS.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;

        // Only PDF files are accepted
        private const string AllowedExtension = ".pdf";
        private const string AllowedMimeType = "application/pdf";
        private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        public bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxFileSizeBytes)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != AllowedExtension)
                return false;

            if (file.ContentType.ToLowerInvariant() != AllowedMimeType)
                return false;

            return true;
        }

        public async Task<string> SaveAgreementAsync(IFormFile file)
        {
            // Build upload path under wwwroot/uploads/agreements
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "agreements");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // UUID prefix ensures unique filenames ro prevents collisions and enumeration
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Agreement file saved: {FileName}", uniqueFileName);

            // Return the relative URL path for storage in the database
            return $"/uploads/agreements/{uniqueFileName}";
        }

        public void DeleteAgreement(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("Agreement file deleted: {FilePath}", filePath);
            }
        }
    }
}