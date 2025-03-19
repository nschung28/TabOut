using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DistractionDetector : MonoBehaviour
{
    private Collider detectorCollider;
    private Professor professor;
    private GameObject playerObject;
    private Player player;
    [SerializeField] private KeyGameManager keyGameManager;



    // Start is called before the first frame update
    void Start()
    {
        // Get the collider attached to this GameObject
        detectorCollider = GetComponent<SphereCollider>();
        professor = GetComponent<Professor>();
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
        // See if player is distracted
        bool isDistracted = player.getIsDistracted();
        if(isDistracted)
            keyGameManager.HandleGameOver();
        // Handle detection
        professor.HandleDetect(isDistracted);
    }
}
