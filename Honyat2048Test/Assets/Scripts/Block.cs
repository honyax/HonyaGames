using UnityEngine;

public class Block : MonoBehaviour
{
    Rigidbody _rb;

    private void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    public void Throw()
    {
        _rb.isKinematic = false;
        _rb.AddForce(Vector3.back * 750);
    }
}
