using System.Collections;
using System.Collections.Generic;
using GabrielBissonnette.Primo;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public GameObject menuCanvas;

    public GameObject homeButton;
    public GameObject playButton;
    public GameObject settingButton;
    public GameObject trainingButton;
    public GameObject exitButton;
    public GameObject yesButton;
    public GameObject noButton;
    public GameObject resetButton;

    public AudioSource audioSource;

    public GameObject homePanel;
    public GameObject playPanel;
    public GameObject trainingPanel;
    public GameObject resultPanel;
    public GameObject settingPanel;
    public GameObject exitPanel;
    public GameObject background;

    public Transform menuCanvasTransform;
    public Transform cameraRotation;

    public ResultPanelManager resultPanelManager;

    public void menuBehaviour()
    {
        menuCanvas.SetActive(!menuCanvas.activeSelf);
        menuCanvas.transform.position = menuCanvasTransform.position;
        menuCanvas.transform.rotation = cameraRotation.rotation;
        Debug.Log("Menu canvas toggled!");

    }

    public void TurnOffAllPanels()
    {
        homePanel.SetActive(false);
        playPanel.SetActive(false);
        trainingPanel.SetActive(false);
        resultPanel.SetActive(false);
        settingPanel.SetActive(false);
        exitPanel.SetActive(false);
        
        Debug.Log("All panels turned off!");
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked!");
        audioSource.Play();

        TurnOffAllPanels();
        playPanel.SetActive(true);
    }

    // Khi bấm nút Home
    public void OnHomeButtonClicked()
    {
        Debug.Log("Home button clicked!");
        audioSource.Play();

        TurnOffAllPanels();
        homePanel.SetActive(true);
    }

    public void OnTrainingButtonClicked()
    {
        Debug.Log("Training button clicked!");
        audioSource.Play();

        TurnOffAllPanels();
        trainingPanel.SetActive(true);
    }

    public void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        audioSource.Play();

        exitPanel.SetActive(true);
    }

    public void OnYesButtonClicked()
    {
        Debug.Log("Yes button clicked!");
        audioSource.Play();

        Application.Quit();

        // #if UNITY_EDITOR
        // UnityEditor.EditorApplication.isPlaying = false;
        // #else
        //     Application.Quit();
        // #endif
    }

    public void OnNoButtonClicked()
    {
        Debug.Log("No button clicked!");
        audioSource.Play();

        exitPanel.SetActive(false);
    }

    public void OnSettingButtonClicked()
    {
        Debug.Log("Setting button clicked!");
        audioSource.Play();

        TurnOffAllPanels();
        settingPanel.SetActive(true);
    }

    public void OnResetButtonClicked()
    {
        Debug.Log("Reset button clicked!");
        audioSource.Play();

        // Reset all settings to default
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Reload the scene to apply changes
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowResultPanel(int score, int accuracy)
    {
        Debug.Log("Show result panel!");

        TurnOffAllPanels();
        resultPanel.SetActive(true);

        resultPanelManager.ShowResult(score, accuracy);
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MainMenuManager started!");

        Cursor.lockState = CursorLockMode.None;
        
        TurnOffAllPanels();
        homePanel.SetActive(true); // Show the home panel by default

        if (ResultData.hasResult)
        {
            ShowResultPanel(ResultData.finalScore, ResultData.finalAccuracy);
            ResultData.hasResult = false; // reset flag sau khi hiển thị
        }
    }
}
