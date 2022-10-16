using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScriptsJKO
{
    public class DuelCarbonMovement : LevelModule
    {
        public static List<GameObject> Plats = new List<GameObject>();
        public List<Vector3> PlatStartPos = new List<Vector3>();
        public List<Vector3> PlatEndPos = new List<Vector3>();
        public static List<List<GameObject>> platsPersonsInside = new List<List<GameObject>>();
        public float PlatMoveTime;
        public List<bool> PlatsAtTop = new List<bool>();
        public List<GameObject> platPeopleToRemove = new List<GameObject>();

        public AudioClip PlatMoving;

        public static Timer platTimer = new Timer(1000);
        public static bool canMove;


        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            // Called when the level load
            //initialized = true; // Set it to true when your script are loaded

            PlatMoving = GameObject.Find("normalplat_move").GetComponent<AudioSource>().clip;
            canMove = false;
            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_plat")
                {
                    Plats.Add(g);
                    g.AddComponent<DuelCarbonTriggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    platsPersonsInside.Add(dict[g.name]);
                    PlatsAtTop.Add(true);
                    g.AddComponent<AudioSource>();
                    g.GetComponent<AudioSource>().clip = PlatMoving;
                    g.GetComponent<AudioSource>().playOnAwake = false;
                    g.GetComponent<AudioSource>().volume = 1;
                    g.GetComponent<AudioSource>().spatialBlend = 0.6f;
                    g.GetComponent<AudioSource>().maxDistance = 5f;
                    PlatStartPos.Add(g.transform.position);
                    float y;
                    y = g.transform.position.y + 3.25f;
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    PlatEndPos.Add(topOfMovement);
                }
            }

            PlatMoveTime = 2f;

            return base.OnLoadCoroutine();

        }

        public static void SetTimer()
        {
            // Create a timer with a 1 second interval.
            // Hook up the Elapsed event for the timer. 
            platTimer.Start();
            platTimer.Elapsed += OnTimedEvent;
            platTimer.AutoReset = false;
        }
        private static void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            platTimer.Stop();
            canMove = true;
        }

        public override void Update()
        {

            //Platforms
            for (int i = 0; i < Plats.Count; i++)
            {
                if (canMove)
                {
                    if (platsPersonsInside[i].Count > 0 && Plats[i].transform.position != PlatEndPos[i])
                    {
                        var pos = (PlatMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        Plats[i].transform.position = Vector3.MoveTowards(Plats[i].transform.position, PlatEndPos[i], pos);
                        if (!Plats[i].GetComponent<AudioSource>().isPlaying)
                        {
                            Plats[i].GetComponent<AudioSource>().Play();
                        }

                    }
                    else if (platsPersonsInside[i].Count == 0 && Plats[i].transform.position != PlatStartPos[i])
                    {
                        var pos = (PlatMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        Plats[i].transform.position = Vector3.MoveTowards(Plats[i].transform.position, PlatStartPos[i], pos);
                        if (!Plats[i].GetComponent<AudioSource>().isPlaying)
                        {
                            Plats[i].GetComponent<AudioSource>().Play();
                        }
                    }
                    else if (platsPersonsInside[i].Count > 0 && Plats[i].transform.position == PlatEndPos[i])
                    {
                        foreach (GameObject personInside in platsPersonsInside[i])
                        {
                            //bool containsPerson = doors[i].GetComponentInChildren<BoxCollider>().bounds.Contains(personInside.transform.position);
                            bool containsPerson = TargetInsideCollider(personInside.transform.position, Plats[i].GetComponentInChildren<BoxCollider>());
                            if (containsPerson)
                            {
                                //Debug.Log("Person is still inside");
                            }
                            else
                            {
                                //Debug.Log("Person left, add to list of people to be removed");
                                platPeopleToRemove.Add(personInside);
                            }
                        }
                        //check if list of people to remove is greater than 0, if so remove those people from the door list
                        if (platPeopleToRemove.Count > 0)
                        {
                            foreach (GameObject person2 in platPeopleToRemove)
                            {
                                platsPersonsInside[i].Remove(person2);
                                canMove = false;
                                SetTimer();
                            }
                        }
                        platPeopleToRemove.Clear();
                    }

                    if (Plats[i].transform.position == PlatStartPos[i] || Plats[i].transform.position == PlatEndPos[i])
                    {
                        Plats[i].GetComponent<AudioSource>().Stop();
                    }
                }
            }
        }

        bool TargetInsideCollider(Vector3 target, BoxCollider box)
        {
            target = box.transform.InverseTransformPoint(target);
            var c = box.center;
            var s = box.size;
            float X = s.x * 0.5f + target.x - c.x;
            if (X < 0 || X > s.x)
                return false;
            float Y = s.y * 0.5f + target.y - c.y;
            if (Y < 0 || Y > s.y)
                return false;
            float Z = s.z * 0.5f + target.z - c.z;
            if (Z < 0 || Z > s.z)
                return false;
            return true;
        }

        public override void OnUnload()
        {
            // Called when the level unload
            Plats.Clear();
            PlatStartPos.Clear();
            PlatEndPos.Clear();
            platsPersonsInside.Clear();
            PlatsAtTop.Clear();
            platPeopleToRemove.Clear();
            //initialized = false;
        }
    }

    public class DuelCarbonTriggers : ColliderListener
    {
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
                //plats
                if (transform.name.Contains("func_plat"))
                {
                    int i = DuelCarbonMovement.Plats.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);
                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (DuelCarbonMovement.platsPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        DuelCarbonMovement.platsPersonsInside[i].Add(person);
                        DuelCarbonMovement.canMove = false;
                        DuelCarbonMovement.SetTimer();
                    }

                }
            }
        }
    }
}
