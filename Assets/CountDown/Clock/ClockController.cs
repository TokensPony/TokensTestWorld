
using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ClockController : UdonSharpBehaviour
{
    private int clockState = 0;
    private float clockNormalizedTime = 0f;
    private float clockThemeTime = 0f;

    [UdonSynced]
    private int globalClockState = 0;
    [UdonSynced]
    private float globalClockNormalizedTime = 0f;
    [UdonSynced]
    private float globalClockThemeTime = 0f;

    [SerializeField] Animator ClockAnimationController;
    [SerializeField] AudioSource clockTheme;

    private float startTime = 0;

    VRCPlayerApi player;

    void Start()
    {
        Debug.Log("Clock has been summoned!");
        
    }

    private void Update()
    {
        if(Networking.IsOwner(this.gameObject) && clockState == 1)
        {
            if (ClockAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                clockNormalizedTime = ClockAnimationController.GetCurrentAnimatorStateInfo(0).normalizedTime;
                globalClockNormalizedTime = clockNormalizedTime;

                clockThemeTime = clockTheme.time;
                globalClockThemeTime = clockThemeTime;
            }
            //Debug.Log("GlobalClockNormalizedTime = " + globalClockNormalizedTime);
            if (Time.time - startTime >= 32f)
            {
                clockState = 0;
                globalClockState = clockState;
                ClockAnimationController.SetInteger("ClockState", clockState);
                clockTheme.gameObject.SetActive(false);
                clockNormalizedTime = 0f;
                globalClockNormalizedTime = clockNormalizedTime;
                
                //RequestSerialization();
                //Debug.Log("Clock Finished/30 sec elapsed");
                //Debug.Log("GlobalClockState: " + globalClockState);
                //Debug.Log("ClockAnimationController ClockState: " + ClockAnimationController.GetInteger("ClockState"));
            }
            RequestSerialization();

            //Debug.Log("globalClockThemeTime = " + globalClockThemeTime);
        }
    }

    public override void Interact()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
        }
        if (Networking.IsOwner(this.gameObject) && clockState == 0)
        {
            startTime = Time.time;
            clockState = 1;
            globalClockState = clockState;

            clockNormalizedTime = 0f;
            globalClockNormalizedTime = clockNormalizedTime;

            clockThemeTime = 0f;
            globalClockThemeTime = clockThemeTime;
            clockTheme.time = clockThemeTime;
            //clockTheme.Play();

            ClockAnimationController.SetInteger("ClockState", clockState);

            clockTheme.gameObject.SetActive(true);


            RequestSerialization();
            //Debug.Log("Clock Interacted with.");
            //Debug.Log("GlobalClockState: " + globalClockState);
            //Debug.Log("ClockAnimationController ClockState: " + ClockAnimationController.GetInteger("ClockState"));
        }
    }

    public override void OnDeserialization()
    {
        if (!Networking.IsOwner(this.gameObject))
        {
            clockState = globalClockState;
            clockNormalizedTime = globalClockNormalizedTime;
            clockThemeTime = globalClockThemeTime;
            ClockAnimationController.SetInteger("ClockState", clockState);
            if(clockState == 1)
            {
                clockTheme.gameObject.SetActive(true);
            }
            else
            {
                clockTheme.gameObject.SetActive(false);
            }
            //Debug.Log("Deserialization Data Recieved:");
            //Debug.Log("GlobalClockState: " + globalClockState);
            //Debug.Log("ClockAnimationController ClockState: " + ClockAnimationController.GetInteger("ClockState"));

        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        this.player = player;
        if (globalClockState == 1)
        {
            ClockAnimationController.Play("Running", 0, clockNormalizedTime);
            clockTheme.time = clockThemeTime;
            //clockTheme.Play();
        }
    }




}
