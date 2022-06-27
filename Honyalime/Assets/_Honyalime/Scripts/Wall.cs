using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                var player = other.GetComponent<Player>();
                player.HitWall(this);
                break;
            case "Follower":
                var follower = other.GetComponent<Follower>();
                follower.HitWall(this);
                break;
            default:
                return;
        }
    }
}
