using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinancialManager.Web.Pages.BankAccounts;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IBankAccountService _bankAccountService;

    public IndexModel(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;
    }

    public List<BankAccountListItem> BankAccounts { get; set; } = new();

    public async Task OnGetAsync()
    {
        BankAccounts = await _bankAccountService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var success = await _bankAccountService.DeleteAsync(id);
        if (success)
        {
            TempData["Success"] = "Conta excluída com sucesso!";
        }
        else
        {
            TempData["Error"] = "Não é possível excluir conta com movimentações.";
        }

        return RedirectToPage();
    }
}
