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
            CheckIsContentInitialised();

            var totalTagContent = GetTotalTagContent();
            if (totalTagContent == null)
            {
                throw new InvalidContentException($"Cannot find {Tags.Total} tag");
            }
            if (totalTagContent.Length == 0)
            {
                throw new InvalidContentException($"{Tags.Total} tag is empty");
            }
            if (!decimal.TryParse(totalTagContent, out _))
            {
                throw new InvalidContentException($"{Tags.Total} must be decimal: {totalTagContent}");
            }
        }

        public void ValidateContent()
        {
            CheckIsContentInitialised();
            CheckUnclosedTags();
            CheckExpenseTag();
            CheckTotalTag();
        }

        public ExpenseViewModel GetExpense()
        {
            ValidateContent();

            var totalTagContent = GetTotalTagContent();
            var total = decimal.Parse(totalTagContent); // It is safe to parse total directly. It has passed validation.

            var expenseTagContent = GetExpenseTagContent();
            var costCentre = GetTagContent(Tags.CostCentre, expenseTagContent);
            var paymentMethod = GetTagContent(Tags.PaymentMethod, expenseTagContent);

            var vendor = GetTagContent(Tags.Vendor, _content);
            var description = GetTagContent(Tags.Description, _content);

            var expenseViewModel = new ExpenseViewModel
            {
                CostCentre = string.IsNullOrEmpty(costCentre) ? CostCentres.Default : costCentre,
                Total = total,
                PaymentMethod = paymentMethod,
                Vendor = vendor,
                Description = description
            };

            var dateTagContent = GetTagContent(Tags.Date, _content);
            if (string.IsNullOrEmpty(dateTagContent))
            {
                return expenseViewModel;
            }

            if (DateTime.TryParse(dateTagContent, out var date))
            {
                expenseViewModel.Date = date;
            }
            else
            {
                throw new InvalidContentException($"Invalid {Tags.Date}: {dateTagContent}");
            }
            return expenseViewModel;
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

        private string GetTotalTagContent()
        {
            var expenseTagContent = GetExpenseTagContent();
            var totalTagContent = GetTagContent(Tags.Total, expenseTagContent ?? string.Empty);
            return totalTagContent;
        }
    }
}
