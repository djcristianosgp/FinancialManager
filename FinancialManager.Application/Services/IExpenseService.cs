using FinancialManager.Application.Dtos;

namespace FinancialManager.Application.Services;

public interface IExpenseService
{
    Task<List<ExpenseListItem>> GetAllAsync();
    Task<ExpenseUpdateDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(ExpenseCreateDto dto);
    Task<bool> UpdateAsync(ExpenseUpdateDto dto);
    Task<bool> DeleteAsync(Guid id);
}
