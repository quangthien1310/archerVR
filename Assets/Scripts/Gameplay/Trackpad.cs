using UnityEngine;
using Valve.VR;

public class Trackpad : MonoBehaviour
{
    public float turnSpeed = 45f;

    public GameObject player;

    public void SnapTurnRight()
    {
        player.transform.Rotate(Vector3.up, turnSpeed);
    }

    public void SnapTurnLeft()
    {
        player.transform.Rotate(Vector3.up, -turnSpeed);
    }
}