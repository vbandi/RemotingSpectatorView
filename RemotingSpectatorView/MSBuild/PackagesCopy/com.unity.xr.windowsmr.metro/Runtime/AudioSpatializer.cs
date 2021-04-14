using UnityEngine;
using System.Collections;

namespace UnityEngine.Audio
{
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Audio/Audio Spatializer/Audio Spatializer Microsoft (Deprecated)", 30)]
    [System.Obsolete("Support for built-in VR will be removed in Unity 2020.1. Please update to the new Unity XR Plugin System. More information about the new XR Plugin System can be found at https://docs.google.com/document/d/1AMk4NwRVAtnG-LScXT2ne_s5mD4rmWK_C9dyn39ZDbc/edit.", false)]
    public sealed class AudioSpatializerMicrosoft : MonoBehaviour
    {
        public enum RoomSize
        {
            Small,
            Medium,
            Large,
            Outdoors
        }

        // the RoomSize enum and values are forwarded to that source's IXAPOHrtfParameters via SetEnvironment
        // (see https://msdn.microsoft.com/en-us/library/windows/desktop/mt186609(v=vs.85).aspx and https://msdn.microsoft.com/en-us/library/windows/desktop/mt186604(v=vs.85).aspx)

        public RoomSize roomSize
        {
            get
            {
                return m_RoomSize;
            }
            set
            {
                m_RoomSize = value;
                SetSpatializerRoomSize();
            }
        }

        void Awake()
        {
            SetSpatializerFloats();
        }

        void OnValidate()
        {
            SetSpatializerFloats();
        }

        void OnDidAnimateProperty()
        {
            SetSpatializerFloats();
        }

        private void SetSpatializerFloats()
        {
            SetSpatializerRoomSize();
        }

        private AudioSource audioSource
        {
            get { return GetComponent<AudioSource>(); }
        }

        private void SetSpatializerRoomSize()
        {
            audioSource.SetSpatializerFloat(0, (float)m_RoomSize);
        }

        [SerializeField]
        private RoomSize m_RoomSize = RoomSize.Small;
    }
}
