using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GhostRecoder : MonoBehaviour
{
    public Ghost ghost;
    private float timer;
    private float timeValue;

    private void Awake()
    {
        if (ghost.isRecord)
        {
            ghost.ResetData();
            timeValue = 0;
            timer = 0;
        }

    }

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        timeValue += Time.unscaledDeltaTime;

        if (ghost.isRecord & timer >= 1 / ghost.recordFrequency)
        {
            ghost.timeStamp.Add(timeValue);
            ghost.position.Add(this.transform.position);
            ghost.rotation.Add(this.transform.eulerAngles);

            timer = 0;
        }
    }

}