using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LevelDesign : MonoBehaviour
{
    public static LevelDesign Instance;
    public GameObject targetPrefab;
    public Collider spawnArea;
    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;
    public float zSpeed = 2f;
    private Vector3 horizontalDirection = Vector3.right; // Hướng di chuyển ban đầu
    private Vector3 verticalDirection = Vector3.up; // Hướng di chuyển ban đầu
    private Vector3 zDirection = Vector3.forward; // Hướng di chuyển ban đầu
    public int level = 2;
    public Vector3 originalSize = new Vector3(100f, 100f, 20f); // Kích thước ban đầu của targetPrefab

    bool sizechange = false;
    public float spawnTime = 2f;
    public float activeTime = 0f;
    public int maxLife = 3;

    //Level12 Design
    private List<GameObject> activeTargets = new List<GameObject>();
    public float noHitTimer = 0f;
    public float noHitDuration = 5f;
    private bool hitBoundary = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy đi
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        originalSize = targetPrefab.transform.localScale; // Lưu kích thước ban đầu
        level = TaskSettingsManager.currentTask.level; // Lấy level từ TaskSettingsManager

        if (level == 12)
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnNewTarget();
            }

            targetPrefab.SetActive(false); // Ẩn targetPrefab khi bắt đầu
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayManager.Instance.isGameRunning == false) return;
        switch (level)
        {
            case 1:
                horizontalSpeed = 0f;
                verticalSpeed = 0f;
                break;
            case 2:
                HorizontalMove(horizontalSpeed);
                break;
            case 3:
                HorizontalMove(horizontalSpeed);
                VerticalMove(verticalSpeed);
                break;
            case 4:
                sizechange = true;
                break;
            case 5:
                SpawnTimeLimit();
                break;
            case 6:
                Level6Design();
                break;
            case 7:
                horizontalSpeed = 0f;
                verticalSpeed = 0f;
                break;
            case 8:
                ZMove(zSpeed);
                break;
            case 9:
                ZMove(zSpeed);
                VerticalMove(verticalSpeed);
                break;
            case 10:
                ZMove(zSpeed);
                VerticalMove(verticalSpeed);
                HorizontalMove(horizontalSpeed);
                break;
            case 11:
                SpawnTimeLimit();
                break;
            case 12:
                Level12Design();
                break;
            default:
                break;
        }
    }

    void HorizontalMove(float speed)
    {
        Vector3 newPosition = targetPrefab.transform.position + horizontalDirection * speed * Time.deltaTime;

        // Lấy biên của spawnArea
        Bounds bounds = spawnArea.bounds;
        float halfWidth = targetPrefab.GetComponent<Renderer>().bounds.extents.x;

        // Kiểm tra nếu chạm biên trái hoặc phải thì đổi hướng
        if (newPosition.x - halfWidth < bounds.min.x)
        {
            newPosition.x = bounds.min.x + halfWidth;
            horizontalDirection = Vector3.right;
        }
        else if (newPosition.x + halfWidth > bounds.max.x)
        {
            newPosition.x = bounds.max.x - halfWidth;
            horizontalDirection = Vector3.left;
        }

        targetPrefab.transform.position = newPosition;
    }

    void VerticalMove(float speed)
    {
        Vector3 newPosition = targetPrefab.transform.position + verticalDirection * speed * Time.deltaTime;

        // Lấy biên của spawnArea
        Bounds bounds = spawnArea.bounds;
        float halfHeight = targetPrefab.GetComponent<Renderer>().bounds.extents.y;

        // Kiểm tra nếu chạm biên trên hoặc dưới thì đổi hướng
        if (newPosition.y - halfHeight < bounds.min.y)
        {
            newPosition.y = bounds.min.y + halfHeight;
            verticalDirection = Vector3.up;
        }
        else if (newPosition.y + halfHeight > bounds.max.y)
        {
            newPosition.y = bounds.max.y - halfHeight;
            verticalDirection = Vector3.down;
        }

        targetPrefab.transform.position = newPosition;
    }

    void ZMove(float speed)
    {
        Vector3 newPosition = targetPrefab.transform.position + zDirection * speed * Time.deltaTime;

        // Lấy biên của spawnArea
        Bounds bounds = spawnArea.bounds;
        float halfDepth = targetPrefab.GetComponent<Renderer>().bounds.extents.z;

        // Kiểm tra nếu chạm biên trước hoặc sau thì đổi hướng
        if (newPosition.z - halfDepth < bounds.min.z)
        {
            hitBoundary = true;
            newPosition.z = bounds.min.z + halfDepth;
            zDirection = Vector3.forward;
        }
        else if (newPosition.z + halfDepth > bounds.max.z)
        {
            newPosition.z = bounds.max.z - halfDepth;
            zDirection = Vector3.back;
        }

        targetPrefab.transform.position = newPosition;
    }

    public void SizeChange()
    {
        if (sizechange == false) return;
        Vector3 currentSize = targetPrefab.transform.localScale;
        Vector3 newSize = currentSize * 0.8f;

        // Nếu nhỏ hơn minSize thì reset về size ban đầu
        if (newSize.x <= 25f)
        {
            newSize = originalSize;
        }

        targetPrefab.transform.localScale = newSize;
    }

    void SpawnTimeLimit()
    {
        // Thay đổi thời gian spawn của targetPrefab
        activeTime += Time.deltaTime;
        if (activeTime >= spawnTime)
        {
            activeTime = 0f;
            targetPrefab.GetComponent<Target>().RespawnTarget();
            maxLife--;
        }
        ;
    }

    void Level6Design()
    {
        HorizontalMove(horizontalSpeed);
        VerticalMove(verticalSpeed);
        sizechange = true;
        SpawnTimeLimit();
        if (maxLife <= 0)
        {
            GameplayManager.Instance.EndGame();
        }
    }

    void Level12Design()
    {
        // Di chuyển tất cả target
        for (int i = activeTargets.Count - 1; i >= 0; i--)
        {
            GameObject target = activeTargets[i];
            if (target == null)
            {
                activeTargets.RemoveAt(i);
                continue;
            }

            // Di chuyển target về phía đầu (theo trục -Z)
            target.transform.position += Vector3.back * zSpeed * Time.deltaTime;

            // Kiểm tra chạm biên spawnArea
            Bounds bounds = spawnArea.bounds;
            float halfDepth = target.GetComponent<Renderer>().bounds.extents.z;

            if (target.transform.position.z - halfDepth < bounds.min.z)
            {
                hitBoundary = true;
                target.GetComponent<Target>().RespawnTarget();
                //activeTargets.RemoveAt(i);
            }
        }

        // Xử lý trừ mạng nếu có target chạm biên
        if (hitBoundary)
        {
            maxLife--;
            Debug.Log("Target hit boundary! Lives left: " + maxLife);
            hitBoundary = false;
        }

        // Nếu hết mạng, kết thúc game
        if (maxLife <= 0)
        {
            GameplayManager.Instance.EndGame();
            return;
        }

        // Đếm thời gian không bắn được target nào
        noHitTimer += Time.deltaTime;

        // Nếu 5 giây không bắn trúng target nào thì spawn thêm 1 target mới
        if (noHitTimer >= noHitDuration)
        {
            noHitTimer = 0f;
            SpawnNewTarget();
        }
    }

    void SpawnNewTarget()
    {
        Vector3 spawnPos = new Vector3(0f, 0f, 0f);
        GameObject newTarget = Instantiate(targetPrefab, spawnPos, Quaternion.Euler(-180f, 0f, 0f));
        newTarget.GetComponent<Target>().RespawnTarget(); // Gọi hàm RespawnTarget để đặt lại vị trí và trạng thái của target mới
        activeTargets.Add(newTarget);
    }
    
    public void OnTargetHit()
    {
        noHitTimer = 0f;
    }
}