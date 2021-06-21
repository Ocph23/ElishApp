using Microsoft.AspNetCore.SignalR;
using ShareModels;
using ShareModels.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebClient.Services;

namespace WebClient
{
    public class ElishAppHub: Hub
    {
        private IncommingService _incomingService;

        public ElishAppHub(IncommingService incomingService)
        {
            _incomingService = incomingService;
        }
        public Task SendNewProduct(string user, string message)
        {
            return Clients.All.SendAsync("ReceiveNewProduct", user, message);
        }

        public Task UpdateIncomingItem(IncomingItem model)
        {
            var item = _incomingService.Datas.Where(x => x.Product.Id == model.Product.Id).FirstOrDefault();
            if (item != null && item.ActualValue != model.ActualValue)
            {
                item.ActualValue= model.ActualValue;
                return Clients.Others.SendAsync("RecieveIncomingItem", model);
            }
            return Task.CompletedTask;
        }
    }
}
