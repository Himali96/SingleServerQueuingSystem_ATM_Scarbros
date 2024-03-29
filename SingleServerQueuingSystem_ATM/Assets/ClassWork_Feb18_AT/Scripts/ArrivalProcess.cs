﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrivalProcess : MonoBehaviour
{

    public GameObject carPrefab;
    public Transform carSpawnPlace;
    public Transform carParent;

    public int maxNumberOfCarInQueue = 10;
    public float arrivalRateAsCarsPerHour = 20; // car/hour
    public float interArrivalTimeInHours; // = 1.0 / arrivalRateAsCarsPerHour;
    private float interArrivalTimeInMinutes;
    private float interArrivalTimeInSeconds;

    public bool generateArrivals = true;

    //Simple generation distribution - Uniform(min,max)
    //
    public float minInterArrivalTimeInSeconds = 3; 
    public float maxInterArrivalTimeInSeconds = 60;
    //
    public enum ArrivalIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ArrivalIntervalTimeStrategy arrivalIntervalTimeStrategy=ArrivalIntervalTimeStrategy.UniformIntervalTime;

    QueueManager queueManager;

    //UI debugging
#if DEBUG_AP
    public Text txtDebug;
#endif

    // Start is called before the first frame update
    void Start()
    {
        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        interArrivalTimeInHours = 1.0f / arrivalRateAsCarsPerHour;
        interArrivalTimeInMinutes = interArrivalTimeInHours * 60;
        interArrivalTimeInSeconds = interArrivalTimeInMinutes * 60;
        StartCoroutine(GenerateArrivals());
#if DEBUG_AP
        print("proc#:" + System.Environment.ProcessorCount);
        txtDebug.text = "\nproc#:" + System.Environment.ProcessorCount;
#endif
    }
   
    IEnumerator GenerateArrivals()
    {
        while (generateArrivals)
        {
            if (queueManager.queue.Count < maxNumberOfCarInQueue)
            {
                GameObject carGO = Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity, carParent);
            }

            float timeToNextArrivalInSec = interArrivalTimeInSeconds;
            switch (arrivalIntervalTimeStrategy)
            {
                case ArrivalIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextArrivalInSec= interArrivalTimeInSeconds;
                    break;
                case ArrivalIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds, maxInterArrivalTimeInSeconds);
                    break;
                case ArrivalIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / arrivalRateAsCarsPerHour;
                    timeToNextArrivalInSec = Utilities.GetExp(U,Lambda);
                    break;
                case ArrivalIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextArrivalInSec = interArrivalTimeInSeconds;
                    break;
                default:
                    print("No acceptable arrivalIntervalTimeStrategy:" + arrivalIntervalTimeStrategy);
                    break;

            }

            //float timeToNextArrivalInSec = Random.Range(minInterArrivalTimeInSeconds,maxInterArrivalTimeInSeconds);
            yield return new WaitForSeconds(timeToNextArrivalInSec);
        }

    }
}
