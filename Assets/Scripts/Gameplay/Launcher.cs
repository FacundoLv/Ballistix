using System.IO;
using Photon.Pun;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] private Ball ball;
    [SerializeField] private float randomness = 15f;

    public void Launch()
    {
        var randomRot = Quaternion.Euler(0, Random.Range(-randomness, randomness), 0);
        var newBall = PhotonNetwork.Instantiate(Path.Combine("Ballistix", ball.name),
            transform.position, 
            transform.rotation);
        var direction = randomRot * newBall.transform.forward;
        newBall.GetComponent<Rigidbody>().AddForce(direction * 2f, ForceMode.Impulse);
    }
}