using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class CategoryValidator   : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nama Tidak Boleh Kosong.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Deskripsikan kategori.");
        }
    }

}
