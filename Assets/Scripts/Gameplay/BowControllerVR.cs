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
    private float pullDistance = 0f;

    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);

    private GameObject currentArrow;
    private GameObject arrowTail;
    private AudioSource arrowAudioSource;
    private bool isPulling = false;

    // SteamVR input actions
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Input_Sources handTypeLeft = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources handTypeRight = SteamVR_Input_Sources.RightHand;

    [SerializeField] private SteamVR_Behaviour_Pose poseLeft;
    [SerializeField] private SteamVR_Behaviour_Pose poseRight;

    [Header("Haptics")]
    public SteamVR_Action_Vibration hapticAction;

    [Header("Haptics Pull Settings")]
    public float hapticDuration = 0.01f;  // thời gian rung mỗi frame
    public float hapticFrequency = 50f;   // tần số rung
    public float maxHapticAmplitude = 0.9f; // độ rung mạnh nhất

    void Start()
    {
        StartCoroutine(SpawnArrowDelayed(0.5f));
    }

    void Update()
    {
        if (GameplayManager.Instance.isGameRunning == false) return;
        if (grabAction.GetState(handTypeRight) && currentArrow != null && arrowTail != null)
        {
            if (Vector3.Distance(poseRight.transform.position, arrowTail.transform.position) < 0.1f)
            {
                isPulling = true;
            }

            if (isPulling)
            {
                pullDistance = Vector3.Distance(poseLeft.transform.position, poseRight.transform.position);
                pullDistance = Mathf.Clamp(pullDistance, 0f, maxPullDistance);
                currentArrow.transform.localPosition = new Vector3(0f, 0f, -pullDistance + 0.15f); // Kéo mũi tên về sau

                // Rung theo lực kéo
                float normalizedPull = pullDistance / maxPullDistance;  // từ 0 → 1
                float amplitude = Mathf.Lerp(0f, maxHapticAmplitude, normalizedPull);

                hapticAction.Execute(0f, hapticDuration, hapticFrequency, amplitude, handTypeLeft);
            }
        }
        else if (grabAction.GetStateUp(handTypeRight) && isPulling)
        {
            FireCurrentArrow();
            isPulling = false;
            StartCoroutine(SpawnArrowDelayed(0.5f));
        }
    }

    void SpawnArrow()
    {
        Quaternion arrowRotation = Quaternion.LookRotation(arrowHoldPoint.forward) * Quaternion.Euler(rotationOffset);
        currentArrow = Instantiate(arrowPrefab, arrowHoldPoint.position, arrowRotation, arrowHoldPoint);

        arrowTail = currentArrow.transform.Find("ArrowTail").gameObject;
        arrowAudioSource = arrowTail.GetComponent<AudioSource>();

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
            if (arrowAudioSource != null)
            {
                arrowAudioSource.Play();
                Debug.Log("Arrow audio played");
            }

            rb.isKinematic = false;

            float force = minShootForce +  (shootForce - minShootForce) * ((pullDistance - 0.15f) / (maxPullDistance - 0.15f));

            rb.AddForce(arrowHoldPoint.forward * force, ForceMode.Impulse);

            Vector3 windForce = WindZoneManager.Instance.GetWindForce();
            rb.AddForce(windForce, ForceMode.Impulse);

            hapticAction.Execute(0f, 0.01f, 75f, 1f, handTypeRight);
        }

        GameplayManager.Instance.RegisterShot();

        currentArrow = null;
    }

    private IEnumerator SpawnArrowDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnArrow();
    }
}
