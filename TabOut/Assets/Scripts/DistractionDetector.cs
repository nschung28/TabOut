using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

DistractonDetector

Usage:  

A script that is to be placed on the view cone of a TA or Professor.

Fields :

Collider detectorCollider : The collider of the view cone the script is placed on.

Professor professor : The object which control's the professor's actions.

Methods :

OnTriggerEnter(Collider other)

When the viewcone hits another object with both a collider and rigidbody, the code first checks
if the object which the viewcone collided with is a player (has tag : "Player"). If so, it calls 
OnPlayerDetect() in order to handle the increased intensity / capture.

OnPlayerDetect() 

If the professor's / ta's viewcone intersects with the player, we need to check if the player is distracted or not

We get a boolean which signifies whether or not the player is distracted, and pass that to HandleDetection(bool isDetected)
through the professor object.

If the player is distracted, we need to handle this
by either increasing the intensity of the course staff's surveying, or trigger other mechanics 
related to the player being caught distracted. 

If the player is not distracted, we subltly decrease the course staff's intensity.
*/



public class DistractionDetector : MonoBehaviour
{
    private Collider detectorCollider;
    private Professor professor;
    private GameObject playerObject;
    private Player player;



    // Start is called before the first frame update
    void Start()
    {
        // Get the collider attached to this GameObject
        detectorCollider = GetComponent<Collider>();
        professor = GetComponentInParent<Professor>();
        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<Player>();


        // Ensure the collider is set to trigger
        if (detectorCollider != null)
        {
            detectorCollider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("No Collider found on " + gameObject.name);
        }
    }

    // This method is called when another collider enters the trigger
    void OnTriggerEnter(Collider other)
    {
        // Check if the object that triggered the collider has the tag "Player"
        if (other.CompareTag("Player"))
        {
            OnPlayerDetect();
        }
    }

    void OnPlayerDetect()
    {
        Debug.Log("Player detected by " + gameObject.name);
        // see if player is distracted
        bool isDistracted = player.getIsDistracted();
        // handle detection
        professor.HandleDetect(isDistracted);
    }
}
