using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TaskListManager : MonoBehaviour
{
    [Header("Prefabs & UI")]
    public GameObject taskButtonPrefab;  // Prefab cho mỗi task
    public Transform contentPanel;       // Panel chứa nút (Grid Layout 3x2)
    public GameObject prevButton;
    public GameObject nextButton;

    private int currentPage = 0;
    private int taskPerPage = 6;

    private TaskSettings[] allTasks;

    void Start()
    {
        if (TaskSettingsManager.Instance == null)
        {
            Debug.LogError("❌ Không có Task nào trong TaskSettingsManager!");
            return;
        }

        allTasks = TaskSettingsManager.Instance.allTaskSettings;

        prevButton.GetComponent<Button>().onClick.AddListener(OnPrevPage);
        nextButton.GetComponent<Button>().onClick.AddListener(OnNextPage);

        ShowPage(currentPage);
        Debug.Log($"✅ Đã tải {allTasks.Length} task từ TaskSettingsManager.");
    }

    void ShowPage(int page)
    {
        // Xóa các button cũ
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        int startIndex = page * taskPerPage;
        int endIndex = Mathf.Min(startIndex + taskPerPage, allTasks.Length);

        for (int i = startIndex; i < endIndex; i++)
        {
            var task = allTasks[i];
            GameObject buttonObj = Instantiate(taskButtonPrefab, contentPanel);
            TMP_Text label = buttonObj.GetComponentInChildren<TMP_Text>();
            label.text = task.levelName;

            Button btn = buttonObj.GetComponent<Button>();
            btn.interactable = !task.isLocked;

            if ((i + 1) % 6 == 0)
            {
                btn.GetComponent<Image>().color = new Color(0.980f, 0.282f, 0.325f);
            }
            else
            {
                btn.GetComponent<Image>().color = Color.white; // Màu trắng cho các nút khác
            }

            btn.onClick.AddListener(() => OnTaskButtonClicked(task.taskName));
        }

        // Bật / Tắt nút chuyển trang
        prevButton.SetActive(page > 0);
        nextButton.SetActive((page + 1) * taskPerPage < allTasks.Length);
    }

    public void OnTaskButtonClicked(string taskName)
    {
        TaskSettingsManager.Instance.SetCurrentTask(taskName);
        if (TaskSettingsManager.currentTask.isLocked == false)
        {
            SceneManager.LoadScene(taskName);
        }
    }

    void OnNextPage()
    {
        currentPage++;
        ShowPage(currentPage);
    }

    void OnPrevPage()
    {
        currentPage--;
        ShowPage(currentPage);
    }
}
