using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAMovement : MonoBehaviour
{
    
    public Transform[] waypoints;
    public int currentWaypoint;
    private int previousWaypoint;
    public float movementSpeed;
    public float rotationSpeed = 5.0f;

    private int bounces;

    private Dictionary<int, int[]> waypointOdds;
    private System.Random rand = new System.Random();
    
    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint = 0;
        previousWaypoint = -1;
        bounces = 0;
        
        int[] pointOneNeighbors = new int[10]{2, 2, 2, 2, 2, 2, 12, 12, 12, 12};
        int[] pointTwoNeighbors = new int[10]{1, 3, 3, 3, 3, 7, 7, 7, 7, 7};
        // int[] pointOneNeighbors = new int[6]{2, 2, 2, 2, 2, 2};
        // int[] pointTwoNeighbors = new int[1]{1};
        int[] pointThreeNeighbors = new int[10]{2, 4, 4, 4, 6, 6, 6, 6, 12, 12};
        int[] pointFourNeighbors = new int[10]{3, 5, 5, 5, 5, 5, 11, 11, 11, 11};
        int[] pointFiveNeighbors = new int[10]{4, 4, 6, 6, 6, 6, 6, 10, 10, 10};
        int[] pointSixNeighbors = new int[10]{3, 3, 5, 5, 7, 7, 7, 9, 9, 9}; // maybe not 5 this many times
        int[] pointSevenNeighbors = new int[10]{2, 2, 2, 6, 6, 6, 8, 8, 8, 8};
        int[] pointEightNeighbors = new int[10]{7, 7, 7, 9, 9, 9, 9, 9, 9, 9};
        int[] pointNineNeighbors = new int[10]{6, 6, 6, 6, 8, 8, 10, 10, 10, 10}; //  ?
        int[] pointTenNeighbors = new int[10]{5, 5, 5, 5, 5, 5, 5, 9, 9, 9}; // ?
        int[] pointElevenNeighbors = new int[10]{4, 4, 4, 4, 4, 4, 4, 12, 12, 12};
        int[] pointTwelveNeighbors = new int[10]{1, 1, 3, 3, 3, 3, 3, 3, 11, 11};
        
        waypointOdds = new Dictionary<int, int[]>();

        waypointOdds.Add(1, pointOneNeighbors);
        waypointOdds.Add(2, pointTwoNeighbors);
        waypointOdds.Add(3, pointThreeNeighbors);
        waypointOdds.Add(4, pointFourNeighbors);
        waypointOdds.Add(5, pointFiveNeighbors);
        waypointOdds.Add(6, pointSixNeighbors);
        waypointOdds.Add(7, pointSevenNeighbors);
        waypointOdds.Add(8, pointEightNeighbors);
        waypointOdds.Add(9, pointNineNeighbors);
        waypointOdds.Add(10, pointTenNeighbors);
        waypointOdds.Add(11, pointElevenNeighbors);
        waypointOdds.Add(12, pointTwelveNeighbors);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = waypoints[currentWaypoint].position;
        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        }
        
        if (waypoints[currentWaypoint].position != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                waypoints[currentWaypoint].position, movementSpeed * Time.deltaTime);
            
        }
        else
        {
            // randomness
            // Debug.Log(bounces);
            int[] oddsForPoint = waypointOdds[currentWaypoint + 1];
            
            int nextPossiblePoint = (oddsForPoint[rand.Next(0, oddsForPoint.Length)] - 1) % waypoints.Length;
            if (nextPossiblePoint == previousWaypoint)
            {
                bounces++;
            }
            
            previousWaypoint = currentWaypoint;
            if (bounces == 3)
            {
                currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                bounces = 0;
            }
            else
            {
                currentWaypoint = nextPossiblePoint;
            }
        }
    }
}
