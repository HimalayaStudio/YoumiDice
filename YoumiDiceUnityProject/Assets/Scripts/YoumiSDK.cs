using UnityEngine;
using System.Collections;

public class YoumiSDK : MonoBehaviour
{
    public static YoumiSDK Instance;
    private AndroidJavaClass mJc;
    private AndroidJavaObject mJo;
    internal int mPoints;

    private void Awake()
    {
        Instance = this;
        YoumiSDKInit();
    }
    //TODO: 积分广告
    internal void YoumiAddOffersBarnner()
    {
        mJo.Call("addOffersBanner");//show offers banner
    }
    internal void YoumiShowOffers()
    {
        mJo.Call("showOffers"); //show full screen offers wall
    }
    internal void YoumiShowOffersDialog()
    {
        mJo.Call("showOffersDialog"); //show dialog offers wall
    }

    //TODO: 积分点数
    internal void YoumiQueryPoints()
    {
        this.mPoints = mJo.Call<int>("queryPoints");//query the points of user's point account
    }
    internal void YoumiAwardPoints(int point)
    {
        if (mJo.Call<bool>("awardPoints", point))
        {//demo of award 10 points
            this.mPoints = mJo.Call<int>("queryPoints");    //the function return true when award point success , then change the value of this.mPoints to display
        }
    }
    internal void YoumiSpendPoints(int point)
    {
        if (mJo.Call<bool>("spendPoints", point))
        { //demo of spend 5 points
            this.mPoints = mJo.Call<int>("queryPoints"); //the function return true when spend point success , then change the value of this.mPoints to display
        }
    }

    //TODO : 无积分广告
    internal void YoumiShowSpot()
    {
        mJo.Call("showSpot"); //show spot 
    }
    internal void YoumiAddBanner()
    {
        mJo.Call("addBanner"); //show banner 
    }

    // Use this for initialization
    public void YoumiSDKInit()
    {
        if (Application.platform != RuntimePlatform.Android) return;
        mJc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        mJo = mJc.GetStatic<AndroidJavaObject>("currentActivity");        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Home))
        {
            Application.Quit(); //exit when key back
        }
    }
}