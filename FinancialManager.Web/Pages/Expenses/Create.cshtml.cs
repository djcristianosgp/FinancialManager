using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using FinancialManager.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.Expenses;

[Authorize]
public class CreateModel : PageModel
{
    private readonly IExpenseService _expenseService;
    private readonly IBankAccountService _bankAccountService;
    private readonly ICreditCardService _creditCardService;

    public CreateModel(
        IExpenseService expenseService,
        IBankAccountService bankAccountService,
        ICreditCardService creditCardService)
    {
        _expenseService = expenseService;
        _bankAccountService = bankAccountService;
        _creditCardService = creditCardService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<BankAccountListItem> BankAccounts { get; set; } = new();
    public List<CreditCardListItem> CreditCards { get; set; } = new();

    public async Task OnGetAsync()
    {
        BankAccounts = await _bankAccountService.GetAllAsync();
        CreditCards = await _creditCardService.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            if (!ModelState.IsValid)
            {
                BankAccounts = await _bankAccountService.GetAllAsync();
                CreditCards = await _creditCardService.GetAllAsync();
                return Page();
            }

            // Validate payment method fields
            if (Input.PaymentMethod == PaymentMethod.Credit && !Input.CreditCardId.HasValue)
            {
                ModelState.AddModelError("Input.CreditCardId", "Selecione um cartão de crédito.");
                BankAccounts = await _bankAccountService.GetAllAsync();
                CreditCards = await _creditCardService.GetAllAsync();
                return Page();
            }

            if ((Input.PaymentMethod == PaymentMethod.Cash || Input.PaymentMethod == PaymentMethod.Debit) 
                && !Input.BankAccountId.HasValue)
            {
                ModelState.AddModelError("Input.BankAccountId", "Selecione uma conta bancária.");
                BankAccounts = await _bankAccountService.GetAllAsync();
                CreditCards = await _creditCardService.GetAllAsync();
                return Page();
            }

            var dto = new ExpenseCreateDto(
                Input.Description,
                Input.Amount,
                Input.Date,
                Input.Category,
                Input.PaymentMethod,
                Input.Status,
                Input.BankAccountId,
                Input.CreditCardId
            );

            await _expenseService.CreateAsync(dto);

            TempData["Success"] = "Despesa criada com sucesso!";
            return RedirectToPage("/Expenses/Index");
        }
        catch (Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException)
        {
            TempData["Error"] = "Sua sessão expirou. Por favor, recarregue a página e tente novamente.";
            return RedirectToPage("/Expenses/Create");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erro ao criar despesa: {ex.Message}");
            BankAccounts = await _bankAccountService.GetAllAsync();
            CreditCards = await _creditCardService.GetAllAsync();
            return Page();
        }
    }

    public class InputModel
    {
        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "A forma de pagamento é obrigatória")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Paid;

        public Guid? BankAccountId { get; set; }

        public Guid? CreditCardId { get; set; }
    }
}
