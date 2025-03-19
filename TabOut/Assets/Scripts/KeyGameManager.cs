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
    int successCounter;
    int requiredSuccessesToLevelUp = 3;
    int maxLevel = 6;
    [SerializeField] private GameObject teachingAssistant;
    
    [SerializeField] public KeySequenceDetector inputManager;
    [SerializeField] private GameObject textObject;
    [SerializeField] public TMP_Text textMeshPro;
    [SerializeField] public TMP_Text levelText;
    [SerializeField] public TMP_Text progressText;

    private bool isOver;
    
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
    
    private float incorrectInputWaitTime = 5.0f;
    private bool isHandlingIncorrectInput = false;

    [SerializeField]private GameObject gameOver;

    void Start()
    {
        isOver = false;
        curTime = 0.0f;
        timeLimit = 7.0f;
        delay = 0.5f;
        gameLevel = 1;
        successCounter = 0;
        
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
        KeyCode[] targetKeys = new KeyCode[gameLevel];
        

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
            targetKeys[i] = alphabetKeys[randomIndex];
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
        textMeshPro.text = "";
        levelText.text = "";
        progressText.text = "";
    }
        
    public void OnTabIn()
    {
        curKeys = GenerateTargetKeys(gameLevel);
        UpdateTargetKeys(curKeys);
        UpdateLevelUI();
        DisplayNewKeys();
        progressText.text = "Progress: " + successCounter + "/" + requiredSuccessesToLevelUp;
        levelText.text = "Level: " + gameLevel.ToString();
    }
}