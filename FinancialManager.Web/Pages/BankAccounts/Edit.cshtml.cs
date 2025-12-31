using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.BankAccounts;

[Authorize]
public class EditModel : PageModel
{
    private readonly IBankAccountService _bankAccountService;

    public EditModel(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var account = await _bankAccountService.GetByIdAsync(id);
        if (account == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = account.Id,
            Bank = account.Bank,
            Type = account.Type
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dto = new BankAccountUpdateDto(
            Input.Id,
            Input.Bank,
            Input.Type
        );

        await _bankAccountService.UpdateAsync(dto);

        TempData["Success"] = "Conta bancária atualizada com sucesso!";
        return RedirectToPage("/BankAccounts/Index");
    }

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome do banco é obrigatório")]
        public string Bank { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de conta é obrigatório")]
        public AccountType Type { get; set; }
    }
}
