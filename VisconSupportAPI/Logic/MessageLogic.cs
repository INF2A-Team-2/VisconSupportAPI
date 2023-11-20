using VisconSupportAPI.Controllers;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Logic;

public static class MessageLogic
{
    static readonly DatabaseContext Context;
    public static List<RetourMessage>? getMessages(User? user, int issueId)
    {
        if (user == null)
            return null;

        var retour = new List<RetourMessage>();
        var messages = Context.Messages.Where(h => h.IssueId == issueId).ToList();
        foreach (var message in messages)
        {
            retour.Add(new RetourMessage
            {
                ID = message.Id,
                Name = Context.Users.First(h => h.Id == message.UserId).Username,
                Body = message.Body,
                Timestamp = message.TimeStamp,
                UserID = message.UserId
            });
        }

        return retour;
    }
}