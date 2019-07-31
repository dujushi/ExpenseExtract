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
        [TestMethod]
        public void GetGstExclusiveTotal_ReturnsCorrectValue()
        {
            var mock = new Mock<ILogger<GstCalculateService>>();
            var gstCalculateOptions = Options.Create(new GstCalculateOptions
            {
                Rate = 0.15m
            });
            var gstCalculateService = new GstCalculateService(mock.Object, gstCalculateOptions);
            var total = gstCalculateService.GetGstExclusiveTotal(115m);
            Assert.AreEqual(100, total);
        }
    }
}
