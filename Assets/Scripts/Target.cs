using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    void Damage();
}

public class Target : MonoBehaviour, IDamageable
{
    public GameObject hitEffect;
    public UnityEvent OnTargetShot;
    public AudioClip[] hitSounds;
    public AudioClip extra;

    public void Damage()
    {
        OnTargetShot.Invoke();
        GameObject hitEffectObject = Instantiate(hitEffect, transform.position + transform.forward * 0.03f, transform.rotation);
        hitEffectObject.GetComponent<AudioSource>().PlayOneShot(hitSounds[(int)UnityEngine.Random.Range(0, hitSounds.Length)]);
        if(UnityEngine.Random.Range(0f, 1f) > 0.8f) hitEffectObject.GetComponent<AudioSource>().PlayOneShot(extra, 0.5f);
        Destroy(gameObject);
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Damage();
    //    }
    //}
}
