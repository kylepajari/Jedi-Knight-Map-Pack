using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class Duel3Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_door_dplat1" || g.name == "func_door_t29")
                {
                    float y = g.transform.position.y + 6.9f;
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

