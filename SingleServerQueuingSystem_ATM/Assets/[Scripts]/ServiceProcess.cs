using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceProcess : MonoBehaviour
{
    
    public Transform carExitPlace;

    public float serviceRateAsCarsPerHour = 25; // car/hour
    public float interServiceTimeInHours; // = 1.0 / ServiceRateAsCarsPerHour;
    private float interServiceTimeInMinutes;
    private float interServiceTimeInSeconds;

    public bool generateServices = false;

    //Simple generation distribution - Uniform(min,max)
    public float minInterServiceTimeInSeconds = 3;
    public float maxInterServiceTimeInSeconds = 60;
    //
    public int customersCont;

    [System.NonSerialized] public float currentServiceTimer;

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

            generateServices = true;
            StartCoroutine(GenerateServices(car));
        }
    }

    IEnumerator GenerateServices(CarController carInService)
    {
        //while (generateServices)
        {
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

            Debug.Log(carInService + " time: " + timeToNextServiceInSec, carInService);
            yield return new WaitForSeconds(timeToNextServiceInSec);
        }
        
        carInService.ExitService(carExitPlace);
        customersCont++;        
    }

}
