using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
 namespace ShareModels 
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        public string Merk { get; set; }

        public string Name { get; set; }

        public string CodeName { get; set; }

        public string CodeArticle { get; set; }

        public string Size { get; set; }
        public string Description { get; set; }


        public virtual Category Category { get; set; }

        public virtual ICollection<Unit> Units { get; set; }
    }
}


