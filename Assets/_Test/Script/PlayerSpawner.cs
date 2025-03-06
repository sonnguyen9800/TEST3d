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
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player Join");
        if (player != Runner.LocalPlayer) return;
        Runner.Spawn(PlayerPrefab, _spawnPosTrans.position, Quaternion.identity);
        _onPlayerSpawned?.Invoke();
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player != Runner.LocalPlayer) return;
        _onPlayerLeft?.Invoke();
    }
}