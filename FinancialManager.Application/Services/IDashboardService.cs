using FinancialManager.Application.Dtos;

namespace FinancialManager.Application.Services;

public interface IDashboardService
{
    Task<DashboardSummary> GetSummaryAsync(DateTime referenceDate, CancellationToken cancellationToken = default);
}
