using FinancialManager.Application.Dtos;

namespace FinancialManager.Application.Services;

public interface ICreditCardService
{
    Task<List<CreditCardListItem>> GetAllAsync();
    Task<CreditCardDetailDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreditCardCreateDto dto);
    Task<bool> UpdateAsync(CreditCardUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<Guid> AddTransactionAsync(CreditCardTransactionCreateDto dto);
}
