﻿using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IIncommingService
    {                                                      
        Task<PembelianModel> CreateNew(int pembelianid);
        Task<PembelianModel> Load(bool force = false);
        void LoadPembelian();
        Task Invoike(IncomingItem data);
        Task Save();
        ObservableCollection<PembelianDataModel> Pembelians { get; }
        ObservableCollection<IncomingItem> Datas { get;  }
        Pembelian PembelianSelected { get; set; }

    }
}
