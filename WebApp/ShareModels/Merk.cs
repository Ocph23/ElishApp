using System.Collections.Generic;

namespace ShareModels
{
    public class Merk :IEntity
   {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}


