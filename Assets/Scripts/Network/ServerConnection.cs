using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ServerConnection : MonoBehaviourPunCallbacks
{
    [SerializeField] private PanelGroup displayPanel;
    [SerializeField] private int playWindowIndex;
    [SerializeField] private int waitingWindowIndex;

    private string _createdRoomName;
    private string _joinedRoomName;

    private void Start()
    {
        PhotonNetwork.SendRate = 50;
        PhotonNetwork.SerializationRate = 25;
        if (!PhotonNetwork.IsConnected) ConnectToServer();
    }

    private static void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void SetNewRoomName(string roomName)
    {
        _createdRoomName = roomName;
    }

    public void SetJoinedRoomName(string roomName)
    {
        _joinedRoomName = roomName;
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_createdRoomName)) return;

        var options = new RoomOptions
        {
            MaxPlayers = 4,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.JoinOrCreateRoom(_createdRoomName, options, TypedLobby.Default);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(_joinedRoomName)) return;

        PhotonNetwork.JoinRoom(_joinedRoomName);
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public override void OnJoinedRoom()
    {
        displayPanel.SetPageIndex(waitingWindowIndex);
    }

    public override void OnLeftRoom()
    {
        displayPanel.SetPageIndex(playWindowIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}