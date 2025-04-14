using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nama Tidak Boleh Kosong.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Email tidak boleh kosong.");

            RuleFor(x => x.Telepon)
                .NotEmpty()
                .WithMessage("Telepon tidak boleh kosong.");

            RuleFor(x => x.ContactName)
                .NotEmpty()
                .WithMessage("Nama yang dighubungi tidak boleh kosong.");

            RuleFor(x => x.Address)
              .NotEmpty()
              .WithMessage("Alamat tidak boleh kosong.");

            RuleFor(x => x.Telepon)
             .NotEmpty()
             .WithMessage("Telepon Alamat tidak boleh kosong.");
        }
    }



}
