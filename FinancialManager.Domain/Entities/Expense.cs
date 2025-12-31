using FinancialManager.Domain.Enums;

namespace FinancialManager.Domain.Entities;

public class Expense : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public RecurrenceType Recurrence { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public ExpenseStatus Status { get; set; }

    public Guid? BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }

    public Guid? CreditCardId { get; set; }
    public CreditCard? CreditCard { get; set; }
}
