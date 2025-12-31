using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinancialManager.Web.Pages.Incomes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IIncomeService _incomeService;

    public IndexModel(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    public List<IncomeListItem> Incomes { get; set; } = new();

    public async Task OnGetAsync()
    {
        Incomes = await _incomeService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var success = await _incomeService.DeleteAsync(id);
        if (success)
        {
            TempData["Success"] = "Receita excluída com sucesso!";
        }
        else
        {
            TempData["Error"] = "Erro ao excluir receita.";
        }

        return RedirectToPage();
    }
}
