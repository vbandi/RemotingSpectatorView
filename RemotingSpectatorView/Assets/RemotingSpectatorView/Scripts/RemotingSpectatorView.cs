using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RemotingSpectatorView : MonoBehaviour
{
    public bool IsRunning { get; private set; }

    public float CameraDistanceFromHead
    {
        get => _cameraDistanceFromHead;
        set
        {
            _cameraDistanceFromHead = value;
            SetPositionInFrontOfMainCamera();
        }
    }

    [HideInInspector]
    public int CameraIndex = 0;

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

    [SerializeField]
    private float _cameraDistanceFromHead;

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


#if UNITY_EDITOR
public class RemotingSpectatorViewWindow : EditorWindow
{
    [SerializeField]
    private bool _foldout;

    private bool IsCameraRunning => EditorApplication.isPlaying ? _target.IsRunning : _isRunning;
    private Texture RenderTexture => _renderTexture ? _renderTexture : _webCamtexture;

    private float _margin = 10f;

    private bool EditorIsPlaying
    {
        get
        {
            if (_editorIsPlaying != EditorApplication.isPlaying)
            {
                _editorIsPlaying = EditorApplication.isPlaying;


                SetPlayerprefsToTarget();


                if (_editorIsPlaying && _isRunning)
                {
                    StopPreview();
                    StartCamera();
                }
            }

            return _editorIsPlaying;
        }
    }

    enum RectPosition
    {
        Start,
        End,
        Full,
        Image
    }

    private Rect GetNextRect(int line, float startPosition, RectPosition rectPosition, float width, int height = 0)
    {
        var rect = new Rect();

        rect.y = 5f + line * 18f + line * 5f;
        rect.height = height == 0f ? 18f : height;

        switch (rectPosition)
        {
            case RectPosition.Start:
                rect.x = _margin;
                rect.width = position.width * width - _margin * 1.5f;
                break;
            case RectPosition.End:
                rect.x = position.width * startPosition + _margin * 0.5f;
                rect.width = position.width - (position.width * startPosition) - _margin * 1.5f;
                break;
            case RectPosition.Image:
                rect.height = position.height - rect.y - _margin;
                rect.x = _margin;
                rect.width = position.width - _margin * 2f;
                break;
            case RectPosition.Full:
                rect.x = _margin;
                rect.width = position.width - _margin * 2f;
                break;
        }


        return rect;
    }

    private static void SetPlayerprefsToTarget()
    {
        EnsurePrefabLoaded();
        _target.CameraIndex = EditorPrefs.GetInt("CameraIndex", 1) - 1;
        _target.CameraDistanceFromHead = EditorPrefs.GetFloat("DistanceFromHead", .6f);
        _target.Camera.fieldOfView = EditorPrefs.GetFloat("FieldOfView", 60);
    }

    [SerializeField]
    private bool _editorIsPlaying; //needs to be SerializeField, because new instance of the window will be created on application play

    [SerializeField]
    private bool _isRunning;

    [SerializeField]
    private WebCamTexture _webCamtexture;

    private Texture _renderTexture;

    private static RemotingSpectatorView _target;

    [SerializeField]
    private string[] _deviceNames;


    [MenuItem("Remoting Spectator View/Show window")]
    private static void ShowWindow()
    {
        GetWindow<RemotingSpectatorViewWindow>("Remoting Spectator View");
        EnsurePrefabLoaded();
    }

    private void OnDestroy()
    {
        if (IsCameraRunning)
        {
            StopCamera();
        }
    }

    private static void EnsurePrefabLoaded()
    {
        if (_target == null)
        {
            _target = FindObjectOfType<RemotingSpectatorView>();
            if (_target == null)
            {
                var go = Resources.Load("SpectatorCameraRoot");
                var instance = Instantiate(go);
                _target = FindObjectOfType<RemotingSpectatorView>();
            }
        }
    }

    private void Awake()
    {
        LoadDevices();
        SetPlayerprefsToTarget();
    }

    private void LoadDevices()
    {
        List<string> deviceNames = new List<string>();
        deviceNames.Add("None | Off");

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            deviceNames.Add(WebCamTexture.devices[i].name);
        }

        _deviceNames = deviceNames.ToArray();
    }

    private void Update()
    {
        if (IsCameraRunning)
        {
            Repaint();
        }
    }

    private void OnGUI()
    {
        int rowNumber = 0;
        EnsurePrefabLoaded();

        var isPlaying = EditorIsPlaying;

        if (_deviceNames == null)
        {
            LoadDevices();
        }

        int selectedDevice = EditorGUI.Popup(GetNextRect(rowNumber, 0f, RectPosition.Start, 0.66f), _target.CameraIndex + 1, _deviceNames);

        if (selectedDevice - 1 != _target.CameraIndex)
        {
            bool isRunning = IsCameraRunning;
            StopCamera();
            _target.CameraIndex = selectedDevice - 1;

            EditorPrefs.SetInt("CameraIndex", selectedDevice);

            if (isRunning)
            {
                StartCamera();
            }
        }

        if (GUI.Button(GetNextRect(rowNumber, .66f, RectPosition.End, 0.33f), (IsCameraRunning ? "Stop" : "Play") + (isPlaying ? "" : " preview")))
        {
            if (IsCameraRunning)
            {
                StopCamera();
            }
            else
            {
                StartCamera();
            }
        }

        rowNumber++;

        _foldout = EditorGUI.Foldout(GetNextRect(rowNumber, 0f, RectPosition.Full, 0f), _foldout, "Settings");
        rowNumber++;

        if (_foldout)
        {
            EditorGUI.BeginDisabledGroup(!isPlaying);

            Rect distanceRect;
            Rect fovRect;
            if (position.width > 400)
            {
                distanceRect = GetNextRect(rowNumber, 0f, RectPosition.Start, .5f);
                fovRect = GetNextRect(rowNumber, .5f, RectPosition.End, .5f);
                rowNumber++;
            }
            else
            {
                distanceRect = GetNextRect(rowNumber, 0f, RectPosition.Full, 0f);
                rowNumber++;
                fovRect = GetNextRect(rowNumber, 0f, RectPosition.Full, 0f);
                rowNumber++;
            }

            float targetDistance = EditorGUI.FloatField(distanceRect, "Distance from camera", _target.CameraDistanceFromHead);
            if (Math.Abs(targetDistance - _target.CameraDistanceFromHead) > 0.001f)
            {
                EditorPrefs.SetFloat("DistanceFromHead", targetDistance);
                _target.CameraDistanceFromHead = targetDistance;
            }

            float fov = EditorGUI.FloatField(fovRect, "Camera FOV", _target.Camera.fieldOfView);
            if (Math.Abs(fov - _target.Camera.fieldOfView) > 0.001f)
            {
                EditorPrefs.SetFloat("FieldOfView", fov);
                _target.Camera.fieldOfView = fov;
            }

            if (GUI.Button(GetNextRect(rowNumber, 0f, RectPosition.Full, 0f), "Reset Spectator Camera Position"))
            {
                _target.SetPositionInFrontOfMainCamera();
            }

            rowNumber++;
            EditorGUI.EndDisabledGroup();
        }

        if (RenderTexture != null)
        {
            var imageRect = GetNextRect(rowNumber, 0f, RectPosition.Image, 0f);

            float widthAspect = RenderTexture.width / (float) RenderTexture.height;
            float rectAspect = imageRect.width / imageRect.height;

            if (rectAspect > widthAspect)
            {
                float aspectRatio = imageRect.height / RenderTexture.height;
                imageRect.width = RenderTexture.width * aspectRatio;
                imageRect.x = (position.width - imageRect.width) / 2f;
            }
            else
            {
                float aspectRatio = imageRect.width / RenderTexture.width;
                imageRect.height = RenderTexture.height * aspectRatio;
            }

            EditorGUI.DrawPreviewTexture(imageRect, RenderTexture, null, ScaleMode.StretchToFill);
        }

        if (!IsCameraRunning)
        {
            GUI.Label(GetNextRect(rowNumber, 0f, RectPosition.Full, 0f), "Press start to stream from camera");
        }
    }

    private void StartCamera()
    {
        EnsurePrefabLoaded();

        if (_target.CameraIndex != -1)
        {
            if (!EditorIsPlaying)
            {
                _isRunning = true;
                var devices = WebCamTexture.devices;
                _webCamtexture = new WebCamTexture(devices[_target.CameraIndex].name);
                _webCamtexture.Play();
            }
            else
            {
                _renderTexture = _target.StartCamera();
            }
        }
    }

    private void StopCamera()
    {
        if (!EditorIsPlaying)
        {
            StopPreview();
        }
        else
        {
            _target.StopCamera();
        }
    }

    private void StopPreview()
    {
        _isRunning = false;
        if (_webCamtexture != null)
        {
            _webCamtexture.Stop();
            _webCamtexture = null;
            _renderTexture = null;
        }
    }
}
#endif