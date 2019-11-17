#if UNITY_EDITOR
using Sabresaurus.SabreCSG;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace vnc.Development
{
    [System.Serializable]
    public class TaggedVolume : Volume
    {
        [SerializeField]
        public Material _material;

        [SerializeField]
        public string _tag = "Untagged";

        [SerializeField]
        public bool isTrigger = false;

        public override Material BrushPreviewMaterial
        {
            get
            {
                if (_material)
                    return _material;

                return base.BrushPreviewMaterial;
            }
        }

        public override bool OnInspectorGUI(Volume[] selectedVolumes)
        {
            var taggedVolumes = selectedVolumes.Cast<TaggedVolume>();

            Material previousMaterial;
            _material = (Material)EditorGUILayout.ObjectField("Material", previousMaterial = _material, typeof(Material), allowSceneObjects: false);
            if (previousMaterial != _material)
            {
                foreach (var taggedVolume in taggedVolumes)
                    taggedVolume._material = _material;
            }

            string previousWaterTag;
            _tag = EditorGUILayout.TagField("Tag", previousWaterTag = _tag);
            if (previousWaterTag != _tag)
            {
                foreach (var taggedVolume in taggedVolumes)
                    taggedVolume._tag = _tag;
            }

            bool previousIsTrigger;
            isTrigger = EditorGUILayout.Toggle("Is Trigger?", previousIsTrigger = isTrigger);
            if (previousIsTrigger != isTrigger)
            {
                foreach (var taggedVolume in taggedVolumes)
                    taggedVolume.isTrigger = isTrigger;
            }

            return true;
        }

        public override void OnCreateVolume(GameObject volume)
        {
            TaggedVolumeComponent component = volume.AddComponent<TaggedVolumeComponent>();
            component.material = _material;
            component._tag = _tag;
            component.isTrigger = isTrigger;

            component.meshRenderer = component.gameObject.AddComponent<MeshRenderer>();
            component.meshCollider = component.gameObject.GetComponent<MeshCollider>();
            component.gameObject.hideFlags = HideFlags.DontSave;
        }
    }
}
#endif