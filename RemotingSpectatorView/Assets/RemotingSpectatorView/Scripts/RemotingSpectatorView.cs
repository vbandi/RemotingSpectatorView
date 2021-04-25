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
    private bool IsRunning => EditorApplication.isPlaying ? _target.IsRunning : _isRunning;
    private Texture RenderTexture => _renderTexture ? _renderTexture : _webCamtexture;

    private bool EditorIsPlaying
    {
        get
        {
            if (_editorIsPlaying != EditorApplication.isPlaying)
            {
                _editorIsPlaying = EditorApplication.isPlaying;

                if (_editorIsPlaying && _isRunning)
                {
                    StopPreview();
                    StartCamera();
                }
            }

            return _editorIsPlaying;
        }
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


    [MenuItem("RemotingSpectatorView/Show window")]
    private static void ShowWindow()
    {
        GetWindow<RemotingSpectatorViewWindow>();
        EnsurePrefabLoaded();
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
        if (IsRunning)
        {
            Repaint();
        }
    }

    private void OnGUI()
    {
        EnsurePrefabLoaded();

        var isPlaying = EditorIsPlaying;

        if (_deviceNames == null)
        {
            LoadDevices();
        }

        int selectedDevice = EditorGUI.Popup(new Rect(0, 0, position.width, 15), _target.CameraIndex + 1, _deviceNames);

        if (selectedDevice - 1 != _target.CameraIndex)
        {
            bool isRunning = IsRunning;
            StopCamera();
            _target.CameraIndex = selectedDevice - 1;
            if (isRunning)
            {
                StartCamera();
            }
        }

        if (IsRunning)
        {
            if (GUI.Button(new Rect(0, 20, position.width, 15), "Stop" + (isPlaying ? "" : " preview")))
            {
                StopCamera();
            }

            if (EditorIsPlaying)
            {
                float targetDistance = EditorGUI.FloatField(new Rect(0, 40, position.width / 2f, 15), "Distance from camera", _target.CameraDistanceFromHead);
                if (Math.Abs(targetDistance - _target.CameraDistanceFromHead) > 0.001f)
                {
                    _target.CameraDistanceFromHead = targetDistance;
                }

                float fov = EditorGUI.FloatField(new Rect(position.width / 2f, 40, position.width / 2f, 15), "Camera FOV", _target.Camera.fieldOfView);
                if (Math.Abs(fov - _target.Camera.fieldOfView) > 0.001f)
                {
                    _target.Camera.fieldOfView = fov;
                }

                if (GUI.Button(new Rect(0, 60, position.width, 15), "Reset Spectator Camera Position"))
                {
                    _target.SetPositionInFrontOfMainCamera();
                }
            }

            if (RenderTexture != null)
            {
                float aspectRatio = RenderTexture.height / (float) RenderTexture.width;
                EditorGUI.DrawPreviewTexture(new Rect(0, EditorIsPlaying ? 80 : 40, position.width, position.width * aspectRatio), RenderTexture, null, ScaleMode.StretchToFill);
                // GUILayout.Label(RenderTexture);
            }
        }
        else
        {
            if (GUI.Button(new Rect(0, 20, position.width, 15), "Start" + (isPlaying ? "" : " preview")))
            {
                StartCamera();
            }
        }
        
        GUI.Label(new Rect(0, 20, position.width, 50), "Press start to stream from camera");
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