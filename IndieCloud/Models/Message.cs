namespace IndieCloud.Models;

public class Message
{
    public required string Chat { get; set; }
    public string Device { get; set; }
    public DateTime TimeStamp { get; set; }
}
