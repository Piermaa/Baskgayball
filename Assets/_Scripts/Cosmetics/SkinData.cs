using UnityEngine;

[CreateAssetMenu(menuName = "Skin Data")]
public class SkinData : ScriptableObject
{
    public int cost;
    public string displayName;
    public Sprite headSprite;
    public Sprite bodySprite;
    public Sprite handSprite;
}
