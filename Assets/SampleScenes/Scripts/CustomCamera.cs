using UnityEngine;

public class CustomCamera : MonoBehaviour
{
    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    private float rotationAngle;
    private float rotationYAngle;

    public float rotationSpeed;

    public float stickMinZoom, stickMaxZoom;

    private Transform swivel, stick;

    public float swivelMinZoom, swivelMaxZoom;

    private float zoom = 1f;

    private void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);

        var origin = transform.localEulerAngles;
        rotationAngle = origin.y;
    }

    private void Update()
    {
        var zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f) AdjustZoom(zoomDelta);

        var rotationDelta = InputManager.GetMouse_X(); ;
        if (rotationDelta != 0f) AdjustRotation(rotationDelta);
        
        var rotationYDelta = InputManager.GetMouse_Y(); ;
        if (rotationYDelta != 0f) AdjustYRotation(rotationYDelta);
        
        transform.localRotation = Quaternion.Euler(rotationYAngle, rotationAngle, 0f);

        var xDelta = Input.GetAxis("Horizontal");
        var zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f) AdjustPosition(xDelta, zDelta);
    }

    private void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        var distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        var angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
            rotationAngle += 360f;
        else if (rotationAngle >= 360f) rotationAngle -= 360f;
        
    }

    private void AdjustYRotation(float delta)
    {
        rotationYAngle += -delta * rotationSpeed * Time.deltaTime * 10f;
        if (rotationYAngle <= -90f)
            rotationYAngle = -90f;
        else if (rotationYAngle >= 90) rotationAngle = 90;
    }

    private void AdjustPosition(float xDelta, float zDelta)
    {
        var direction =
            transform.localRotation *
            new Vector3(xDelta, 0f, zDelta).normalized;
        var damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        var distance =
            Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
            damping * Time.deltaTime;

        var position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        return position;
    }
}