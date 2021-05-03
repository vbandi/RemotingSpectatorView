using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attached to the spectator camera, 
/// </summary>
public class RemotingSpectatorView : MonoBehaviour
{
    public bool IsRunning { get; private set; }

    
    [Tooltip("Distance of spectator camera from the head when SetPositionInFrontOfMainCamera is called")]
    [SerializeField]
    private float _cameraDistanceFromHead;

    
    /// <summary>
    /// Distance of spectator camera from the head when SetPositionInFrontOfMainCamera is called
    /// </summary>
    public float CameraDistanceFromHead
    {
        get => _cameraDistanceFromHead;
        set
        {
            _cameraDistanceFromHead = value;
            SetPositionInFrontOfMainCamera();
        }
    }

    public Camera Camera
    {
        get
        {
            if (_camera == null)
            {
                _camera = GetComponentInChildren<Camera>(true);
            }

            return _camera;
        }
    }


    [HideInInspector]
    public int CameraIndex = 0;


    private WebCamTexture _webCamtexture;
    private Camera _camera;
    private RawImage _image;


    void Awake()
    {
        _image = GetComponentInChildren<RawImage>(true);
    }

    [ContextMenu("Set Position In Front of Main Camera")]
    public void SetPositionInFrontOfMainCamera()
    {
        var mainCamera = Camera.main;
        var mainCameraTransform = mainCamera.transform;
        transform.position = mainCameraTransform.position + (mainCameraTransform.forward.normalized * CameraDistanceFromHead);
        transform.LookAt(mainCameraTransform);
    }

    private void OnEnable()
    {
        SetPositionInFrontOfMainCamera();
    }

    private void OnDisable()
    {
        StopCamera();
    }

    public Texture StartCamera()
    {
        IsRunning = true;
        var devices = WebCamTexture.devices;
        _webCamtexture = new WebCamTexture(devices[CameraIndex].name);
        _image.texture = _webCamtexture;
        _webCamtexture.Play();
        Camera.targetTexture = new RenderTexture(_webCamtexture.width, _webCamtexture.height, 32);
        return Camera.targetTexture;
    }

    public void StopCamera()
    {
        IsRunning = false;
        if (_webCamtexture != null)
        {
            _webCamtexture.Stop();
        }
    }
}
