using UnityEngine;
using System.Collections.Generic;

public class KeySequenceDetector : MonoBehaviour
{
    [Tooltip("The keys that need to be pressed simultaneously")]
    [SerializeField] public KeyCode[] requiredKeys;
    
    [Tooltip("How long all keys must be held together (in seconds)")]
    [SerializeField] private float requiredHoldTime = 0.1f;
    
    [Tooltip("Time window in which all keys must be pressed (in seconds)")]
    [SerializeField] private float timeWindow = 0.5f;
    [SerializeField] private KeyGameManager keyGameManager;
    [SerializeField] private Player player;
    [SerializeField] private Material tabbedInMat;
    [SerializeField] private Material tabbedOutMat;
    [SerializeField] private GameObject playerLaptop;
    
    // Track which keys are currently pressed
    private Dictionary<KeyCode, bool> keyStates = new Dictionary<KeyCode, bool>();
    
    // Track when keys were pressed
    private Dictionary<KeyCode, float> keyPressTimes = new Dictionary<KeyCode, float>();
    
    // Track how long all keys have been pressed together
    private float allKeysHeldTime = 0f;
    
    // Has the handler been called for the current key combo
    private bool handlerCalled = false;
    
    // Track any key presses for detecting bad inputs
    private bool anyKeyWasPressed = false;
    private bool isTabbedOut = false;
    
    private void Start()
    {
        // Initialize dictionaries
        foreach (KeyCode key in requiredKeys)
        {
            keyStates[key] = false;
            keyPressTimes[key] = 0f;
        }
    }
    
    private void Update()
    {
        CheckKeyPresses();
    }
    
    private void CheckKeyPresses()
    {
        bool anyRequiredKeyPressed = false;
        
        // Update key states
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            isTabbedOut = !isTabbedOut;

            if(isTabbedOut)
            {
                OnTabOut();
            } else
            {
                OnTabIn();
            }
        } else {

        foreach (KeyCode key in requiredKeys)
        {
            // Key was just pressed down
            if (Input.GetKeyDown(key))
            {
                keyStates[key] = true;
                keyPressTimes[key] = Time.time;
                anyRequiredKeyPressed = true;
                anyKeyWasPressed = true;
            }
            
            // Key was just released
            if (Input.GetKeyUp(key))
            {
                keyStates[key] = false;
                handlerCalled = false;
                allKeysHeldTime = 0f;
            }
        }
        
        // Check for bad input - any key press that's not in our required set
        if (!anyRequiredKeyPressed && Input.anyKeyDown)
        {
            // Only call if we've started pressing keys in this sequence attempt
            if (anyKeyWasPressed)
            {
                HandleBadKeyInput();
                ResetKeyStates();
                return;
            }
        }
        
        // Check if all keys are currently pressed
        bool allKeysPressed = true;
        float earliestKeyPressTime = float.MaxValue;
        float latestKeyPressTime = 0f;
        
        foreach (KeyCode key in requiredKeys)
        {
            if (!keyStates[key])
            {
                allKeysPressed = false;
                break;
            }
            
            // Track the earliest and latest key press times
            earliestKeyPressTime = Mathf.Min(earliestKeyPressTime, keyPressTimes[key]);
            latestKeyPressTime = Mathf.Max(latestKeyPressTime, keyPressTimes[key]);
        }
        
        // If all keys are pressed, check if they were pressed within the time window
        if (allKeysPressed)
        {
            float keyPressTimeSpan = latestKeyPressTime - earliestKeyPressTime;
            
            if (keyPressTimeSpan <= timeWindow)
            {
                // Increment hold time
                allKeysHeldTime += Time.deltaTime;
                
                // If keys have been held long enough and handler hasn't been called yet
                if (allKeysHeldTime >= requiredHoldTime && !handlerCalled)
                {
                    HandleSuccessfulKeyPress();
                    handlerCalled = true;
                }
            }
            else
            {
                // Keys were not pressed close enough together
                HandleBadKeyInput();
                ResetKeyStates();
            }
        }
        }
    }
    
    private void ResetKeyStates()
    {
        foreach (KeyCode key in requiredKeys)
        {
            keyStates[key] = false;
        }
        allKeysHeldTime = 0f;
        handlerCalled = false;
        anyKeyWasPressed = false;
    }
    
    private void HandleSuccessfulKeyPress()
    {
        // This is your placeholder method to handle when all keys are pressed correctly
        Debug.Log("All required keys were pressed simultaneously!");
        
        // TODO: Add your game logic here
        // Examples:
        // - Award points
        // - Avoid damage
        // - Progress to next sequence
        // - Play success animation/sound
        keyGameManager.HandleGoodInput();
    }
    
    private void HandleBadKeyInput()
    {
        // Call this when a wrong key is pressed or keys aren't pressed correctly
        Debug.Log("Bad key input detected!");
        
        // Call the manager's method for handling bad input
        keyGameManager.HandleBadInput();
    }
    
    // Public method to set a new key sequence at runtime
    public void SetKeySequence(KeyCode[] newSequence)
    {
        requiredKeys = newSequence;
        
        // Reset state for new sequence
        keyStates.Clear();
        keyPressTimes.Clear();
        
        foreach (KeyCode key in requiredKeys)
        {
            keyStates[key] = false;
            keyPressTimes[key] = 0f;
        }
        
        allKeysHeldTime = 0f;
        handlerCalled = false;
        anyKeyWasPressed = false;
    }

    public void OnTabOut()
    {
        Debug.Log("Player tabbed out of game.");
        player.isDistracted = false;
        playerLaptop.GetComponent<MeshRenderer>().material = tabbedOutMat;
        keyGameManager.OnTabOut();
    }

    public void OnTabIn()
    {
         Debug.Log("Player tabbed into game.");
         player.isDistracted = true;
         playerLaptop.GetComponent<MeshRenderer>().material = tabbedInMat;
         keyGameManager.OnTabIn();
    }
}