using UnityEngine;

public class Honyagdoll : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _mainRigidbody;

    public Rigidbody MainRigidbody { get { return _mainRigidbody; } }
}
