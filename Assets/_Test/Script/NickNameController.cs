using Fusion;
using TMPro;
using UnityEngine;

public class NickNameController : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmpName = null;

    [Networked, OnChangedRender(nameof(ColorChanged))]
    public string Name { get; set; }

    private void Start()
    {
        _tmpName.text = Name;
    }

    private void ColorChanged()
    {
        _tmpName.text = Name;

    }
}
