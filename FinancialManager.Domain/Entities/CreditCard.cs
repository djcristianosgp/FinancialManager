namespace FinancialManager.Domain.Entities;

public class CreditCard : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Bank { get; set; } = string.Empty;
    public decimal Limit { get; set; }
    public int ClosingDay { get; set; }
    public int DueDay { get; set; }

    public ICollection<CreditCardTransaction> Transactions { get; set; } = new List<CreditCardTransaction>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

    public decimal AvailableLimit => Limit - Transactions.Sum(t => t.RemainingAmount);
}
