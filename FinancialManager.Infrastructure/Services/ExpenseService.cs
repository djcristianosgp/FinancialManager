using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Entities;
using FinancialManager.Domain.Enums;
using FinancialManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Services;

public class ExpenseService : IExpenseService
{
    private readonly ApplicationDbContext _context;

    public ExpenseService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseListItem>> GetAllAsync()
    {
        return await _context.Expenses
            .Include(e => e.BankAccount)
            .Include(e => e.CreditCard)
            .OrderByDescending(e => e.ExpenseDate)
            .Select(e => new ExpenseListItem(
                e.Id,
                e.Title,
                e.Amount,
                e.ExpenseDate,
                e.Category,
                e.PaymentMethod,
                e.Status,
                e.BankAccount != null ? e.BankAccount.Bank : null,
                e.CreditCard != null ? e.CreditCard.Name : null
            ))
            .ToListAsync();
    }

    public async Task<ExpenseUpdateDto?> GetByIdAsync(Guid id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return null;

        return new ExpenseUpdateDto(
            expense.Id,
            expense.Title,
            expense.Amount,
            expense.ExpenseDate,
            expense.Category,
            expense.PaymentMethod,
            expense.Status,
            expense.BankAccountId,
            expense.CreditCardId,
            expense.Installments
        );
    }

    public async Task<Guid> CreateAsync(ExpenseCreateDto dto)
    {
        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Title = dto.Description,
            Amount = dto.Amount,
            ExpenseDate = dto.Date,
            Category = dto.Category,
            PaymentMethod = dto.PaymentMethod,
            Status = dto.Status,
            BankAccountId = dto.BankAccountId,
            CreditCardId = dto.CreditCardId,
            Installments = dto.Installments,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);

