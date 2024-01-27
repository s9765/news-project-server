using BL.Models;
using Microsoft.AspNetCore.SignalR;

public class NewsHub : Hub
{
    /// <summary>
    /// Implement SingelR
    /// </summary>
    /// <param name="updatedNews"></param>
    public async Task SendNewsUpdate(IEnumerable<NewItem> updatedNews)
    {
        await Clients.All.SendAsync("ReceiveNewsUpdate", updatedNews);
    }
}


