using BaskgayBall.Input;

namespace BaskgayBall.PowerUps
{
    public readonly struct PowerUpContext
    {
        public readonly PowerUpDefinition Definition;
        public readonly EPlayerSide CollectorSide;
        public readonly EPlayerSide TargetSide;

        public PowerUpContext(PowerUpDefinition definition, EPlayerSide collectorSide, EPlayerSide targetSide)
        {
            Definition = definition;
            CollectorSide = collectorSide;
            TargetSide = targetSide;
        }
    }
}