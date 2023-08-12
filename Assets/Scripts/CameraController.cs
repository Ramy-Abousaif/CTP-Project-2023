using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float speedMultiplier = 1f;
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    public Transform target;
    public string focusTag = "Planet";
    public bool isOrbiting = false;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isRightClicking = false;
    private float radius = 0f;
    private float distanceToTarget = 5f;
    private float minDistanceToTarget = 2f;
    private float maxDistanceToTarget = 20f;

    void Update()
    {
        // Check if right mouse button is held down
        isRightClicking = Input.GetMouseButton(1);

        if (Input.GetKey(KeyCode.LeftShift))
            speedMultiplier = 2f;
        else
            speedMultiplier = 1f;

        // Camera rotation based on right-click
        if (isRightClicking)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            rotationX += Input.GetAxis("Mouse X") * rotationSpeed;
            rotationY -= Input.GetAxis("Mouse Y") * rotationSpeed;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);
            transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Camera movement using WASD
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (!isRightClicking)
        {
            CheckFocusObject();
        }

        if (isOrbiting)
        {
            if (target == null)
            {
                isOrbiting = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                distanceToTarget -= Input.GetAxis("Mouse ScrollWheel") * movementSpeed;
                distanceToTarget = Mathf.Clamp(distanceToTarget, minDistanceToTarget + radius, maxDistanceToTarget + radius);

                Vector3 focusPosition = target.position;
                Vector3 dirToCamera = Quaternion.Euler(rotationY, rotationX, 0f) * Vector3.back;
                transform.position = focusPosition + dirToCamera * distanceToTarget;
                transform.LookAt(focusPosition);
            }
        }
        else
        {
            Vector3 moveDirection = transform.forward * vertical + transform.right * horizontal;
            transform.position += moveDirection * movementSpeed * Time.deltaTime * speedMultiplier;
        }
    }

    void CheckFocusObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag(focusTag))
            {
                // Focus on the clicked object
                target = hit.transform;
                isOrbiting = true;
                radius = hit.transform.GetComponent<GravitationalBody>().radius;
                distanceToTarget = radius + minDistanceToTarget;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                target = null;
                isOrbiting = false;
            }
        }
    }
}
