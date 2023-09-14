using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MessageRelay : MonoBehaviour
{
    private const float FadeDuration = .35f;

    private TextMeshProUGUI _message;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _message = GetComponent<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();

        GameManager.Instance.OnWinMatch += OnWin;
        GameManager.Instance.OnLostMatch += OnLoose;
    }

    private void OnDestroy()
    {
        if (!GameManager.Instance) return;
        
        GameManager.Instance.OnWinMatch -= OnWin;
        GameManager.Instance.OnLostMatch -= OnLoose;
    }

    private void OnWin(int id)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 != id) return;
        RelayMessage("You win!");
    }

    private void OnLoose(int id)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 != id) return;
        RelayMessage("You loose!");
    }

    private void RelayMessage(string message)
    {
        StartCoroutine(MessageCo(message));
    }

    private IEnumerator MessageCo(string message)
    {
        _canvasGroup.alpha = 1f;
        _message.text = message;
        yield return new WaitForSeconds(1f);

        var elapsed = 0f;
        do
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / FadeDuration);
            yield return null;
        } while (elapsed < FadeDuration);

        _canvasGroup.alpha = 0;
    }
}