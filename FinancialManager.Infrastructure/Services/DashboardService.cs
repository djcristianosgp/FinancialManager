using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardSummary> GetSummaryAsync(DateTime referenceDate, CancellationToken cancellationToken = default)
    {
        var month = referenceDate.Month;
        var year = referenceDate.Year;

        var bankAccounts = await _db.BankAccounts.AsNoTracking().ToListAsync(cancellationToken);
        var incomes = await _db.Incomes
            .AsNoTracking()
            .Where(i => i.ReceivedDate.Month == month && i.ReceivedDate.Year == year)
            .ToListAsync(cancellationToken);

        var expenses = await _db.Expenses
            .AsNoTracking()
            .Where(e => e.ExpenseDate.Month == month && e.ExpenseDate.Year == year)
            .ToListAsync(cancellationToken);

        var creditCards = await _db.CreditCards.AsNoTracking().ToListAsync(cancellationToken);
        var cardTransactions = await _db.CreditCardTransactions
            .AsNoTracking()
            .Include(t => t.CreditCard)
            .ToListAsync(cancellationToken);

        var totalBalance = bankAccounts.Sum(a => a.CurrentBalance);
        var monthlyIncome = incomes.Sum(i => i.Amount);
        var monthlyExpenses = expenses.Sum(e => e.Amount);

        var expensesByCategory = expenses
            .GroupBy(e => e.Category)
            .Select(g => new { Name = g.Key, Amount = g.Sum(e => e.Amount) })
            .OrderByDescending(g => g.Amount)
            .ToList();

        var categoryTotal = expensesByCategory.Sum(c => c.Amount);
        var categoryBreakdown = expensesByCategory
            .Select(c => new CategoryBreakdownItem(
                string.IsNullOrWhiteSpace(c.Name) ? "Uncategorized" : c.Name,
                c.Amount,
                categoryTotal == 0 ? 0 : Math.Round((c.Amount / categoryTotal) * 100, 0)))
            .ToList();

        var recentMovements = expenses
            .OrderByDescending(e => e.ExpenseDate)
            .Take(5)
            .Select(e => new MovementItem(e.ExpenseDate, e.Title, e.Category, -e.Amount))
            .Union(
                incomes
                    .OrderByDescending(i => i.ReceivedDate)
                    .Take(5)
                    .Select(i => new MovementItem(i.ReceivedDate, i.Title, i.Category, i.Amount)))
            .OrderByDescending(m => m.Date)
            .Take(8)
            .ToList();

        var upcomingInvoices = cardTransactions
            .Where(t => t.CurrentInstallment <= t.Installments) // Apenas parcelas não pagas
            .GroupBy(t => new { t.CreditCardId, t.CreditCard!.Name, DueMonth = t.FirstDueDate.Month, DueYear = t.FirstDueDate.Year })
            .Select(g => new
            {
                CardName = g.Key.Name,
                DueDate = new DateTime(g.Key.DueYear, g.Key.DueMonth, g.First().CreditCard!.DueDay),
                Amount = g.Sum(t => t.InstallmentAmount)
            })
            .Where(i => i.DueDate >= referenceDate.Date && i.DueDate <= referenceDate.AddMonths(2)) // Próximos 2 meses
            .OrderBy(i => i.DueDate)
            .Take(5)
            .Select(i => new InvoiceItem(i.CardName, i.DueDate, i.Amount))
            .ToList();

        var creditLimit = creditCards.Sum(c => c.Limit);
        var creditCurrent = cardTransactions
            .Where(t => t.CurrentInstallment <= t.Installments) // Apenas parcelas ativas
            .Sum(t => t.RemainingAmount); // Soma das parcelas restantes

        var creditUsagePercent = creditLimit == 0 ? 0 : Math.Round((creditCurrent / creditLimit) * 100, 0);

        var metrics = new DashboardMetrics(totalBalance, monthlyIncome, monthlyExpenses, creditUsagePercent, creditCurrent, creditLimit);
        var creditUsage = new CreditUsageSummary(creditCurrent, creditLimit, creditUsagePercent);

        return new DashboardSummary(metrics, categoryBreakdown, recentMovements, upcomingInvoices, creditUsage);
    }
}
