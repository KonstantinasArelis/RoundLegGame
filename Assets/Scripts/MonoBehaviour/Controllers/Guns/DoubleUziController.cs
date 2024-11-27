using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class DoubleUziController : MonoBehaviour, IFireable
{

    
    public GameObject uzi1;
    public GameObject uzi2;

    public IFireable uzi1Controller;
    public IFireable uzi2Controller;

    void Start()
    {
        uzi1Controller = uzi1.GetComponent<IFireable>();
        uzi2Controller = uzi2.GetComponent<IFireable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        uzi1Controller.Fire();
        uzi2Controller.Fire();
    }
}
