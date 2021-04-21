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
    
    [SerializeField]
    private float _cameraDistanceFromHead;

    private WebCamTexture _webCamtexture;
    private Camera _camera;
    private RawImage _image;


    void Awake()
    {
        _camera = GetComponentInChildren<Camera>(true);
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
        _camera.targetTexture = new RenderTexture(_webCamtexture.width, _webCamtexture.height, 32);
        return _camera.targetTexture;
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
public class FixedCameraWindow : EditorWindow
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
    
    private RemotingSpectatorView _target;


    [MenuItem("FixedCamera/Show window")]
    private static void ShowWindow()
    {
        GetWindow<FixedCameraWindow>();
    }

    private void EnsurePrefabLoaded()
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

        List<string> deviceNames = new List<string>();
        deviceNames.Add("None | Off");

        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            deviceNames.Add(WebCamTexture.devices[i].name);
        }

        int selectedDevice = EditorGUILayout.Popup(_target.CameraIndex + 1, deviceNames.ToArray());

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
            if (GUILayout.Button("Stop" + (isPlaying ? "" : " preview")))
            {
                StopCamera();
            }

            if (EditorIsPlaying)
            {
                GUILayout.BeginHorizontal();
                float targetDistance = EditorGUILayout.FloatField("Distance from camera", _target.CameraDistanceFromHead);
                if (Math.Abs(targetDistance - _target.CameraDistanceFromHead) > 0.001f)
                {
                    _target.CameraDistanceFromHead = targetDistance;
                }


                if (GUILayout.Button("Reset Spectator Camera Position"))
                {
                    _target.SetPositionInFrontOfMainCamera();
                }
                
                GUILayout.EndHorizontal();
            }

            if (RenderTexture != null)
            {
                float aspectRatio = RenderTexture.height / (float) RenderTexture.width;
                EditorGUI.DrawPreviewTexture(new Rect(0, EditorIsPlaying ? 65 : 45, position.width, position.width * aspectRatio), RenderTexture, null, ScaleMode.StretchToFill);
            }
        }
        else
        {
            EditorGUI.BeginDisabledGroup(_target.CameraIndex == -1);
            if (GUILayout.Button("Start" + (isPlaying ? "" : " preview")))
            {
                StartCamera();
            }

            EditorGUI.EndDisabledGroup();
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