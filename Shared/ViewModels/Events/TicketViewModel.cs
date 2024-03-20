namespace Shared.ViewModels.Events;
public class TicketViewModel
{
    public int Id { get; set; }
    public string? TicketName { get; set; }  
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public Guid EventId { get; set; }
    public int NoOfTickets { get; set; }
    public string? TicketType { get; set; }
}
