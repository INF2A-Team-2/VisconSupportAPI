public class Issue
{


    public string Department {get; set;}
    public string HeadLine {get; set;}
    public string Descprition{get; set;}

    public Issue(string dep, string head, string desc)
    {
        Department = dep;
        HeadLine = head;
        Descprition = desc;
    }
}