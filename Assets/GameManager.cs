using System;
using _Test.Script;
using Fusion;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoSingleton<GameManager>
{

    [SerializeField] private CanvasGroup _canvas = null;
    [SerializeField] private NetworkRunner _runner = null;
    private MeshColorChanger _meshColorChanger = null;
    private NetworkObject _cachedLocalPlayer = null;

    [SerializeField] private PlayerSpawner _playerSpawner = null;
    private int _gameManagerInstanceId;

    public void SetLocalPlayer(NetworkObject networkObject)
    {
        _cachedLocalPlayer = networkObject;
    }
    public void ToggleCanvas(bool enable)
    {
        _canvas.interactable = enable;
        _canvas.alpha = enable ? 1 : 0;
    }

    private void Start()
    {
        _gameManagerInstanceId = GetInstanceID();
        Debug.Log($"GameManager Awake: Instance ID {_gameManagerInstanceId}");
        _playerSpawner.OnLocalPlayerSpawned += OnLocalPlayerSpawned;

        ToggleCanvas(true);
    }

    private void OnLocalPlayerSpawned(NetworkObject player)
    {
        Debug.Log($"GameManager (ID: {_gameManagerInstanceId}) received player spawned event: {player != null}");

        Debug.Log($"GameManager received player spawned event: {player != null}");
        _cachedLocalPlayer = player;
    }
    

    public void OnRandomColorClick()
    {
        Debug.Log($"OnRandomColorClick called on GameManager (ID: {_gameManagerInstanceId})");

        if (_cachedLocalPlayer == null)
        {
            Debug.LogWarning("Local player object is null in OnRandomColorClick");
            return;
        }
        

        SamplePlayerController characterCtrl = _cachedLocalPlayer.GetComponent<SamplePlayerController>();
        
        if (characterCtrl == null)
        {
            Debug.LogWarning("MeshColorChanger component not found on local player");
            return;
        }
        characterCtrl.UpdateColor();
    }
    
}
