//*******************
//Power By CosmosFlow
//2014-01-08
//*******************
using UnityEngine;
using System.Collections;

public class RollTheDice : MonoBehaviour
{
    public GameObject dice_1;
    public GameObject dice_2;
    public float force;
    public float addGravity;
    public GUISkin DiceGUISkin;

    private AudioClip _rollDiceEndAudio;
    private Vector3 old_pos;
    private Vector3 new_pos;
    private Vector3 delta_pos;

    private bool couldRoll;
    private bool isRolling;
    private bool isStart;

    private float rollingTime = 0;
    private int dicePoint = 0;
	private int PlayerPrefsPoints = 0;

    private int PlayerPrefsForAnZhi = 0;

	private bool QueryOrNot = true;

    // Use this for initialization
    private void Start()
    {
		_rollDiceEndAudio = Resources.Load("sound/sz") as AudioClip;

		//LocalPlayerPrefs();
        LocalPlayerPrefsForAnZhi();

		SetRestart();
		SetDeltaPosition();
		SetGravity();
    }
	
    /// <summary>
    /// 显示有米顶部广告条
    /// </summary>
    private void LocalInitYoumi()
    {
        return;//不显示顶部广告条
        if (YoumiSDK.Instance != null && Application.platform == RuntimePlatform.Android)
            YoumiSDK.Instance.YoumiAddBanner();
    }

    //----------------------------
    /// <summary>
    /// 开始时，积分如果超过50，去掉顶部广告条
    /// </summary>
	private void LocalPlayerPrefs(){
		int tempPoints = PlayerPrefs.GetInt("LocalPoints");
		if (tempPoints >= 50)
			QueryOrNot = false;
		else 
			LocalInitYoumi();
	}
    /// <summary>
    /// 轮询检查积分，如果超过50，去除一切广告包括左下角按钮
    /// </summary>
    private void QueryToChackPoints()
    {
        if (QueryOrNot)
        {
            if (YoumiSDK.Instance != null && Application.platform != RuntimePlatform.Android) return;
            YoumiSDK.Instance.YoumiQueryPoints();

            if (YoumiSDK.Instance.mPoints >= 50)
            {
                PlayerPrefs.SetInt("LocalPoints", YoumiSDK.Instance.mPoints);
                QueryOrNot = false;
            }
        }
    }
    //----------------------------

    /// <summary>
    /// 安智市场不支持有米广告，为通过审核，第一次加载不显示有米广告的任何入口
    /// </summary>
    private void LocalPlayerPrefsForAnZhi()
    {
        PlayerPrefsForAnZhi = PlayerPrefs.GetInt("PrefsAnZhi");
        if (PlayerPrefsForAnZhi == 0)
        {
            QueryOrNot = false;

            PlayerPrefsForAnZhi += 1;
            PlayerPrefs.SetInt("PrefsAnZhi", PlayerPrefsForAnZhi);
        }
        else
        {
            QueryOrNot = true;
            //LocalPlayerPrefs();
        }
    }


	
	//----------------------------
    private void SetDeltaPosition()
    {
        new_pos = Input.acceleration;
        old_pos = Input.acceleration;
        delta_pos = new_pos - old_pos;
    }
    private void SetGravity()
    {
        Vector3 gravity;
        gravity.x = 0;
        gravity.y = -addGravity;
        gravity.z = 0;
        Physics.gravity = gravity;
        Physics.bounceThreshold = 4;
    }

    // TODO : Restart
    private void SetRestart()
    {
        couldRoll = true;
        isRolling = false;
        rollingTime = 0;
    }

    private void AvoidToInTogether()
    {
        Vector3 p1 = dice_1.rigidbody.position;
        Vector3 p2 = dice_2.rigidbody.position;
        Vector3 dis = p1 - p2;
        if (dis.magnitude < 1.92f)
        {
            if (p1.magnitude > p2.magnitude)
            {
                Vector3 pos = dice_2.rigidbody.position - dice_2.rigidbody.position.normalized * 0.5f;
                dice_2.rigidbody.MovePosition(pos);
            }
            else
            {
                Vector3 pos = dice_1.rigidbody.position - dice_1.rigidbody.position.normalized * 0.5f;
                dice_1.rigidbody.MovePosition(pos);
            }
        }
    }

