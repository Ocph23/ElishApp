﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ApsMobileApp.Models;

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

   

    public Profile Karyawan { get; set; }

}
