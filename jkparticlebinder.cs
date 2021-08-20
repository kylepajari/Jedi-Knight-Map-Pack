using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ThunderRoad;
using UnityEngine.VFX;

namespace jkparticlebinder
{
    public class jkparticlebinder : LevelModule
    {
        public static GameObject playerCamera;
        public static GameObject effect;
        public static List<GameObject> barriers = new List<GameObject>();

        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {
            return base.OnLoadCoroutine(levelDefinition);
        }

        public override void Update(Level levelDefinition)
        {
            if (playerCamera == null)
            {
                playerCamera = GameObject.FindGameObjectWithTag("MainCamera");

                if (Application.loadedLevelName == "duel9" || Application.loadedLevelName == "ctf2")//hoth canyon and wasteland
                {
                    effect = GameObject.Find("Snow");
                    effect.GetComponent<VisualEffect>().enabled = false;
                }
                effect.transform.parent = playerCamera.transform;
                effect.transform.localPosition = playerCamera.transform.localPosition;

                object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
                foreach (object o in obj)
                {
                    GameObject g = (GameObject)o;
                    if (g.name.Contains("Barrier"))
                    {
                        barriers.Add(g);
                        g.AddComponent<ColliderListener>();
                    }
                }
            }
        }

        public override void OnUnload(Level levelDefinition)
        {
            // Called when the level unload
            playerCamera = null;
            effect = null;
            barriers.Clear();
        }

    }

    public class ColliderBridge : MonoBehaviour
    {
        ColliderListener _listener;
        public void Initialize(ColliderListener l)
        {
            _listener = l;
        }
        void OnTriggerExit(Collider other)
        {
            _listener.OnTriggerExit(other);
        }
    }

    public class ColliderListener : MonoBehaviour
    {
        void Awake()
        {
            // Check if Colider is in another GameObject
            Collider collider = transform.GetComponent<BoxCollider>();
            if (collider.gameObject != gameObject)
            {
                ColliderBridge cb = collider.gameObject.AddComponent<ColliderBridge>();
                cb.Initialize(this);
            }
        }

        public void OnTriggerExit(Collider col)
        {   
            if (col.transform.root.name.Contains("PlayerDefault"))
            {
                Vector3 playerPos = col.transform.root.position;
                //bool inside = transform.GetComponent<BoxCollider>().bounds.Contains(playerPos);
                //if snow is currently active and barrier crossed, disable (for indoors)
                if (jkparticlebinder.effect.GetComponent<VisualEffect>().enabled) 
                {
                    jkparticlebinder.effect.GetComponent<VisualEffect>().enabled = false;
                } 
                //else if snow if not active, activate snow (for outdoors)
                else if(!jkparticlebinder.effect.GetComponent<VisualEffect>().enabled)
                {
                    jkparticlebinder.effect.GetComponent<VisualEffect>().enabled = true;
                }
            }
        }
    }
}
