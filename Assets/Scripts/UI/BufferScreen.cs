using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BufferScreen : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(NotifyReady);
        Cursor.visible = true;
    }

    private void NotifyReady()
    {
        Cursor.visible = false;
        var playerNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        GameManager.Instance.AddPlayer(playerNum);
        Destroy(gameObject);
    }
}