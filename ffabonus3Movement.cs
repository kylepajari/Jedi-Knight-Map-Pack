using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFAbonus3Movement : LevelModule
    {

        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door"))
                {
                    float y = g.transform.position.y + 7f;
                    if (g.name == "func_door_t496")
                    {
                        y = g.transform.position.y + 7.52f;
                    }
                    else if (g.name == "func_door_t497")
                    {
                        y = g.transform.position.y + 14.02f;
                    }
                    else if (g.name == "func_door_t498")
                    {
                        y = g.transform.position.y + 14.437f;
                    }
                    else if (g.name == "func_door_t499" || g.name == "func_door_t501")
                    {
                        y = g.transform.position.y + 6.52f;
                    }
                    else if (g.name == "func_door_t500")
                    {
                        y = g.transform.position.y + 8.53f;
                    }
                    else if (g.name == "func_door_t502")
                    {
                        y = g.transform.position.y + 5.69f;
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

