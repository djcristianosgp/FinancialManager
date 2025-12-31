using FinancialManager.Domain.Enums;

namespace FinancialManager.Domain.Entities;

public class BankAccount : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public AccountType Type { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal CurrentBalance { get; set; }

    public ICollection<BankTransaction> Transactions { get; set; } = new List<BankTransaction>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
