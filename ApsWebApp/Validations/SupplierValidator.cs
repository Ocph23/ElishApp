using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class SupplierValidator : AbstractValidator<Supplier>
    {
        public SupplierValidator()
        {
            RuleFor(x => x.Nama)
                .NotEmpty()
                .WithMessage("Nama Tidak Boleh Kosong.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email tidak boleh kosong.");

            RuleFor(x => x.Telepon)
                .NotEmpty()
                .WithMessage("Telepon tidak boleh kosong.");
            RuleFor(x => x.ContactPersonName)
               .NotEmpty()
               .WithMessage("Nama yang dihubungi tidak boleh kosong.");

            RuleFor(x => x.ContactPerson)
              .NotEmpty()
              .WithMessage("Nomor HP/Telp yang dihubungi tidak boleh kosong.");


            RuleFor(x => x.Address)
              .NotEmpty()
              .WithMessage("Alamat tidak boleh kosong.");

            RuleFor(x => x.Telepon)
             .NotEmpty()
             .WithMessage("Telepon Alamat tidak boleh kosong.");
        }
    }

}
