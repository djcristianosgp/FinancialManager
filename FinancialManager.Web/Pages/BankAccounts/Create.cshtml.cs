using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.BankAccounts;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IBankAccountService _bankAccountService;

    public CreateModel(IBankAccountService bankAccountService)
    {
        _bankAccountService = bankAccountService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Nome do banco é obrigatório")]
        public string Bank { get; set; } = string.Empty;

        [Required]
        public AccountType Type { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Saldo inicial não pode ser negativo")]
        public decimal InitialBalance { get; set; } = 0;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dto = new BankAccountCreateDto(
            Input.Bank,
            Input.Type,
            Input.InitialBalance
        );

        await _bankAccountService.CreateAsync(dto);
        TempData["Success"] = "Conta criada com sucesso!";
        return RedirectToPage("./Index");
    }
}
