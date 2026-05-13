using GLMS.Web.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace GLMS.Tests
{
    public class FileServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _environmentMock;
        private readonly Mock<ILogger<FileService>> _loggerMock;
        private readonly FileService _fileService;

        public FileServiceTests()
        {
            _environmentMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<FileService>>();

            // Point WebRootPath to a temp folder for testing
            _environmentMock.Setup(e => e.WebRootPath)
                .Returns(Path.GetTempPath());

            _fileService = new FileService(_environmentMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void IsValidPdf_ValidPdfFile_ReturnsTrue()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("agreement.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.Length).Returns(1024);

            var result = _fileService.IsValidPdf(fileMock.Object);

            Assert.True(result);
        }

        [Fact]
        public void IsValidPdf_NonPdfFile_ReturnsFalse()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("malicious.exe");
            fileMock.Setup(f => f.ContentType).Returns("application/octet-stream");
            fileMock.Setup(f => f.Length).Returns(1024);

            var result = _fileService.IsValidPdf(fileMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void IsValidPdf_FileTooLarge_ReturnsFalse()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("large.pdf");
            fileMock.Setup(f => f.ContentType).Returns("application/pdf");
            fileMock.Setup(f => f.Length).Returns(11 * 1024 * 1024);

            var result = _fileService.IsValidPdf(fileMock.Object);

            Assert.False(result);
        }

        [Fact]
        public void IsValidPdf_NullFile_ReturnsFalse()
        {
            var result = _fileService.IsValidPdf(null!);

            Assert.False(result);
        }
    }
}