using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Revolver : MonoBehaviour
{
    private ParticleSystem firePart;
    private Interactable interactable;
    private Rigidbody revolverBody;
    public SteamVR_Action_Boolean fireAction;

    public UnityEvent OnShoot;

    public float kickbackForce = -10f;
    public Transform recoilGuide;

    public AudioClip[] gunshots;
    private AudioSource muzzleSource;

    [SerializeField] private GameObject sandHitPrefab;

    private bool canShoot = true;

    private void Start()
    {
        firePart = GetComponentInChildren<ParticleSystem>();
        interactable = GetComponent<Interactable>();
        revolverBody = GetComponent<Rigidbody>();
        muzzleSource = firePart.GetComponent<AudioSource>();

    }

    private void Update()
    {
        if(interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;
            if (fireAction[source].stateDown)
            {
                Fire();
            }
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Fire();
        //}
    }

    public void stopShooting()
    {
        canShoot = false;
    }

    public void continueShooting()
    {
        canShoot = true;
    }

    void Fire()
    {
        if (canShoot == false) return;

        OnShoot.Invoke();
        if (Physics.Raycast(firePart.transform.position, firePart.transform.right, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.DrawRay(firePart.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

            if (hit.transform.CompareTag("Terrain"))
            {
                Debug.Log("Hit ground lol");
                Instantiate(sandHitPrefab, hit.point, Quaternion.identity);
            }
            else
            {
                IDamageable t = hit.transform.GetComponent<IDamageable>();
                if (t != null)
                {
                    //Debug.Log("Hit a target!");
                    t.Damage();
                }
            }

        }


        firePart.Play();

        if(gunshots.Length > 0)
            muzzleSource.PlayOneShot(gunshots[(int)Random.Range(0, gunshots.Length)], 1f);

        revolverBody.AddForceAtPosition((recoilGuide.position - firePart.transform.position) * kickbackForce, firePart.transform.position, ForceMode.Impulse);
    }

    
}
