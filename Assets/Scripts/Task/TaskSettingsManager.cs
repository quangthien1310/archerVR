using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSettingsManager : MonoBehaviour
{
    public static TaskSettingsManager Instance;
    public TaskSettings[] allTaskSettings; // Danh sách tất cả TaskSettings
    public static TaskSettings currentTask { get; private set; } // TaskSettings hiện tại

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Giữ TaskSettingsManager xuyên suốt các scene
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy đi
        }

        LoadTaskLockStatus();
        LoadHighScore();
    }

    public void SetCurrentTask(string taskName)
    {
        foreach (var task in allTaskSettings)
        {
            if (task.taskName == taskName)
            {
                currentTask = task;
                Debug.Log($"✅ Đã chọn Task: {currentTask.taskName}");
                return;
            }
        }

        Debug.LogWarning($"❌ Không tìm thấy Task: {taskName}");
    }

    public void SaveTaskLockStatus()
    {
        foreach (var task in allTaskSettings)
        {
            PlayerPrefs.SetInt(task.taskName + "_isLocked", task.isLocked ? 1 : 0);
        }
        PlayerPrefs.Save(); // Lưu các thay đổi
    }

    public void LoadTaskLockStatus()
    {
        foreach (var task in allTaskSettings)
        {
            int isLocked = PlayerPrefs.GetInt(task.taskName + "_isLocked", 1); // Mặc định là 1 (locked)
            task.isLocked = isLocked == 0 ? false : true;
            if (task.level == 1)
            {
                task.isLocked = false; // Mở khóa task đầu tiên
            }
        }
    }

    public void LoadHighScore()
    {
        foreach (var task in allTaskSettings)
        {
            string key = task.taskName + "_highScore";
            task.highScore = PlayerPrefs.GetInt(key, 0); // Mặc định là 0
        }
    }
}
