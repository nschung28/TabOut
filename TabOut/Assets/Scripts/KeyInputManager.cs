using UnityEngine;
using System.Collections.Generic;

public class KeySequenceDetector : MonoBehaviour
{
    [Tooltip("The keys that need to be pressed simultaneously")]
    [SerializeField] public KeyCode[] requiredKeys;
    
    [Tooltip("How long all keys must be held together (in seconds)")]
    [SerializeField] private float requiredHoldTime = 0.001f;
    
    [Tooltip("Time window in which all keys must be pressed (in seconds)")]
    [SerializeField] private float timeWindow = 0.5f;
    [SerializeField] private KeyGameManager keyGameManager;
    [SerializeField] private Player player;
    [SerializeField] private Material tabbedInMat;
    [SerializeField] private Material tabbedOutMat;
    [SerializeField] private GameObject playerLaptop;
    
    private Dictionary<KeyCode, bool> keyStates = new Dictionary<KeyCode, bool>();
    private Dictionary<KeyCode, float> keyPressTimes = new Dictionary<KeyCode, float>();
    private float allKeysHeldTime = 0f;
    private bool handlerCalled = false;
    private bool anyKeyWasPressed = false;
    private bool isTabbedOut = false;

    private void Start()
    {
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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTabbedOut = !isTabbedOut;
            if (isTabbedOut) OnTabOut();
            else OnTabIn();
            return;
        }

        foreach (KeyCode key in requiredKeys)
        {
            if (Input.GetKeyDown(key))
            {
                keyStates[key] = true;
                keyPressTimes[key] = Time.time;
                anyRequiredKeyPressed = true;
                anyKeyWasPressed = true;
            }

            if (Input.GetKeyUp(key))
            {
                keyStates[key] = false;
                handlerCalled = false;
                allKeysHeldTime = 0f;
            }
        }

        if (!anyRequiredKeyPressed && Input.anyKeyDown)
        {
            if (anyKeyWasPressed)
            {
                HandleBadKeyInput();
                ResetKeyStates();
                return;
            }
        }

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
            earliestKeyPressTime = Mathf.Min(earliestKeyPressTime, keyPressTimes[key]);
            latestKeyPressTime = Mathf.Max(latestKeyPressTime, keyPressTimes[key]);
        }

        if (allKeysPressed)
        {
            float keyPressTimeSpan = latestKeyPressTime - earliestKeyPressTime;

            if (keyPressTimeSpan <= timeWindow)
            {
                allKeysHeldTime += Time.deltaTime;

                if (allKeysHeldTime >= requiredHoldTime && !handlerCalled)
                {
                    handlerCalled = true;
                    HandleSuccessfulKeyPress();
                    ResetKeyStates();
                }
            }
            else
            {
                HandleBadKeyInput();
                ResetKeyStates();
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
        Debug.Log("All required keys were pressed simultaneously!");
        keyGameManager.HandleGoodInput();
    }

    private void HandleBadKeyInput()
    {
        Debug.Log("Bad key input detected!");
        keyGameManager.HandleBadInput();
    }

    public void SetKeySequence(KeyCode[] newSequence)
    {
        requiredKeys = newSequence;
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
