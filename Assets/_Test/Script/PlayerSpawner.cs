using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    [SerializeField] private Transform _spawnPosTrans;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(PlayerPrefab, _spawnPosTrans.position, Quaternion.identity);
        }
    }
}