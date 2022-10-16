using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFA3Movement : LevelModule
    {

        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_plat" || g.name == "func_door_lift1" || g.name == "func_door_lift2")
                {
                    float y = g.transform.position.y;
                    if (g.name == "func_plat")
                    {
                        y = g.transform.position.y + 7.7f;
                    }
                    else if (g.name == "func_door_lift2")
                    {
                        y = g.transform.position.y + 7.2f;
                    }
                    else
                    {
                        y = g.transform.position.y + 7.3f;
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

