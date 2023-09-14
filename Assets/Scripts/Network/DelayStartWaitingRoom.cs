using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class DelayStartWaitingRoom : MonoBehaviourPunCallbacks
{
    [SerializeField] private int minPlayersToStart = 1;

    [SerializeField] private TextMeshProUGUI roomCountDisplay;
    [SerializeField] private TextMeshProUGUI timerToStartDisplay;

    [SerializeField] private float maxWaitTime;
    [SerializeField] private float maxFullRoomWaitTime;

    private PhotonView _photonView;

    private int _playerCount;
    private int _roomSize;

    private bool _readyToCountDown;
    private bool _readyToStart;
    private bool _startingGame;

    private float _timerToStartGame;
    private float _notFullGameTimer;
    private float _fullGameTimer;

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        Init();
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
        Init();
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Init()
    {
        ResetTimer();
        PlayerCountUpdate();
    }

    private void PlayerCountUpdate()
    {
        _playerCount = PhotonNetwork.PlayerList.Length;
        _roomSize = PhotonNetwork.CurrentRoom.MaxPlayers;

        roomCountDisplay.text = $"Players: {_playerCount}/{_roomSize}";

        if (_playerCount == _roomSize)
        {
            _readyToStart = true;
        }
        else if (_playerCount >= minPlayersToStart)
        {
            _readyToCountDown = true;
        }
        else
        {
            _readyToCountDown = false;
            _readyToStart = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();
        if (PhotonNetwork.IsMasterClient) _photonView.RPC(nameof(RPC_SendTimer), RpcTarget.Others, _timerToStartGame);
    }

    [PunRPC]
    private void RPC_SendTimer(float timeIn)
    {
        _timerToStartGame = timeIn;
        _notFullGameTimer = timeIn;

        if (timeIn < _fullGameTimer) _fullGameTimer = timeIn;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    private void Update()
    {
        WaitingForPlayers();
    }

    private void WaitingForPlayers()
    {
        if (_playerCount <= 1) ResetTimer();

        if (_readyToStart)
        {
            _fullGameTimer -= Time.deltaTime;
            _timerToStartGame = _fullGameTimer;
        }
        else if (_readyToCountDown)
        {
            _notFullGameTimer -= Time.deltaTime;
            _timerToStartGame = _notFullGameTimer;
        }

        timerToStartDisplay.text = _readyToStart || _readyToCountDown 
            ? $"Starting in: {_timerToStartGame:00}" 
            : "Waiting...";

        if (_timerToStartGame > 0f || _startingGame) return;

        StartGame();
    }

    private void ResetTimer()
    {
        _timerToStartGame = maxWaitTime;
        _notFullGameTimer = maxWaitTime;
        _fullGameTimer = maxFullRoomWaitTime;
    }

    private void StartGame()
    {
        _startingGame = true;
        PhotonNetwork.LoadLevel(1);
    }

    public void DelayCancel()
    {
        PhotonNetwork.LeaveRoom();
    }
}