using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI display;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        GameManager.Instance.OnTracked += ShowWaiting;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance) GameManager.Instance.OnTracked -= ShowWaiting;
    }

    private void ShowWaiting(int id)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 == id) _canvasGroup.alpha = 1;
    }

    public void StartTimer(float time, Action callback)
    {
        StartCoroutine(TimerCo(time, callback));
    }

    private IEnumerator TimerCo(float time, Action callback)
    {
        do
        {
            display.text = $"{Mathf.Ceil(time)}";
            yield return new WaitForSeconds(1f);
            time -= 1;
        } while (time > 0);

        display.text = "Go!";
        callback?.Invoke();

        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}