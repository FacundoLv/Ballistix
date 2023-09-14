using Photon.Pun;
using UnityEngine;

public class Ball : MonoBehaviourPun
{
    [SerializeField] private float maxVelocity;
    [SerializeField] private float springStrength;
    [SerializeField] private float springDamper;
    [SerializeField] private LayerMask planeLayer;

    [Header("Feedback")]
    [SerializeField] private ParticleSystem eolParticle;

    private Rigidbody _rb;
    private Plane _plane;
    private bool _hasPlane;
    private readonly float _bounceForceFactor = 2f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        Gate.OnGoal += OnGoal;
        GameManager.Instance.OnWinMatch += OnWinMatch;
    }

    private void OnDestroy()
    {
        Gate.OnGoal -= OnGoal;
        if (GameManager.Instance) GameManager.Instance.OnWinMatch -= OnWinMatch;
    }

    private void OnGoal(ScoreInfo info)
    {
        if (info.Ball != this) return;
        RemoveBall();
    }

    private void OnWinMatch(int id)
    {
        RemoveBall();
    }

    private void RemoveBall()
    {
        Instantiate(eolParticle, transform.position, transform.rotation);
        if (photonView.IsMine) PhotonNetwork.Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine || !_hasPlane) return;

        var position = transform.position;

        var projection = _plane.ClosestPointOnPlane(position);

        var delta = projection - position;

        var relVel = Vector3.Dot(delta.normalized, _rb.velocity);

        var springForce = delta.magnitude * springStrength - relVel * springDamper;

        _rb.AddForce(delta.normalized * springForce);

        _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (planeLayer != (planeLayer | (1 << other.gameObject.layer))) return;

        var tr = other.transform;
        _plane = new Plane(tr.up, tr.position);
        _hasPlane = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!photonView.IsMine) return;
        if (other.gameObject.TryGetComponent(out PlayerController player))
        {
            var bounceForce = _rb.velocity * _bounceForceFactor;
            var direction = other.contacts[0].normal;
            _rb.AddForce(direction * bounceForce.magnitude, ForceMode.Impulse);
        }
    }
}