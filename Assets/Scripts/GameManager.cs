using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState currentState{ get; private set; }

    [Header("Game State")]
    public GameObject mainMenuUI;
    //public GameObject gamePlayUI;
    //public GameObject levelCompleteUI;

    void Awake()
    {
        // Singleton pattern (chỉ có 1 instance của GameManager)
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);  // GameManager tồn tại xuyên suốt các scene
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(GameState.MainMenu); // Bắt đầu ở Main Menu
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        mainMenuUI?.SetActive(currentState == GameState.MainMenu);
        //gamePlayUI?.SetActive(currentState == GameState.GamePlay);
        //levelCompleteUI?.SetActive(currentState == GameState.LevelComplete);

        Debug.Log($"🔄 GameState: {currentState}");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Gameplay 1");  // Chuyển sang màn chơi
    }

    public void OnLevelComplete()
    {
        ChangeState(GameState.LevelComplete);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Quay lại menu chính
        ChangeState(GameState.MainMenu);
    }

    // Hàm thoát game
    public void QuitGame()
    {
        Application.Quit();
    }
}
