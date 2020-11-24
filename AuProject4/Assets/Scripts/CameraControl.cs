using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float mouseSensitivity;
    float verticalRotation = 0f;
    public bool autoLocked = false;
    private Transform target;

    public GameObject autoLockCursor;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity", 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (!autoLocked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;


            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

            transform.parent.transform.Rotate(Vector3.up * mouseX);
        }
        else
        {
            transform.LookAt(target);
            transform.parent.transform.LookAt(target);
            autoLockCursor.transform.position = new Vector3(target.position.x, target.position.y + 2.3f, target.position.z);
            autoLockCursor.transform.LookAt(transform);
        }
        
    }

    public void SetTarget(Transform newTarget)
    {
        autoLockCursor.SetActive(true);
        autoLocked = true;
        target = newTarget;
    }

    public void RemoveTarget()
    {
        autoLocked = false;
        autoLockCursor.SetActive(false);
    }
}
