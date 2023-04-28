using UnityEngine;
using GoogleMobileAds;
using System.Collections;
using GoogleMobileAds.Api;
 
public class Interstitial : MonoBehaviour // , IUnityAdsLoadListener, IUnityAdsShowListener
{

    public static Interstitial instance;

    InterstitialAd interstitialAd;

    // These ad units are configured to always serve test ads.
    #if UNITY_ANDROID
        string _adUnitId = "ca-app-pub-6871595514298555/5112880166";
    #elif UNITY_IPHONE
        string _adUnitId = "ca-app-pub-6871595514298555/6093273002";
    #else
        string _adUnitId = "unused";
    #endif

    void Awake() {
        if(instance != null){ 
            Debug.LogWarning("Instance of Interstitial already exists in the scene !");
            Destroy(this.gameObject);
            return;
        } else instance = this;

        MobileAds.Initialize((InitializationStatus initStatus) => { });
    }

    void Start() { CheckUnityAds(); }

    void CheckUnityAds() {
        GameData gameData = SaveManager.LoadData();
        gameData.gamePlayed += 1;
        SaveManager.SaveData(gameData);

        //Shows Interstitial Ad when game starts (except for the first time) 
        if (Time.time != Time.timeSinceLevelLoad && gameData.gamePlayed > GameManager.instance.gamePlayedToShowAd){
                gameData.gamePlayed = 0;
                SaveManager.SaveData(gameData);
                Interstitial.instance.ShowAd();
        }
    }

    void LoadInterstitialAd(){
        // Clean up the old ad before loading a new one.
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest.Builder()
                .AddKeyword("unity-admob-sample")
                .Build();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) => {
                // if error is not null, the load request failed.
                if (error != null || ad == null){
                    Debug.LogWarning("interstitial ad failed to load an ad " + "with error : " + error);
                    return;
                }
                interstitialAd = ad;
            });
    }

    public void ShowAd() {
        LoadInterstitialAd();
        StopCoroutine(WaitForAd());
        StartCoroutine(WaitForAd());
    }

    IEnumerator WaitForAd(){
        while(interstitialAd == null)  yield return null;
        while(!interstitialAd.CanShowAd()) yield return null;
        interstitialAd.Show();
    }
}