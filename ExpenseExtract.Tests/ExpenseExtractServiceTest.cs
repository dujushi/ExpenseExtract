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
    }
}
