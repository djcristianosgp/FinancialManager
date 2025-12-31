using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.Incomes;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IIncomeService _incomeService;
    private readonly IBankAccountService _bankAccountService;

    public CreateModel(IIncomeService incomeService, IBankAccountService bankAccountService)
    {
        _incomeService = incomeService;
        _bankAccountService = bankAccountService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<BankAccountListItem> BankAccounts { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Conta bancária é obrigatória")]
        public Guid BankAccountId { get; set; }

        public RecurrenceType Recurrence { get; set; }
    }

    public async Task OnGetAsync()
    {
        BankAccounts = await _bankAccountService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            BankAccounts = await _bankAccountService.GetAllAsync();
            return Page();
        }

        var dto = new IncomeCreateDto(
            Input.Description,
            Input.Amount,
            Input.Date,
            Input.Category,
            Input.BankAccountId,
            Input.Recurrence
        );

        await _incomeService.CreateAsync(dto);
        TempData["Success"] = "Receita criada com sucesso!";
        return RedirectToPage("./Index");
    }
}
