using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Dtos;

public record IncomeListItem(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    string Category,
    string BankAccountName,
    RecurrenceType Recurrence
);

public record IncomeCreateDto(
    string Description,
    decimal Amount,
    DateTime Date,
    string Category,
    Guid BankAccountId,
    RecurrenceType Recurrence
);

public record IncomeUpdateDto(
    Guid Id,
    string Description,
    decimal Amount,
    DateTime Date,
    string Category,
    Guid BankAccountId,
    RecurrenceType Recurrence
);