        // Se for cartão de crédito, criar transação no cartão
        if (dto.PaymentMethod == PaymentMethod.Credit && dto.CreditCardId.HasValue)
        {
            var creditCard = await _context.CreditCards.FindAsync(dto.CreditCardId.Value);
            if (creditCard != null)
            {
                var transaction = new CreditCardTransaction
                {
                    Id = Guid.NewGuid(),
                    CreditCardId = dto.CreditCardId.Value,
                    Description = dto.Description,
                    Category = dto.Category,
                    Amount = dto.Amount,
                    Installments = dto.Installments,
                    CurrentInstallment = 1,
                    TransactionDate = dto.Date,
                    FirstDueDate = CalculateFirstDueDate(creditCard, dto.Date),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CreditCardTransactions.Add(transaction);
            }
        }
        // Atualizar saldo se for débito/dinheiro e já pago
        else if (dto.Status == ExpenseStatus.Paid && dto.PaymentMethod != PaymentMethod.Credit && dto.BankAccountId.HasValue)
        {
            var account = await _context.BankAccounts.FindAsync(dto.BankAccountId.Value);
            if (account != null)
            {
                account.CurrentBalance -= dto.Amount;
                account.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return expense.Id;
    }

    private DateTime CalculateFirstDueDate(CreditCard card, DateTime transactionDate)
    {
        var closingDate = new DateTime(transactionDate.Year, transactionDate.Month, card.ClosingDay);
        
        // Se a transação foi após o fechamento, a fatura é do próximo mês
        if (transactionDate > closingDate)
        {
            closingDate = closingDate.AddMonths(1);
        }
        
        // Data de vencimento é no mês seguinte ao fechamento
        var dueDate = closingDate.AddMonths(1);
        dueDate = new DateTime(dueDate.Year, dueDate.Month, card.DueDay);
        
        return dueDate;
    }

    public async Task<bool> UpdateAsync(ExpenseUpdateDto dto)
    {
        var expense = await _context.Expenses.FindAsync(dto.Id);
        if (expense == null) return false;

        var oldAmount = expense.Amount;
        var oldStatus = expense.Status;
        var oldAccountId = expense.BankAccountId;
        var oldCreditCardId = expense.CreditCardId;
        var oldPaymentMethod = expense.PaymentMethod;
        var oldInstallments = expense.Installments;

        // Se estava no cartão, remover transação antiga
        if (oldPaymentMethod == PaymentMethod.Credit && oldCreditCardId.HasValue)
        {
            var oldTransaction = await _context.CreditCardTransactions
                .FirstOrDefaultAsync(t => t.Description == expense.Title && 
                                         t.CreditCardId == oldCreditCardId.Value &&
                                         t.Amount == oldAmount &&
                                         t.TransactionDate == expense.ExpenseDate);
            if (oldTransaction != null)
            {
                _context.CreditCardTransactions.Remove(oldTransaction);
            }
        }

        expense.Title = dto.Description;
        expense.Amount = dto.Amount;
        expense.ExpenseDate = dto.Date;
        expense.Category = dto.Category;
        expense.PaymentMethod = dto.PaymentMethod;
        expense.Status = dto.Status;
        expense.BankAccountId = dto.BankAccountId;
        expense.CreditCardId = dto.CreditCardId;
        expense.Installments = dto.Installments;
        expense.UpdatedAt = DateTime.UtcNow;

        // Se agora é cartão de crédito, criar nova transação
        if (dto.PaymentMethod == PaymentMethod.Credit && dto.CreditCardId.HasValue)
        {
            var creditCard = await _context.CreditCards.FindAsync(dto.CreditCardId.Value);
            if (creditCard != null)
            {
                var transaction = new CreditCardTransaction
                {
                    Id = Guid.NewGuid(),
                    CreditCardId = dto.CreditCardId.Value,
                    Description = dto.Description,
                    Category = dto.Category,
                    Amount = dto.Amount,
                    Installments = dto.Installments,
                    CurrentInstallment = 1,
                    TransactionDate = dto.Date,
                    FirstDueDate = CalculateFirstDueDate(creditCard, dto.Date),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.CreditCardTransactions.Add(transaction);
            }
        }
        // Ajustar saldos apenas para débito/dinheiro
        else if (oldPaymentMethod != PaymentMethod.Credit && dto.PaymentMethod != PaymentMethod.Credit)
        {
            // Se mudou de pendente para pago
            if (oldStatus == ExpenseStatus.Pending && dto.Status == ExpenseStatus.Paid && dto.BankAccountId.HasValue)
            {
                var account = await _context.BankAccounts.FindAsync(dto.BankAccountId.Value);
                if (account != null)
                {
                    account.CurrentBalance -= dto.Amount;
                    account.UpdatedAt = DateTime.UtcNow;
                }
            }
            // Se mudou de pago para pendente
            else if (oldStatus == ExpenseStatus.Paid && dto.Status == ExpenseStatus.Pending && oldAccountId.HasValue)
            {
                var account = await _context.BankAccounts.FindAsync(oldAccountId.Value);
                if (account != null)
                {
                    account.CurrentBalance += oldAmount;
                    account.UpdatedAt = DateTime.UtcNow;
                }
            }
            // Se ambos são pagos mas mudou valor ou conta
            else if (oldStatus == ExpenseStatus.Paid && dto.Status == ExpenseStatus.Paid)
            {
                if (oldAccountId == dto.BankAccountId && dto.BankAccountId.HasValue)
                {
                    // Mesma conta - ajustar diferença
                    var account = await _context.BankAccounts.FindAsync(dto.BankAccountId.Value);
                    if (account != null)
                    {
                        account.CurrentBalance = account.CurrentBalance + oldAmount - dto.Amount;
                        account.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Contas diferentes
                    if (oldAccountId.HasValue)
                    {
                        var oldAccount = await _context.BankAccounts.FindAsync(oldAccountId.Value);
                        if (oldAccount != null)
                        {
                            oldAccount.CurrentBalance += oldAmount;
                            oldAccount.UpdatedAt = DateTime.UtcNow;
                        }
                    }

                    if (dto.BankAccountId.HasValue)
                    {
                        var newAccount = await _context.BankAccounts.FindAsync(dto.BankAccountId.Value);
                        if (newAccount != null)
                        {
                            newAccount.CurrentBalance -= dto.Amount;
                            newAccount.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }
            }
        }
        // Se mudou de débito/dinheiro pago para crédito, devolver o saldo
        else if (oldPaymentMethod != PaymentMethod.Credit && dto.PaymentMethod == PaymentMethod.Credit)
        {
            if (oldStatus == ExpenseStatus.Paid && oldAccountId.HasValue)
            {
                var account = await _context.BankAccounts.FindAsync(oldAccountId.Value);
                if (account != null)
                {
                    account.CurrentBalance += oldAmount;
                    account.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        // Se mudou de crédito para débito/dinheiro pago, debitar do saldo
        else if (oldPaymentMethod == PaymentMethod.Credit && dto.PaymentMethod != PaymentMethod.Credit)
        {
            if (dto.Status == ExpenseStatus.Paid && dto.BankAccountId.HasValue)
            {
                var account = await _context.BankAccounts.FindAsync(dto.BankAccountId.Value);
                if (account != null)
                {
                    account.CurrentBalance -= dto.Amount;
                    account.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return false;

        // Se era cartão de crédito, remover transação
        if (expense.PaymentMethod == PaymentMethod.Credit && expense.CreditCardId.HasValue)
        {
            var transaction = await _context.CreditCardTransactions
                .FirstOrDefaultAsync(t => t.Description == expense.Title && 
                                         t.CreditCardId == expense.CreditCardId.Value &&
                                         t.Amount == expense.Amount &&
                                         t.TransactionDate == expense.ExpenseDate);
            if (transaction != null)
            {
                _context.CreditCardTransactions.Remove(transaction);
            }
        }
        // Reverter saldo se estava pago e não era crédito
        else if (expense.Status == ExpenseStatus.Paid && 
                 expense.PaymentMethod != PaymentMethod.Credit && 
                 expense.BankAccountId.HasValue)
        {
            var account = await _context.BankAccounts.FindAsync(expense.BankAccountId.Value);
            if (account != null)
            {
                account.CurrentBalance += expense.Amount;
                account.UpdatedAt = DateTime.UtcNow;
            }
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
        return true;
    }
}
