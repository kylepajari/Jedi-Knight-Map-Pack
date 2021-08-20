using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class FFAbonus4Movement : LevelModule
    {
        //Doors variables
        public static List<GameObject> doors = new List<GameObject>();
        public List<Vector3> doorStartPos = new List<Vector3>();
        public List<Vector3> doorEndPos = new List<Vector3>();
        public static List<List<GameObject>> doorPersonsInside = new List<List<GameObject>>();
        public float doorMoveTime;
        public List<GameObject> peopleToRemove = new List<GameObject>();


        public override System.Collections.IEnumerator OnLoadCoroutine(Level levelDefinition)
        {

            object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (object o in obj)
            {
                GameObject g = (GameObject)o;
                if (g.name.Contains("func_door"))
                {
                    doors.Add(g);
                    g.AddComponent<ColliderListener>();
                    g.AddComponent<FFAbonus4Triggers>();
                    var dict = new Dictionary<string, List<GameObject>>();
                    dict[g.name] = new List<GameObject>();
                    doorPersonsInside.Add(dict[g.name]);
                    doorStartPos.Add(g.transform.position);
                    float y = g.transform.position.y + 3.0f;
                    if (g.name == "func_door")
                    {
                        y = g.transform.position.y + 5.7f;
                    }
                    else if (g.name == "func_door_1")
                    {
                        y = g.transform.position.y + 3.0f;
                    }
                    else if (g.name == "func_door_2")
                    {
                        y = g.transform.position.y + 7.0f;
                    }
                    Vector3 topOfMovement = new Vector3(g.transform.position.x, y, g.transform.position.z);
                    doorEndPos.Add(topOfMovement);
                }
            }
            doorMoveTime = 4f;
            return base.OnLoadCoroutine(levelDefinition);
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
        }
    }

    public class FFAbonus4Triggers : ColliderListener
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
                if (transform.name.Contains("func_door"))
                {
                    int i = FFAbonus4Movement.doors.IndexOf(transform.gameObject);
                    bool fullyInside = transform.GetComponentInChildren<BoxCollider>().bounds.Contains(person.transform.position);
                    //if player or npc is not currently in list and is fully inside container, add to list to prevent reopening until exit
                    if (FFAbonus4Movement.doorPersonsInside[i].IndexOf(person) == -1 && fullyInside)
                    {
                        //add person to list of people inside
                        FFAbonus4Movement.doorPersonsInside[i].Add(person);
                        if (FFAbonus4Movement.doorPersonsInside[i].Count > 0)
                        {
                            FFAbonus4Movement.doors[i].GetComponentInChildren<AudioSource>().Play();
                        }
                    }
                }
            }
        }
    }
}

