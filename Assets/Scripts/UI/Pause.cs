using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button leaveButton;

    private CanvasGroup _canvasGroup;
    private GameInputs _gameInputs;

    private bool _isPaused;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _gameInputs = new GameInputs();
        _gameInputs.UI.Pause.performed += ShowPause;

        leaveButton.onClick.AddListener(LeaveRoom);
    }

    private void Start()
    {
        GameManager.Instance.OnTracked += Activate;
    }
    
    private void Activate(int id)
    {
        _gameInputs.UI.Enable();
    }

    private void OnDestroy()
    {
        _gameInputs.UI.Disable();
        if (GameManager.Instance) GameManager.Instance.OnTracked += Activate;
    }

    private void ShowPause(InputAction.CallbackContext context)
    {
        _isPaused = !_isPaused;
        Cursor.visible = _isPaused;
        _canvasGroup.alpha = _isPaused ? 1 : 0;
        _canvasGroup.interactable = _isPaused;
        _canvasGroup.blocksRaycasts = _isPaused;
    }

    private void LeaveRoom()
    {
        Cursor.visible = true;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadSceneAsync(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        GameManager.Instance.RemovePlayer(otherPlayer.ActorNumber - 1);
    }
}