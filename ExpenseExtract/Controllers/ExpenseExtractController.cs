using ExpenseExtract.Exceptions;
using ExpenseExtract.Services;
using ExpenseExtract.Dtos;
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

        public ExpenseExtractController(
            ILogger<ExpenseExtractController> logger,
            IExpenseExtractService expenseExtractService
        )
        {
            _logger = logger;
            _expenseExtractService = expenseExtractService;
        }

        [HttpPost]
        public ActionResult<ExpenseDto> Post([FromBody] string content)
        {
            try
            {
                _expenseExtractService.SetContent(content);
                var expense = _expenseExtractService.GetExpense();
                return expense;
            }
            catch (InvalidContentException invalidContentException)
            {
                _logger.LogError("Invalid Content", invalidContentException);

                var errorsViewModel = new ErrorsDto
                {
                    Errors = new[]
                    {
                        new ErrorDto
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
