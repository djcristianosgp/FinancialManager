using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Services;

public class IncomeService : IIncomeService
{
    private readonly ApplicationDbContext _context;

    public IncomeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IncomeListItem>> GetAllAsync()
    {
        return await _context.Incomes
            .Include(i => i.BankAccount)
            .OrderByDescending(i => i.ReceivedDate)
            .Select(i => new IncomeListItem(
                i.Id,
                i.Title,
                i.Amount,
                i.ReceivedDate,
                i.Category,
                i.BankAccount != null ? i.BankAccount.Bank : "N/A",
                i.Recurrence
            ))
            .ToListAsync();
    }

    public async Task<IncomeUpdateDto?> GetByIdAsync(Guid id)
    {
        var income = await _context.Incomes.FindAsync(id);
        if (income == null) return null;

        return new IncomeUpdateDto(
            income.Id,
            income.Title,
            income.Amount,
            income.ReceivedDate,
            income.Category,
            income.BankAccountId,
            income.Recurrence
        );
    }

    public async Task<Guid> CreateAsync(IncomeCreateDto dto)
    {
        var income = new Income
        {
            Id = Guid.NewGuid(),
            Title = dto.Description,
            Amount = dto.Amount,
            ReceivedDate = dto.Date,
            Category = dto.Category,
            BankAccountId = dto.BankAccountId,
            Recurrence = dto.Recurrence,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Incomes.Add(income);

        // Atualizar saldo da conta
        var account = await _context.BankAccounts.FindAsync(dto.BankAccountId);
        if (account != null)
        {
            account.CurrentBalance += dto.Amount;
            account.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return income.Id;
    }

    public async Task<bool> UpdateAsync(IncomeUpdateDto dto)
    {
        var income = await _context.Incomes.FindAsync(dto.Id);
        if (income == null) return false;

        var oldAmount = income.Amount;
        var oldAccountId = income.BankAccountId;

        income.Title = dto.Description;
        income.Amount = dto.Amount;
        income.ReceivedDate = dto.Date;
        income.Category = dto.Category;
        income.BankAccountId = dto.BankAccountId;
        income.Recurrence = dto.Recurrence;
        income.UpdatedAt = DateTime.UtcNow;

        // Ajustar saldos das contas
        if (oldAccountId == dto.BankAccountId)
        {
            // Mesma conta - ajustar diferença
            var account = await _context.BankAccounts.FindAsync(dto.BankAccountId);
            if (account != null)
            {
                account.CurrentBalance = account.CurrentBalance - oldAmount + dto.Amount;
                account.UpdatedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // Contas diferentes - reverter na antiga e adicionar na nova
            var oldAccount = await _context.BankAccounts.FindAsync(oldAccountId);
            if (oldAccount != null)
            {
                oldAccount.CurrentBalance -= oldAmount;
                oldAccount.UpdatedAt = DateTime.UtcNow;
            }

            var newAccount = await _context.BankAccounts.FindAsync(dto.BankAccountId);
            if (newAccount != null)
            {
                newAccount.CurrentBalance += dto.Amount;
                newAccount.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var income = await _context.Incomes.FindAsync(id);
        if (income == null) return false;

        // Reverter saldo da conta
        var account = await _context.BankAccounts.FindAsync(income.BankAccountId);
        if (account != null)
        {
            account.CurrentBalance -= income.Amount;
            account.UpdatedAt = DateTime.UtcNow;
        }

        _context.Incomes.Remove(income);
        await _context.SaveChangesAsync();
        return true;
    }
}
