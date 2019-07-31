using ExpenseExtract.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExpenseExtract.Tests
{
    [TestClass]
    public class GstCalculateServiceTest
    {
        private IGstCalculateService _gstCalculateService;

        [TestInitialize]
        public void TestInitialize()
        {
            var mockLogger = new Mock<ILogger<GstCalculateService>>();
            var gstCalculateOptions = Options.Create(new GstCalculateOptions
            {
                Rate = 0.15m
            });
            _gstCalculateService = new GstCalculateService(mockLogger.Object, gstCalculateOptions);
        }

        [TestMethod]
        public void GetGstExclusiveTotal_ReturnsCorrectValue()
        {
            var total = _gstCalculateService.GetGstExclusiveTotal(115m);
            Assert.AreEqual(100, total);
        }
    }
}
