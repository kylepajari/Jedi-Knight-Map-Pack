using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFAbonus3Movement : LevelModule
    {
        public static List<GameObject> plats = new List<GameObject>();
        public List<Vector3> platsStartPos = new List<Vector3>();
        public List<Vector3> platsEndPos = new List<Vector3>();
        public static List<List<GameObject>> platsPersonsInside = new List<List<GameObject>>();
        public float platMoveTime;
        public List<bool> platsAtBottom = new List<bool>();
        public List<GameObject> platPeopleToRemove = new List<GameObject>();
        public static Timer platTimer = new Timer(1000);
        public static bool canMove;

        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {

            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door"))
                {
                    plats.Add(g);
                    g.AddComponent<ColliderListener>();
                    g.AddComponent<FFAbonus3Triggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    platsPersonsInside.Add(dict[g.name]);
                    platsAtBottom.Add(true);
                    platsStartPos.Add(g.transform.position);
                    float y = g.transform.position.y;
                    y = g.transform.position.y + 7f;
                    if (g.name == "func_door_t496")
                    {
                        y = g.transform.position.y + 7.52f;
                    }
                    else if (g.name == "func_door_t497")
                    {
                        y = g.transform.position.y + 14.02f;
                    }
                    else if (g.name == "func_door_t498")
                    {
                        y = g.transform.position.y + 14.437f;
                    }
                    else if (g.name == "func_door_t499" || g.name == "func_door_t501")
                    {
                        y = g.transform.position.y + 6.52f;
                    }
                    else if (g.name == "func_door_t500")
                    {
                        y = g.transform.position.y + 8.53f;
                    }
                    else if (g.name == "func_door_t502")
                    {
                        y = g.transform.position.y + 5.69f;
                    }


                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    platsEndPos.Add(topOfMovement);
                }
            }
            platMoveTime = 2.0f;
            canMove = false;
            return base.OnLoadCoroutine(levelDefinition);
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
        public static bool TargetInsideCollider(Vector3 target, BoxCollider box)
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

        public override void Update(Level levelDefinition)
        {
            //platforms
            for (int i = 0; i < plats.Count; i++)
            {
                //if timer has elapsed, can move platform
                if (canMove)
                {
                    if (platsPersonsInside[i].Count > 0 && plats[i].transform.position != platsEndPos[i])
                    {
                        var pos = (platMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        plats[i].transform.position = Vector3.MoveTowards(plats[i].transform.position, platsEndPos[i], pos);
                        if (!plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Play();
                        }

                    }
                    else if (platsPersonsInside[i].Count == 0 && plats[i].transform.position != platsStartPos[i])
                    {
                        var pos = (platMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        plats[i].transform.position = Vector3.MoveTowards(plats[i].transform.position, platsStartPos[i], pos);
                        if (!plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }
                    else if (platsPersonsInside[i].Count > 0 && plats[i].transform.position == platsEndPos[i])
                    {
                        foreach (GameObject personInside in platsPersonsInside[i])
                        {
                            //bool containsPerson = doors[i].GetComponentInChildren<BoxCollider>().bounds.Contains(personInside.transform.position);
                            bool containsPerson = TargetInsideCollider(personInside.transform.position, plats[i].GetComponentInChildren<BoxCollider>());
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

                    if (plats[i].transform.position == platsStartPos[i] || plats[i].transform.position == platsEndPos[i])
                    {
                        plats[i].GetComponentInChildren<AudioSource>().Stop();
                    }
                }
            }
        }

        public override void OnUnload(Level levelDefinition)
        {
            // Called when the level unload
            plats.Clear();
            platsStartPos.Clear();
            platsEndPos.Clear();
            platsAtBottom.Clear();
            platsPersonsInside.Clear();
            platPeopleToRemove.Clear();
        }
    }

    public class FFAbonus3Triggers : ColliderListener
    {

        public override void OnTriggerEnter(Collider col)
        {
            GameObject person = null;
            if (col.GetComponentInParent<Creature>() != null)
            {
                if (Creature.list.Contains(col.GetComponentInParent<Creature>()))
                {
                    int i = Creature.list.IndexOf(col.GetComponentInParent<Creature>());
                    person = Creature.list[i].gameObject;
                }
            }
            if (person != null)
            {
                //plats
                if (transform.name.Contains("func_door"))
                {
                    int i = FFAbonus3Movement.plats.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);
                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFAbonus3Movement.platsPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFAbonus3Movement.platsPersonsInside[i].Add(person);
                        FFAbonus3Movement.canMove = false;
                        FFAbonus3Movement.SetTimer();
                    }

                }
            }
        }
    }
}

