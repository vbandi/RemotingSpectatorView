using System.Timers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class RemotingSpectatorView : MonoBehaviour
{
    private WebCamTexture _webCamtexture;
    public RawImage Image;
    public int CameraIndex = 0;
    public float CameraDistanceFromHead = 0.6f;
    
    void Awake()
    {
        SetPositionInFrontOfMainCamera();
    }

    [ContextMenu("Set Position In Front of Main Camera")]
    private void SetPositionInFrontOfMainCamera()
    {
        var mainCamera = Camera.main;
        var mainCameraTransform = mainCamera.transform;
        transform.position = mainCameraTransform.position + (mainCameraTransform.forward.normalized * CameraDistanceFromHead);
        transform.LookAt(mainCameraTransform);
    }


    private void OnEnable()
    {
        var devices = WebCamTexture.devices; 
        _webCamtexture = new WebCamTexture(devices[CameraIndex].name);
        Image.texture = _webCamtexture;
        _webCamtexture.Play();
    }

    private void OnDisable()
    {
        _webCamtexture.Stop();
    }
    
}


#if UNITY_EDITOR

public class FixedCameraWindow : EditorWindow
{
    private Texture _texture;
    private Timer _timer;

    [MenuItem("FixedCamera/Show window")]
    private static void ShowWindow()
    {
        GetWindow<FixedCameraWindow>();
        EnsurePrefabLoaded();
    }
    
    private static void EnsurePrefabLoaded()
    {
        if (Application.isPlaying && FindObjectOfType<RemotingSpectatorView>())
        {
            GameObject go = (GameObject) Resources.Load("SpectatorCameraRoot");
            Instantiate(go);
        }
    }

    private void Update()
    {
        Repaint();
        // if (_timer == null)
        // {
        //     _timer = new Timer(1000 / 10f);
        //     _timer.Elapsed += (sender, args) => Repaint();
        //     _timer.AutoReset = true;
        // }
        //
        // if (EditorApplication.isPlaying && !_timer.Enabled)
        // {
        //     Debug.Log("Timer start");
        //     _timer.Start();
        // }
        //
        // if (!EditorApplication.isPlaying && _timer.Enabled)
        // {
        //     Debug.Log("Timer stop");
        //     _timer.Stop();
        // }
    }

    private void OnGUI()
    {
        _texture = (Texture) EditorGUILayout.ObjectField(_texture, typeof(Texture));
        GUILayout.Label(_texture);
        // GUILayout.Label(AssetPreview.GetAssetPreview(_texture));
    }
}

public class FixedCameraWindow2 : EditorWindow
{

    private Texture _texture;

    private Timer _timer;

    private bool _isRunning = false;
    
    private WebCamTexture _webCamtexture;
    public RawImage Image;

    public int CameraIndex = 0;

    public float CameraDistanceFromHead = 0.6f;

    [MenuItem("FixedCamera/Show window2")]
    private static void ShowWindow()
    {
        GetWindow<FixedCameraWindow2>();
    }

    private void Update()
    {
        if (_isRunning)
            Repaint();
    }

    private void OnGUI()
    {
        CameraIndex = EditorGUILayout.IntField(CameraIndex);

        if (_isRunning)
        {
            if (GUILayout.Button("Stop"))
                Stop();

            GUILayout.Label(_webCamtexture);
        }
        else
        {
            if (GUILayout.Button("Start"))
                Start();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Start()
    {
        _isRunning = true;
        var devices = WebCamTexture.devices; 
        _webCamtexture = new WebCamTexture(devices[CameraIndex].name);
        // Image.texture = _webCamtexture;
        _webCamtexture.Play();
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private void Stop()
    {
        _isRunning = false;
        if (_webCamtexture != null)
            _webCamtexture.Stop();
    }
}
#endif
