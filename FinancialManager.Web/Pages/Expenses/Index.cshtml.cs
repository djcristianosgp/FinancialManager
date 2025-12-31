using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinancialManager.Web.Pages.Expenses;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public IndexModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public List<ExpenseListItem> Expenses { get; set; } = new();

    public async Task OnGetAsync()
    {
        Expenses = await _expenseService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var success = await _expenseService.DeleteAsync(id);
        if (success)
        {
            TempData["Success"] = "Despesa excluída com sucesso!";
        }
        else
        {
            TempData["Error"] = "Erro ao excluir despesa.";
        }

        return RedirectToPage();
    }
}
