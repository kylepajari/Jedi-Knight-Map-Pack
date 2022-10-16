using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class CTF1Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door") || g.name.Contains("func_plat"))
                {
                    float y = g.transform.position.y;
                    if (g.name == "func_door_bluedowndoor" || g.name == "func_door_blueupdoor" || g.name == "func_door_t72" || g.name == "func_door_t87")
                    {
                        y = g.transform.position.y + 22.75f;
                    }
                    else if (g.name == "func_plat_midlift1" || g.name == "func_plat_t56")
                    {
                        y = g.transform.position.y + 9.75f;
                    }
                    else if (g.name == "func_door_blueriser" || g.name == "func_door_t89")
                    {
                        y = g.transform.position.y + 22.33f;
                    }
                    else if (g.name == "func_door_bluelift2" || g.name == "func_door_t175")
                    {
                        y = g.transform.position.y + 4.87f;
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

