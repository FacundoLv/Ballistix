using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviourPun
{
    [Header("Hovering values")] 
    [SerializeField] private float rayLenght;
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    [SerializeField] private LayerMask nonWalkable;

    [Header("Upright forces values")] 
    [SerializeField] private float uprightJointSpringStrength;
    [SerializeField] private float uprightJointSpringDamper;

    [Header("Movement values")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxAcceleration;
    [SerializeField] private AnimationCurve accelerationFactor;
    [SerializeField] private float maxAccelerationForce;
    [SerializeField] private AnimationCurve maxAccelerationForceFactor;

    [Header("Impulse data")]
    [SerializeField] private float impulseRadius;
    [SerializeField] private float impulseStrength;

    private Rigidbody _rb;
    private GameInputs _gameInputActions;

    private Vector3 _inputDirection;
    private Vector3 _goalVelocity;
    private Vector3 _otherVelocity;
    private Quaternion _initialRotation;

    private void Awake()
    {
        if (!photonView.IsMine) return;

        _rb = GetComponent<Rigidbody>();
        _gameInputActions = new GameInputs();
        _gameInputActions.Player.Impulse.started += HandleImpulse;
    }

    private void Start()
    {
        if (!photonView.IsMine) return;

        _initialRotation = _rb.rotation;
        _initialRotation.x = 0;
        _initialRotation.z = 0;

        _rb.constraints = (PhotonNetwork.LocalPlayer.ActorNumber - 1) % 2 == 0
            ? RigidbodyConstraints.FreezePositionZ
            : RigidbodyConstraints.FreezePositionX;

        GameManager.Instance.OnMatchStart += Activate;
        GameManager.Instance.OnLostMatch += Deactivate;
    }

    private void OnDestroy()
    {
        if (!GameManager.Instance) return; 
        
        GameManager.Instance.OnMatchStart -= Activate;
        GameManager.Instance.OnLostMatch -= Deactivate;
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        var dirInput = _gameInputActions.Player.Strafe.ReadValue<float>();
        _inputDirection = transform.TransformDirection(new Vector3(dirInput, 0, 0));
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine) return;

        Hover();
        ApplyUprightTorque();
        ApplyMovementForce();
    }

    private void HandleImpulse(InputAction.CallbackContext context)
    {
        photonView.RPC(nameof(CastImpulse), RpcTarget.All);
    }

    [PunRPC]
    private void CastImpulse()
    {
        var collisions = Physics.OverlapSphere(transform.position, impulseRadius);
        foreach (var collision in collisions)
        {
            if (!collision.TryGetComponent(out Ball ball)) continue;
            var direction = (ball.transform.position - transform.position).normalized;
            var ballRb = ball.GetComponent<Rigidbody>();
            ballRb.AddForce(direction * impulseStrength, ForceMode.Impulse);
        }
    }

    private void Activate()
    {
        _gameInputActions.Player.Enable();
    }

    private void Deactivate(int id)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber - 1 != id || !photonView.IsMine) return;
        _gameInputActions.Player.Disable();
    }

    private void Hover()
    {
        var rayDir = transform.TransformDirection(Vector3.down);

        // float upwards above ground
        if (Physics.Raycast(transform.position, rayDir, out var hit, rayLenght, ~nonWalkable))
        {
            var hitBody = hit.rigidbody;
            _otherVelocity = hitBody != null ? hitBody.velocity : Vector3.zero;

            var rayDirVel = Vector3.Dot(rayDir, _rb.velocity);
            var otherDirVel = Vector3.Dot(rayDir, _otherVelocity);

            var relVel = rayDirVel - otherDirVel;

            var x = hit.distance - rideHeight;

            var springForce = x * rideSpringStrength - relVel * rideSpringDamper;

            _rb.AddForce(Vector3.down * springForce);

            if (hitBody != null) hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);

            Debug.DrawRay(transform.position, rayDir * rayLenght, Color.yellow);
        }
    }

    private void ApplyUprightTorque()
    {
        // apply torque force to stay upright
        var deltaRotation = _initialRotation * Quaternion.Inverse(_rb.rotation);
        var torque = new Vector3(deltaRotation.x, deltaRotation.y, deltaRotation.z) * uprightJointSpringStrength -
                     _rb.angularVelocity * uprightJointSpringDamper;

        _rb.AddTorque(torque, ForceMode.Force);
    }

    private void ApplyMovementForce()
    {
        var goalVelocity = _inputDirection * maxSpeed; // speed factor
        var velocityDot = Vector3.Dot(_inputDirection, _goalVelocity.normalized);

        var acceleration = maxAcceleration * accelerationFactor.Evaluate(velocityDot);
        _goalVelocity = Vector3.MoveTowards(_goalVelocity,
            goalVelocity + _otherVelocity,
            acceleration * Time.fixedDeltaTime);

        var neededAcceleration = (_goalVelocity - _rb.velocity) / Time.fixedDeltaTime;
        var maxAccel = maxAccelerationForce * maxAccelerationForceFactor.Evaluate(velocityDot); // maxAcceleratioFactor
        neededAcceleration = Vector3.ClampMagnitude(neededAcceleration, maxAccel);

        _rb.AddForce(Vector3.Scale(neededAcceleration * _rb.mass, new Vector3(1, 0, 1)));
    }
}