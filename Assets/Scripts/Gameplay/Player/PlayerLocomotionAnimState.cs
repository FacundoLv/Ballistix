using Photon.Pun;
using UnityEngine;

public class PlayerLocomotionAnimState : StateMachineBehaviour
{
    private float _currentRoll;
    private GameInputs _inputs;

    private const float TargetFrameRate = 60f;
    private static readonly int Roll = Animator.StringToHash("Roll");
    private static readonly int Impulse = Animator.StringToHash("Impulse");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var photonView = animator.GetComponentInParent<PhotonView>();
        if (!photonView || !photonView.IsMine) Destroy(this);

        _inputs = new GameInputs();
        _inputs.Player.Enable();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var newValue = _inputs.Player.Strafe.ReadValue<float>();
        var frameRateIndependentSmoothing = 1f - Mathf.Pow(.85f, Time.deltaTime * TargetFrameRate);

        _currentRoll = Mathf.Lerp(_currentRoll, newValue, frameRateIndependentSmoothing);
        animator.SetFloat(Roll, _currentRoll);
        
        if (_inputs.Player.Impulse.triggered) animator.SetBool(Impulse, true);
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Impulse, false);
    }
}