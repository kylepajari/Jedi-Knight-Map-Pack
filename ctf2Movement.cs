using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class CTF2Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_door" || g.name == "func_door_1" || g.name == "func_door_2" || g.name == "func_door_3")
                {
                    float x = g.transform.position.x + 2f;
                    if (g.name == "func_door" || g.name == "func_door_2")
                    {
                        x = g.transform.position.x - 2f;
                    }
                    else if (g.name == "func_door_1" || g.name == "func_door_3")
                    {
                        x = g.transform.position.x + 2f;
                    }
                    MoveableObjects.AddDoor(g, g.transform.position.y, x);
                }
                else if (g.name == "func_plat" || g.name == "func_plat_1" || g.name == "func_door_t215" || g.name == "func_door_t216")
                {
                    float y = g.transform.position.y + 9.75f;
                    if (g.name == "func_door_t215" || g.name == "func_door_t216")
                    {
                        y = g.transform.position.y - 9.75f;
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

