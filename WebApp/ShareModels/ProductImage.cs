using ShareModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ShareModels
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Thumb { get; set; }
        public FileType FileType { get; set; }
        public int ProductId { get; set; }

        [NotMapped]
        public byte[] Buffer { get; set; }

        

    }
}
