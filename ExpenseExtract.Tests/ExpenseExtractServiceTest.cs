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
    }
}
