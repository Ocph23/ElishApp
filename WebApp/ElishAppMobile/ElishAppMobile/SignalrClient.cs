using Microsoft.AspNetCore.SignalR.Client;
using ShareModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ElishAppMobile
{
    public class SignalrClient
    {
        private readonly HubConnection hubConnection = new HubConnectionBuilder()
         .WithUrl($"{Helper.Url}/elishapp")
         .Build();


        public HubConnection Connection => hubConnection;

        public SignalrClient()
        {
            Task.Run(() => Connect());
        }

        public async Task Connect()
        {
            try
            {

                await hubConnection.StartAsync();
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }
        }




        public async Task Disconnect()
        {
            await hubConnection.StopAsync();
        }


        public async Task UpdateIncomingItem(IncomingItem model)
        {

            try
            {
                if (hubConnection.State == HubConnectionState.Disconnected)
                    await Connect();

                if (hubConnection.State == HubConnectionState.Connecting)
                    await Task.Delay(3000);

                if (hubConnection.State == HubConnectionState.Reconnecting)
                    await Task.Delay(3000);

                if (hubConnection.State == HubConnectionState.Connected)
                {
                    await hubConnection.InvokeAsync("UpdateIncomingItem", model);
                }
                else
                {
                    throw new SystemException($"Data Tidak Terkirim !, {hubConnection.State}");
                }
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }

        }


    }
}
