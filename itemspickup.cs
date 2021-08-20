using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace MapScriptsJKO
{
    public class itemspickup : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            //Add collider listener to item
            item.gameObject.AddComponent<ColliderListener>();
        }
    }
}
