using GoogleMobileAds.Api;
using UnityEngine;

public class InterstitialAdManager : MonoBehaviour
{
    private InterstitialAd interstitialAd;
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
        LoadInterstitial();
    }

    private string GetInterstitialAdUnitId()
    {
#if UNITY_ANDROID
        return "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IOS
        return "ca-app-pub-3940256099942544/4411468910";
#else
        return "unused";
#endif
    }

    public void LoadInterstitial()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();

        InterstitialAd.Load(
            GetInterstitialAdUnitId(),
            request,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError(
                        $"Interstitial did not load. Error: {error}"
                    );

                    return;
                }

                interstitialAd = ad;

                RegisterInterstitialEvents();

                ShowInterstitial();
            }
        );
    }

    public void ShowInterstitial()
    {
        if (
            interstitialAd == null ||
            !interstitialAd.CanShowAd()
        )
        {
            Debug.Log("Interstitial unavailable");
            return;
        }

        interstitialAd.Show();
    }

    private void RegisterInterstitialEvents()
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        };

        interstitialAd.OnAdFullScreenContentFailed += error =>
        {
            Debug.Log(
                $"Error showing interstitial: {error}"
            );
        };
    }

    private void OnDestroy()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
    }
}

