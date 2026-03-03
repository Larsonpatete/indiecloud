namespace IndieCloud.Models;

public class Message
{
    public int Id { get; set; }
    public required string Chat { get; set; }
    public required string Device { get; set; }
    public DateTime TimeStamp { get; set; }
}
