using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FinancialManager.Application.Dtos;
using FinancialManager.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace FinancialManager.Web.Pages.CreditCards;

[Authorize]
public class EditModel : PageModel
{
    private readonly ICreditCardService _creditCardService;

    public EditModel(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var card = await _creditCardService.GetByIdAsync(id);
        if (card == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            Id = card.Id,
            Name = card.Name,
            Bank = card.Bank,
            Limit = card.Limit,
            ClosingDay = card.ClosingDay,
            DueDay = card.DueDay
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dto = new CreditCardUpdateDto(
            Input.Id,
            Input.Name,
            Input.Bank,
            Input.Limit,
            Input.ClosingDay,
            Input.DueDay
        );

        await _creditCardService.UpdateAsync(dto);

        TempData["Success"] = "Cartão de crédito atualizado com sucesso!";
        return RedirectToPage("/CreditCards/Index");
    }

    public class InputModel
    {
        public Guid Id { get; set; }

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
