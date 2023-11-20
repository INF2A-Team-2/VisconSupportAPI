using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Hubs;

public class MessageHub : Hub
{
    static readonly DatabaseContext DataContext;
    public async Task SendMessage(string issueId, string message)
    {
        string? userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return;
        }
        await Clients.Group(issueId).SendAsync("message", message);
    }
    public async Task AddToGroup(string groupName)
    {
        string? userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null) return;
        User? user = DataContext.Users.FirstOrDefault(h => h.Username == userId);
        if (user == null) return;
        if(user.Type is AccountType.Admin or AccountType.Helpdesk || DataContext.Issues.FirstOrDefault(h => h.UserId == user.Id) != null)
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        await Clients.Group(groupName).SendAsync("Send", $"{Context.ConnectionId} has joined the group {groupName}.");
    }

    public Task RemoveFromGroup(string groupName)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}
