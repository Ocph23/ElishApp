using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class MerkValidator : AbstractValidator<Merk>
    {
        public MerkValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nama Tidak Boleh Kosong.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Deskripsikan Merek.");
        }
    }
  
}
