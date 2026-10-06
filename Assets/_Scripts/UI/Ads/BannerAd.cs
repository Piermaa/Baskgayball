using UnityEngine;
using GoogleMobileAds.Api;

public class BannerAd : MonoBehaviour
{
    private BannerView bannerView;

    private void Start()
    {
        LoadBanner();   
    }

    public void LoadBanner()
    {
        DestroyBanner();

        bannerView = new BannerView(
            GetBannerAdUnitId(),
            AdSize.IABBanner,
            AdPosition.Bottom
        );

        bannerView.OnBannerAdLoaded += ShowBanner;
        AdRequest request = new AdRequest();

        bannerView.LoadAd(request);
    }

    public void DestroyBanner()
    {
        if (bannerView == null)
        {
            return;
        }
        bannerView.OnBannerAdLoaded -= ShowBanner;
        bannerView.Destroy();
        bannerView = null;
    }

    private string GetBannerAdUnitId()
    {
        return "ca-app-pub-3940256099942544/6300978111";
        return "unused";
    }

    public void ShowBanner()
    {
        bannerView.Show();
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded successfully.");
        ShowBanner();
    }
}
