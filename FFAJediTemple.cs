using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace JediTemple
{
    public class FFAJediTemple : LevelModule
    {
        public static List<GameObject> doors = new List<GameObject>();
        public List<Vector3> doorStartPos = new List<Vector3>();
        public List<Vector3> doorEndPos = new List<Vector3>();
        public static List<List<GameObject>> doorPersonsInside = new List<List<GameObject>>();
        public float doorMoveTime;
        public List<GameObject> peopleToRemove = new List<GameObject>();


        public static List<GameObject> plats = new List<GameObject>();
        public static List<Vector3> platsStartPos = new List<Vector3>();
        public static List<Vector3> platsEndPos = new List<Vector3>();
        //public static List<List<GameObject>> platsPersonsInside = new List<List<GameObject>>();
        public static List<bool> platsCalledUp = new List<bool>();
        public static List<bool> platsCalledDown = new List<bool>();
        public static List<string> platsSendDir = new List<string>();
        public float platMoveTime;
        public List<bool> platsAtBottom = new List<bool>();

        public static List<GameObject> doorsPD = new List<GameObject>();
        public List<Vector3> doorPDStartPos = new List<Vector3>();
        public List<Vector3> doorPDEndPos = new List<Vector3>();

        public static AudioSource beep;
        public static Timer platTimer = new Timer(3000);
        public static bool canMove;

        public static List<GameObject> Panels = new List<GameObject>();
        public static Texture PanelOnEmission;
        public static Texture PanelOffEmission;

        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {
            canMove = false;
            beep = GameObject.Find("button_15").GetComponent<AudioSource>();
            PanelOnEmission = GameObject.Find("panelon").GetComponent<Renderer>().material.GetTexture("_EmissionMap");
            PanelOffEmission = GameObject.Find("paneloff").GetComponent<Renderer>().material.GetTexture("_EmissionMap");

            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                //panel faces
                if (g.name.Contains("textures_byss_byss_switch"))
                {
                    Panels.Add(g);
                }
                //doors
                if (g.name.Contains("func_door") && !g.name.Contains("lift"))
                {
                    doors.Add(g);
                    g.AddComponent<JediTempleTriggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    doorPersonsInside.Add(dict[g.name]);

                    doorStartPos.Add(g.transform.position);
                    float y = g.transform.position.y + 1.0f;
                    float x= g.transform.position.x + 1.0f;
                    Vector3 topOfMovement = new Vector3(0,0,0);
                    if (g.name == "func_door")
                    {
                        x = g.transform.position.x - 1.4f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    else if (g.name == "func_door_1")
                    {
                        x = g.transform.position.x + 1.4f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    else if (g.name == "func_door_t24573")
                    {
                        y = g.transform.position.y + 2.35f;
                        topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    }
                    else if (g.name.Contains("func_door_labdoor"))
                    {
                        y = g.transform.position.y + 3.2f;
                        topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    }
                    doorEndPos.Add(topOfMovement);
                }

                //platform doors
                if (g.name.Contains("func_pd"))
                {
                    doorsPD.Add(g);
                    //var dict = new Dictionary<string, List<GameObject>>();
                    //dict[g.name] = new List<GameObject>();

                    doorPDStartPos.Add(g.transform.position);
                    float y = g.transform.position.y + 1.0f;
                    float x = g.transform.position.x + 1.0f;
                    float z = g.transform.position.z + 1.0f;
                    Vector3 topOfMovement = new Vector3(0, 0, 0);
                    if (g.name == "func_pd_updoor")
                    {
                        x = g.transform.position.x - 1.1f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    else if (g.name == "func_pd_updoor_1")
                    {
                        x = g.transform.position.x + 1.1f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    else if (g.name == "func_pd_downdoor")
                    {
                        z = g.transform.position.z - 1.2f;
                        topOfMovement = new Vector3(g.transform.position.x, g.transform.position.y, z);
                    }
                    else if (g.name == "func_pd_downdoor_1")
                    {
                        z = g.transform.position.z + 1.2f;
                        topOfMovement = new Vector3(g.transform.position.x, g.transform.position.y, z);
                    }
                    if (g.name == "func_pd_updoor2" || g.name == "func_pd_downdoor2_1")
                    {
                        x = g.transform.position.x + 1.4f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    else if (g.name == "func_pd_updoor2_1" || g.name == "func_pd_downdoor2")
                    {
                        x = g.transform.position.x - 1.4f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    if (g.name == "func_pd_116" || g.name == "func_pd_118")
                    {
                        x = g.transform.position.x - 1f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    else if (g.name == "func_pd_117" || g.name == "func_pd_119")
                    {
                        x = g.transform.position.x + 1f;
                        topOfMovement = new Vector3(x, g.transform.position.y, g.transform.position.z);
                    }
                    doorPDEndPos.Add(topOfMovement);
                }

                //plats
                if (g.name.Contains("func_door_lift"))
                {
                    plats.Add(g);
                    g.AddComponent<JediTempleTriggers>();
                    //var dict = new Dictionary<string, List<GameObject>>();
                    //dict[g.name] = new List<GameObject>();
                    //platsPersonsInside.Add(dict[g.name]);
                    platsCalledUp.Add(false);
                    platsCalledDown.Add(false);
                    platsSendDir.Add("down");
                    platsAtBottom.Add(true);
                    Transform PlatformHolder = g.transform.parent;

                    platsStartPos.Add(g.transform.position);
                    float y = 1.0f;
                    if (g.name == "func_door_lift1")
                    {
                        y = g.transform.position.y + 26.35f;
                    }
                    if (g.name == "func_door_lift2")
                    {
                        y = g.transform.position.y + 23.52f;
                    }
                    if (g.name == "func_door_lift3")
                    {
                        y = g.transform.position.y + 65.025f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    platsEndPos.Add(topOfMovement);
                }
                else if (g.name.Contains("DetectionTop_Lift") || g.name.Contains("DetectionBottom_Lift"))
                {
                    g.AddComponent<JediTempleTriggers>();
                }
            }

            doorMoveTime = 4f;
            platMoveTime = 2f;
            return base.OnLoadCoroutine(levelDefinition);
        }


        public static void SetTimer(float time)
        {
            // Create a timer with a 1 second interval.
            // Hook up the Elapsed event for the timer. 
            platTimer.Interval = time;
            platTimer.Start();
            platTimer.Elapsed += OnTimedEvent;
            platTimer.AutoReset = false;
        }
        private static void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            platTimer.Stop();
            canMove = true;
        }

        public override void Update(Level levelDefinition)
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
                    //if person is waiting for platform at top and platform is not at top, send to top
                    if (platsCalledUp[i] == true && plats[i].transform.position != platsEndPos[i])
                    {
                        var pos = (platMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        plats[i].transform.position = Vector3.MoveTowards(plats[i].transform.position, platsEndPos[i], pos);
                        if (!plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Play();
                        }

                    }
                    //if person is waiting for platform at bottom and platform is not at bottom, send to bottom
                    else if (platsCalledDown[i] == true && plats[i].transform.position != platsStartPos[i])
                    {
                        var pos = (platMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        plats[i].transform.position = Vector3.MoveTowards(plats[i].transform.position, platsStartPos[i], pos);
                        if (!plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Play();
                        }

                    }
                    //if person activate platform from the bottom, and not at top, send to top
                    else if (!platsCalledDown[i] && !platsCalledUp[i] && platsSendDir[i] == "up" && plats[i].transform.position != platsEndPos[i])
                    {
                        var pos = (platMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        plats[i].transform.position = Vector3.MoveTowards(plats[i].transform.position, platsEndPos[i], pos);
                        if (!plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }
                    //if person activate platform from the top, and not at bottom, send to bottom
                    else if (!platsCalledDown[i] && !platsCalledUp[i] && platsSendDir[i] == "down" && plats[i].transform.position != platsStartPos[i])
                    {
                        var pos = (platMoveTime * Time.deltaTime);
                        //for each bigPlat, move its position closer to end position over each frame
                        plats[i].transform.position = Vector3.MoveTowards(plats[i].transform.position, platsStartPos[i], pos);
                        if (!plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }

                    if (plats[i].transform.position == platsStartPos[i] || plats[i].transform.position == platsEndPos[i])
                    {
                        if (plats[i].GetComponentInChildren<AudioSource>().isPlaying)
                        {
                            plats[i].GetComponentInChildren<AudioSource>().Stop();
                        }
                        if (plats[i].transform.position == platsStartPos[i] && !platsAtBottom[i])
                        {
                            platsAtBottom[i] = true;
                            foreach (GameObject panel in Panels)
                            {
                                panel.GetComponent<Renderer>().material.SetTexture("_EmissionMap", PanelOffEmission);
                            }
                            
                        }
                        if (plats[i].transform.position == platsEndPos[i] && platsAtBottom[i])
                        {
                            platsAtBottom[i] = false;
                            foreach (GameObject panel in Panels)
                            {
                                panel.GetComponent<Renderer>().material.SetTexture("_EmissionMap", PanelOffEmission);
                            }
                        }

                    }
                }

                //platform doors
                for (int iDoor = 0; iDoor < doorsPD.Count; iDoor++)
                {
                    //plat 1, top doors
                    if(doorsPD[iDoor].transform.name == "func_pd_updoor" || doorsPD[iDoor].transform.name == "func_pd_updoor_1")
                    {
                        GameObject plat = GameObject.Find("func_door_lift1");
                        int iPlat = plats.IndexOf(plat);
                        //if platform is at bottom and top doors are not closed, close them
                        if (platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDStartPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDStartPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        //if platform is at top and doors are not open, open doors
                        else if (!platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDEndPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDEndPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                    }

                    //plat 1, bottom doors
                    if (doorsPD[iDoor].transform.name == "func_pd_downdoor" || doorsPD[iDoor].transform.name == "func_pd_downdoor_1")
                    {
                        GameObject plat = GameObject.Find("func_door_lift1");
                        int iPlat = plats.IndexOf(plat);
                        //if platform is at top and bottom doors are not closed, close them
                        if (!platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDStartPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDStartPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        //if platform is at bottom and doors are not open, open doors
                        else if (platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDEndPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDEndPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                    }


                    //plat 2, top doors
                    if (doorsPD[iDoor].transform.name == "func_pd_updoor2" || doorsPD[iDoor].transform.name == "func_pd_updoor2_1")
                    {
                        GameObject plat = GameObject.Find("func_door_lift2");
                        int iPlat = plats.IndexOf(plat);
                        //if platform is at bottom and top doors are not closed, close them
                        if (platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDStartPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDStartPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        //if platform is at top and doors are not open, open doors
                        else if (!platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDEndPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDEndPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                    }

                    //plat 2, bottom doors
                    if (doorsPD[iDoor].transform.name == "func_pd_downdoor2" || doorsPD[iDoor].transform.name == "func_pd_downdoor2_1")
                    {
                        GameObject plat = GameObject.Find("func_door_lift2");
                        int iPlat = plats.IndexOf(plat);
                        //if platform is at top and bottom doors are not closed, close them
                        if (!platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDStartPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDStartPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        //if platform is at bottom and doors are not open, open doors
                        else if (platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDEndPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDEndPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                    }


                    //plat 3, top doors
                    if (doorsPD[iDoor].transform.name == "func_pd_116" || doorsPD[iDoor].transform.name == "func_pd_117")
                    {
                        GameObject plat = GameObject.Find("func_door_lift3");
                        int iPlat = plats.IndexOf(plat);
                        //if platform is at bottom and top doors are not closed, close them
                        if (platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDStartPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDStartPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        //if platform is at top and doors are not open, open doors
                        else if (!platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDEndPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDEndPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                    }

                    //plat 3, bottom doors
                    if (doorsPD[iDoor].transform.name == "func_pd_118" || doorsPD[iDoor].transform.name == "func_pd_119")
                    {
                        GameObject plat = GameObject.Find("func_door_lift3");
                        int iPlat = plats.IndexOf(plat);
                        //if platform is at top and bottom doors are not closed, close them
                        if (!platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDStartPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDStartPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
                        //if platform is at bottom and doors are not open, open doors
                        else if (platsAtBottom[iPlat] && doorsPD[iDoor].transform.position != doorPDEndPos[iDoor])
                        {
                            var pos = (doorMoveTime * Time.deltaTime);
                            //for each door, move its position closer to end position over each frame
                            doorsPD[iDoor].transform.position = Vector3.MoveTowards(doorsPD[iDoor].transform.position, doorPDEndPos[iDoor], pos);
                            if (!doorsPD[iDoor].GetComponentInChildren<AudioSource>().isPlaying)
                            {
                                doorsPD[iDoor].GetComponentInChildren<AudioSource>().Play();
                            }
                        }
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


        public override void OnUnload(Level levelDefinition)
        {
            // Called when the level unload
            doors.Clear();
            doorPersonsInside.Clear();
            doorStartPos.Clear();
            doorEndPos.Clear();
            peopleToRemove.Clear();
            plats.Clear();
            platsStartPos.Clear();
            platsEndPos.Clear();
            platsCalledUp.Clear();
            platsCalledDown.Clear();
            platsAtBottom.Clear();
            platsSendDir.Clear();
            doorsPD.Clear();
            doorPDStartPos.Clear();
            doorPDEndPos.Clear();
            Panels.Clear();
    }



    }

    public class JediTempleTriggers : ColliderListener
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
                if (transform.name.Contains("func_door") && !transform.name.Contains("lift"))
                {
                    int i = FFAJediTemple.doors.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);

                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFAJediTemple.doorPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFAJediTemple.doorPersonsInside[i].Add(person);
                        if (FFAJediTemple.doorPersonsInside[i].Count > 0)
                        {
                            FFAJediTemple.doors[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }
                }


                //plats
                if (transform.name.Contains("func_door_lift"))
                {
                    int i = FFAJediTemple.plats.IndexOf(transform.gameObject);

                    if(FFAJediTemple.plats[i].transform.position == FFAJediTemple.platsStartPos[i])
                    {
                        FFAJediTemple.platsSendDir[i] = "up";
                        FFAJediTemple.platsCalledUp[i] = false;
                        FFAJediTemple.platsCalledDown[i] = false;
                        FFAJediTemple.canMove = false;
                        FFAJediTemple.SetTimer(1000);
                    }
                    else if (FFAJediTemple.plats[i].transform.position == FFAJediTemple.platsEndPos[i])
                    {
                        FFAJediTemple.platsSendDir[i] = "down";
                        FFAJediTemple.platsCalledUp[i] = false;
                        FFAJediTemple.platsCalledDown[i] = false;
                        FFAJediTemple.canMove = false;
                        FFAJediTemple.SetTimer(1000);
                    }

                    if (!FFAJediTemple.beep.isPlaying)
                    {
                        FFAJediTemple.beep.Play();
                        //turn panel emission to green
                        foreach (GameObject panel in FFAJediTemple.Panels)
                        {
                            panel.GetComponent<Renderer>().material.SetTexture("_EmissionMap", FFAJediTemple.PanelOnEmission);
                        }
                    }

                }

                //plat switches
                if (transform.name.Contains("DetectionTop_Lift"))
                {
                    if (transform.name == "DetectionTop_Lift1")
                    {
                        GameObject plat = GameObject.Find("func_door_lift1");
                        int i = FFAJediTemple.plats.IndexOf(plat);
                        FFAJediTemple.platsCalledUp[i] = true;
                        FFAJediTemple.platsCalledDown[i] = false;
                    }
                    else if (transform.name == "DetectionTop_Lift2")
                    {
                        GameObject plat = GameObject.Find("func_door_lift2");
                        int i = FFAJediTemple.plats.IndexOf(plat);
                        FFAJediTemple.platsCalledUp[i] = true;
                        FFAJediTemple.platsCalledDown[i] = false;
                    }
                    else if (transform.name == "DetectionTop_Lift3")
                    {
                        GameObject plat = GameObject.Find("func_door_lift3");
                        int i = FFAJediTemple.plats.IndexOf(plat);
                        FFAJediTemple.platsCalledUp[i] = true;
                        FFAJediTemple.platsCalledDown[i] = false;
                    }
                    if (!FFAJediTemple.beep.isPlaying)
                    {
                        FFAJediTemple.beep.Play();
                        FFAJediTemple.canMove = false;
                        FFAJediTemple.SetTimer(1000);
                        //turn panel emission to green
                        foreach (GameObject panel in FFAJediTemple.Panels)
                        {
                            panel.GetComponent<Renderer>().material.SetTexture("_EmissionMap", FFAJediTemple.PanelOnEmission);
                        }
                    }
                }
                else if (transform.name.Contains("DetectionBottom_Lift"))
                {
                    if (transform.name == "DetectionBottom_Lift1")
                    {
                        GameObject plat = GameObject.Find("func_door_lift1");
                        int i = FFAJediTemple.plats.IndexOf(plat);
                        FFAJediTemple.platsCalledDown[i] = true;
                        FFAJediTemple.platsCalledUp[i] = false;
                    }
                    else if (transform.name == "DetectionBottom_Lift2")
                    {
                        GameObject plat = GameObject.Find("func_door_lift2");
                        int i = FFAJediTemple.plats.IndexOf(plat);
                        FFAJediTemple.platsCalledDown[i] = true;
                        FFAJediTemple.platsCalledUp[i] = false;
                    }
                    else if (transform.name == "DetectionBottom_Lift3")
                    {
                        GameObject plat = GameObject.Find("func_door_lift3");
                        int i = FFAJediTemple.plats.IndexOf(plat);
                        FFAJediTemple.platsCalledDown[i] = true;
                        FFAJediTemple.platsCalledUp[i] = false;
                    }

                    if (!FFAJediTemple.beep.isPlaying)
                    {
                        FFAJediTemple.beep.Play();
                        FFAJediTemple.canMove = false;
                        FFAJediTemple.SetTimer(1000);
                        //turn panel emission to green
                        foreach (GameObject panel in FFAJediTemple.Panels)
                        {
                            panel.GetComponent<Renderer>().material.SetTexture("_EmissionMap", FFAJediTemple.PanelOnEmission);
                        }
                    }
                }

            }
        }
    }
}
