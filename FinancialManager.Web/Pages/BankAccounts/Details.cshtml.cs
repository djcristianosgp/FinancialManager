using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;

namespace FinancialManager.Web.Pages.BankAccounts;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IBankAccountService _bankAccountService;

    public DetailsModel(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;
    }

    public BankAccountDetailDto? Account { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        Account = await _bankAccountService.GetByIdAsync(id);
        
        if (Account == null)
        {
            return NotFound();
        }

        return Page();
    }
}
