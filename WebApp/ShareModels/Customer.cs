using System; 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public class Customer :BaseNotify, IEntity
    {
       
        [Key]
        public int Id { get => _id; set => SetProperty(ref _id, value); }

        public string Name { get => _name; set => SetProperty(ref _name, value); }

        public string Email { get => _email; set => SetProperty(ref _email, value); }

        public string ContactName { get => _contactName; set => SetProperty(ref _contactName, value); }

        public string Telepon { get => _telp; set => SetProperty(ref _telp, value); }

        public string NPWP { get => _npwp; set => SetProperty(ref _npwp, value); }

        public string Address { get => _address; set => SetProperty(ref _address, value); }

       // public int KaryawanId { get => _karyawanId; set => SetProperty(ref _karyawanId, value); }
        //public int UserId { get => _userid; set => SetProperty(ref _userid, value); }

        public string Location { get => _location; set => SetProperty(ref _location, value); }


        private string _name;
        //private int _userid;
        private string _address;
        private string _npwp;
        private string _telp;
        private string _contactName;
        private string _email;
        private int _id;
        private string _location;

        public virtual User User { get; set; }
        public virtual Karyawan Karyawan{ get; set; }

    }
}
