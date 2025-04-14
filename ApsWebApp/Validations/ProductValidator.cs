using FluentValidation;
using ShareModels;

namespace ApsWebApp.Validations
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Nama Tidak Boleh Kosong.");

            RuleFor(x => x.Supplier)
               .NotEmpty()
               .WithMessage("Supplier Tidak Boleh Kosong.");
            
            RuleFor(x => x.Category)
               .NotEmpty()
               .WithMessage("Kategori Tidak Boleh Kosong.");
            
            RuleFor(x => x.CodeName)
                .NotEmpty()
                .WithMessage("Kode Produk Tidak Boleh Kosong."); 

            RuleFor(x => x.Merk)
                .NotEmpty()
                .WithMessage("Merek Tidak Boleh Kosong."); 

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nama Tidak Boleh Kosong."); 
          
        }
    }



}
