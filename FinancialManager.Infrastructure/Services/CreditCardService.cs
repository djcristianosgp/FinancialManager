using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Services;

public class CreditCardService : ICreditCardService
{
    private readonly ApplicationDbContext _context;

    public CreditCardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CreditCardListItem>> GetAllAsync()
    {
        var cards = await _context.CreditCards
            .Include(c => c.Transactions)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return cards.Select(c => new CreditCardListItem(
            c.Id,
            c.Name,
            c.Bank,
            c.Limit,
            c.Limit - c.Transactions.Sum(t => t.RemainingAmount),
            c.ClosingDay,
            c.DueDay
        )).ToList();
    }

    public async Task<CreditCardDetailDto?> GetByIdAsync(Guid id)
    {
        var card = await _context.CreditCards
            .Include(c => c.Transactions.OrderByDescending(t => t.TransactionDate).Take(20))
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null) return null;

        var usedLimit = card.Transactions.Sum(t => t.RemainingAmount);
        var transactions = card.Transactions
            .Select(t => new CreditCardTransactionItem(
                t.Id,
                t.TransactionDate,
                t.Description,
                t.Amount,
                t.Category,
                t.Installments,
                t.CurrentInstallment,
                t.InstallmentAmount,
                t.RemainingAmount
            ))
            .ToList();

        return new CreditCardDetailDto(
            card.Id,
            card.Name,
            card.Bank,
            card.Limit,
            card.Limit - usedLimit,
            card.ClosingDay,
            card.DueDay,
            usedLimit,
            transactions
        );
    }

    public async Task<Guid> CreateAsync(CreditCardCreateDto dto)
    {
        var card = new CreditCard
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Bank = dto.Bank,
            Limit = dto.Limit,
            ClosingDay = dto.ClosingDay,
            DueDay = dto.DueDay,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CreditCards.Add(card);
        await _context.SaveChangesAsync();
        return card.Id;
    }

    public async Task<bool> UpdateAsync(CreditCardUpdateDto dto)
    {
        var card = await _context.CreditCards.FindAsync(dto.Id);
        if (card == null) return false;

        card.Name = dto.Name;
        card.Bank = dto.Bank;
        card.Limit = dto.Limit;
        card.ClosingDay = dto.ClosingDay;
        card.DueDay = dto.DueDay;
        card.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var card = await _context.CreditCards
            .Include(c => c.Transactions)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (card == null) return false;

        // Verificar se tem transações pendentes
        if (card.Transactions.Any(t => t.RemainingAmount > 0))
        {
            return false; // Não permitir exclusão se houver parcelas pendentes
        }

        _context.CreditCards.Remove(card);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Guid> AddTransactionAsync(CreditCardTransactionCreateDto dto)
    {
        var transaction = new CreditCardTransaction
        {
            Id = Guid.NewGuid(),
            CreditCardId = dto.CreditCardId,
            Description = dto.Description,
            Amount = dto.TotalAmount,
            TransactionDate = dto.Date,
            Category = dto.Category,
            Installments = dto.Installments,
            CurrentInstallment = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.CreditCardTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return transaction.Id;
    }
}
