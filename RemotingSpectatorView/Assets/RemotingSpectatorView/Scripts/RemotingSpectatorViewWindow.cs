using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Displays an editor window for the remoting spectator view 
/// </summary>
public class RemotingSpectatorViewWindow : EditorWindow
{
    private int _userManualHeight = 200;
    
    private string _userManual = @"How to use

- Start and connect Holographic Remoting
- Start your app in the editor.
- Select your webcam from the dropdown list (DirectShow cams are not supported)
- Press ""Play"" above
- Make sure your webcam is horizontal
- Position your head right in front of the webcam, at the distance specified by ""Distance from head""
- Press ""Reset Spectator Camera Position""
- Hold up your hand so that the Hololens can see it
- Set camera FOV so that the detected and actual hands are of the same size
- Enjoy!
";        
    
    private bool IsCameraRunning => EditorApplication.isPlaying ? _target.IsRunning : _isRunning;
    private Texture RenderTexture => _renderTexture ? _renderTexture : _webCamTexture;

    private float _margin = 10f;

    private bool EditorIsPlaying
    {
        get
        {
            if (_editorIsPlaying != EditorApplication.isPlaying)
            {
                _editorIsPlaying = EditorApplication.isPlaying;

                SetPlayerPrefsToTarget();

                if (_editorIsPlaying && _isRunning)
                {
                    StopPreview();
                    StartCamera();
                }
            }

            return _editorIsPlaying;
        }
    }

    private enum RectPosition
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

    private static void SetPlayerPrefsToTarget()
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
    private WebCamTexture _webCamTexture;

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
        SetPlayerPrefsToTarget();
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

        EditorGUI.BeginDisabledGroup(!isPlaying);

        float fov = EditorGUI.FloatField(GetNextRect(rowNumber, 0, RectPosition.Start, 1), "Camera FOV", _target.Camera.fieldOfView);

        if (Math.Abs(fov - _target.Camera.fieldOfView) > 0.001f)
        {
            EditorPrefs.SetFloat("FieldOfView", fov);
            _target.Camera.fieldOfView = fov;
        }

        rowNumber++;
        
        Rect distanceRect;
        Rect resetButtonRect;

        if (position.width > 400)
        {
            distanceRect = GetNextRect(rowNumber, 0f, RectPosition.Start, .5f);
            resetButtonRect = GetNextRect(rowNumber, .5f, RectPosition.End, .5f);
            rowNumber++;
        }
        else
        {
            distanceRect = GetNextRect(rowNumber, 0f, RectPosition.Full, 0f);
            rowNumber++;
            resetButtonRect = GetNextRect(rowNumber, 0f, RectPosition.Full, 0f);
            rowNumber++;
        }

        float targetDistance = EditorGUI.FloatField(distanceRect, "Distance from head (m)", _target.CameraDistanceFromHead);

        if (Math.Abs(targetDistance - _target.CameraDistanceFromHead) > 0.001f)
        {
            EditorPrefs.SetFloat("DistanceFromHead", targetDistance);
            _target.CameraDistanceFromHead = targetDistance;
        }

        if (GUI.Button(resetButtonRect, "Reset Spectator Camera Position"))
        {
            _target.SetPositionInFrontOfMainCamera();
        }

        rowNumber++;
        EditorGUI.EndDisabledGroup();
        
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
            GUI.Label(GetNextRect(rowNumber, 0f, RectPosition.Full, 0f, _userManualHeight), _userManual);
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
                _webCamTexture = new WebCamTexture(devices[_target.CameraIndex].name);
                _webCamTexture.Play();
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
        if (_webCamTexture != null)
        {
            _webCamTexture.Stop();
            _webCamTexture = null;
            _renderTexture = null;
        }
    }
}