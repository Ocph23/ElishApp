using System;
using System.Collections.Generic;
using System.Text;

namespace ElishAppMobile.Models
{
    public class Profile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string ContactName { get; set; }

        public string Telepon { get; set; }

        public string NPWP { get; set; }

        public string Address { get; set; }
        public int UserId { get; set; }

        public string Location { get; set; }

        public Tuple<double, double> LocationView => GetLocationView();

        private Tuple<double, double> GetLocationView()
        {
            if (string.IsNullOrEmpty(Location))
                return null;
            else
            {
                var datas = Location.Split(',');
                return Tuple.Create(Convert.ToDouble(datas[0]), Convert.ToDouble(datas[1]));
            }
        }

        public Profile Karyawan { get; set; }

    }
}
