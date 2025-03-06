using System;
using _Test.Script;
using Fusion;
using TMPro;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{

    [SerializeField] private CanvasGroup _canvas = null;
    [SerializeField] private NetworkRunner _runner = null;
    private MeshColorController _meshColorController = null;
    private NetworkObject _cachedLocalPlayer = null;

    [SerializeField] private PlayerSpawner _playerSpawner = null;
    private int _gameManagerInstanceId;


    [SerializeField] private TMP_InputField _inputFieldNickName = null;
    private SamplePlayerController _charController = null;
    public void SetLocalPlayer(NetworkObject networkObject)
    {
        _cachedLocalPlayer = networkObject;
        _charController = _cachedLocalPlayer.GetComponent<SamplePlayerController>();

    }
    public void ToggleCanvas(bool enable)
    {
        _canvas.interactable = enable;
        _canvas.alpha = enable ? 1 : 0;
    }

    private void Start()
    {
        ToggleCanvas(false);
    }
    
    public void OnRandomColorClick()
    {

        if (_charController == null)
        {
            Debug.LogWarning("MeshColorChanger component not found on local player");
            return;
        }
        _charController.UpdateColor();
    }

    public void OnRenameButtonClick()
    {
        if (_charController == null)
        {
            Debug.LogWarning("MeshColorChanger component not found on local player");
            return;
        }
        
        string name = _inputFieldNickName.text;
        _charController.UpdateName(name);
        _inputFieldNickName.DeactivateInputField(true);
    }

    public void LockMovement()
    {
        if (_charController == null)
        {
            Debug.LogWarning("MeshColorChanger component not found on local player");
            return;
        }

        _charController.LockMovement(true);
    }
    public void UnlockMovement()
    {
        if (_charController == null)
        {
            Debug.LogWarning("MeshColorChanger component not found on local player");
            return;
        }

        _charController.LockMovement(false);
    }

    
}
