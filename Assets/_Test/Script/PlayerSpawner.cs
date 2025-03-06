using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject PlayerPrefab;
    [SerializeField] private Transform _spawnPosTrans;
    public void PlayerJoined(PlayerRef player)
    {
        Debug.Log("Player Join");
        if (player == Runner.LocalPlayer)
        {
            var networkObject = Runner.Spawn(PlayerPrefab, _spawnPosTrans.position, Quaternion.identity);
            //networkObject.gameObject.transform.SetParent( _spawnPosTrans.parent.transform);
        }
    }
}