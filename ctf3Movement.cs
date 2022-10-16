using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class CTF3Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_door_t5" || g.name == "func_door_t4" || g.name == "func_door_t3" || g.name == "func_door_bluesidelift" || g.name == "func_door_bluesidelift2" || g.name == "func_door_bluesidelift3")
                {
                    float y = g.transform.position.y + 6.05f;
                    if (g.name == "func_door_t5" || g.name == "func_door_bluesidelift3")
                    {
                        y = g.transform.position.y + 6.47f;
                    }
                    else if (g.name == "func_door_t4" || g.name == "func_door_bluesidelift")
                    {
                        y = g.transform.position.y + 12.75f;
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

