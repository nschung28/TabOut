using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isDistracted;
    // Start is called before the first frame update
    void Start()
    {
        isDistracted = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool getIsDistracted()
    {
        return isDistracted;
    }

}
