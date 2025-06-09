using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultPanelManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text accuracyText;
    public TMP_Text highScoreText;

    public Button nextTaskButton;
    public Button replayTaskButton;
    public TMP_Text resultText;

    public AudioSource audioSource;

    public void ShowResult(int score, int accuracy)
    {
        SaveHighScore(score); // Lưu điểm cao
        scoreText.text = $"{score}";
        accuracyText.text = $"{accuracy}%";
        highScoreText.text = $"{TaskSettingsManager.currentTask.highScore}";

        replayTaskButton.onClick.AddListener(() => OnTaskButtonClicked(TaskSettingsManager.currentTask.taskName));

        if (score >= TaskSettingsManager.currentTask.passScore && accuracy >= TaskSettingsManager.currentTask.passAccuracy)
        {
            Debug.Log("✅ Passed!");
            resultText.text = "PASSED!";
            UnlockNextTask(); // Mở khóa task tiếp theo
        }
        else
        {
            nextTaskButton.interactable = false; // Vô hiệu hóa nút chuyển sang task tiếp theo
            resultText.text = "FAILED!";
            Debug.Log("❌ Failed!");
        }
    }

    void UnlockNextTask()
    {
        int nextLevel = TaskSettingsManager.currentTask.level + 1; // Lấy level tiếp theo
        Debug.Log($"🔑 Mở khóa Task với level: {nextLevel}");
        TaskSettings nextTask = null;

        // Tìm task có level tiếp theo
        foreach (var task in TaskSettingsManager.Instance.allTaskSettings)
        {
            if (task.level == nextLevel)
            {
                nextTask = task;
                break;
            }
        }

        string taskName = nextTask.taskName;

        nextTaskButton.interactable = true; // Kích hoạt nút chuyển sang task tiếp theo
        nextTaskButton.onClick.AddListener(() => OnTaskButtonClicked(taskName));

        // Nếu tìm thấy task với level tiếp theo và task đó bị khóa
        if (nextTask != null && nextTask.isLocked)
        {
            nextTask.isLocked = false; // Mở khóa task tiếp theo
            TaskSettingsManager.Instance.SaveTaskLockStatus(); // Lưu lại trạng thái mới
            Debug.Log($"✅ Mở khóa Task tiếp theo: {nextTask.taskName}");
        }
        else
        {
            Debug.LogWarning("❌ Không tìm thấy Task với level tiếp theo hoặc task đã mở khóa.");
        }
    }

    public void SaveHighScore(int currentScore)
    {
        string key = TaskSettingsManager.currentTask.taskName + "_highScore";
        int previousHigh = TaskSettingsManager.currentTask.highScore;

        if (currentScore > previousHigh)
        {
            PlayerPrefs.SetInt(key, currentScore);
            PlayerPrefs.Save();

            TaskSettingsManager.currentTask.highScore = currentScore;
        }
        else
        {
            TaskSettingsManager.currentTask.highScore = previousHigh;
        }
    }

    public void OnTaskButtonClicked(string taskName)
    {
        TaskSettingsManager.Instance.SetCurrentTask(taskName);
        if (TaskSettingsManager.currentTask.isLocked == false)
        {
            audioSource.Play();
            SceneManager.LoadScene(taskName);
        }
    }
}
