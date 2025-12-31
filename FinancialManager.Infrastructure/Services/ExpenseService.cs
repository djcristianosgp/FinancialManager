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
            expense.CreditCardId
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);

        // Atualizar saldo se for débito/dinheiro e já pago
        if (dto.Status == ExpenseStatus.Paid && dto.PaymentMethod != PaymentMethod.Credit && dto.BankAccountId.HasValue)
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

    public async Task<bool> UpdateAsync(ExpenseUpdateDto dto)
    {
        var expense = await _context.Expenses.FindAsync(dto.Id);
        if (expense == null) return false;

        var oldAmount = expense.Amount;
        var oldStatus = expense.Status;
        var oldAccountId = expense.BankAccountId;
        var oldPaymentMethod = expense.PaymentMethod;

        expense.Title = dto.Description;
        expense.Amount = dto.Amount;
        expense.ExpenseDate = dto.Date;
        expense.Category = dto.Category;
        expense.PaymentMethod = dto.PaymentMethod;
        expense.Status = dto.Status;
        expense.BankAccountId = dto.BankAccountId;
        expense.CreditCardId = dto.CreditCardId;
        expense.UpdatedAt = DateTime.UtcNow;

        // Ajustar saldos apenas para débito/dinheiro
        if (oldPaymentMethod != PaymentMethod.Credit && dto.PaymentMethod != PaymentMethod.Credit)
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

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return false;

        // Reverter saldo se estava pago e não era crédito
        if (expense.Status == ExpenseStatus.Paid && 
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
