using Microsoft.AspNetCore.SignalR;

namespace GestioneOrdini.Hubs
{
    public class OrderHub : Hub
    {
        // Questo metodo può essere chiamato dai client per inviare messaggi a tutti i connessi
        public async Task SendOrderUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveOrderUpdate", message);
        }
    }

}
