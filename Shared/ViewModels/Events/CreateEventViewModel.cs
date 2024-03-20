namespace Shared.ViewModels.Events;
public class CreateEventViewModel
{
    public string? Title { get; set; }
    public string? ShortDescription { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool Paid { get; set; }
    public string? StartTime { get; set; }
    public int? Duration { get; set; }
}
