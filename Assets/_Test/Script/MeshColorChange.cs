using UnityEngine.Serialization;

namespace _Test.Script
{
    using UnityEngine;

    public class MeshColorChanger : MonoBehaviour
    {
        public Color objectColor = Color.white; // Exposed color variable

        [FormerlySerializedAs("meshRenderer")] [SerializeField]
        private MeshRenderer _meshRenderer;
        
        private Material objectMaterial;

        void Start()
        {

            // Ensure we don't modify the shared material
            objectMaterial = _meshRenderer.material;
            objectMaterial.color = objectColor;
        }

        void Update()
        {
            // Apply color change in real-time
            objectMaterial.color = objectColor;
        }
    }

}