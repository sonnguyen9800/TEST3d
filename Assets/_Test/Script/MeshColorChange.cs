using Fusion;
using UnityEngine.Serialization;

namespace _Test.Script
{
    using UnityEngine;

    public class MeshColorChanger : NetworkBehaviour
    {
        [Networked, OnChangedRender(nameof(ColorChanged))]
        public Color MeshColor { get; set; } // Exposed color variable

        [FormerlySerializedAs("meshRenderer")] [SerializeField]
        private MeshRenderer _meshRenderer;
        
        private Material objectMaterial;
        

        private void ColorChanged()
        {
            _meshRenderer.material.color = MeshColor;

        }
        
    }

}