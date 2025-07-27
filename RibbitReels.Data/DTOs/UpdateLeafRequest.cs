namespace RibbitReels.Data.DTOs;

public class UpdateLeafRequest
{
    public string Title { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public int Order { get; set; }
}
