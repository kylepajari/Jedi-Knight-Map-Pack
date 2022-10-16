using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class CTF4Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_door_lift" || g.name == "func_door_lift2" || g.name == "func_door_lift3" || g.name == "func_door_t158" || g.name == "func_door_t175" || g.name == "func_door_t217")
                {
                    float y = g.transform.position.y + 9.73f;
                    if (g.name == "func_door_lift2" || g.name == "func_door_t175")
                    {
                        y = g.transform.position.y + 16.16f;
                    }
                    else if (g.name == "func_door_lift3" || g.name == "func_door_t158")
                    {
                        y = g.transform.position.y + 15.84f;
                    }
                    MoveableObjects.AddPlatform(g, y); 
                }
            }
            MoveableObjects.SetMembers();

            return base.OnLoadCoroutine();
        }

        public override void Update()
        {
            MoveableObjects.MoveObjects();
        }

        public override void OnUnload()
        {
            // Called when the level unload
            MoveableObjects.ClearLists();
        }
    }

}
