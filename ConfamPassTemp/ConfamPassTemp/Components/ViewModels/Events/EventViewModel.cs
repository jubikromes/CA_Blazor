using Microsoft.AspNetCore.Components;

namespace ConfamPassTemp.Components.ViewModels.Events
{
    public class EventViewModel
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Creator { get; set; }
        public string? EventDate { get; set; }
        public string? EventImage { get; set; }
        public string? CreatorImage { get; set; }
    }
}
