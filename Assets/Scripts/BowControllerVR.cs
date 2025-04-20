using System.Collections;
using UnityEngine;
using Valve.VR;

public class BowControllerVR : MonoBehaviour
{
    [Header("Prefabs & Transforms")]
    public GameObject arrowPrefab;
    public Transform shootPoint;
    public Transform arrowHoldPoint;
    public LayerMask aimLayerMask;

    [Header("Bow Shooting Settings")]
    public float shootForce = 30f;
    public float minShootForce = 10f;
    public float maxPullDistance = 0.7f; // Giới hạn khoảng cách kéo tối đa
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);

    private GameObject currentArrow;
    private bool isPulling = false;

    // SteamVR input actions
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Input_Sources handTypeLeft = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources handTypeRight = SteamVR_Input_Sources.RightHand;

    [SerializeField] private SteamVR_Behaviour_Pose poseLeft;
    [SerializeField] private SteamVR_Behaviour_Pose poseRight;
    private float pullDistance = 0f;

    void Start()
    {
        SpawnArrow();
    }

    void Update()
    {
        if (grabAction.GetState(handTypeRight) && currentArrow != null)
        {
            if (Vector3.Distance(poseRight.transform.position, arrowHoldPoint.position) < 0.8f)
            {
                isPulling = true;
            }

            if (isPulling)
            {
                pullDistance = Vector3.Distance(poseLeft.transform.position, poseRight.transform.position);
                pullDistance = Mathf.Clamp(pullDistance, 0f, maxPullDistance);
                Debug.Log("Pull Distance: " + pullDistance);
                currentArrow.transform.localPosition = new Vector3(0f, 0f, -pullDistance); // Kéo mũi tên về sau
            }
        }
        else if (grabAction.GetStateUp(handTypeRight) && isPulling)
        {
            FireCurrentArrow();
            StartCoroutine(SpawnArrowDelayed(1f));
            isPulling = false;
        }
    }

    void SpawnArrow()
    {
        Quaternion arrowRotation = Quaternion.LookRotation(arrowHoldPoint.forward) * Quaternion.Euler(rotationOffset);
        currentArrow = Instantiate(arrowPrefab, arrowHoldPoint.position, arrowRotation, arrowHoldPoint);

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void FireCurrentArrow()
    {
        if (currentArrow == null) return;

        currentArrow.transform.parent = null;

        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;

            // pullDistance = Vector3.Distance(poseLeft.transform.position, poseRight.transform.position);
            // pullDistance = Mathf.Clamp(pullDistance, 0f, maxPullDistance);
            float force = minShootForce + ((pullDistance - 0.15f) / (maxPullDistance - 0.15f)) * (shootForce - minShootForce);

            Debug.Log("Force applied to arrow: " + force);

            rb.AddForce(arrowHoldPoint.forward * force, ForceMode.Impulse);

            Vector3 windForce = WindZoneManager.Instance.GetWindForce();
            rb.AddForce(windForce, ForceMode.Impulse);
        }

        currentArrow = null;
    }

    private IEnumerator SpawnArrowDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrow();
    }
}
