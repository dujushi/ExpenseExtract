using System;
using System.Linq;
using System.Text.RegularExpressions;
using ExpenseExtract.Dtos;
using ExpenseExtract.Exceptions;

namespace ExpenseExtract.Services
{
    public class ExpenseExtractService : IExpenseExtractService
    {
        private readonly IGstCalculateService _gstCalculateService;

        public ExpenseExtractService(IGstCalculateService gstCalculateService)
        {
            _gstCalculateService = gstCalculateService;
        }

        public void CheckUnclosedTags(string content)
        {
            // all open tags
            const string openTagPattern = @"<([a-zA-Z_]*)>";
            var openTagRegex = new Regex(openTagPattern);
            var openTagRegexMatches = openTagRegex.Matches(content);

            // all close tags
            const string closeTagPattern = @"</([a-zA-Z_]*)>";
            var closeTagRegex = new Regex(closeTagPattern);
            var closeTagRegexMatches = closeTagRegex.Matches(content);

            // If cannot find matching close tag, unclosed tags exist.
            var openTags = openTagRegexMatches.Select(match => match.Groups[1].Value).ToList();
            var closeTags = closeTagRegexMatches.Select(match => match.Groups[1].Value).ToList();
            var unclosedTags = openTags.Except(closeTags).ToList();
            if (unclosedTags.Any())
            {
                throw new InvalidContentException($"Cannot find matching close tag for {string.Join(", ", unclosedTags)}");
            }
        }

        public void CheckExpenseTag(string content)
        {
            var expenseTagContent = GetExpenseTagContent(content);
            if (expenseTagContent == null)
            {
                throw new InvalidContentException($"Cannot find {Tags.Expense} tag");
            }
            if (expenseTagContent.Length == 0)
            {
                throw new InvalidContentException($"{Tags.Expense} tag is empty");
            }
        }

        public void CheckTotalTag(string content)
        {
            var totalTagContent = GetTotalTagContent(content);
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

        public void ValidateContent(string content)
        {
            CheckUnclosedTags(content);
            CheckExpenseTag(content);
            CheckTotalTag(content);
        }

        public ExpenseDto GetExpense(string content)
        {
            ValidateContent(content);

            var totalTagContent = GetTotalTagContent(content);
            var total = decimal.Parse(totalTagContent); // It is safe to parse total directly. It has passed validation.
            var gstExclusiveTotal = _gstCalculateService.GetGstExclusiveTotal(total);

            var expenseTagContent = GetExpenseTagContent(content);
            var costCentre = GetTagContent(Tags.CostCentre, expenseTagContent);
            var paymentMethod = GetTagContent(Tags.PaymentMethod, expenseTagContent);

            var vendor = GetTagContent(Tags.Vendor, content);
            var description = GetTagContent(Tags.Description, content);

            var expenseViewModel = new ExpenseDto
            {
                CostCentre = string.IsNullOrEmpty(costCentre) ? CostCentres.Default : costCentre,
                Total = total,
                GstExclusiveTotal = gstExclusiveTotal,
                PaymentMethod = paymentMethod,
                Vendor = vendor,
                Description = description
            };

            var dateTagContent = GetTagContent(Tags.Date, content);
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

        private static string GetTotalTagContent(string content)
        {
            var expenseTagContent = GetExpenseTagContent(content);
            var totalTagContent = GetTagContent(Tags.Total, expenseTagContent ?? string.Empty);
            return totalTagContent;
        }

        private static string GetExpenseTagContent(string content)
        {
            var expenseTagContent = GetTagContent(Tags.Expense, content);
            return expenseTagContent;
        }

        private static string GetTagContent(string tagName, string content)
        {
            var tagPattern = $@"<({tagName})>(.*)</\1>";
            var tagRegex = new Regex(tagPattern, RegexOptions.Singleline);
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
