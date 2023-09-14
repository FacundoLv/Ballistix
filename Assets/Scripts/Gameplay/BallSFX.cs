using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BallSFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var randomSfx = Random.Range(0, clips.Length);
        _source.PlayOneShot(clips[randomSfx]);
    }
}