public class Issue
{

    public long Id {get; set;}
    public string Department {get; set;}
    public string Headline {get; set;}
    public string Description {get; set;}

    public Issue(long id, string dep, string head, string desc)
    {
        Id = id;
        Department = dep;
        Headline = head;
        Description = desc;
    }
}