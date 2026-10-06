using GoogleMobileAds.Api;
using UnityEngine;
using GoogleMobileAds.Api;

public class InterstitialAdManager : MonoBehaviour
{
    [SerializeField] private GameObject adContainer;
    private InterstitialAd intersitialView;
    private void OnEnable()
    {
        GlobalEvents.OnFinishMatch += GlobalEvents_OnFinishMatch;
    }

    private void OnDisable()
    {
        GlobalEvents.OnFinishMatch -= GlobalEvents_OnFinishMatch;
    }
    private void GlobalEvents_OnFinishMatch()
    {
        adContainer.SetActive(true);
    }

    //public void LoadBanner()
    //{
    //    DestroyBanner();

    //    intersitialView = new InterstitialAd();
        
    //    intersitialView.OnBannerAdLoaded += ShowBanner;
    //    AdRequest request = new AdRequest();

    //    intersitialView.LoadAd(request);
    //}

    //public void DestroyBanner()
    //{
    //    if (intersitialView == null)
    //    {
    //        return;
    //    }
    //    intersitialView.OnBannerAdLoaded -= ShowBanner;
    //    intersitialView.Destroy();
    //    intersitialView = null;
    //}

    //private string GetBannerAdUnitId()
    //{
    //    return "ca-app-pub-3940256099942544/6300978111";
    //    return "unused";
    //}

    //public void ShowBanner()
    //{
    //    intersitialView.Show();
    //}

    //private void OnBannerLoaded()
    //{
    //    Debug.Log("Banner ad loaded successfully.");
    //    ShowBanner();
    //}
}

