using ExpenseExtract.Exceptions;
using ExpenseExtract.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpenseExtract.Services
{
    public class ExpenseExtractService : IExpenseExtractService
    {
        private string _content;

        public void SetContent(string content)
        {
            _content = content;
        }

        public void CheckUnclosedTags()
        {
            CheckIsContentInitialised();

            // all open tags
            const string openTagPattern = @"<([a-zA-Z_]*)>";
            var openTagRegex = new Regex(openTagPattern);
            var openTagRegexMatches = openTagRegex.Matches(_content);

            // all close tags
            const string closeTagPattern = @"</([a-zA-Z_]*)>";
            var closeTagRegex = new Regex(closeTagPattern);
            var closeTagRegexMatches = closeTagRegex.Matches(_content);

            // If cannot find matching close tag, unclosed tags exist.
            var openTags = openTagRegexMatches.Select(match => match.Groups[1].Value).ToList();
            var closeTags = closeTagRegexMatches.Select(match => match.Groups[1].Value).ToList();
            var unclosedTags = openTags.Except(closeTags).ToList();
            if (unclosedTags.Any())
            {
                throw new InvalidContentException($"Cannot find matching close tag for {string.Join(", ", unclosedTags)}");
            }
        }

        public void CheckExpenseTag()
        {
            CheckIsContentInitialised();

            var expenseTagContent = GetExpenseTagContent();
            if (expenseTagContent == null)
            {
                throw new InvalidContentException($"Cannot find {Tags.Expense} tag");
            }
            if (expenseTagContent.Length == 0)
            {
                throw new InvalidContentException($"{Tags.Expense} tag is empty");
            }
        }

        public void CheckTotalTag()
        {
            throw new NotImplementedException();
        }

        public void ValidateContent()
        {
            throw new NotImplementedException();
        }

        public ExpenseViewModel GetExpense()
        {
            throw new NotImplementedException();
        }

        private void CheckIsContentInitialised()
        {
            if (_content == null)
            {
                throw new InvalidContentException("Content is not set");
            }
        }

        private string GetExpenseTagContent()
        {
            var expenseTagContent = GetTagContent(Tags.Expense, _content);
            return expenseTagContent;
        }

        private static string GetTagContent(string tagName, string content)
        {
            var tagPattern = $@"<({tagName})>(.*)</\1>";
            var tagRegex = new Regex(tagPattern);
            var tagRegexMatches = tagRegex.Matches(content);
            if (tagRegexMatches.Count == 0)
            {
                return null;
            }
            // Use the latest tag if there are many.
            var expense = tagRegexMatches.First().Groups[2].Value.Trim();
            return expense;
        }
    }
}
