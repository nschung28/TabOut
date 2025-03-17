using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    float timePassed, curTime, prevTime, timeLimit, dTime, delay;
    KeyCode[] curKeys;
    bool keysPressed;
    bool isSuccesful;
    int gameLevel;
    int successCounter;  // Track consecutive successful inputs
    int requiredSuccessesToLevelUp = 3;  // How many successful inputs needed to level up
    int maxLevel = 10;  // Maximum difficulty level
    
    [SerializeField] public KeySequenceDetector inputManager;
    [SerializeField] private GameObject textObject;
    [SerializeField] public TMP_Text textMeshPro;
    [SerializeField] public TMP_Text levelText;  // New UI element to show current level
    [SerializeField] public TMP_Text progressText;  // New UI element to show progress to next level
    
    Color red = new Color(1f, 0f, 0f);
    Color green = new Color(0f, 1f, 0f);
    Color black = new Color(0f,0f,0f);
    Color yellow = new Color(1f, 0.92f, 0.016f);
    
    string outOfTimeMessage = "You have ran out of time to enter the keys, BOOM BOOM BOOM BOOM BOOM";
    string levelUpMessage = "LEVEL UP!";

    void Start()
    {
        curTime = 0.0f;
        timeLimit = 5.0f;
        delay = 0.5f;
        gameLevel = 1;  // Starting with 3 keys
        successCounter = 0;
        
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        DisplayNewKeys();
        UpdateLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        if(curTime == 0.0f)
        {
            curTime = System.DateTime.Now.Second;
        }
        prevTime = curTime;
        curTime = System.DateTime.Now.Second;

        dTime = curTime - prevTime;

        if(dTime > timeLimit)
        {
          HandleOutOfTime();   
        }
    }
    
    public KeyCode[] GenerateTargetKeys(int gameLevel)
    {
        // Create an array with length based on gameLevel
        KeyCode[] targetKeys = new KeyCode[gameLevel];
        
        // Create a list of all alphabetical key codes
        List<KeyCode> alphabetKeys = new List<KeyCode>();
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
        {
            alphabetKeys.Add(key);
        }
        
        // Randomly select keys from the alphabet
        System.Random random = new System.Random();
        for (int i = 0; i < gameLevel; i++)
        {
            // Pick a random index from the remaining alphabet keys
            int randomIndex = random.Next(0, alphabetKeys.Count);
            
            // Add the selected key to our target array
            targetKeys[i] = alphabetKeys[randomIndex];
            
            // Remove the key so it doesn't get chosen again
            alphabetKeys.RemoveAt(randomIndex);
        }
        return targetKeys;
    }

    void UpdateTargetKeys(KeyCode[] targetKeys)
    {
        // Send to input manager
        inputManager.SetKeySequence(targetKeys);
    }

    public void HandleGoodInput()
    {
        textMeshPro.color = green;
        
        // Increase success counter
        successCounter++;
        
        // Check if player should level up
        if (successCounter >= requiredSuccessesToLevelUp && gameLevel < maxLevel)
        {
            LevelUp();
        }
        else
        {
            // Generate new keys at current level
            curKeys = GenerateTargetKeys(gameLevel);
            UpdateTargetKeys(curKeys);
            DisplayNewKeys();
        }
        
        // Update UI
        UpdateLevelUI();
    }
    
    private void LevelUp()
    {
        // Increase level
        gameLevel++;
        
        // Reset success counter
        successCounter = 0;
        
        // Display level up message
        StartCoroutine(ShowLevelUpMessage());
        
        // Slightly decrease time limit each level to make it harder
        if (timeLimit > 2.0f)  // Don't go below 2 seconds
        {
            timeLimit -= 0.3f;
        }
        
        // Generate new keys for new level
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
    }
    
    IEnumerator ShowLevelUpMessage()
    {
        textMeshPro.text = levelUpMessage;
        textMeshPro.color = yellow;
        
        // Play level up sound or animation here
        // audioSource.PlayOneShot(levelUpSound);
        
        yield return new WaitForSeconds(1.5f);
        
        DisplayNewKeys();
    }

    void DisplayNewKeys()
    {
        textMeshPro.text = KeySetToString(curKeys);
        textMeshPro.color = black;
    }

    public void HandleBadInput()
    {
        textMeshPro.color = red;
        Debug.Log("Bad Inputs");
        
        // Reset success counter or reduce it
        if (successCounter > 0)
        {
            successCounter--;
        }
        
        // Update UI
        UpdateLevelUI();
        
        // raise TA suspicion and play noise
        // trigger animation?
    }

    void HandleOutOfTime()
    {
        textMeshPro.text = outOfTimeMessage;
        textMeshPro.color = red;
        
        // Reset success counter
        successCounter = 0;
        
        // Update UI
        UpdateLevelUI();
    }
    
    void UpdateLevelUI()
    {
        // Update level display
        if (levelText != null)
        {
            levelText.text = "Level: " + gameLevel.ToString();
        }
        
        // Update progress to next level
        if (progressText != null)
        {
            progressText.text = "Progress: " + successCounter + "/" + requiredSuccessesToLevelUp;
        }
    }

    string KeySetToString(KeyCode[] keySet)
    {
        string result = "";
        foreach (var key in keySet)
        {
            result += key.ToString() + " ";
        }
        return result.Trim();
    }

    public void OnTabOut()
    {
        textMeshPro.text = "";
        levelText.text = "";
        progressText.text = "";
    }

    
    public void OnTabIn()
    {
        gameLevel = 1;
        successCounter = 0;
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        UpdateLevelUI();
        DisplayNewKeys();
        progressText.text = "Progress: " + successCounter + "/" + requiredSuccessesToLevelUp;
        levelText.text = "Level: " + gameLevel.ToString();
    }
}