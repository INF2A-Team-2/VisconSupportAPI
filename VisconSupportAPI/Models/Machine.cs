public class Machine{
    public long Id{ get; set; }
    public string Name{ get; set; }
    public long UserId{ get; set; }


    public Machine(long id, string name,long userId)
    {
        Id = id;
        Name = name;
        UserId = userId;
    }
}