    private void EndGetThePoints()
    {
        if (!isRolling) return;
        if (dice_1.rigidbody.GetPointVelocity(dice_1.rigidbody.centerOfMass).magnitude == 0 &&
                   dice_1.rigidbody.GetRelativePointVelocity(dice_1.rigidbody.centerOfMass).magnitude == 0 &&
                   dice_2.rigidbody.GetPointVelocity(dice_2.rigidbody.centerOfMass).magnitude == 0 &&
                   dice_2.rigidbody.GetRelativePointVelocity(dice_2.rigidbody.centerOfMass).magnitude == 0)
        {
            if (GetDicePoint(dice_1.transform) != 0 && GetDicePoint(dice_2.transform) != 0)
            {
                dicePoint = GetDicePoint(dice_1.transform) + GetDicePoint(dice_2.transform);
                Debug.Log("Dice Point : " + dicePoint);
                SetRestart();
            }
        }
    }
    // Update is called once per frame
    private void Update()
    {
		QueryToChackPoints();

        AvoidToInTogether();
        EndGetThePoints();

        if (couldRoll)
        {
            if (isRolling)
            {
                rollingTime += Time.deltaTime;
                if (rollingTime > 1.0)
                    couldRoll = false;
            }
            new_pos = Input.acceleration;
            delta_pos = new_pos - old_pos;
            old_pos = Input.acceleration;
            if (delta_pos.magnitude > 1.0f)
            {
                if (delta_pos.magnitude > 15.0f)
                {
                    Vector3 sclae;
                    sclae.x = 15.0f / delta_pos.magnitude;
                    sclae.y = 15.0f / delta_pos.magnitude;
                    sclae.z = 15.0f / delta_pos.magnitude;
                    delta_pos.Scale(sclae);
                }
                StartRollTheDice(-delta_pos * force);
                rollingTime = 0;
                isRolling = true;
                Handheld.Vibrate();
            }
        }
    }

    private void OnGUI()
    {
        GUI.skin = DiceGUISkin;
        if (QueryOrNot)
        {
            if (GUI.Button(new Rect(20, Screen.height - 84, 64, 64), "", "ShowOfferButton"))
            {
                Debug.Log("ShowOfferButton");
                if (YoumiSDK.Instance != null && Application.platform != RuntimePlatform.Android) return;
                YoumiSDK.Instance.YoumiShowOffersDialog();
            }
            GUI.Label(new Rect(90, Screen.height - 60, 160, 20), "50积分即可永久消除广告哦~", "ShowOfferLabel");
        }

        /*if (GUILayout.Button("Roll The Dice", GUILayout.Height(60)))
        {
            float a1 = Random.Range(2.3f, 4.5f) * force;
            float b1 = Random.Range(2.3f, 4.5f) * 0;
            float c1 = Random.Range(2.3f, 4.5f) * force;
            dice_1.rigidbody.AddForce(a1, b1, c1);
            float a2 = Random.Range(2.3f, 4.5f) * force;
            float b2 = Random.Range(2.3f, 4.5f) * 0;
            float c2 = Random.Range(2.3f, 4.5f) * force;
            dice_2.rigidbody.AddForce(a2, b2, c2);

            couldRoll = true;
            isRolling = true;
            rollingTime = 0;
        }
        else if (GUILayout.Button("Restart", GUILayout.Height(60)))
        {
            SetRestart();
        }*/
    }
    private void StartRollTheDice(Vector3 dir)
    {
        dice_1.rigidbody.AddForce(-dir.y, -dir.z, dir.x);
        dice_2.rigidbody.AddForce(-dir.y, -dir.z, dir.x);
    }

    private int GetDicePoint(Transform transform)
    {
        int point = 0;
        Vector3 dir;
        dir.x = 0;
        dir.y = 1;
        dir.z = 0;
        Vector3 up = transform.forward;
        Vector3 right = transform.right;
        Vector3 forward = -transform.up;
        Vector3 down = -transform.forward;
        Vector3 left = -transform.right;
        Vector3 back = transform.up;
        if (Vector3.Angle(up, dir) < 45)
            point = 4;
        else if (Vector3.Angle(down, dir) < 45)
            point = 3;
        else if (Vector3.Angle(right, dir) < 45)
            point = 2;
        else if (Vector3.Angle(left, dir) < 45)
            point = 5;
        else if (Vector3.Angle(forward, dir) < 45)
            point = 6;
        else if (Vector3.Angle(back, dir) < 45)
            point = 1;
        return point;
    }
}
