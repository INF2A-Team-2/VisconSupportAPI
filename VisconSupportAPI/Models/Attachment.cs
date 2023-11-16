public class Attachment
{
    public long Id { get; set; }
    public long IssueId { get; set; }
    public string MimeType { get; set; }
    
    public List<FileChunk> Chunks { get; set; }
}