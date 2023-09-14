using System.Collections;
using Photon.Pun;
using UnityEngine;

public class DeathFeedback : MonoBehaviourPun
{
    [SerializeField] private ParticleSystem deathParticle;
    [SerializeField] private AnimationCurve scaleCurve;

    private void Awake()
    {
        GameManager.Instance.OnLostMatch += OnLoose;
    }

    private void OnDestroy()
    {
        if (!GameManager.Instance) return;

        GameManager.Instance.OnLostMatch -= OnLoose;
    }

    private void OnLoose(int id)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 != id || !photonView.IsMine) return;
        photonView.RPC(nameof(Die), RpcTarget.All);
    }

    [PunRPC]
    private void Die()
    {
        StartCoroutine(DeathCo());
    }

    private IEnumerator DeathCo()
    {
        if (scaleCurve.length != 0)
        {
            var duration = scaleCurve[scaleCurve.length - 1].time;
            var elapsed = 0f;
            do
            {
                transform.localScale = Vector3.one * scaleCurve.Evaluate(elapsed);
                elapsed += Time.deltaTime;
                yield return null;
            } while (elapsed < duration);
        }

        Instantiate(deathParticle, transform.position, Quaternion.identity);
        if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
    }
}