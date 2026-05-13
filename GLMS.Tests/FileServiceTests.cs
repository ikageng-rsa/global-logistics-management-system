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
    }
}