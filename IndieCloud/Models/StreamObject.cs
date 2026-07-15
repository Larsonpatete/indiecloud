namespace IndieCloud.Models;

public class StreamObject
{
  public Guid Id { get; set; }
  public StreamDataType Type { get; set; }
  public string? TextContent { get; set; }
  public string? FileName { get; set; }
  public DateTime TimeStamp { get; set; }
  public string Device { get; set; }
}

public enum StreamDataType {
  Text = 0, // 0
  Image = 1,  // 1
  Audio = 2,  // 2
}
