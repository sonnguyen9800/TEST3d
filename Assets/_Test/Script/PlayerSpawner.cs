using Fusion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private UnityEvent _onPlayerSpawned;
    [SerializeField] private UnityEvent _onPlayerLeft;

    public GameObject PlayerPrefab;
    [SerializeField] private Transform _spawnPosTrans;
    public delegate void PlayerSpawnedEvent(NetworkObject player);

    public NetworkObject LocalCharacter => _localCharacter;

    private NetworkObject _localCharacter = null;

    public event PlayerSpawnedEvent OnLocalPlayerSpawned;

    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player Join");
        if (player != Runner.LocalPlayer) return;
        _localCharacter =Runner.Spawn(PlayerPrefab, _spawnPosTrans.position, Quaternion.identity);
        _onPlayerSpawned?.Invoke();
        OnLocalPlayerSpawned?.Invoke(_localCharacter);
        GameManager.Instance.SetLocalPlayer(_localCharacter);
        GameManager.Instance.ToggleCanvas(true);
    }
    
    public void PlayerLeft(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;
        _onPlayerLeft?.Invoke();
        _localCharacter = null;
        GameManager.Instance.ToggleCanvas(false);

    }
}