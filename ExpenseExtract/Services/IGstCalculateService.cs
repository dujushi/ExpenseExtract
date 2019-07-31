namespace ExpenseExtract.Services
{
    public interface IGstCalculateService
    {
        decimal GetGstExclusiveTotal(decimal gstInclusiveTotal);
    }
}
