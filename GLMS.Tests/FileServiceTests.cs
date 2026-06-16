using GLMS.Api.Services;
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

        [Fact]
        public async Task SaveAgreementAsync_ValidFile_ReturnsRelativePath()
        {
            var content = new byte[] { 1, 2, 3 };
            var stream = new MemoryStream(content);

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("agreement.pdf");
            fileMock.Setup(f => f.Length).Returns(content.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Callback<Stream, CancellationToken>((s, _) => stream.CopyTo(s))
                    .Returns(Task.CompletedTask);

            var result = await _fileService.SaveAgreementAsync(fileMock.Object);

            Assert.StartsWith("/uploads/agreements/", result);
            Assert.EndsWith(".pdf", result);
        }

        [Fact]
        public async Task SaveAgreementAsync_TwoUploads_GeneratesDifferentPaths()
        {
            var content = new byte[] { 1, 2, 3 };

            IFormFile BuildFileMock()
            {
                var stream = new MemoryStream(content);
                var mock = new Mock<IFormFile>();
                mock.Setup(f => f.FileName).Returns("agreement.pdf");
                mock.Setup(f => f.Length).Returns(content.Length);
                mock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Callback<Stream, CancellationToken>((s, _) => stream.CopyTo(s))
                    .Returns(Task.CompletedTask);
                return mock.Object;
            }

            var path1 = await _fileService.SaveAgreementAsync(BuildFileMock());
            var path2 = await _fileService.SaveAgreementAsync(BuildFileMock());

            // Assert uuid prefix ensures paths are never identical
            Assert.NotEqual(path1, path2);
        }

        [Fact]
        public void DeleteAgreement_FileDoesNotExist_DoesNotThrow()
        {
            var fakePath = "/uploads/agreements/nonexistent.pdf";

            // should handle gracefully, not throw
            var exception = Record.Exception(() => _fileService.DeleteAgreement(fakePath));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteAgreement_NullPath_DoesNotThrow()
        {
            var exception = Record.Exception(() => _fileService.DeleteAgreement(null!));
            Assert.Null(exception);
        }
    }
}