using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFAbonus1Movement : LevelModule
    {

        //Doors variables
        public static List<GameObject> doors = new List<GameObject>();
        public static List<Vector3> doorStartPos = new List<Vector3>();
        public static List<Vector3> doorEndPos = new List<Vector3>();
        public static List<List<GameObject>> doorPersonsInside = new List<List<GameObject>>();
        public float doorMoveTime;
        public static List<bool> doorsAtTop = new List<bool>();
        public List<GameObject> peopleToRemove = new List<GameObject>();

        public GameObject fan;

        //Circle platform 2 level variables
        public static List<GameObject> circlePlatforms = new List<GameObject>();
        public static List<Vector3> circleStartPos = new List<Vector3>();
        public static List<Vector3> circleEndPos = new List<Vector3>();
        public static List<List<GameObject>> circlePersonsInside = new List<List<GameObject>>();
        public float circleMoveTime;
        public static List<bool> circlesAtBottom = new List<bool>();
        public List<GameObject> circlePeopleToRemove = new List<GameObject>();

        //Rectangle platform 2 level variables
        public List<GameObject> rectPlatforms = new List<GameObject>();
        public List<Vector3> rectStartPos = new List<Vector3>();
        public List<Vector3> rectEndPos = new List<Vector3>();
        public List<bool> rectPlayerInside = new List<bool>();
        public float rectMoveTime;
        public List<bool> rectsAtTop = new List<bool>();
        public List<BoxCollider> rectDetectionBottoms = new List<BoxCollider>();

        public GameObject player;

        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {

            //set up sound sources
            AudioSource circlePlatStart = GameObject.Find("normalplat_start").GetComponent<AudioSource>();
            AudioSource rectPlatStart = GameObject.Find("platform_move_lp").GetComponent<AudioSource>();


            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door") && !g.name.Contains("plat") && !g.name.Contains("t"))
                {
                    doors.Add(g);
                    g.AddComponent<FFAbonus1Triggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    doorPersonsInside.Add(dict[g.name]);
                    doorStartPos.Add(g.transform.position);
                    float y = g.transform.position.y + 3f;
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    doorEndPos.Add(topOfMovement);
                }
                if (g.name.Contains("func_rotating"))
                {
                    fan = g;
                }
                else if (g.name == "func_door_plat1" || g.name == "func_door_plat2")
                {
                    circlePlatforms.Add(g);
                    g.AddComponent<FFAbonus1Triggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    circlePersonsInside.Add(dict[g.name]);

                    g.AddComponent<AudioSource>();
                    g.GetComponent<AudioSource>().clip = circlePlatStart.clip;
                    g.GetComponent<AudioSource>().playOnAwake = circlePlatStart.playOnAwake;
                    g.GetComponent<AudioSource>().volume = circlePlatStart.volume;
                    g.GetComponent<AudioSource>().spatialBlend = circlePlatStart.spatialBlend;
                    g.GetComponent<AudioSource>().maxDistance = circlePlatStart.maxDistance;
                    circleStartPos.Add(g.transform.position);
                    float y = g.transform.position.y + 7.51f;
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    circleEndPos.Add(topOfMovement);
                }
                else if (g.name.Contains("func_door_plat3") || g.name.Contains("func_door_t"))
                {
                    rectPlatforms.Add(g);
                    g.AddComponent<ColliderListener>();
                    g.AddComponent<FFAbonus1Triggers>();
                    rectPlayerInside.Add(false);
                    rectsAtTop.Add(true);
                    Transform PlatformHolder = g.transform.parent;
                    BoxCollider bottom = PlatformHolder.Find("DetectBottom").GetComponent<BoxCollider>();
                    rectDetectionBottoms.Add(bottom);
                    g.AddComponent<AudioSource>();
                    g.GetComponent<AudioSource>().clip = rectPlatStart.clip;
                    g.GetComponent<AudioSource>().playOnAwake = rectPlatStart.playOnAwake;
                    g.GetComponent<AudioSource>().volume = rectPlatStart.volume;
                    g.GetComponent<AudioSource>().spatialBlend = rectPlatStart.spatialBlend;
                    g.GetComponent<AudioSource>().maxDistance = rectPlatStart.maxDistance;
                    rectStartPos.Add(g.transform.position);
                    float y = g.transform.position.y - 7.1f;
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    rectEndPos.Add(topOfMovement);
                }
            }
            doorMoveTime = 4f;
            circleMoveTime = 1.5f;
            rectMoveTime = 1.5f;
            return base.OnLoadCoroutine(levelDefinition);
        }

        public override void Update(Level levelDefinition)
        {
            //spin fan
            fan.transform.Rotate(0.0f, 540.0f * Time.deltaTime, 0.0f, Space.Self);

            if (player == null)
            {
                object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
                foreach (object o in obj)
                {
                    GameObject g = (GameObject)o;
                    if (g.name.Contains("PlayerDefault") || g.name.Contains("PlayerTest"))
                    {
                        player = g;
                    }
                }
            }

            ////doors
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


            //circle platforms
            for (int i = 0; i < circlePlatforms.Count; i++)
            {
                if (circlePersonsInside[i].Count > 0 && circlePlatforms[i].transform.position != circleEndPos[i])
                {
                    var pos = (circleMoveTime * Time.deltaTime);
                    //for each circle, move its position closer to end position over each frame
                    circlePlatforms[i].transform.position = Vector3.MoveTowards(circlePlatforms[i].transform.position, circleEndPos[i], pos);
                    if (!circlePlatforms[i].GetComponent<AudioSource>().isPlaying)
                    {
                        circlePlatforms[i].GetComponent<AudioSource>().Play();
                    }
                }
                else if (circlePersonsInside[i].Count == 0 && circlePlatforms[i].transform.position != circleStartPos[i])
                {
                    var pos = (circleMoveTime * Time.deltaTime);
                    //for each circle, move its position closer to end position over each frame
                    circlePlatforms[i].transform.position = Vector3.MoveTowards(circlePlatforms[i].transform.position, circleStartPos[i], pos);
                    if (!circlePlatforms[i].GetComponent<AudioSource>().isPlaying)
                    {
                        circlePlatforms[i].GetComponent<AudioSource>().Play();
                    }
                }
                else if (circlePersonsInside[i].Count > 0 && circlePlatforms[i].transform.position == circleEndPos[i])
                {
                    foreach (GameObject personInside in circlePersonsInside[i])
                    {
                        //bool containsPerson = doors[i].GetComponentInChildren<BoxCollider>().bounds.Contains(personInside.transform.position);
                        bool containsPerson = TargetInsideCollider(personInside.transform.position, circlePlatforms[i].GetComponentInChildren<BoxCollider>());
                        if (containsPerson)
                        {
                            //Debug.Log("Person is still inside");
                        }
                        else
                        {
                            //Debug.Log("Person left, add to list of people to be removed");
                            circlePeopleToRemove.Add(personInside);
                        }
                    }
                    //check if list of people to remove is greater than 0, if so remove those people from the door list
                    if (circlePeopleToRemove.Count > 0)
                    {
                        foreach (GameObject person2 in circlePeopleToRemove)
                        {
                            circlePersonsInside[i].Remove(person2);
                        }
                    }
                    circlePeopleToRemove.Clear();
                }
            }

            if (player != null)
            {
                //rect platforms
                for (int i = 0; i < rectPlatforms.Count; i++)
                {

                    // if player not standing on platform directly
                    if (!rectPlayerInside[i] && !TargetInsideCollider(player.transform.position, rectPlatforms[i].GetComponentInChildren<BoxCollider>()))
                    {
                        //play moving sound
                        if (!rectPlatforms[i].GetComponent<AudioSource>().isPlaying)
                        {
                            rectPlatforms[i].GetComponent<AudioSource>().Play();
                        }

                        //if player is at bottom and platform is at top
                        if (rectDetectionBottoms[i].bounds.Contains(player.transform.position) && rectPlatforms[i].transform.position != rectEndPos[i])
                        {
                            //call platform down
                            var pos = (rectMoveTime * Time.deltaTime);
                            //for each rect, move its position closer to end position over each frame
                            rectPlatforms[i].transform.position = Vector3.MoveTowards(rectPlatforms[i].transform.position, rectEndPos[i], pos);
                        }
                        //if player is outside of detection is not at top, return to top
                        else if (!rectDetectionBottoms[i].bounds.Contains(player.transform.position) && rectPlatforms[i].transform.position != rectStartPos[i])
                        {
                            //call platform up
                            var pos = (rectMoveTime * Time.deltaTime);
                            //for each rect, move its position closer to end position over each frame
                            rectPlatforms[i].transform.position = Vector3.MoveTowards(rectPlatforms[i].transform.position, rectStartPos[i], pos);
                        }
                    }
                    //otherwise if player is on platform
                    else
                    {
                        //play moving sound
                        if (!rectPlatforms[i].GetComponent<AudioSource>().isPlaying)
                        {
                            rectPlatforms[i].GetComponent<AudioSource>().Play();
                        }
                        if (rectsAtTop[i] && rectPlatforms[i].transform.position != rectEndPos[i])
                        {
                            var pos = (rectMoveTime * Time.deltaTime);
                            //for each rect, move its position closer to end position over each frame
                            rectPlatforms[i].transform.position = Vector3.MoveTowards(rectPlatforms[i].transform.position, rectEndPos[i], pos);
                        }
                        else if (!rectsAtTop[i] && rectPlatforms[i].transform.position != rectStartPos[i])
                        {
                            var pos = (rectMoveTime * Time.deltaTime);
                            //for each rect, move its position closer to end position over each frame
                            rectPlatforms[i].transform.position = Vector3.MoveTowards(rectPlatforms[i].transform.position, rectStartPos[i], pos);
                        }
                    }
                    if (rectPlatforms[i].transform.position == rectStartPos[i])
                    {
                        rectsAtTop[i] = true;
                        rectPlatforms[i].GetComponent<AudioSource>().Stop();
                    }
                    else if (rectPlatforms[i].transform.position == rectEndPos[i])
                    {
                        rectsAtTop[i] = false;
                        rectPlatforms[i].GetComponent<AudioSource>().Stop();
                    }
                }
            }
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

        public override void OnUnload(Level levelDefinition)
        {
            // Called when the level unload
            doors.Clear();
            doorsAtTop.Clear();
            doorStartPos.Clear();
            doorEndPos.Clear();
            doorPersonsInside.Clear();
            peopleToRemove.Clear();
            circlePlatforms.Clear();
            circlesAtBottom.Clear();
            circleStartPos.Clear();
            circleEndPos.Clear();
            circlePersonsInside.Clear();
            circlePeopleToRemove.Clear();
            rectPlatforms.Clear();
            rectPlayerInside.Clear();
            rectsAtTop.Clear();
            rectStartPos.Clear();
            rectEndPos.Clear();
            rectDetectionBottoms.Clear();
        }
    }

    public class FFAbonus1Triggers : ColliderListener
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
                //doors
                if (transform.name.Contains("func_door") && !transform.name.Contains("plat") && !transform.name.Contains("t"))
                {
                    int i = FFAbonus1Movement.doors.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);
                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFAbonus1Movement.doorPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFAbonus1Movement.doorPersonsInside[i].Add(person);
                        if (FFAbonus1Movement.doorPersonsInside[i].Count > 0)
                        {
                            FFAbonus1Movement.doors[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }
                }
                //plats
                else if (transform.name.Contains("func_door_plat1") || transform.name.Contains("func_door_plat2"))
                {
                    int i = FFAbonus1Movement.circlePlatforms.IndexOf(transform.gameObject);
                    List<GameObject> personList = FFAbonus1Movement.circlePersonsInside[i];
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);
                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (personList.IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFAbonus1Movement.circlePersonsInside[i].Add(person);
                    }

                }
            }
        }
    }
}


