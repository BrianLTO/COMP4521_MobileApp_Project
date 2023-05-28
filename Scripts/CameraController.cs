using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    static public CameraController instance = null;

    Cinemachine.CinemachineVirtualCamera vCamera;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        vCamera = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    //set the camera to follow the transform (used only on the player object)
    public void SetCameraFollow(Transform transform)
    {
        vCamera.Follow = transform;
    }
}
