using BaskgayBall.Input;
using UnityEngine;

[CreateAssetMenu(menuName = "Skin Database")]
public class SkinDataBase : ScriptableObject
{
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;
    public SkinData[] skins;

    public Color GetPlayerColor(EPlayerSide player)
    {
        switch (player)
        {
            case EPlayerSide.Player1:
                return player1Color;
            case EPlayerSide.Player2:
                return player2Color;
        }
        return Color.white;
    }
}
