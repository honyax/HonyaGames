using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    GameObject follower;

    void Update()
    {
        var followerPos = follower.transform.position;
        transform.position = new Vector3(followerPos.x, followerPos.y, transform.position.z);
    }
}
