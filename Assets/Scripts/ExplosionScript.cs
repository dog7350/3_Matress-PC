using UnityEngine;
using Photon.Pun;

public class ExplosionScript : MonoBehaviourPunCallbacks
{
    AudioSource Sound;

    void Start()
    {
        Sound = this.gameObject.GetComponent<AudioSource>();
        Sound.PlayOneShot(BoomScript.instance.BoomSound);
    }

    void Update()
    {
        Destroy(this.gameObject, 1.0f);
    }
}
