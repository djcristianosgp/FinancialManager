using FinancialManager.Domain.Enums;

namespace FinancialManager.Domain.Entities;

public class BankTransaction : BaseEntity
{
    public Guid BankAccountId { get; set; }
    public BankAccount? BankAccount { get; set; }

    public BankTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    public Guid? TransferCorrelationId { get; set; }
}
