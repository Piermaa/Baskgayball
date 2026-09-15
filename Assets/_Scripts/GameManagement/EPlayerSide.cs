namespace BaskgayBall.Input
{
    public interface ISided
    {
        EPlayerSide Side { get; }
    }
    public enum EPlayerSide
    {
        Player1,
        Player2
    }
}
