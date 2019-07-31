using ExpenseExtract.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseExtract.Tests
{
    [TestClass]
    public class GstCalculateServiceTest
    {
        [TestMethod]
        public void GetGstExclusiveTotal_ReturnsCorrectValue()
        {
            var gstCalculateService = new GstCalculateService();
            var total = gstCalculateService.GetGstExclusiveTotal(115m);
            Assert.AreEqual(100, total);
        }
    }
}
