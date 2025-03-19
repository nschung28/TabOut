using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Add this line

public class GameManager : MonoBehaviour
{
    public int suspicionLevel;
    
    // Start is called before the first frame update
    public void EndGame()
    {
        Debug.Log("Game Over! The professor caught you!");
        // Add other game-ending logic here
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}