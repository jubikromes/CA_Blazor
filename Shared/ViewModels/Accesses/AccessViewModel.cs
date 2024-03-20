namespace Shared.ViewModels.Accesses;
public class AccessViewModel
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? CreatedUtc { get; set; }
    public Guid CompanyId { get;set; } 
    public string? CompanyName { get; set;}
}
