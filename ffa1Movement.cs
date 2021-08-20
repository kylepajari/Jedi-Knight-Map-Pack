using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFA1Movement : LevelModule
    {
        public List<GameObject> bigPlats = new List<GameObject>();
        public List<Vector3> bigPlatStartPos = new List<Vector3>();
        public List<Vector3> bigPlatEndPos = new List<Vector3>();
        //public List<bool> bigPlatPlayerInside = new List<bool>();
        public List<bool> bigPlatEnemyInside = new List<bool>();
        public float bigPlatMoveTime;
        public List<bool> bigPlatsAtTop = new List<bool>();
        public List<BoxCollider> bigDetectionBottoms = new List<BoxCollider>();

        public static List<GameObject> plats = new List<GameObject>();
        public List<Vector3> platsStartPos = new List<Vector3>();
        public List<Vector3> platsEndPos = new List<Vector3>();
        public static List<List<GameObject>> platsPersonsInside = new List<List<GameObject>>();
        public float platMoveTime;
        public List<bool> platsAtBottom = new List<bool>();
        public List<GameObject> platPeopleToRemove = new List<GameObject>();

        public GameObject player;

        public AudioClip bigPlatStart;
        public AudioClip bigPlatMoving;
        public AudioClip bigPlatStop;
        public List<bool> playBigStopSound = new List<bool>();
        public static Timer platTimer = new Timer(1000);
        public static bool canMove;

        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {
            // Called when the level load
            //initialized = true; // Set it to true when your script are loaded

            bigPlatStart = GameObject.Find("hugeplat_start").GetComponent<AudioSource>().clip;
            bigPlatMoving = GameObject.Find("hugeplat_move_lp").GetComponent<AudioSource>().clip;
            bigPlatStop = GameObject.Find("hugeplat_stop").GetComponent<AudioSource>().clip;

            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name == "func_door_dplat1" || g.name == "func_door_t63")
                {
                    bigPlats.Add(g);
                    //bigPlatPlayerInside.Add(false);
                    bigPlatEnemyInside.Add(false);
                    playBigStopSound.Add(false);
                    bigPlatsAtTop.Add(true);
                    Transform PlatformHolder = g.transform.parent;
                    BoxCollider bottom = PlatformHolder.Find("DetectBottom").GetComponent<BoxCollider>();
                    bigDetectionBottoms.Add(bottom);
                    g.AddComponent<AudioSource>();
                    g.GetComponent<AudioSource>().clip = bigPlatStart;
                    g.GetComponent<AudioSource>().playOnAwake = false;
                    g.GetComponent<AudioSource>().volume = 1;
                    g.GetComponent<AudioSource>().spatialBlend = 0.6f;
                    g.GetComponent<AudioSource>().maxDistance = 5f;
                    bigPlatStartPos.Add(g.transform.position);
                    float y;
                    if (g.name == "func_door_dplat1")
                    {
                        y = g.transform.position.y - 22.75f;
                    }
                    else
                    {
                        y = g.transform.position.y - 24.3f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    bigPlatEndPos.Add(topOfMovement);
                }
                else if (g.name == "func_door_t67" || g.name == "func_door_t90_4" || g.name == "func_door_t118")
                {
                    plats.Add(g);
                    g.AddComponent<FFA1Triggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    platsPersonsInside.Add(dict[g.name]);
                    platsAtBottom.Add(true);
                    Transform PlatformHolder = g.transform.parent;
                    platsStartPos.Add(g.transform.position);


                    float y;
                    if (g.name == "func_door_t67")
                    {
                        y = g.transform.position.y + 12.9f;
                    }
                    else if (g.name == "func_door_t90_4")
                    {
                        y = g.transform.position.y + 9.2f;
                    }
                    else if (g.name == "func_door_t118")
                    {
                        y = g.transform.position.y + 10.864f;
                    }
                    else
                    {
                        y = g.transform.position.y + 12f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    platsEndPos.Add(topOfMovement);
                }
            }

            bigPlatMoveTime = 1f;
            platMoveTime = 2.0f;
            canMove = false;
            return base.OnLoadCoroutine(levelDefinition);
        }

        public override void Update(Level levelDefinition)
        {
            if (player == null)
            {
                object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
                foreach (object o in obj)
                {
                    GameObject g = (GameObject)o;
                    if (g.name.Contains("PlayerDefault"))
                    {
                        player = g;
                    }
                }
            }

            if (player != null)
            {
                //Big Platforms
                for (int i = 0; i < bigPlats.Count; i++)
                {

                    if (Creature.list.Count != 0)
                    {
                        foreach (Creature c in Creature.list)
                        {
                            bigPlatEnemyInside[i] = TargetInsideCollider(c.transform.position, bigPlats[i].GetComponentInChildren<BoxCollider>());
                        }
                    }

                    if (bigPlats[i].transform.position == bigPlatStartPos[i] && playBigStopSound[i])
                    {
                        bigPlatsAtTop[i] = true;
                        bigPlats[i].GetComponent<AudioSource>().Stop();
                        bigPlats[i].GetComponent<AudioSource>().clip = bigPlatStop;
                        bigPlats[i].GetComponent<AudioSource>().loop = false;
                        bigPlats[i].GetComponent<AudioSource>().Play();
                        playBigStopSound[i] = false;
                    }
                    else if (bigPlats[i].transform.position == bigPlatEndPos[i] && playBigStopSound[i])
                    {
                        bigPlatsAtTop[i] = false;
                        bigPlats[i].GetComponent<AudioSource>().Stop();
                        bigPlats[i].GetComponent<AudioSource>().clip = bigPlatStop;
                        bigPlats[i].GetComponent<AudioSource>().loop = false;
                        bigPlats[i].GetComponent<AudioSource>().Play();
                        playBigStopSound[i] = false;
                    }

                    //bigPlatPlayerInside[i] = TargetInsideCollider(player.transform.position, bigPlats[i].GetComponentInChildren<BoxCollider>());

                    // if player or enemy not standing on platform directly
                    if (!bigPlatEnemyInside[i])
                    {

                        //if player is at bottom and platform is at top
                        if (bigDetectionBottoms[i].bounds.Contains(player.transform.position) && bigPlats[i].transform.position != bigPlatEndPos[i])
                        {
                            playBigStopSound[i] = true;
                            //call platform down
                            var pos = (bigPlatMoveTime * Time.deltaTime);
                            //for each bigPlat, move its position closer to end position over each frame
                            bigPlats[i].transform.position = Vector3.MoveTowards(bigPlats[i].transform.position, bigPlatEndPos[i], pos);
                            //play moving sound
                            if (!bigPlats[i].GetComponent<AudioSource>().isPlaying)
                            {
                                bigPlats[i].GetComponent<AudioSource>().clip = bigPlatMoving;
                                bigPlats[i].GetComponent<AudioSource>().loop = true;
                                bigPlats[i].GetComponent<AudioSource>().Play();
                            }
                        }
                        //if player is outside of detection is not at top, return to top
                        else if (!bigDetectionBottoms[i].bounds.Contains(player.transform.position) && bigPlats[i].transform.position != bigPlatStartPos[i])
                        {
                            playBigStopSound[i] = true;
                            //call platform up
                            var pos = (bigPlatMoveTime * Time.deltaTime);
                            //for each bigPlat, move its position closer to end position over each frame
                            bigPlats[i].transform.position = Vector3.MoveTowards(bigPlats[i].transform.position, bigPlatStartPos[i], pos);
                            //play moving sound
                            if (!bigPlats[i].GetComponent<AudioSource>().isPlaying)
                            {
                                bigPlats[i].GetComponent<AudioSource>().clip = bigPlatMoving;
                                bigPlats[i].GetComponent<AudioSource>().loop = true;
                                bigPlats[i].GetComponent<AudioSource>().Play();
                            }
                        }
                    }
                    //otherwise if player is on platform
                    else
                    {

                        if (bigPlatsAtTop[i] && bigPlats[i].transform.position != bigPlatEndPos[i])
                        {
                            var pos = (bigPlatMoveTime * Time.deltaTime);
                            //for each bigPlat, move its position closer to end position over each frame
                            bigPlats[i].transform.position = Vector3.MoveTowards(bigPlats[i].transform.position, bigPlatEndPos[i], pos);
                            //play moving sound
                            if (!bigPlats[i].GetComponent<AudioSource>().isPlaying)
                            {
                                bigPlats[i].GetComponent<AudioSource>().clip = bigPlatMoving;
                                bigPlats[i].GetComponent<AudioSource>().loop = true;
                                bigPlats[i].GetComponent<AudioSource>().Play();
                            }
                        }
                        if (!bigPlatsAtTop[i] && bigPlats[i].transform.position != bigPlatStartPos[i])
                        {
                            var pos = (bigPlatMoveTime * Time.deltaTime);
                            //for each bigPlat, move its position closer to end position over each frame
                            bigPlats[i].transform.position = Vector3.MoveTowards(bigPlats[i].transform.position, bigPlatStartPos[i], pos);
                            //play moving sound
                            if (!bigPlats[i].GetComponent<AudioSource>().isPlaying)
                            {
                                bigPlats[i].GetComponent<AudioSource>().clip = bigPlatMoving;
                                bigPlats[i].GetComponent<AudioSource>().loop = true;
                                bigPlats[i].GetComponent<AudioSource>().Play();
                            }
                        }
                    }
                }
            }


            //Small Platforms
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

        public override void OnUnload(Level levelDefinition)
        {
            // Called when the level unload
            bigPlats.Clear();
            bigPlatStartPos.Clear();
            bigPlatEndPos.Clear();
            //bigPlatPlayerInside.Clear();
            bigPlatsAtTop.Clear();
            bigDetectionBottoms.Clear();
            playBigStopSound.Clear();

            plats.Clear();
            platsStartPos.Clear();
            platsEndPos.Clear();
            platsPersonsInside.Clear();
            platsAtBottom.Clear();
            platPeopleToRemove.Clear();
        }
    }

    public class FFA1Triggers : ColliderListener
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
                if (transform.name == "func_door_t67" || transform.name == "func_door_t90_4" || transform.name == "func_door_t118")
                {
                    int i = FFA1Movement.plats.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);
                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFA1Movement.platsPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFA1Movement.platsPersonsInside[i].Add(person);
                        FFA1Movement.canMove = false;
                        FFA1Movement.SetTimer();
                    }
                }
            }
        }
    }
}

