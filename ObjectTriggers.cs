using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ThunderRoad;

namespace MapScripts
{
    class ObjectTriggers: ColliderListener
    {
        public string type = "default";
        public override void OnTriggerEnter(Collider col)
        {
            GameObject person = null;
            if (col.GetComponentInParent<Creature>() != null)
            {
                if (Creature.allActive.Contains(col.GetComponentInParent<Creature>()))
                {
                    int i = Creature.allActive.IndexOf(col.GetComponentInParent<Creature>());
                    person = Creature.allActive[i].gameObject;
                }
            }
            if (person != null)
            {
                MoveableObjects.AddPersonToObjectList(person, transform, type);
            }
        }
    }
}
