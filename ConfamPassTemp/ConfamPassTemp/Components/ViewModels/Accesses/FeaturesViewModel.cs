namespace ConfamPassTemp.Components.ViewModels.Accesses
{
    public class FeaturesViewModel
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; } 
        public FeatureType FeatureType { get; set; }
    }

    public enum FeatureType
    {
        Service, Facility
    }
}
