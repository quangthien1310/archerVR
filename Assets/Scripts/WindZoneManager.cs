using UnityEngine;

public class WindZoneManager : MonoBehaviour
{
    public static WindZoneManager Instance;
    public WindZone windZone;

    [Header("Wind Zone Settings")]
    public float minStrength = 1.0f;
    public float maxStrength = 2.0f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Vector3 randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep the wind direction horizontal
        windZone.transform.rotation = Quaternion.LookRotation(randomDirection);

        windZone.windMain = Random.Range(minStrength, maxStrength);

        Debug.Log($"🌬️ Gió hướng: {randomDirection.normalized}, Lực: {windZone.windMain}");
    }

    public Vector3 GetWindForce()
    {
        if (windZone == null) return Vector3.zero;

        Vector3 dir = windZone.transform.forward;
        float strength = windZone.windMain;

        return dir.normalized * strength;
    }
}
