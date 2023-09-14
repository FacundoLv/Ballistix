using UnityEngine;

public class PlayParticleState : StateMachineBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private ParticleSystem particleSystem;

    private float _elapsed;
    private bool _triggered;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _elapsed = 0f;
        _triggered = false;
    }
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_triggered) return;
    
        _elapsed += Time.deltaTime;
        if (_elapsed < delay) return;

        var transform = animator.transform;
        Instantiate(particleSystem, transform.position, Quaternion.identity, transform);
        _triggered = true;
    }
}