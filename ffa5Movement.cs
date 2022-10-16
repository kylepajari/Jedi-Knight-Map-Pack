using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFA5Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door"))
                {
                    float y = g.transform.position.y + 2.8f;
                    MoveableObjects.AddDoor(g, y);
                }
                else if (g.name.Contains("func_plat") || g.name == "func_train")
                {
                    float y = g.transform.position.y;
                    if (g.name == "func_plat")
                    {
                        y = g.transform.position.y + 16.66f;
                    }
                    else if (g.name == "func_plat_1")
                    {
                        y = g.transform.position.y + 13.0f;
                    }
                    else if (g.name == "func_plat_2")
                    {
                        y = g.transform.position.y + 16.25f;
                    }
                    else if (g.name == "func_plat_3")
                    {
                        y = g.transform.position.y - 16.25f;
                    }
                    else if (g.name == "func_train")
                    {
                        y = g.transform.position.y + 31.67f;
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

