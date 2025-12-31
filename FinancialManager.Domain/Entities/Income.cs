using FinancialManager.Domain.Enums;

namespace FinancialManager.Domain.Entities;

public class Income : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ReceivedDate { get; set; }
    public RecurrenceType Recurrence { get; set; }

    public Guid BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }
}
