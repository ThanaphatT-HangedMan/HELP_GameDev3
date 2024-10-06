using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSystemManager : MonoBehaviour
{
    public Ghost ghost;

    private bool managerRecord = false;
    private bool managerReplay = false;

    void Start()
    {
    }

    void Update()
    {
        // Handle input for switching between recording and replaying
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartRecording();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            StopRecording();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            StartReplaying();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            StopReplaying();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ResetRecord();
        }

    }

    void StartRecording()
    {
        ghost.isRecord = true;
        ghost.isReplay = false; // Make sure replay is off
        managerRecord = true;
        managerReplay = false;

        Debug.Log("Started recording.");
    }

    void StopRecording()
    {
        ghost.isRecord = false;
        managerRecord = false;

        Debug.Log("Stopped recording.");
    }

    void StartReplaying()
    {
        ghost.isReplay = true;
        ghost.isRecord = false; // Make sure recording is off
        managerReplay = true;
        managerRecord = false;

        Debug.Log("Started replaying.");
    }

    void StopReplaying()
    {
        ghost.isReplay = false;
        managerReplay = false;

        Debug.Log("Stopped replaying.");
    }

    void ResetRecord()
    {
        ghost.ResetData();
    }
}