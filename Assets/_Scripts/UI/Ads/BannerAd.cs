using UnityEngine;
using GoogleMobileAds.Api;

namespace BaskgayBall
{
    public class BannerAd : MonoBehaviour
    {
        private BannerView bannerView;

        private void Start()
        {
            LoadBanner();
        }

        private void OnDestroy()
        {
            DestroyBanner();
        }

        public void LoadBanner()
        {
            DestroyBanner();

            // Full width adaptive banner. Height is chosen by AdMob based on the width,
            // and in landscape it ends up short and wide.
            AdSize adaptiveSize =
                AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

            bannerView = new BannerView(
                GetBannerAdUnitId(),
                adaptiveSize,
                AdPosition.Bottom
            );

            AdRequest request = new AdRequest();
            bannerView.LoadAd(request);
        }

        public void DestroyBanner()
        {
            if (bannerView == null)
            {
                return;
            }

            bannerView.Destroy();
            bannerView = null;
        }

        private string GetBannerAdUnitId()
        {
#if UNITY_ANDROID
            return "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
            return "ca-app-pub-3940256099942544/2934735716";
#else
            return "unused";
#endif
        }
    }
}