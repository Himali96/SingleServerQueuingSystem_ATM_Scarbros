using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Customer
{
    public float arrivalTime;
    public float serviceTime;

    public Customer(float arrivalTime, float serviceTime)
    {
        this.arrivalTime = arrivalTime;
        this.serviceTime = serviceTime;
    }
}

public class ReadData : MonoBehaviour
{
    void Start()
    {
        string filePath = "Assets/Files/Data.txt";
        List<Customer> customerList = new List<Customer>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            reader.ReadLine(); //Skip first line

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split('\t');                

                float arrivalTime = float.Parse(values[1]);
                float serviceTime = float.Parse(values[2]);

                Customer customer = new Customer(arrivalTime, serviceTime);
                customerList.Add(customer);
            }
        }

        Customer[] customerArray = customerList.ToArray();

        foreach (var item in customerArray)
        {
            Debug.Log("arrival time: "+ item.arrivalTime + " service time: " + item.serviceTime);
        }        
    }
}
