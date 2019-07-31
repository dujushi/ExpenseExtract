using ExpenseExtract.Dtos;

namespace ExpenseExtract.Services
{
    public interface IExpenseExtractService
    {
        void CheckUnclosedTags(string content);
        void CheckExpenseTag(string content);
        void CheckTotalTag(string content);
        void ValidateContent(string content);
        ExpenseDto GetExpense(string content);
    }
}
