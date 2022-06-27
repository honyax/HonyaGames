using UnityEngine;

public class Follower : MonoBehaviour
{
    private SpringJoint _springJoint;

    void Start()
    {
        _springJoint = GetComponent<SpringJoint>();
    }

    public void Initialize(Player player)
    {
        _springJoint.connectedBody = player.GetComponent<Rigidbody>();
        var playerPos = player.transform.position;
        var randCircle = Random.insideUnitCircle;
        var sphereCollider = GetComponent<SphereCollider>();
        transform.position = new Vector3(playerPos.x + randCircle.x, sphereCollider.radius, playerPos.z + randCircle.y);
        _springJoint.anchor = new Vector3(randCircle.x, 0, randCircle.y);
    }

    public void HitWall(Wall wall)
    {
        _springJoint.connectedBody = wall.GetComponent<Rigidbody>();
    }
}
