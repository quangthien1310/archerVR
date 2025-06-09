using Unity.Mathematics;
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

    public Color noWindColor = new Color(0, 0, 0, 0);
    public Color weakWindColor = new Color(0, 1, 0, 1); // Màu sắc của mũi tên khi có gió
    public Color normalWindColor = new Color(0, 1, 1, 1);
    public Color strongWindColor = new Color(1, 0, 0, 1);

    void Update()
    {
        // Lấy hướng gió từ WindZoneManager
        windDirection = windZone.transform.rotation * Vector3.forward; // Lấy hướng gió từ WindZone

        playerDirection = player.transform.rotation * Vector3.forward; // Lấy hướng nhìn của người chơi (nếu cần)

        float windStrength = WindZoneManager.Instance.GetWindStrength();

        switch (windStrength)
        {
            case 0f:
                windDirectionArrow.color = noWindColor; // Không có gió
                break;
            case 1f:
                windDirectionArrow.color = weakWindColor; // Gió yếu
                break;
            case 2f:
                windDirectionArrow.color = normalWindColor; // Gió bình thường
                break;
            case 3f:
                windDirectionArrow.color = strongWindColor; // Gió mạnh
                break;
            default:
                windDirectionArrow.color = noWindColor; // Mặc định không có gió
                break;
        }

        // Chuyển đổi từ Vector3 thành góc quay cho mũi tên (chỉ tính theo X và Z)
        float angle = Vector3.SignedAngle(windDirection, playerDirection, Vector3.up); // Tính góc giữa hướng nhìn của người chơi và hướng gió

        //Debug.Log($" {windDirection.y}, {playerDirection.y}  Góc: {angle}");

        windDirectionArrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
