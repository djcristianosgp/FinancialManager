namespace FinancialManager.Application.Dtos;

public record CreditCardListItem(
    Guid Id,
    string Name,
    string Bank,
    decimal Limit,
    decimal AvailableLimit,
    int ClosingDay,
    int DueDay
);

public record CreditCardDetailDto(
    Guid Id,
    string Name,
    string Bank,
    decimal Limit,
    decimal AvailableLimit,
    int ClosingDay,
    int DueDay,
    decimal CurrentInvoiceAmount,
    List<CreditCardTransactionItem> Transactions
);

public record CreditCardTransactionItem(
    Guid Id,
    DateTime Date,
    string Description,
    decimal Amount,
    string Category,
    int Installments,
    int CurrentInstallment,
    decimal InstallmentAmount,
    decimal RemainingAmount
);

public record CreditCardCreateDto(
    string Name,
    string Bank,
    decimal Limit,
    int ClosingDay,
    int DueDay
);

public record CreditCardUpdateDto(
    Guid Id,
    string Name,
    string Bank,
    decimal Limit,
    int ClosingDay,
    int DueDay
);

public record CreditCardTransactionCreateDto(
    Guid CreditCardId,
    string Description,
    decimal TotalAmount,
    DateTime Date,
    string Category,
    int Installments
);
