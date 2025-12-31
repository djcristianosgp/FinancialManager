using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Dtos;

public record BankAccountListItem(
    Guid Id,
    string Bank,
    AccountType Type,
    decimal Balance
);

public record BankAccountDetailDto(
    Guid Id,
    string Bank,
    AccountType Type,
    decimal InitialBalance,
    decimal CurrentBalance,
    List<TransactionItem> RecentTransactions
);

public record TransactionItem(
    Guid Id,
    DateTime Date,
    string Description,
    decimal Amount,
    BankTransactionType Type
);

public record BankAccountCreateDto(
    string Bank,
    AccountType Type,
    decimal InitialBalance
);

public record BankAccountUpdateDto(
    Guid Id,
    string Bank,
    AccountType Type
);
