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
            if (target.gameObject.activeSelf != false)
            {
                transform.LookAt(target);
                //transform.parent.transform.LookAt(target);
                Vector3 lookVector = target.transform.position - transform.parent.transform.position;

                Quaternion rot = Quaternion.LookRotation(lookVector, Vector3.up);
                transform.parent.transform.rotation = Quaternion.Slerp(transform.parent.transform.rotation, rot, 2 * Time.deltaTime);
                autoLockCursor.transform.position = new Vector3(target.position.x, target.position.y + 2.5f, target.position.z);
                autoLockCursor.transform.LookAt(transform);
            }
            else
            {
                RemoveTarget();
            }
            
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
