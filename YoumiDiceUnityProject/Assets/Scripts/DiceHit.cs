using UnityEngine;
using System.Collections;

public class DiceHit : MonoBehaviour
{
    public AudioClip clip;
    AudioListener mListener;

    private void OnCollisionEnter(Collision info)
    {
        if (mListener == null) 
            mListener = GameObject.FindObjectOfType(typeof(AudioListener)) as AudioListener;
        AudioSource source = mListener.audio;

        if (source == null) 
            source = mListener.gameObject.AddComponent<AudioSource>();
        //source.pitch = pitch;
        if (info.impactForceSum.magnitude > 20.0f)
            source.PlayOneShot(clip, 1.0f);
        else if (info.impactForceSum.magnitude > 6.0f)
            source.PlayOneShot(clip, (info.impactForceSum.magnitude) / 20.0f);
    }
}
