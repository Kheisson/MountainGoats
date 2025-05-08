namespace Stats
{
    public class PlayerStatsReadonly : IPlayerStats
    {
        public PlayerStatsReadonly(float ropeLength, float horizontalSpeed, float lightIntensity, float energyCapacity)
        {
            RopeLength = ropeLength;
            HorizontalSpeed = horizontalSpeed;
            LightIntensity = lightIntensity;
            EnergyCapacity = energyCapacity;
        }

        public float RopeLength { get; }
        public float HorizontalSpeed { get; }
        public float LightIntensity { get; }
        public float EnergyCapacity { get; }
    }
}