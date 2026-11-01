using UnityEngine;
using UnityEngine.UI;

namespace BaskgayBall.PowerUps
{
    public class PowerUpIndicatorItem : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image radialFillImage;
        [SerializeField] private Color positiveColor = Color.green;
        [SerializeField] private Color negativeColor = Color.red;

        private ActivePowerUp _active;

        public void Bind(ActivePowerUp active)
        {
            _active = active;
            iconImage.sprite = active.Definition.Icon;

            EPowerUpPolarity polarity = active.Definition.Polarity;

            polarity = active.Definition.Target == EPowerUpTarget.Collector ?
                polarity : 
                (polarity == EPowerUpPolarity.Positive ? EPowerUpPolarity.Negative : EPowerUpPolarity.Positive);

            Color fillColor = polarity == EPowerUpPolarity.Positive
                ? positiveColor
                : negativeColor;

            radialFillImage.color = fillColor;

            radialFillImage.fillAmount = active.NormalizedRemaining;
            gameObject.SetActive(true);
        }

        public void Unbind()
        {
            _active = null;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_active == null)
            {
                return;
            }

            radialFillImage.fillAmount = _active.NormalizedRemaining;
        }
    }
}