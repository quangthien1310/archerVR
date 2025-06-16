using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Valve.VR.Extras;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text accuracyText;
    public TMP_Text timeText;
    public TMP_Text noHitTimerText;

    public GameObject scoreHUD;
    public GameObject accuracyHUD;
    public GameObject timeHUD;

    private float timeLeft;
    private int score = 0;
    private int totalShots = 0;
    private int successfulShots = 0;

    public bool isGameRunning = true;

    public GameObject pauseCanvas;

    private AudioSource audioSource; // Âm thanh khi bị bắn trúng
    public GameObject audioPrefab; // Prefab âm thanh
    //public GameObject rightHand;

    public SteamVR_LaserPointer laserPointer; // Tham chiếu đến LaserPointer

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        pauseCanvas.SetActive(false);
        laserPointer.active = false; // Tắt LaserPointer khi bắt đầu game
        audioSource = audioPrefab.GetComponent<AudioSource>();
        timeLeft = TaskSettingsManager.currentTask.timeLimit;
        UpdateHUD();
    }

    void Update()
    {
        if (!isGameRunning) return;
        if (timeLeft != -111)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                EndGame();
            }
        }

        UpdateHUD();
    }

    public void RegisterShot()
    {
        totalShots++;
        UpdateHUD();
    }

    public void RegisterHit()
    {
        successfulShots++;
        score += TaskSettingsManager.currentTask.scorePerHit;
        UpdateHUD();
    }

    void UpdateHUD()
    {
        scoreText.text = $"{score}";
        accuracyText.text = totalShots > 0 ? $"{(int)(100f * successfulShots / totalShots)}%" : "100%";

        if (LevelDesign.Instance.level == 12)
        {
            noHitTimerText.text = $"{5 - LevelDesign.Instance.noHitTimer:F0}";
        }

        if (timeLeft == -111)
        {
            timeText.text = LevelDesign.Instance.maxLife.ToString();
            return;
        }

        // Chuyển đổi thời gian thành định dạng mm:ss
        int minutes = Mathf.FloorToInt(timeLeft / 60f);
        int seconds = Mathf.FloorToInt(timeLeft % 60f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void EndGame()
    {
        isGameRunning = false;
        Debug.Log($"🎯 Kết thúc game! Điểm: {score} | Accuracy: {(int)(100f * successfulShots / Mathf.Max(1, totalShots))}%");

        ResultData.finalScore = score;
        ResultData.finalAccuracy = totalShots > 0 ? (int)(100f * successfulShots / totalShots) : 100;
        ResultData.hasResult = true;

        SceneManager.LoadScene("MainMenu");

    }

    public void HUDBehaviour()
    {
        scoreHUD.SetActive(!scoreHUD.activeSelf);
        accuracyHUD.SetActive(!accuracyHUD.activeSelf);
        timeHUD.SetActive(!timeHUD.activeSelf);
    }

    public void TargetHitSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log("Âm thanh đã phát!");
        }
    }

    public void PauseGameBehaviour()
    {
        pauseCanvas.SetActive(true);
        laserPointer.active = true; // Bật LaserPointer khi pause
        isGameRunning = false;
    }

    public void OnResumeButtonClicked()
    {
        Debug.Log("Resume button clicked!");
        laserPointer.active = false; // Tắt LaserPointer khi resume
        //audioSource.Play();

        pauseCanvas.SetActive(false);
        isGameRunning = true;
    }

    public void OnReturnButtonClicked()
    {
        Debug.Log("Return button clicked!");
        //audioSource.Play();

        SceneManager.LoadScene("MainMenu");
    }
}
