using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScriptsJKO
{
    public class FFADeathstarMovement : LevelModule
    {
        //Doors variables
        public static List<GameObject> doors = new List<GameObject>();
        public List<Vector3> doorStartPos = new List<Vector3>();
        public List<Vector3> doorEndPos = new List<Vector3>();
        public static List<List<GameObject>> doorPersonsInside = new List<List<GameObject>>();
        public float doorMoveTime;
        public List<GameObject> peopleToRemove = new List<GameObject>();

        public static List<GameObject> plats = new List<GameObject>();
        public List<Vector3> platsStartPos = new List<Vector3>();
        public List<Vector3> platsEndPos = new List<Vector3>();
        public static List<List<GameObject>> platsPersonsInside = new List<List<GameObject>>();
        public float platMoveTime;
        public List<bool> platsAtBottom = new List<bool>();
        public List<GameObject> platPeopleToRemove = new List<GameObject>();

        public static List<GameObject> garPlats = new List<GameObject>();
        public List<Vector3> garPlatsStartPos = new List<Vector3>();
        public List<Vector3> garPlatsEndPos = new List<Vector3>();
        public float garPlatMoveTimeIn;
        public float garPlatMoveTimeOut;

        public AudioClip garPlatMoving;
        public AudioSource garCrush;
        public bool garReachedEnd;
        public static Timer garWaitTimer = new Timer(3000);
        public static bool garCanMove;

        public static Timer platTimer = new Timer(1000);
        public static bool canMove;

        public override System.Collections.IEnumerator OnLoadCoroutine()
        {
            // Called when the level load
            //initialized = true; // Set it to true when your script are loaded

            //set up sound sources
            garPlatMoving = GameObject.Find("hugeplat_move_lp").GetComponent<AudioSource>().clip;
            garCrush = GameObject.Find("garbage_crush").GetComponent<AudioSource>();
            garCrush.volume = 1.0f;
            garCrush.spatialBlend = 1.0f;
            garCrush.rolloffMode = AudioRolloffMode.Linear;
            garCrush.minDistance = 6.0f;
            garCrush.maxDistance = 35.0f;
            garReachedEnd = false;
            canMove = false;
            garCanMove = true;

            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door")  && !g.GetComponent<BoxCollider>())
                {
                    doors.Add(g);
                    g.AddComponent<FFADeathstarTriggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    doorPersonsInside.Add(dict[g.name]);

                    doorStartPos.Add(g.transform.position);
                    float y = g.transform.position.y + 3.3f;
                    if(g.name == "func_door")
                    {
                        y = g.transform.position.y + 2.3f;
                    }
                    else if (g.name == "func_door_1")
                    {
                        y = g.transform.position.y + 3.1f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    doorEndPos.Add(topOfMovement);
                }

                if (g.name.Contains("func_plat"))
                {
                    plats.Add(g);
                    g.AddComponent<FFADeathstarTriggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    platsPersonsInside.Add(dict[g.name]);

                    platsAtBottom.Add(true);
                    Transform PlatformHolder = g.transform.parent;

                    platsStartPos.Add(g.transform.position);
                    float y = 1.0f;
                    if (g.name == "func_plat" || g.name == "func_plat_1")
                    {
                        y = g.transform.position.y + 3.2f;
                    }
                    if (g.name == "func_plat_t1")
                    {
                        y = g.transform.position.y + 5.7f;
                    }
                    if (g.name == "func_plat_t2")
                    {
                        y = g.transform.position.y - 5.7f;
                    }
                    else if (g.name == "func_plat_t3")
                    {
                        y = g.transform.position.y + 9.75f;
                    }
                    else if (g.name == "func_plat_t4")
                    {
                        y = g.transform.position.y + 5.65f;
                    }
                    else if (g.name == "func_plat_t5")
                    {
                        y = g.transform.position.y - 6.5f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    platsEndPos.Add(topOfMovement);
                }

                if (g.name.Contains("func_t6") || g.name.Contains("func_t6_1"))
                {
                    garPlats.Add(g);
                    garPlatsStartPos.Add(g.transform.position);
                    float z = 0f;
                    if (g.name == "func_t6_1")
                    {
                        z = g.transform.position.z - 4f;
                    }
                    if (g.name == "func_t6")
                    {
                        z = g.transform.position.z + 4f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, g.transform.position.x, z);
                    garPlatsEndPos.Add(topOfMovement);
                }

            }

            doorMoveTime = 4f;
            platMoveTime = 2f;
            garPlatMoveTimeOut = 0.5f;
            garPlatMoveTimeIn = 3f;
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
        public static void SetGarTimer()
        {
            // Create a timer with a 2 second interval.
            // Hook up the Elapsed event for the timer. 
            garWaitTimer.Start();
            garWaitTimer.Elapsed += OnTimedEvent_Garbage;
            garWaitTimer.AutoReset = false;
        }
        private static void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            platTimer.Stop();
            canMove = true;
        }

        private static void OnTimedEvent_Garbage(System.Object source, ElapsedEventArgs e)
        {
            garWaitTimer.Stop();
            garCanMove = true;
        }

        public override void Update()
        {

            //doors
            for (int i = 0; i < doors.Count; i++)
            {
                if (doorPersonsInside[i].Count > 0 && doors[i].transform.position != doorEndPos[i])
                {
                    var pos = (doorMoveTime * Time.deltaTime);
                    //for each door, move its position closer to end position over each frame
                    doors[i].transform.position = Vector3.MoveTowards(doors[i].transform.position, doorEndPos[i], pos);
                }
                else if (doorPersonsInside[i].Count == 0 && doors[i].transform.position != doorStartPos[i])
                {
                    var pos = (doorMoveTime * Time.deltaTime);
                    //for each door, move its position closer to start position over each frame
                    doors[i].transform.position = Vector3.MoveTowards(doors[i].transform.position, doorStartPos[i], pos);
                }
                //if door trigger has enemies inside, continue checking that they are still inside, and if not remove from list
                else if (doorPersonsInside[i].Count > 0 && doors[i].transform.position == doorEndPos[i])
                {
                    foreach (GameObject personInside in doorPersonsInside[i])
                    {
                        //bool containsPerson = doors[i].GetComponentInChildren<BoxCollider>().bounds.Contains(personInside.transform.position);
                        bool containsPerson = TargetInsideCollider(personInside.transform.position, doors[i].GetComponentInChildren<BoxCollider>());
                        if (containsPerson)
                        {
                            //Debug.Log("Person is still inside");
                        }
                        else
                        {
                            //Debug.Log("Person left, add to list of people to be removed");
                            peopleToRemove.Add(personInside);
                        }
                    }
                    //check if list of people to remove is greater than 0, if so remove those people from the door list
                    if (peopleToRemove.Count > 0)
                    {
                        foreach (GameObject person2 in peopleToRemove)
                        {
                            doorPersonsInside[i].Remove(person2);
                            if (doorPersonsInside[i].Count == 0)
                            {
                                doors[i].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                    }
                    peopleToRemove.Clear();
                }
            }


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



            //garbage compactor
            for (int i = 0; i < garPlats.Count; i++)
            {
                if (garCanMove)
                {

                    if (!garReachedEnd && garPlats[i].transform.position != garPlatsEndPos[i])
                    {
                        var pos = (garPlatMoveTimeIn * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        garPlats[i].transform.position = Vector3.MoveTowards(garPlats[i].transform.position, garPlatsEndPos[i], pos);
                    }
                    else if (garReachedEnd && garPlats[i].transform.position != garPlatsStartPos[i])
                    {
                        var pos = (garPlatMoveTimeOut * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        garPlats[i].transform.position = Vector3.MoveTowards(garPlats[i].transform.position, garPlatsStartPos[i], pos);
                    }

                    if (garPlats[i].transform.position == garPlatsEndPos[i])
                    {
                        garReachedEnd = true;
                        garCrush.Play();
                        garCanMove = false;
                        //force both compactors into full close position
                        garPlats[0].transform.position = garPlatsEndPos[0];
                        garPlats[1].transform.position = garPlatsEndPos[1];
                        SetGarTimer();
                    }
                    else if (garPlats[i].transform.position == garPlatsStartPos[i] && garReachedEnd)
                    {
                        garReachedEnd = false;
                        garCanMove = false;
                        //force both compactors into full opem position
                        garPlats[0].transform.position = garPlatsStartPos[0];
                        garPlats[1].transform.position = garPlatsStartPos[1];
                        SetGarTimer();
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
            doors.Clear();
            doorPersonsInside.Clear();
            doorStartPos.Clear();
            doorEndPos.Clear();
            plats.Clear();
            platsPersonsInside.Clear();
            platsStartPos.Clear();
            platsEndPos.Clear();
            garPlats.Clear();
            garPlatsStartPos.Clear();
            garPlatsEndPos.Clear();
            peopleToRemove.Clear();
            platPeopleToRemove.Clear();
        }
    }

    public class FFADeathstarTriggers : ColliderListener
    {

        public override void OnTriggerEnter(Collider col)
        {
            GameObject person = null;
            if(col.GetComponentInParent<Creature>() != null)
            {
                if (Creature.allActive.Contains(col.GetComponentInParent<Creature>())){
                    int i = Creature.allActive.IndexOf(col.GetComponentInParent<Creature>());
                    person = Creature.allActive[i].gameObject;
                }
            }
            if (person != null)
            {               
                //doors
                if (transform.name.Contains("func_door"))
                {
                    int i = FFADeathstarMovement.doors.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);

                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFADeathstarMovement.doorPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFADeathstarMovement.doorPersonsInside[i].Add(person);
                        if (FFADeathstarMovement.doorPersonsInside[i].Count > 0)
                        {
                            FFADeathstarMovement.doors[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }
                }

                //plats
                if (transform.name.Contains("func_plat"))
                {
                    int i = FFADeathstarMovement.plats.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);

                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFADeathstarMovement.platsPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFADeathstarMovement.platsPersonsInside[i].Add(person);
                        FFADeathstarMovement.canMove = false;
                        FFADeathstarMovement.SetTimer();
                    }

                }
            }
        }
    }
}
