using UnityEngine;

public class CamZoomEffect : MonoBehaviour
{
    public static CamZoomEffect Instance;

    private Camera cam;
    private float originalSize;
    private float targetSize;
    private float zoomSpeed = 5f;
    private bool isZooming = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        originalSize = cam.orthographicSize;
    }

    void Update()
    {
        if (isZooming)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * zoomSpeed);

            // Nếu đã gần bằng target thì trả về original
            if (Mathf.Abs(cam.orthographicSize - targetSize) < 0.05f)
            {
                isZooming = false;
                cam.orthographicSize = targetSize;
            }
        }
    }

    /// <summary>
    /// Zoom đến kích thước chỉ định, rồi tự trở lại sau delay
    /// </summary>
    public void ZoomTemporarily(float zoomAmount = 0.8f, float duration = 0.3f)
    {
        targetSize = originalSize * zoomAmount;
        isZooming = true;
        CancelInvoke(nameof(ResetZoom));
        Invoke(nameof(ResetZoom), duration);
    }

    private void ResetZoom()
    {
        targetSize = originalSize;
        isZooming = true;
    }
}
