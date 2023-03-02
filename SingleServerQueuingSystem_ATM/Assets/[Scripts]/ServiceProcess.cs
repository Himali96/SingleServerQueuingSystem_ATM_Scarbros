using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//New as of Feb.25rd

public class ServiceProcess : MonoBehaviour
{
    
    public Transform carExitPlace;

    public float serviceRateAsCarsPerHour = 25; // car/hour
    public float interServiceTimeInHours; // = 1.0 / ServiceRateAsCarsPerHour;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    //public float ServiceRateAsCarsPerHour = 20; // car/hour
    public bool generateServices = false;

    //New as of Feb.23rd
    //Simple generation distribution - Uniform(min,max)
    //
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;
    //
    public int customersCont;

    [System.NonSerialized] public float currentServiceTimer;

    //New as Feb.25th
    //CarController carController;
    QueueManager queueManager; //=new QueueManager();

    public enum ServiceIntervalTimeStrategy
    {
        ConstantIntervalTime,
        UniformIntervalTime,
        ExponentialIntervalTime,
        ObservedIntervalTime
    }

    public ServiceIntervalTimeStrategy serviceIntervalTimeStrategy = ServiceIntervalTimeStrategy.UniformIntervalTime;

    // Start is called before the first frame update
    void Start()
    {
        customersCont = 0;
        interServiceTimeInHours = 1.0f / serviceRateAsCarsPerHour;
        interServiceTimeInMinutes = interServiceTimeInHours * 60;
        interServiceTimeInSeconds = interServiceTimeInMinutes * 60;
        //queueManager = this.GetComponent<QueueManager>();
        //queueManager = new QueueManager();
        //StartCoroutine(GenerateServices());
    }
    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_SP
        print("ServiceProcess.OnTriggerEnter:otherID=" + other.gameObject.GetInstanceID());
#endif

        if (other.gameObject.tag == "Car")
        {
            CarController car = other.gameObject.GetComponent<CarController>();
            car.SetInService(true);

            //if (queueManager.Count() == 0)
            //{
            //    queueManager.Add(carInService);
            //}
            Debug.Log("Collided with car");
            generateServices = true;
            //carController = carInService.GetComponent<CarController>();
            StartCoroutine(GenerateServices(car));
        }
    }

    IEnumerator GenerateServices(CarController carInService)
    {
        //while (generateServices)
        {
            //Instantiate(carPrefab, carSpawnPlace.position, Quaternion.identity);
            float timeToNextServiceInSec = interServiceTimeInSeconds;
            switch (serviceIntervalTimeStrategy)
            {
                case ServiceIntervalTimeStrategy.ConstantIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                case ServiceIntervalTimeStrategy.UniformIntervalTime:
                    timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds, maxInterServiceTimeInSeconds);
                    break;
                case ServiceIntervalTimeStrategy.ExponentialIntervalTime:
                    float U = Random.value;
                    float Lambda = 1 / serviceRateAsCarsPerHour;
                    timeToNextServiceInSec = Utilities.GetExp(U, Lambda);
                    break;
                case ServiceIntervalTimeStrategy.ObservedIntervalTime:
                    timeToNextServiceInSec = interServiceTimeInSeconds;
                    break;
                default:
                    print("No acceptable ServiceIntervalTimeStrategy:" + serviceIntervalTimeStrategy);
                    break;

            }
            currentServiceTimer = timeToNextServiceInSec + 1f;

            //New as of Feb.23rd
            //float timeToNextServiceInSec = Random.Range(minInterServiceTimeInSeconds,maxInterServiceTimeInSeconds);
           // generateServices = false;
           Debug.Log(carInService + " time: " + timeToNextServiceInSec, carInService);
            yield return new WaitForSeconds(timeToNextServiceInSec);

            //yield return new WaitForSeconds(interServiceTimeInSeconds);
        }
        
        carInService.ExitService(carExitPlace);
        customersCont++;        
    }

    private void OnDrawGizmos()
    {
        //BoxCollidercarInService.GetComponent<BoxCollider>
        // if (carInService)
        // {
        //     Renderer r = carInService.GetComponent<Renderer>();
        //     r.material.color = Color.green;

        // }


    }

}
