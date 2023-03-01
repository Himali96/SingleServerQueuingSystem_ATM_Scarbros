using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class CarController : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    public Transform targetWindow;
    public Transform targetCar=null;
    public Transform targetExit = null;

    public bool InService { get; set; }
    public GameObject driveThruWindow;
    public QueueManager queueManager;

    public enum CarState
    {
        None=-1,
        Entered,  //going towards the DriveThruWindow (don't bump into fron cars)
        Waiting,
        InService,
        Serviced
    }
    public CarState carState = CarState.None;
    // Start is called before the first frame update
    void Start()
    {
        driveThruWindow = GameObject.FindGameObjectWithTag("DriveThruWindow");
        targetWindow = driveThruWindow.transform;
        targetExit = GameObject.FindGameObjectWithTag("CarExit").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
#if DEBUG_CC
        print("Start: this.GO.ID=" + this.gameObject.GetInstanceID());
#endif

        //
        carState = CarState.Entered;
        FSMCar();

    }

    void FSMCar()
    {
#if DEBUG_CC
        print("CC.FSMCar:state="+carState+",ID="+this.gameObject.GetInstanceID());
#endif
        switch (carState)
        {
            case CarState.None: //do nothing - shouldn't happen
                break;
            case CarState.Entered:
                DoEntered();
                break;
            case CarState.Waiting:
                DoWait();
                break;
            case CarState.InService:
                DoInService();
                break;
            case CarState.Serviced:
                DoServiced();
                break;
            default:
                print("carState unknown!:" + carState);
                break;

        }
    }
    void DoEntered()
    {
        targetCar = targetWindow;

        queueManager = GameObject.FindGameObjectWithTag("DriveThruWindow").GetComponent<QueueManager>();
        queueManager.Add(this.gameObject);

        navMeshAgent.SetDestination(targetCar.position);
        navMeshAgent.isStopped = false;
        if (queueManager.First() != this.gameObject && 
            (queueManager.First().GetComponent<CarController>().carState == CarController.CarState.InService ||
            queueManager.First().GetComponent<CarController>().carState == CarController.CarState.Entered))
        {
            this.ChangeState(CarState.Waiting);
        }
    }

    void DoWait()
    {
        Debug.Log("InWaiting");
    }

    void DoInService()
    {
        navMeshAgent.isStopped = true;
        //this.transform.position = targetWindow.position;
        //this.transform.rotation = Quaternion.identity;
    }
    void DoServiced()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetExit.position);

        Renderer r = GetComponent<Renderer>();
        r.material.color = Color.green;
    }
    public void ChangeState(CarState newCarState)
    {
        this.carState = newCarState;
        FSMCar();
    }

    public void SetInService(bool value)
    {

    }
    public void ExitService(Transform target)
    {
        //this.SetInService(false);
        
        queueManager.PopFirst();
        ChangeState(CarState.Serviced);
        ChangeCarWaitingPosition();
        Debug.Log("Count :: " + queueManager.queue.Count);
    }

    void ChangeCarWaitingPosition()
    {
        GameObject go = queueManager.First();
        go.GetComponent<CarController>().ChangeState(CarState.Entered);
        go.GetComponent<CarController>().navMeshAgent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
#if DEBUG_CC
        Debug.LogFormat("CarController(this={0}).OnTriggerEnter:other={1}",this.gameObject.GetInstanceID(), other.gameObject.tag);
#endif
        if (other.gameObject.tag == "Car")
        {
            if (queueManager.First() != this.gameObject &&
            (queueManager.First().GetComponent<CarController>().carState == CarController.CarState.InService ||
            queueManager.First().GetComponent<CarController>().carState == CarController.CarState.Waiting ||
            queueManager.First().GetComponent<CarController>().carState == CarController.CarState.Entered))
            {
                ChangeState(CarState.Waiting);
                navMeshAgent.isStopped = true;
            }
        }
        else if (other.gameObject.tag == "DriveThruWindow")
        {
            ChangeState(CarState.InService);
            //SetInService(true);
        }
        else if (other.gameObject.tag == "CarExit")
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            navMeshAgent.isStopped = false;
        }
    }
        private void OnDrawGizmos()
    {
#if DEBUG_CC
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetCar.gameObject.GetInstanceID());
        print("InCC.OnDrawGizmos:targetCar.ID=" + targetExit.gameObject.GetInstanceID());

#endif
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, targetWindow.transform.position);
        if (targetCar)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(this.transform.position, targetCar.transform.position);

        }
        if (targetExit)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(this.transform.position, targetExit.transform.position);

        }


    }

}
