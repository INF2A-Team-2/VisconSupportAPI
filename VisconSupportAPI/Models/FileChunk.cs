public class FileChunk
{
    public long Id { get; set; }
    public long AttachmentID { get; set; }
    public long ChunkNumber { get; set; }
    public byte[] Data { get; set; }
}