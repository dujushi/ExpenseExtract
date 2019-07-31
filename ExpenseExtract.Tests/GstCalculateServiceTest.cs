using ExpenseExtract.Services;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseExtract.Tests
{
    [TestClass]
    public class GstCalculateServiceTest
    {
        [TestMethod]
        public void GetGstExclusiveTotal_ReturnsCorrectValue()
        {
            var gstCalculateOptions = Options.Create(new GstCalculateOptions
            {
                Rate = 0.15m
            });
            var gstCalculateService = new GstCalculateService(gstCalculateOptions);
            var total = gstCalculateService.GetGstExclusiveTotal(115m);
            Assert.AreEqual(100, total);
        }
    }
}
