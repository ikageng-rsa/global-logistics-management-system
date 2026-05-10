namespace GLMS.Web.Services.Contracts
{
    public interface ICurrencyService
    {
        Task<decimal> GetUsdToZarRateAsync();
        Task<decimal> ConvertUsdToZarAsync(decimal amountUsd);
    }
}