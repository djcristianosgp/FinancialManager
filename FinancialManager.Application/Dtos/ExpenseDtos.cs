using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Dtos;

public record ExpenseListItem(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    string Category,
    PaymentMethod PaymentMethod,
    ExpenseStatus Status,
    string? BankAccountName,
    string? CreditCardName
);

public record ExpenseCreateDto(
    string Description,
    decimal Amount,
    DateTime Date,
    string Category,
    PaymentMethod PaymentMethod,
    ExpenseStatus Status,
    Guid? BankAccountId,
    Guid? CreditCardId,
    int Installments = 1
);

public record ExpenseUpdateDto(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    string Category,
    PaymentMethod PaymentMethod,
    ExpenseStatus Status,
    Guid? BankAccountId,
    Guid? CreditCardId,
    int Installments = 1
);
