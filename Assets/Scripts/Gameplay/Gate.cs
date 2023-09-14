using System;
using UnityEngine;
using UnityEngine.Events;

public class Gate : MonoBehaviour
{
    public static event Action<ScoreInfo> OnGoal;

    public UnityEvent OnLocked;

    [SerializeField] private int gateID;

    private void Start()
    {
        GameManager.Instance.OnLostMatch += HandleLock;
        GameManager.Instance.OnWinMatch += TurnOff;
    }

    private void OnDestroy()
    {
        if (!GameManager.Instance) return;

        GameManager.Instance.OnLostMatch -= HandleLock;
        GameManager.Instance.OnWinMatch -= TurnOff;
    }

    private void HandleLock(int id)
    {
        if (id != gateID) return;

        OnLocked?.Invoke();
    }

    private void TurnOff(int id)
    {
        if (id != gateID) return;

        gameObject.SetActive(false);
    }

    [ContextMenu("Lock")]
    private void Lock()
    {
        OnLocked?.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            OnGoal?.Invoke(new ScoreInfo(gateID, ball));
        }
    }
}

public struct ScoreInfo
{
    public readonly int ID;
    public readonly Ball Ball;

    public ScoreInfo(int id, Ball ball)
    {
        ID = id;
        Ball = ball;
    }
}