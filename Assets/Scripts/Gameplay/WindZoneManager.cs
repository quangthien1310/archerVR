using UnityEngine;
using System.Collections;

public class WindZoneManager : MonoBehaviour
{
    public static WindZoneManager Instance;
    public WindZone windZone;
    public GameObject windSoundPrefab;
    public GameObject windEffectPrefab;
    public GameObject Camera;

    [Header("Wind Zone Settings")]
    public float minStrength = 0f;
    public float maxStrength = 3f;
    public int numberOfWindEffects = 5; // Số lượng hiệu ứng gió tối đa
    public float windEffectDuration = 10f; // Thời gian hiệu ứng gió tồn tại
    public BoxCollider spawnArea; // Gán Box Collider của SpawnArea trong Inspector
    
    Vector3 randomDirection; // Biến lưu trữ hướng gió ngẫu nhiên
    int randomStrength; // Biến lưu trữ lực gió ngẫu nhiên
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        randomDirection = Random.onUnitSphere;
        randomDirection.y = 0; // Keep the wind direction horizontal
        randomDirection.Normalize(); // Normalize the direction vector
        windZone.transform.rotation = Quaternion.LookRotation(randomDirection);

        //windZone.windMain = (int) Random.Range(minStrength, maxStrength);
        randomStrength = Random.Range(0, 4);
        windZone.windMain = randomStrength; // Lực gió ngẫu nhiên từ 0 đến 3

        Debug.Log($"🌬️ Gió hướng: {randomDirection.normalized}, Lực: {windZone.windMain}");

        SpawnWindSound(randomDirection, randomStrength);

        if (randomStrength > 0)
        {
            for (int i = 0; i < numberOfWindEffects; i++)
            {
                SpawnWindEffects(randomDirection, randomStrength);
            }
        }
    }

    public Vector3 GetWindForce()
    {
        if (windZone == null) return Vector3.zero;

        Vector3 dir = windZone.transform.forward;
        float strength = windZone.windMain;

        return dir.normalized * strength;
    }

    public float GetWindStrength()
    {
        if (windZone == null) return 0f;

        return windZone.windMain;
    }

    // Hàm spawn một AudioSource theo hướng gió
    private void SpawnWindSound(Vector3 windDirection, float windStrength)
    {
        GameObject windSound = Instantiate(windSoundPrefab, Camera.transform.position - windDirection * 10f, Quaternion.identity); // Tạo GameObject tại vị trí gió
        AudioSource audioSource = windSound.GetComponent<AudioSource>();

        // Cài đặt âm thanh cho wind sound
        audioSource.spatialBlend = 1.0f; // Đảm bảo âm thanh 3D
        audioSource.loop = true;
        audioSource.volume = windStrength / 3f;
        Debug.Log($"Âm lượng gió: {audioSource.volume}");
        //audioSource.playOnAwake = true;

        // Bắt đầu phát âm thanh
        audioSource.Play();
    }

    private void SpawnWindEffects(Vector3 windDirection, float windStrength)
    {
        // Spawn mỗi hiệu ứng ở một vị trí ngẫu nhiên gần vị trí của windZone
        GameObject windEffect = Instantiate(windEffectPrefab, GetRandomSpawnPosition(), Quaternion.LookRotation(windDirection));

        // Điều chỉnh hướng và lực của gió cho WindEffect (Particle System)
        ParticleSystem windParticles = windEffect.GetComponent<ParticleSystem>();
        ParticleSystem.VelocityOverLifetimeModule velocityModule = windParticles.velocityOverLifetime;
        velocityModule.enabled = true;

        var mainModule = windParticles.main;

        mainModule.startDelay = Random.Range(0f, 2f); // Thời gian delay ngẫu nhiên

        windParticles.Play(); // Bắt đầu phát particle
    }

    Vector3 GetRandomSpawnPosition()
    {
        if (spawnArea == null)
        {
            Debug.LogError("SpawnArea chưa được gán! Hãy kéo Box Collider vào Inspector.");
            return Vector3.zero;
        }

        // Lấy giới hạn của Box Collider
        Vector3 center = spawnArea.bounds.center;
        Vector3 size = spawnArea.bounds.extents;

        // Sinh vị trí ngẫu nhiên bên trong Box Collider
        Vector3 randomOffset = new Vector3(
            Random.Range(-size.x, size.x),
            Random.Range(-size.y, size.y),
            Random.Range(-size.z, size.z)
        );

        return center + randomOffset;
    }
}