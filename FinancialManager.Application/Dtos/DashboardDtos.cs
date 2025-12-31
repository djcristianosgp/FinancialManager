namespace FinancialManager.Application.Dtos;

public record DashboardMetrics(decimal TotalBalance, decimal MonthlyIncome, decimal MonthlyExpenses, decimal CreditUsagePercent, decimal CreditCurrentAmount, decimal CreditLimit);
public record CategoryBreakdownItem(string Name, decimal Amount, decimal Percentage);
public record MovementItem(DateTime Date, string Title, string Category, decimal Amount);
public record InvoiceItem(string CardName, DateTime DueDate, decimal Amount);
public record CreditUsageSummary(decimal CurrentAmount, decimal Limit, decimal CurrentPercentage);

public record DashboardSummary(
    DashboardMetrics Metrics,
    IReadOnlyList<CategoryBreakdownItem> Categories,
    IReadOnlyList<MovementItem> RecentMovements,
    IReadOnlyList<InvoiceItem> UpcomingInvoices,
    CreditUsageSummary CreditUsage);
