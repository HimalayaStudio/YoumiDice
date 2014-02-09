using UnityEngine;
using System.Collections;

public class FXBackground : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        gameObject.GetComponent<GUITexture>().pixelInset = new Rect(-Screen.width / 2, -Screen.height / 2, Screen.width, Screen.height);
    }
}
