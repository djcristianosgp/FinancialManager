using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FinancialManager.Web.Pages.CreditCards;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICreditCardService _creditCardService;

    public IndexModel(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    public List<CreditCardListItem> CreditCards { get; set; } = new();

    public async Task OnGetAsync()
    {
        CreditCards = await _creditCardService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var success = await _creditCardService.DeleteAsync(id);
        if (success)
        {
            TempData["Success"] = "Cartão excluído com sucesso!";
        }
        else
        {
            TempData["Error"] = "Não é possível excluir cartão com parcelas pendentes.";
        }

        return RedirectToPage();
    }
}
