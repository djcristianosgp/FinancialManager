using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.CreditCards;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ICreditCardService _creditCardService;

    public CreateModel(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dto = new CreditCardCreateDto(
            Input.Name,
            Input.Bank,
            Input.Limit,
            Input.ClosingDay,
            Input.DueDay
        );

        await _creditCardService.CreateAsync(dto);

        TempData["Success"] = "Cartão de crédito criado com sucesso!";
        return RedirectToPage("/CreditCards/Index");
    }

    public class InputModel
    {
        [Required(ErrorMessage = "O nome do cartão é obrigatório")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "O banco é obrigatório")]
        public string Bank { get; set; } = string.Empty;

        [Required(ErrorMessage = "O limite é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O limite deve ser maior que zero")]
        public decimal Limit { get; set; }

        [Required(ErrorMessage = "O dia de fechamento é obrigatório")]
        [Range(1, 31, ErrorMessage = "O dia de fechamento deve estar entre 1 e 31")]
        public int ClosingDay { get; set; }

        [Required(ErrorMessage = "O dia de vencimento é obrigatório")]
        [Range(1, 31, ErrorMessage = "O dia de vencimento deve estar entre 1 e 31")]
        public int DueDay { get; set; }
    }
}
