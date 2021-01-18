using System.Collections.Generic;

namespace ShareModels
{
    public class Category  
   {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}


