using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Valve.VR.InteractionSystem;

public class manager : MonoBehaviour{

    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private MeshSpawner spawnArea;

    [SerializeField] private UIButton newRoundBtn;
    [SerializeField] private UIButton targetAmtBtn;

    [SerializeField] private AudioClip ui_click;
    [SerializeField] private AudioClip ui_countdown321;
    [SerializeField] private AudioClip ui_countdownGO;
    [SerializeField] private AudioClip ui_roundFinished;
    private AudioSource mainAudioSource;
    private Text targetAmtBtnText;

    private Transform playerPos;

    [SerializeField] private int[] AmountOptions;

    int amtOption = 0;

    [SerializeField] private Text accuracyText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text countdownText;

    private int shotsFired = 0;
    private int targetsHit = 0;
    private int targetAmt = 5;

    private float timer = 0f;
    private bool roundActive = false;


    void Start(){
        //Teleport.instance.CancelTeleportHint();

        newRoundBtn.OnButtonPressed.AddListener(newRoundButtonPressed);
        targetAmtBtn.OnButtonPressed.AddListener(targetAmtBtnPressed);
        targetAmtBtnText = targetAmtBtn.GetComponentInChildren<Text>();

        Revolver[] revolvers = FindObjectsOfType<Revolver>();
        foreach(Revolver r in revolvers)
        {
            r.OnShoot.AddListener(playerShoot);
        }

        mainAudioSource = GetComponent<AudioSource>();
        playerPos = GameObject.Find("Player").GetComponent<Transform>();
    }

    void Update()
    {
        if (shotsFired != 0)
        {
            float accuracy = (targetsHit / (float)shotsFired) * 100f;
            accuracyText.text = "Точность стрельбы: " + accuracy.ToString() + "%";
        }

        if (roundActive)
        {
            timer += Time.deltaTime;
            timeText.text = "Время раунда: " + timer.ToString("F", CultureInfo.InvariantCulture) + " сек.";
        }

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartNewRound(targetAmt);
        //}
    }

    public void StartNewRound(int targetAmt)
    {
        roundActive = true;
    }

    void newRoundButtonPressed()
    {
        mainAudioSource.PlayOneShot(ui_click);

        shotsFired = 0;
        targetsHit = 0;
        timeText.text = "Время раунда: ---";
        accuracyText.text = "Точность стрельбы: ---";
        timer = 0f;

        for (int i = 0; i < targetAmt; i++)
        {
            Vector3 randomPos = spawnArea.transform.TransformPoint(spawnArea.GetRandomPointOnMesh());
            //randomPos += spawnArea.transform.position;
            GameObject target = Instantiate(targetPrefab, randomPos, Quaternion.identity);
            target.GetComponent<Target>().OnTargetShot.AddListener(targetHit);
            target.transform.LookAt(playerPos);
        }

        newRoundBtn.Fade();
        targetAmtBtn.Fade();

        countdownText.DOFade(1f, 0.1f);
        countdownText.GetComponentInParent<Image>().DOFade(0.8f, 0.1f);

        StartCoroutine(startCountDown());
    }

    private IEnumerator startCountDown()
    {
        Revolver[] revolvers = FindObjectsOfType<Revolver>();
        foreach (Revolver r in revolvers)
        {
            r.stopShooting();
        }

        countdownText.text = "Внимание";
        yield return new WaitForSeconds(1f);
        countdownText.text = "3";
        mainAudioSource.PlayOneShot(ui_countdown321);
        yield return new WaitForSeconds(1f);
        countdownText.text = "2";
        mainAudioSource.PlayOneShot(ui_countdown321);
        yield return new WaitForSeconds(1f);
        countdownText.text = "1";
        mainAudioSource.PlayOneShot(ui_countdown321);
        yield return new WaitForSeconds(1f);

        countdownText.text = "Старт!";
        mainAudioSource.PlayOneShot(ui_countdownGO);

        foreach (Revolver r in revolvers)
        {
            r.continueShooting();
        }

        countdownText.DOFade(0f, 1f);
        countdownText.GetComponentInParent<Image>().DOFade(0f, 1f);

        StartNewRound(targetAmt);
    }

    void targetAmtBtnPressed()
    {
        mainAudioSource.PlayOneShot(ui_click);

        amtOption = (amtOption + 1) % AmountOptions.Length;
        switch (amtOption)
        {
            case 0:
                targetAmtBtnText.text = "Мало (5 целей)";
                break;
            case 1:
                targetAmtBtnText.text = "Средне (10 целей)";
                break;
            case 2:
                targetAmtBtnText.text = "Много (15 целей)";
                break;
            default:
                targetAmtBtnText.text = AmountOptions[amtOption].ToString();
                break;
        }
        targetAmt = AmountOptions[amtOption];
    }

    void playerShoot()
    {
        if(roundActive)
            shotsFired += 1;
    }

    void targetHit()
    {
        targetsHit += 1;
        if(targetsHit >= targetAmt)
        {
            roundActive = false;
            mainAudioSource.PlayOneShot(ui_roundFinished, 0.3f);
            newRoundBtn.UnFade();
            targetAmtBtn.UnFade();
        }
    }

}
