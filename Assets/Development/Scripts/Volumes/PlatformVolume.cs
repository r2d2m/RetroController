#if UNITY_EDITOR
using Sabresaurus.SabreCSG;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vnc.Development
{
    [System.Serializable]
    public class PlatformVolume : TaggedVolume
    {
        [SerializeField]
        public Vector3 startPoint, endPoint;

        [SerializeField]
        public float speed = 6f;

        [SerializeField]
        public float waitTime = 3f;


        public override bool OnInspectorGUI(Volume[] selectedVolumes)
        {
            var platformVolumes = selectedVolumes.Cast<PlatformVolume>();

            Vector3 previousStartPoint;
            startPoint = EditorGUILayout.Vector3Field("Start Point", previousStartPoint = startPoint);
            if (previousStartPoint != startPoint)
            {
                foreach (var platform in platformVolumes)
                    platform.startPoint = startPoint;
            }

            Vector3 previousEndPoint;
            endPoint = EditorGUILayout.Vector3Field("Start Point", previousEndPoint = endPoint);
            if (previousEndPoint != endPoint)
            {
                foreach (var platform in platformVolumes)
                    platform.endPoint = endPoint;
            }

            float previousSpeed;
            speed = EditorGUILayout.FloatField("Speed", previousSpeed = speed);
            if(previousSpeed != speed)
                foreach (var platform in platformVolumes)
                    platform.speed = speed;

            float previousWaitTime;
            waitTime = EditorGUILayout.FloatField("Wait Time", previousWaitTime = waitTime);
            if (previousWaitTime != waitTime)
                foreach (var platform in platformVolumes)
                    platform.waitTime = waitTime;

            return base.OnInspectorGUI(selectedVolumes);
        }

        public override void OnCreateVolume(GameObject volume)
        {
            PlatformVolumeComponent component = volume.AddComponent<PlatformVolumeComponent>();
            component.startPoint = startPoint;
            component.endPoint = endPoint;
            component.speed = speed;
            component.waitTime = waitTime;
            component.material = _material;
            component._tag = _tag;
            component.isTrigger = isTrigger;

            component._rigidbody = component.gameObject.AddComponent<Rigidbody>();
            component._rigidbody.useGravity = false;
            component._rigidbody.isKinematic = true;
            component._rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            component._rigidbody.position = component.currentPoint = startPoint;

            component.meshRenderer = component.gameObject.AddComponent<MeshRenderer>();
            component.meshCollider = component.gameObject.GetComponent<MeshCollider>();
            component.meshFilter = component.gameObject.AddComponent<MeshFilter>();
            component.meshFilter.sharedMesh = component.meshCollider.sharedMesh;
            component.gameObject.hideFlags = HideFlags.DontSave;
        }
    }
}
#endif