using System.Collections;
using UnityEngine;

public class BowController : MonoBehaviour
{
    [Header("Prefabs & Transforms")]
    public GameObject arrowPrefab;          // Prefab mũi tên
    public Transform shootPoint;            // Vị trí bắn tên (nơi bắn ra)
    public Transform arrowHoldPoint;        // Vị trí giữ tên (trên cung)
    public LayerMask aimLayerMask;          // Layer để Raycast quét

    [Header("Bow Shooting Settings")]
    public float shootForce = 30f;          // Lực bắn tối đa
    public float pullTime = 2f;             // Thời gian kéo cung để đạt lực tối đa
    public float minShootForce = 10f;       // Lực bắn tối thiểu
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f); // Offset xoay nếu model mũi tên bị lệch

    private GameObject currentArrow;        // Mũi tên hiện tại đang được giữ
    private Vector3 targetPoint;            // Điểm va chạm raycast từ camera
    private Vector3 predictedShootDirection;// Hướng bắn dự đoán

    private float currentPullTime;          // Thời gian kéo cung hiện tại
    private bool isPulling = false;         // Trạng thái kéo cung

    void Start()
    {
        SpawnArrow(); // Tạo mũi tên đầu tiên
    }

    void Update()
    {
        // UpdateRaycast();

        if (Input.GetMouseButton(0) && currentArrow != null)
        {
            isPulling = true;
            currentPullTime += Time.deltaTime;
            currentPullTime = Mathf.Clamp(currentPullTime, 0f, pullTime);

            float pullDistance = Mathf.Lerp(0f, 0.5f, currentPullTime / pullTime);
            currentArrow.transform.localPosition = new Vector3(0f, 0f, -pullDistance); // Kéo mũi tên về sau
        }
        else if (Input.GetMouseButtonUp(0) && isPulling)
        {
            FireCurrentArrow();
            StartCoroutine(SpawnArrowDelayed(1f)); // Delay tạo mũi tên tiếp theo
            isPulling = false;
            currentPullTime = 0f;
        }
    }

    // void UpdateRaycast()
    // {
    //     Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
    //     RaycastHit hit;

    //     if (Physics.Raycast(ray, out hit, 100f, aimLayerMask))
    //     {
    //         targetPoint = hit.point;
    //         Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
    //     }
    //     else
    //     {
    //         targetPoint = ray.origin + ray.direction * 100f;
    //         Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
    //     }

    //     predictedShootDirection = (targetPoint - shootPoint.position).normalized;

    //     // Ray từ vị trí bắn đến target
    //     Debug.DrawRay(shootPoint.position, predictedShootDirection * 100f, Color.blue);
    // }

    void SpawnArrow()
    {
        Quaternion arrowRotation = Quaternion.LookRotation(arrowHoldPoint.forward) * Quaternion.Euler(rotationOffset);
        currentArrow = Instantiate(arrowPrefab, arrowHoldPoint.position, arrowRotation, arrowHoldPoint);

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Ray chỉ hướng giữ của cung (góc nhìn của VR bow)
        Debug.DrawRay(arrowHoldPoint.position, arrowHoldPoint.forward * 2f, Color.cyan);
    }

    void FireCurrentArrow()
    {
        if (currentArrow == null) return;

        currentArrow.transform.parent = null;

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            float currentShootForce = shootForce * (currentPullTime / pullTime);
            currentShootForce = Mathf.Max(currentShootForce, minShootForce);

            rb.AddForce(arrowHoldPoint.forward * currentShootForce, ForceMode.Impulse);
        }

        currentArrow = null;
    }

    private IEnumerator SpawnArrowDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrow();
    }
}
