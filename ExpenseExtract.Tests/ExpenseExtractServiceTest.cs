using ExpenseExtract.Exceptions;
using ExpenseExtract.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpenseExtract.Tests
{
    [TestClass]
    public class ExpenseExtractServiceTest
    {
        [DataTestMethod]
        [DataRow("<expense>")]
        [DataRow("<expense></ex>")]
        [DataRow("<expense><total></expense>")]
        public void CheckUnclosedTags_WhenContentHasUnclosedTags_ThrowsInvalidContentException(string content)
        {
            var expenseExtractService = new ExpenseExtractService();
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckUnclosedTags());
        }

        [TestMethod]
        public void CheckUnclosedTags_WhenContentDoesNotHaveUnclosedTags_Passes()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense></expense>";
            expenseExtractService.SetContent(content);
            expenseExtractService.CheckUnclosedTags();
        }

        [TestMethod]
        public void CheckExpenseTag_WhenContentDoesNotHaveExpenseTag_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<total></total>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckExpenseTag());
        }

        [TestMethod]
        public void CheckExpenseTag_WhenExpenseTagIsEmpty_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense></expense>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckExpenseTag());
        }

        [TestMethod]
        public void CheckExpenseTag_WhenContentHasExpenseTag_Passes()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense>test</expense>";
            expenseExtractService.SetContent(content);
            expenseExtractService.CheckExpenseTag();
        }

        [TestMethod]
        public void CheckTotalTag_WhenContentDoesNotHaveTotalTag_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense></expense>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckTotalTag());
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalTagNotWithinExpenseTag_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<total></total>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckTotalTag());
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalTagIsEmpty_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense><total></total></expense>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckTotalTag());
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalTagIsNotDecimal_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense><total>text</total></expense>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.CheckTotalTag());
        }

        [TestMethod]
        public void CheckTotalTag_WhenTotalIsValid_Passes()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense><total>11</total></expense>";
            expenseExtractService.SetContent(content);
            expenseExtractService.CheckTotalTag();
        }

        [TestMethod]
        public void GetExpense_WhenContentIsNotSet_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.GetExpense());
        }

        [TestMethod]
        public void GetExpense_WhenDateIsInvalid_ThrowsInvalidContentException()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense><total>11</total></expense><date>invalid date</date>";
            expenseExtractService.SetContent(content);
            Assert.ThrowsException<InvalidContentException>(() => expenseExtractService.GetExpense());
        }

        [TestMethod]
        public void GetExpense_WhenCostCentreIsNotSet_ReturnsDefaultValue()
        {
            var expenseExtractService = new ExpenseExtractService();
            const string content = "<expense><total>11</total></expense>";
            expenseExtractService.SetContent(content);
            var expense = expenseExtractService.GetExpense();
            Assert.AreEqual(CostCentres.Default, expense.CostCentre);
        }
    }
}
