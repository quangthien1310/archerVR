using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowController : MonoBehaviour
{
    public GameObject arrowPrefab; //Prefab of the arrow
    public Transform shootPoint;    //Point from arrow
    public float shootForce = 50f; //Force of the arrow

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Vector3 shootDirection;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            // Hướng bắn từ shootPoint đến mục tiêu (điểm Raycast chạm)
            shootDirection = (hit.point - shootPoint.position).normalized;

            // Vẽ Debug Ray để kiểm tra hướng bắn (Xanh lá nếu trúng mục tiêu)
            Debug.DrawRay(shootPoint.position, shootDirection * hit.distance, Color.green, 2f);
        }
        else
        {
            // Nếu Raycast không trúng gì, bắn theo hướng Camera
            shootDirection = Camera.main.transform.forward;

            // Vẽ Debug Ray không trúng gì (Màu đỏ)
            Debug.DrawRay(shootPoint.position, shootDirection * 100f, Color.red, 2f);
        }

        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, Quaternion.LookRotation(shootDirection) * Quaternion.Euler(-90, 0, 0));
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
    }
}