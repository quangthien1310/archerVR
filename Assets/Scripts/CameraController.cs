using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    private float xRotation = 0f;
    private bool CameraLock = true;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(EnableCameraController());
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraLock)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; //Gets the mouse input on the x axis
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime; //Gets the mouse input on the y axis

        xRotation -= mouseY; //Subtracts the mouseY from xRotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //Clamps the xRotation between -90 and 90

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); //Rotates the camera on the x axis

        playerBody.Rotate(Vector3.up * mouseX); //Rotates the player on the y axis
    }

    IEnumerator EnableCameraController()
    {
        yield return new WaitForSeconds(2f);
        CameraLock = false;
    }
}
