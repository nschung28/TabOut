using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Professor : MonoBehaviour
{
    int suspicionLevel;
    float intensity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        suspicionLevel = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleDetect(bool isDistracted)
    {
        if(isDistracted)
        {
            // user is distracted, handle case
            IncreaseIntensity();
        } else 
        {
            // user is not distracted, handle case
            DecreaseIntensity();
        }
    }

    private void IncreaseIntensity()
    {
        suspicionLevel++;
        intensity += 0.5f;
        Debug.Log("Suspicion Level: " + suspicionLevel + " | Intensity: " + intensity);
    }

    private void DecreaseIntensity()
    {
        if(suspicionLevel >= 1 && intensity >= 0.5f)
        {
            suspicionLevel--;
            intensity -= 0.5f;
        }

        Debug.Log("Suspicion Level: " + suspicionLevel + " | Intensity: " + intensity);
    }
}
