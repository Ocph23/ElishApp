using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class UnitValidator : AbstractValidator<Unit>
    {
        public UnitValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nama Tidak Boleh Kosong.");
            RuleFor(x => x.Quantity)
                .NotEqual(0)
                .WithMessage("Quantity/Jumlah per unit harus lebih besar dari 0");
            RuleFor(x => x.Buy)
                .NotEqual(0)
                .WithMessage("Harga beli harus lebih besar dari 0");
            RuleFor(x => x.Sell)
               .NotEqual(0)
               .WithMessage("Harga jual harus lebih besar dari 0")
               .GreaterThan(x => x.Buy).WithMessage("Harga jual harus lebih besar dari harga beli");
        }
    }

}
