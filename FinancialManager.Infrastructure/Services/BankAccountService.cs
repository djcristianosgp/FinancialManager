using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Services;

public class BankAccountService : IBankAccountService
{
    private readonly ApplicationDbContext _context;

    public BankAccountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BankAccountListItem>> GetAllAsync()
    {
        return await _context.BankAccounts
            .OrderBy(b => b.Bank)
            .Select(b => new BankAccountListItem(
                b.Id,
                b.Bank,
                b.Type,
                b.CurrentBalance
            ))
            .ToListAsync();
    }

    public async Task<BankAccountDetailDto?> GetByIdAsync(Guid id)
    {
        var account = await _context.BankAccounts
            .Include(b => b.Transactions.OrderByDescending(t => t.TransactionDate).Take(10))
            .FirstOrDefaultAsync(b => b.Id == id);

        if (account == null) return null;

        var transactions = account.Transactions
            .Select(t => new TransactionItem(
                t.Id,
                t.TransactionDate,
                t.Description,
                t.Amount,
                t.Type
            ))
            .ToList();

        return new BankAccountDetailDto(
            account.Id,
            account.Bank,
            account.Type,
            account.InitialBalance,
            account.CurrentBalance,
            transactions
        );
    }

    public async Task<Guid> CreateAsync(BankAccountCreateDto dto)
    {
        var account = new BankAccount
        {
            Id = Guid.NewGuid(),
            Bank = dto.Bank,
            Type = dto.Type,
            InitialBalance = dto.InitialBalance,
            CurrentBalance = dto.InitialBalance,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.BankAccounts.Add(account);
        await _context.SaveChangesAsync();
        return account.Id;
    }

    public async Task<bool> UpdateAsync(BankAccountUpdateDto dto)
    {
        var account = await _context.BankAccounts.FindAsync(dto.Id);
        if (account == null) return false;

        account.Bank = dto.Bank;
        account.Type = dto.Type;
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var account = await _context.BankAccounts
            .Include(b => b.Incomes)
            .Include(b => b.Expenses)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (account == null) return false;

        // Verificar se tem movimentações
        if (account.Incomes.Any() || account.Expenses.Any())
        {
            return false; // Não permitir exclusão se houver movimentações
        }

        _context.BankAccounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }
}
