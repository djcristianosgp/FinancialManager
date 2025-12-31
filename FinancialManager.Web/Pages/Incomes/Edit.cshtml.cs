using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.Incomes;

[Authorize]
public class EditModel : PageModel
{
    private readonly IIncomeService _incomeService;
    private readonly IBankAccountService _bankAccountService;

    public EditModel(IIncomeService incomeService, IBankAccountService bankAccountService)
    {
        _incomeService = incomeService;
        _bankAccountService = bankAccountService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<BankAccountListItem> BankAccounts { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var income = await _incomeService.GetByIdAsync(id);
        if (income == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = income.Id,
            Description = income.Description,
            Amount = income.Amount,
            Date = income.Date,
            Category = income.Category,
            BankAccountId = income.BankAccountId,
            Recurrence = income.Recurrence
        };

        BankAccounts = await _bankAccountService.GetAllAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            BankAccounts = await _bankAccountService.GetAllAsync();
            return Page();
        }

        var dto = new IncomeUpdateDto(
            Input.Id,
            Input.Description,
            Input.Amount,
            Input.Date,
            Input.Category,
            Input.BankAccountId,
            Input.Recurrence
        );

        await _incomeService.UpdateAsync(dto);

        TempData["Success"] = "Receita atualizada com sucesso!";
        return RedirectToPage("/Incomes/Index");
    }

    public class InputModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "A conta bancária é obrigatória")]
        public Guid BankAccountId { get; set; }

        public RecurrenceType Recurrence { get; set; }
    }
}
