using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapScripts
{
    public class ColliderBridge : MonoBehaviour
    {
        ColliderListener _listener;
        public void Initialize(ColliderListener l)
        {
            _listener = l;
        }
        void OnTriggerEnter(Collider other)
        {
            _listener.OnTriggerEnter(other);
        }
        void OnTriggerExit(Collider other)
        {
            _listener.OnTriggerExit(other);
        }
    }
}
