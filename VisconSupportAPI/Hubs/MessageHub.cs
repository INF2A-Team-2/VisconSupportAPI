using Microsoft.AspNetCore.SignalR;
using VisconSupportAPI.Data;

namespace VisconSupportAPI.Hubs;

public class MessageHub : Hub
{
    static readonly DatabaseContext DataContext;
    public async Task SendMessage(string issueId, string message)
    {
        await Clients.Group(issueId).SendAsync("message", message);
    }
    public async Task AddToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
    }

    public Task RemoveFromGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
