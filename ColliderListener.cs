using System;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;
using System.Timers;

namespace MapScripts
{
    public class ColliderListener : MonoBehaviour
    {
        private static Timer aTimer = new Timer(35000);
        public static AudioSource pickupSound;
        public bool pickedUp;

        void Awake()
        {
            // Check if Colider is in another GameObject
            Collider collider = GetComponentInChildren<BoxCollider>();
            if (collider.gameObject != gameObject)
            {
                ColliderBridge cb = collider.gameObject.AddComponent<ColliderBridge>();
                cb.Initialize(this);
            }
            if(transform.name.Contains("medpac") || transform.name.Contains("bacta"))
            {
                pickupSound = GameObject.Find("use_bacta").GetComponent<AudioSource>();
                pickedUp = false;
            }
        }

        public virtual void OnTriggerEnter(Collider col)
        {
            if (transform.name.Contains("medpac") || transform.name.Contains("bacta"))
            {
                if(Player.currentCreature != null)
                {
                    if (col.transform.root.name == Player.currentCreature.transform.root.name && !pickedUp)
                    {
                        pickedUp = true;
                        PickupItem(transform.name, col);
                    }
                }
                
            }
        }

        public virtual void OnTriggerExit(Collider col)
        {
        }

        public void PickupItem(string itemname, Collider col)
        {
            bool success = false;
            switch (itemname)
            {
                case string a when a.Contains("medpac"):
                    if (Player.currentCreature.currentHealth != Player.currentCreature.maxHealth)
                    {
                        float health = (Player.currentCreature.maxHealth / 2);
                        Player.currentCreature.Heal(health, null);
                        pickupSound.Play();
                        //cap for full health
                        //if (Player.currentCreature.currentHealth > Player.currentCreature.maxHealth)
                        //{
                        //    Player.currentCreature.currentHealth = Player.currentCreature.maxHealth;
                        //}
                        success = true;
                    }
                    break;
                case string b when b.Contains("bacta"):
                    if (Player.currentCreature.currentHealth != Player.currentCreature.maxHealth)
                    {
                        float health = (Player.currentCreature.maxHealth / 4);
                        Player.currentCreature.Heal(health, null);
                        pickupSound.Play();
                        //cap for full health
                        //if (Player.currentCreature.currentHealth > Player.currentCreature.maxHealth)
                        //{
                        //    Player.currentCreature.currentHealth = Player.currentCreature.maxHealth;
                        //}
                        success = true;
                    }
                    break;
                default:
                    break;
            }
            if (success)
            {
                //disable pickup if it's still active
                if (transform.gameObject.GetComponentInChildren<MeshRenderer>().enabled)
                {
                    EnablePickup(false);
                }
            }
            else
            {
                //could not pickup item, reset pickup bool
                pickedUp = false;
            }
        }

        public void EnablePickup(bool b)
        {
            if (b)
            {
                //reset state and position of item to how it spawned;
                transform.gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
                transform.gameObject.GetComponentInChildren<BoxCollider>().enabled = true;
                //reset pickup bool
                pickedUp = false;

            }
            else
            {
                //start timer to turn back on
                SetTimer();
                transform.gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
                transform.gameObject.GetComponentInChildren<BoxCollider>().enabled = false;
            }
        }

        private void SetTimer()
        {
            // Create a timer with a 35 second interval.
            //aTimer = new System.Timers.Timer(35000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Start();
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
        }

        private void OnTimedEvent(System.Object source, ElapsedEventArgs e)
        {
            aTimer.Stop();
            EnablePickup(true);
        }
    }
}
