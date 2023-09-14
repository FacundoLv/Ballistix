using System.IO;
using Photon.Pun;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] playerPrefabs;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraPosition;
    [SerializeField] private Vector3 cameraRotation;

    private void Start()
    {
        var playerNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        var prefab = playerPrefabs[playerNum].name;
        var spawnPoint = transform.GetChild(playerNum).transform;

        var position = spawnPoint.position;
        var rotation = spawnPoint.rotation;

        var cameraOffset =
            spawnPoint.right * cameraPosition.x +
            spawnPoint.up * cameraPosition.y +
            spawnPoint.forward * cameraPosition.z;

        cameraTransform.position = position + cameraOffset;
        cameraTransform.localRotation = rotation * Quaternion.Euler(cameraRotation);

        PhotonNetwork.Instantiate(Path.Combine("Ballistix", prefab), position, rotation);
        Destroy(gameObject);
    }
}
