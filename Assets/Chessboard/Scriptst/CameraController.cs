using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Drag Settings")]
    public float dragSpeed = 5f;
    public bool invertDrag = false;
    public LayerMask ignoreLayers; // Добавляем маску слоёв

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 50f;
    public bool invertZoom = false;

    private Camera controlledCamera;
    private Vector3 dragOrigin;
    private bool isDragging;
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void Awake()
    {
        controlledCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleDrag();
        HandleZoom();
    }

    private void HandleDrag()
    {
        // Start drag - проверяем, не нажали ли на игнорируемый слой
        if (Input.GetMouseButtonDown(0))
            if (!IsPointerOverIgnoredLayer())
            {
                dragOrigin = GetMouseWorldPositionOnGround();
                isDragging = true;
            }

        // End drag
        if (Input.GetMouseButtonUp(0))
            isDragging = false;

        // During drag
        if (isDragging)
        {
            Vector3 currentPos = GetMouseWorldPositionOnGround();
            Vector3 difference = dragOrigin - currentPos;

            if (invertDrag)
                difference *= -1;

            transform.position += difference * dragSpeed * Time.deltaTime;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            Ray ray = controlledCamera.ScreenPointToRay(Input.mousePosition);
            Vector3 zoomDirection = ray.direction.normalized;

            if (invertZoom)
                scroll *= -1;

            float zoomAmount = scroll * zoomSpeed;
            Vector3 movement = zoomDirection * zoomAmount;

            float proposedDistance = Vector3.Distance(transform.position + movement, GetPointUnderMouse());

            if (proposedDistance >= minZoomDistance && proposedDistance <= maxZoomDistance)
                transform.position += movement;
        }
    }

    private bool IsPointerOverIgnoredLayer()
    {
        RaycastHit hit;
        Ray ray = controlledCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ignoreLayers))
            return true;

        return false;
    }

    private Vector3 GetMouseWorldPositionOnGround()
    {
        float distance;
        Ray ray = controlledCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }

    private Vector3 GetPointUnderMouse()
    {
        float distance;
        Ray ray = controlledCamera.ScreenPointToRay(Input.mousePosition);

        if (groundPlane.Raycast(ray, out distance))
            return ray.GetPoint(distance);

        return transform.position + ray.direction * 10f;
    }
}
