using VisconSupportAPI.Types;

namespace VisconSupportAPI.Tests;

[Collection("IssueTests")]
public class IssueTests : ServiceTest
{
    public IssueTests() : base()
    {}

    public Issue CreateTestIssue()
    {
        return new Issue()
        {
            Actual = "My machine is broken",
            Attachments = new List<Attachment>(),
            Expected = "My machine is working",
            Headline = "Machine broken",
            MachineId = 1,
            Priority = Priority.High,
            Status = Status.Open,
        };
    }
}