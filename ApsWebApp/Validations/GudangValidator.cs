using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class GudangValidator : AbstractValidator<Gudang>
    {
        public GudangValidator()
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
