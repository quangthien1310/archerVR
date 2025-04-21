using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindDirectionUI : MonoBehaviour
{
    public Image windDirectionArrow;  // Mũi tên UI hiển thị hướng gió
    private Vector3 windDirection;    // Hướng gió
    private Vector3 playerDirection;  // Hướng nhìn của người chơi (nếu cần)
    public WindZone windZone; // Tham chiếu đến WindZone (nếu cần) 

    public GameObject player; // Tham chiếu đến đối tượng người chơi (nếu cần)

    void Update()
    {
        // Lấy hướng gió từ WindZoneManager
        windDirection = windZone.transform.rotation * Vector3.forward; // Lấy hướng gió từ WindZone

        playerDirection = player.transform.rotation * Vector3.forward; // Lấy hướng nhìn của người chơi (nếu cần)

        // Chuyển đổi từ Vector3 thành góc quay cho mũi tên (chỉ tính theo X và Z)
        float angle = Vector3.SignedAngle(windDirection, playerDirection, Vector3.up); // Tính góc giữa hướng nhìn của người chơi và hướng gió

        Debug.Log($" {windDirection.y}, {playerDirection.y}  Góc: {angle}");

        windDirectionArrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
