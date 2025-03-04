using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Professor : MonoBehaviour
{
    int suspicionLevel;
    Transform professorGaze;
    float intensity = 1.0f;
    float baseRotation;

    // Start is called before the first frame update
    void Start()
    {
        suspicionLevel = 0;
        professorGaze = gameObject.transform;
        baseRotation = professorGaze.eulerAngles.y; // Store initial Y rotation
    }

    // Update is called once per frame
    void Update()
    {
        // Oscillate between -45 and 45 degrees using a sine function
        float angle = Mathf.Sin(Time.time * intensity) * 45f;
        professorGaze.rotation = Quaternion.Euler(0, baseRotation + angle, 0);
    }

    public void HandleDetect(bool isDistracted)
    {
        if(isDistracted)
        {
            // user is distracted, handle case
            // PLACEHOLDER METHOD
            IncreaseIntensity();
        } else 
        {
            // user is not distracted, handle case
            // PLACEHOLDER METHOD
            DecreaseIntensity();
        }
    }

    private void IncreaseIntensity()
    {
        // placeholder
        suspicionLevel++;
        intensity += 0.5f;
        Debug.Log("Suspicion Level: " + suspicionLevel + " | Intensity: " + intensity);
    }

    private void DecreaseIntensity()
    {
        // placeholder
        if(suspicionLevel >= 1 && intensity >= 0.5f)
        {
            suspicionLevel--;
            intensity -= 0.5f;
        }

        Debug.Log("Suspicion Level: " + suspicionLevel + " | Intensity: " + intensity);
    }
}
