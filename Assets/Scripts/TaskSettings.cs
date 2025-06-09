using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTaskSettings", menuName = "TaskSettings", order = 1)]
public class TaskSettings : ScriptableObject
{
    public string taskName;
    public int level;
    public string levelName;
    public float timeLimit;
    public int scorePerHit;
    public int passScore;
    public float passAccuracy;
    public bool isLocked;
    
    [HideInInspector]
    public int highScore;
}
