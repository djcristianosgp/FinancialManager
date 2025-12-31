using System.ComponentModel.DataAnnotations.Schema;

namespace FinancialManager.Domain.Entities;

public class CreditCardTransaction : BaseEntity
{
    public Guid CreditCardId { get; set; }
    public CreditCard? CreditCard { get; set; }

    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Installments { get; set; } = 1;
    public int CurrentInstallment { get; set; } = 1;
    public DateTime TransactionDate { get; set; }
    public DateTime FirstDueDate { get; set; }

    [NotMapped]
    public decimal InstallmentAmount => Installments <= 0 ? Amount : Math.Round(Amount / Installments, 2);
    [NotMapped]
    public decimal RemainingAmount => InstallmentAmount * Math.Max(Installments - CurrentInstallment + 1, 0);
}
