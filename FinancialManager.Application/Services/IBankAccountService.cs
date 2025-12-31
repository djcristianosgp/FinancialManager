using FinancialManager.Application.Dtos;

namespace FinancialManager.Application.Services;

public interface IBankAccountService
{
    Task<List<BankAccountListItem>> GetAllAsync();
    Task<BankAccountDetailDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(BankAccountCreateDto dto);
    Task<bool> UpdateAsync(BankAccountUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
