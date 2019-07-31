﻿using ExpenseExtract.Dtos;

namespace ExpenseExtract.Services
{
    public interface IExpenseExtractService
    {
        void SetContent(string content);
        void CheckUnclosedTags();
        void CheckExpenseTag();
        void CheckTotalTag();
        void ValidateContent();
        ExpenseDto GetExpense();
    }
}
