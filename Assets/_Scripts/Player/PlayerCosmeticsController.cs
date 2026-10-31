using BaskgayBall.Input;
using UnityEngine;

public class PlayerCosmeticsController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer armRenderer;
    [SerializeField] private SpriteRenderer headRenderer;

    private void Start()
    {
        ISided sided = GetComponent<ISided>();
        SkinData skin = GameManager.Instance.CosmeticsMng.GetPlayerSkinData(sided.Side);
        SetSkin(skin);
    }

    private void SetSkin(SkinData skinData)
    {
        bodyRenderer.sprite = skinData.bodySprite;
        armRenderer.sprite = skinData.handSprite;
        headRenderer.sprite = skinData.headSprite;
    }
}
