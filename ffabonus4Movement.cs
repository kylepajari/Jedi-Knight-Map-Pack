using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFAbonus4Movement : LevelModule
    {
        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                float y = g.transform.position.y;
                if (g.name == "func_door")
                {
                    y = g.transform.position.y + 5.7f;
                    MoveableObjects.AddDoor(g, y);
                }
                else if (g.name == "func_door_1")
                {
                    y = g.transform.position.y + 3.0f;
                    MoveableObjects.AddDoor(g, y);
                }
                else if (g.name == "func_door_2")
                {
                    y = g.transform.position.y + 7.0f;
                    MoveableObjects.AddDoor(g, y);
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

