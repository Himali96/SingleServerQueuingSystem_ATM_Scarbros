using System;
using UnityEngine;
using TMPro;

public class ServicetimerTxt : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerTxt;
    ServiceProcess service;
    [SerializeField] TMP_Text customers;

    // Start is called before the first frame update
    void Start()
    {
        service = GameObject.FindWithTag("DriveThruWindow").GetComponent<ServiceProcess>();
    }
    
    void LateUpdate()
    {
        service.currentServiceTimer -= Time.deltaTime;
        if (service.currentServiceTimer < 0f)
            service.currentServiceTimer = 0f;
        
        TimeSpan ts = TimeSpan.FromSeconds(service.currentServiceTimer);
        timerTxt.SetText($"{ts.Minutes:00}:{ts.Seconds:00}");
        customers.text = service.customersCont.ToString();
    }

    public void ChangeSpeed(float v)
    {
        Time.timeScale = v;
    }
}
