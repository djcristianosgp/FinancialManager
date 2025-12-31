using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;

namespace FinancialManager.Web.Pages.CreditCards;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly ICreditCardService _creditCardService;

    public DetailsModel(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    public CreditCardDetailDto? Card { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Card = await _creditCardService.GetByIdAsync(id);
        
        if (Card == null)
        {
            return NotFound();
        }

        return Page();
    }
}
