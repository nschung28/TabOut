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
    int maxLevel = 6;  // Maximum difficulty level
    [SerializeField] private GameObject teachingAssistant;
    
    [SerializeField] public KeySequenceDetector inputManager;
    [SerializeField] private GameObject textObject;
    [SerializeField] public TMP_Text textMeshPro;
    [SerializeField] public TMP_Text levelText;  // New UI element to show current level
    [SerializeField] public TMP_Text progressText;  // New UI element to show progress to next level

    private bool isOver;
    
    // Sound related variables
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;
    
    Color red = new Color(1f, 0f, 0f);
    Color green = new Color(0f, 1f, 0f);
    Color black = new Color(0f,0f,0f);
    Color yellow = new Color(1f, 0.92f, 0.016f);
    
    string outOfTimeMessage = "OUT OF TIME";
    string incorrectInputMessage = "INCORRECT INPUT!";
    string gameOverMessage = "GAME OVER!";
    string levelUpMessage = "LEVEL UP!";
    
    // Time to wait after incorrect input before showing new keys
    private float incorrectInputWaitTime = 5.0f;
    
    // Flag to prevent multiple incorrect handlers from running simultaneously
    private bool isHandlingIncorrectInput = false;

    [SerializeField]private GameObject gameOver;

    void Start()
    {
        isOver = false;
        curTime = 0.0f;
        timeLimit = 7.0f;
        delay = 0.5f;
        gameLevel = 1;  // Starting with 3 keys
        successCounter = 0;
        
        // Make sure we have an AudioSource component
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        DisplayNewKeys();
        UpdateLevelUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOver)
        {
            if (curTime == 0.0f)
            {
                curTime = System.DateTime.Now.Second;
                prevTime = curTime;
            }

            curTime = System.DateTime.Now.Second;

            dTime = curTime - prevTime;
            
            if (dTime > timeLimit && !isHandlingIncorrectInput)
            {
                HandleOutOfTime();
            }
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
        
        // Play correct sound
        if (correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }
        
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
        prevTime = System.DateTime.Now.Second;
        
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
        
        // Play level up sound
        if (correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }
        
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
        // Prevent multiple incorrect handlers from running at once
        if (isHandlingIncorrectInput)
            return;
        isHandlingIncorrectInput = true;
        
        
        // Start coroutine for incorrect input handling
        StartCoroutine(ShowIncorrectInputScreen());
    }

    public void HandleGameOver()
    {
        gameOver.SetActive(true);
        teachingAssistant.SetActive(false);
        textMeshPro.color = red;
        textMeshPro.text = gameOverMessage;
        Debug.Log("GAME OVER");
        
        // Play incorrect sound
        if (incorrectSound != null)
        {
            audioSource.PlayOneShot(incorrectSound);
        }
        
        // Reset success counter or reduce it
        if (successCounter > 0)
        {
            successCounter--;
        }
        
        isOver = true;
    }
    
    IEnumerator ShowIncorrectInputScreen()
    {
        textMeshPro.color = red;
        textMeshPro.text = incorrectInputMessage;
        Debug.Log("Bad Inputs");
        
        // Play incorrect sound
        if (incorrectSound != null)
        {
            audioSource.PlayOneShot(incorrectSound);
        }
        
        // Reset success counter or reduce it
        if (successCounter > 0)
        {
            successCounter--;
        }
        
        // Update UI
        UpdateLevelUI();
        
        // Wait for 5 seconds
        yield return new WaitForSeconds(incorrectInputWaitTime);
        
        // Generate new set of keys at current level and display them
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        DisplayNewKeys();
        
        // Reset handling flag
        isHandlingIncorrectInput = false;
    }

    void HandleOutOfTime()
    {
        // Prevent multiple incorrect handlers from running at once
        if (isHandlingIncorrectInput)
            return;
            
        isHandlingIncorrectInput = true;
        gameOver.SetActive(true);
        isOver = true;
        teachingAssistant.SetActive(false);
        
        // Start coroutine for out of time handling
        StartCoroutine(ShowOutOfTimeScreen());
        
        
    }
    
    IEnumerator ShowOutOfTimeScreen()
    {
        textMeshPro.text = outOfTimeMessage;
        textMeshPro.color = red;
        
        // Play incorrect sound
        if (incorrectSound != null)
        {
            audioSource.PlayOneShot(incorrectSound);
        }
        
        // Reset success counter
        successCounter = 0;
        
        // Update UI
        UpdateLevelUI();
        
        // Wait for 5 seconds
        yield return new WaitForSeconds(incorrectInputWaitTime);
        
        // Generate new set of keys at current level and display them
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        DisplayNewKeys();
        
        // Reset handling flag
        isHandlingIncorrectInput = false;
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
        // prevTime = -10;
        textMeshPro.text = "";
        levelText.text = "";
        progressText.text = "";
    }
        
    public void OnTabIn()
    {
        // prevTime = System.DateTime.Now.Second;
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        UpdateLevelUI();
        DisplayNewKeys();
        progressText.text = "Progress: " + successCounter + "/" + requiredSuccessesToLevelUp;
        levelText.text = "Level: " + gameLevel.ToString();
    }
}