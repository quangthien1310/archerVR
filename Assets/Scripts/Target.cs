using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public BoxCollider spawnArea; // Gán Box Collider của SpawnArea trong Inspector
    public float respawnDelay = 1.5f; // Thời gian chờ trước khi respawn Target

    void Start()
    {
        RespawnTarget(); // Spawn Target khi game bắt đầu
    }

    void RespawnTarget()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition(); // Lấy vị trí spawn ngẫu nhiên
        transform.position = spawnPosition; 
        gameObject.SetActive(true); // Hiện lại Target
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ElvenArrow")) // Nếu bị bắn trúng bởi mũi tên
        {
            gameObject.SetActive(false); // Ẩn Target
            Invoke(nameof(RespawnTarget), respawnDelay); // Spawn lại sau một khoảng thời gian
        }
    }
}
