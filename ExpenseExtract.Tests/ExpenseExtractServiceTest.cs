using ExpenseExtract.Exceptions;
using ExpenseExtract.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ExpenseExtract.Tests
{
    [TestClass]
    public class ExpenseExtractServiceTest
    {
        private IExpenseExtractService _expenseExtractService;

        [TestInitialize]
        public void TestInitialize()
        {
            var gstCalculateService = Mock.Of<IGstCalculateService>(service => 
                service.GetGstExclusiveTotal(It.IsAny<decimal>()) == 100m);
            _expenseExtractService = new ExpenseExtractService(gstCalculateService);
        }

        [DataTestMethod]
        [DataRow("<expense>")]
        [DataRow("<expense></ex>")]
        [DataRow("<expense><total></expense>")]
        public void CheckUnclosedTags_WhenContentHasUnclosedTags_ThrowsInvalidContentException(string content)
        {
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckUnclosedTags(content));
        }

        [TestMethod]
        public void CheckUnclosedTags_WhenContentDoesNotHaveUnclosedTags_Passes()
        {
            const string content = "<expense></expense>";
            _expenseExtractService.CheckUnclosedTags(content);
        }

        [TestMethod]
        public void CheckExpenseTag_WhenContentDoesNotHaveExpenseTag_ThrowsInvalidContentException()
        {
            const string content = "<total></total>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckExpenseTag(content));
        }

        [TestMethod]
        public void CheckExpenseTag_WhenExpenseTagIsEmpty_ThrowsInvalidContentException()
        {
            const string content = "<expense></expense>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckExpenseTag(content));
        }

        [TestMethod]
        public void CheckExpenseTag_WhenContentHasExpenseTag_Passes()
        {
            const string content = "<expense>test</expense>";
            _expenseExtractService.CheckExpenseTag(content);
        }

        [TestMethod]
        public void CheckTotalTag_WhenContentDoesNotHaveTotalTag_ThrowsInvalidContentException()
        {
            const string content = "<expense></expense>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckTotalTag(content));
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalTagNotWithinExpenseTag_ThrowsInvalidContentException()
        {
            const string content = "<total></total>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckTotalTag(content));
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalTagIsEmpty_ThrowsInvalidContentException()
        {
            const string content = "<expense><total></total></expense>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckTotalTag(content));
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalTagIsNotDecimal_ThrowsInvalidContentException()
        {
            const string content = "<expense><total>text</total></expense>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.CheckTotalTag(content));
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalIsValid_Passes()
        {
            const string content = "<expense><total>11</total></expense>";
            _expenseExtractService.CheckTotalTag(content);
        }

        [TestMethod]
        public void GetExpense_WhenDateIsInvalid_ThrowsInvalidContentException()
        {
            const string content = "<expense><total>11</total></expense><date>invalid date</date>";
            Assert.ThrowsException<InvalidContentException>(() => _expenseExtractService.GetExpense(content));
        }

        [TestMethod]
        public void GetExpense_WhenCostCentreIsNotSet_ReturnsDefaultValue()
        {
            const string content = "<expense><total>11</total></expense>";
            var expense = _expenseExtractService.GetExpense(content);
            Assert.AreEqual(CostCentres.Default, expense.CostCentre);
        }

        [TestMethod]
        public void GetExpense_WhenValuesContainTags_ReturnsEncodedValues()
        {
            const string content = @"
<expense>
    <cost_centre><script>alert('cost_centre')</script></cost_centre>
    <total>11</total>
    <payment_method><script>alert('payment_method')</script></payment_method>
</expense>
<vendor><script>alert('vendor')</script></vendor>
<description><script>alert('description')</script></description>
";
            var expense = _expenseExtractService.GetExpense(content);
            Assert.AreEqual("&lt;script&gt;alert(&#39;cost_centre&#39;)&lt;/script&gt;", expense.CostCentre);
            Assert.AreEqual("&lt;script&gt;alert(&#39;payment_method&#39;)&lt;/script&gt;", expense.PaymentMethod);
            Assert.AreEqual("&lt;script&gt;alert(&#39;vendor&#39;)&lt;/script&gt;", expense.Vendor);
            Assert.AreEqual("&lt;script&gt;alert(&#39;description&#39;)&lt;/script&gt;", expense.Description);
        }
    }
}
