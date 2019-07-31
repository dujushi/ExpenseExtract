using Microsoft.Extensions.Options;
using System;

namespace ExpenseExtract.Services
{
    public class GstCalculateService : IGstCalculateService
    {
        private readonly GstCalculateOptions _options;

        public GstCalculateService(IOptions<GstCalculateOptions> options)
        {
            _options = options.Value;
        }

        public decimal GetGstExclusiveTotal(decimal gstInclusiveTotal)
        {
            var total = gstInclusiveTotal / (1 + _options.Rate);
            var rounded = Math.Round(total, 2);
            return rounded;
        }
    }
}
