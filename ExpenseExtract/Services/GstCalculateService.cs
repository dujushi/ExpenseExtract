using ExpenseExtract.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace ExpenseExtract.Services
{
    public class GstCalculateService : IGstCalculateService
    {
        private readonly GstCalculateOptions _options;

        public GstCalculateService(ILogger<GstCalculateService> logger, IOptions<GstCalculateOptions> options)
        {
            try
            {
                _options = options.Value;
            }
            catch (InvalidConfigurationException invalidConfigurationException)
            {
                logger.LogError("invalid configuration", invalidConfigurationException);
                throw;
            }
        }

        public decimal GetGstExclusiveTotal(decimal gstInclusiveTotal)
        {
            var total = gstInclusiveTotal / (1 + _options.Rate);
            var rounded = Math.Round(total, 2);
            return rounded;
        }
    }
}
