using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CheatManagerZer0
{
    public class SeaTruckOverDrive : MonoBehaviour
    {
        public SeaTruckOverDrive Instance { get; private set; }
        public SeaTruckSegment ThisSeaTruckSegment { get; private set; }
        
        public void Awake()
        {
            Instance = gameObject.GetComponent<SeaTruckOverDrive>();
            ThisSeaTruckSegment = Instance.GetComponent<SeaTruckSegment>();
        }

        public void Start()
        {
            print("[CheatManager] Component Added");
        }

    }
}
