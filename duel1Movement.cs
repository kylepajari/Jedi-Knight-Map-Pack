using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace MapScripts
{
    public class Duel1Movement : LevelModule
    {
        public AnimationCurve myCurveX;
        public AnimationCurve myCurveY;
        public AnimationCurve myCurveZ;

        public GameObject CloudCar;


        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {
            // Called when the level load
            //initialized = true; // Set it to true when your script are loaded

            CloudCar = GameObject.Find("Bob");
            return base.OnLoadCoroutine(levelDefinition);
        }

        public override void Update(Level levelDefinition)
        {
            Vector3 pos = CloudCar.transform.position;
            float newY = Mathf.Sin(Time.time * 2f);
            CloudCar.transform.position = new Vector3(pos.x, newY, pos.z) * 1f;
        }

        public override void OnUnload(Level levelDefinition)
        {
            // Called when the level unload
            //initialized = false;
        }
    }
}

