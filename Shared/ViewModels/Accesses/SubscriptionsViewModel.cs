namespace Shared.ViewModels.Accesses;
public class SubscriptionsViewModel
{
    public Guid Id { get; set; }
    public string? SubscriptionName { get; set; } //eg.. single access, couple access, family access (of 4)
    public string? Description { get; set; }
    public Duration Duration { get; set; }
    public Guid AccessId { get; set; }
    public bool Activated { get;set; }
    //might not be needed
}

public enum Duration
{
    Daily, Weekly, Monthly, Quarterly, Tri, BiAnnual, Yearly
}

//This can be part of the  
public enum AccessType
{
    Single_Access, Couple_Access, Family_Access
}
