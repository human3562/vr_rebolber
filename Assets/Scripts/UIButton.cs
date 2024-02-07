using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class UIButton : MonoBehaviour, IDamageable
{
    public UnityEvent OnButtonPressed;
    [SerializeField]
    private Image faderImg;
    private bool disabled = false;

    public void Damage()
    {
        if(!disabled)
            OnButtonPressed.Invoke();
    }

    public void Fade()
    {
        faderImg.DOFade(0.6f, 0.5f);
        disabled = true;
    }

    public void UnFade()
    {
        faderImg.DOFade(0.0f, 0.5f);
        disabled = false;
    }
}
