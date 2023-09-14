using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunSingleton<GameManager>
{
    public event Action<int> OnTracked;
    public event Action OnMatchStart;
    public event Action<PlayerInfo> OnScoreUpdated;
    public event Action<int> OnLostMatch;
    public event Action<int> OnWinMatch;

    [SerializeField] private int startingPoints = 15;
    [SerializeField] private Launcher[] launchers;
    [SerializeField] private float launchCadence;
    [SerializeField] private AnimationCurve ballsOverTime;

    private bool CanLaunch => Time.time >= _nextLaunchTime && _inGameBallsCount < ballsOverTime.Evaluate(Time.time);

    private readonly Dictionary<int, int> _playerScores = new();
    private bool _matchStarted;
    private bool _matchEnded;
    private float _nextLaunchTime;
    private int _inGameBallsCount;

    protected override void Awake()
    {
        base.Awake();

        Gate.OnGoal += OnScore;
        OnTracked += CheckForMatchStart;
        OnLostMatch += CheckForMatchStart;
    }

    private void OnDestroy()
    {
        Gate.OnGoal -= OnScore;
        OnTracked -= CheckForMatchStart;
        OnLostMatch -= CheckForMatchStart;
    }

    private void Update()
    {
        TryLaunch();
    }

    public void AddPlayer(int playerNum)
    {
        if (_playerScores.ContainsKey(playerNum))
        {
            Debug.LogWarning($"Player {playerNum} already added!");
            return;
        }

        photonView.RPC(nameof(TrackPlayer), RpcTarget.AllBuffered, playerNum);
    }

    public void RemovePlayer(int playerNum)
    {
        photonView.RPC(nameof(UntrackPlayer), RpcTarget.All, playerNum);
    }

    private void TryLaunch()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!_matchStarted || !CanLaunch) return;

        var launcherIndex = Random.Range(0, launchers.Length);
        launchers[launcherIndex].Launch();
        _inGameBallsCount++;
        photonView.RPC(nameof(SyncLaunchData), RpcTarget.All, Time.time + launchCadence, _inGameBallsCount);
    }

    private void OnScore(ScoreInfo info)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!_playerScores.ContainsKey(info.ID)) return;

        photonView.RPC(nameof(UpdateScore), RpcTarget.All, info.ID, _playerScores[info.ID] - 1);
    }

    private void CheckForMatchStart(int id)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (_matchStarted || _playerScores.Count != PhotonNetwork.CurrentRoom.PlayerCount) return;

        photonView.RPC(nameof(StartTimer), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void SyncLaunchData(float nextLaunchTime, int currentBallCount)
    {
        _nextLaunchTime = nextLaunchTime;
        _inGameBallsCount = currentBallCount;
    }

    [PunRPC]
    private void TrackPlayer(int id)
    {
        _playerScores.Add(id, startingPoints);
        OnTracked?.Invoke(id);
    }

    [PunRPC]
    private void UntrackPlayer(int id)
    {
        if (_matchEnded) return;
        _playerScores.Remove(id);
        OnLostMatch?.Invoke(id);

        if (_playerScores.Count != 1) return;
        OnWinMatch?.Invoke(_playerScores.Keys.First());
        _matchEnded = true;
    }

    [PunRPC]
    private void UpdateScore(int id, int score)
    {
        _playerScores[id] = score;
        OnScoreUpdated?.Invoke(new PlayerInfo(id, score));
        _inGameBallsCount--;

        if (score > 0) return;
        RemovePlayer(id);
    }

    [PunRPC]
    private void StartTimer()
    {
        FindObjectOfType<CountdownTimer>()
            .StartTimer(3f, () =>
            {
                _matchStarted = true;
                OnMatchStart?.Invoke();
                foreach (var score in _playerScores)
                    OnScoreUpdated?.Invoke(new PlayerInfo(score.Key, score.Value));
            });
    }
}

public struct PlayerInfo
{
    public readonly int ID;
    public readonly int Score;

    public PlayerInfo(int id, int score)
    {
        ID = id;
        Score = score;
    }
}