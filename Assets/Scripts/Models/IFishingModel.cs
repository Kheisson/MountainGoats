namespace Models
{
    public interface IFishingModel
    {
        float RopeLength { get; }
        float HorizontalSpeed { get; }
        float LightIntensity { get; }
        float EnergyCapacity { get; }
    }
}