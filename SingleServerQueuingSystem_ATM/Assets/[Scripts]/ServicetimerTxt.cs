using System;
using UnityEngine;
using TMPro;

public class ServicetimerTxt : MonoBehaviour
{
    TextMeshProUGUI timerTxt;
    ServiceProcess service;
    public TMP_Text customers;

    // Start is called before the first frame update
    void Start()
    {
        timerTxt = GetComponent<TextMeshProUGUI>();
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
}
