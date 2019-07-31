using ExpenseExtract.Exceptions;
using ExpenseExtract.Services;
using ExpenseExtract.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ExpenseExtract.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseExtractController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IExpenseExtractService _expenseExtractService;
        private readonly IGstCalculateService _gstCalculateService;

        public ExpenseExtractController(
            ILogger<ExpenseExtractController> logger,
            IExpenseExtractService expenseExtractService,
            IGstCalculateService gstCalculateService
        )
        {
            _logger = logger;
            _expenseExtractService = expenseExtractService;
            _gstCalculateService = gstCalculateService;
        }

        [HttpPost]
        public ActionResult<ExpenseViewModel> Post([FromBody] string content)
        {
            try
            {
                _expenseExtractService.SetContent(content);
                var expense = _expenseExtractService.GetExpense();
                expense.GstExclusiveTotal = _gstCalculateService.GetGstExclusiveTotal(expense.Total);
                return expense;
            }
            catch (InvalidContentException invalidContentException)
            {
                _logger.LogError("Invalid Content", invalidContentException);

                var errorsViewModel = new ErrorsViewModel
                {
                    Errors = new[]
                    {
                        new ErrorViewModel
                        {
                            Title = "Invalid Content",
                            Detail = invalidContentException.Message
                        }
                    }
                };
                return BadRequest(errorsViewModel);
            }
        }
    }
}
