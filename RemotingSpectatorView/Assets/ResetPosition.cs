using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    public KeyCode KeyCode = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode))
        {
            transform.localPosition = _initialPosition;
            transform.localRotation = _initialRotation;
        }
            
    }

    private void Awake()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }
}
