namespace BaskgayBall.Input
{
    public interface ISided
    {
        PlayerSide Side { get; }
    }
    public enum PlayerSide
    {
        Player1,
        Player2
    }
}
