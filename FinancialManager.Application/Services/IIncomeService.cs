using FinancialManager.Application.Dtos;

namespace FinancialManager.Application.Services;

public interface IIncomeService
{
    Task<List<IncomeListItem>> GetAllAsync();
    Task<IncomeUpdateDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(IncomeCreateDto dto);
    Task<bool> UpdateAsync(IncomeUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
