using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro[] _scoreTexts;

    public Rigidbody Rb { get; private set; }

    public int Id { get; private set; }
    public int Point { get; private set; }
    public bool Broken { get; private set; } = false;

    private void Start()
    {
        Rb = this.GetComponent<Rigidbody>();
        Rb.isKinematic = true;
    }

    public void Initialize(int id, int point)
    {
        Id = id;
        Point = point;
        UpdateScore();
    }

    public void Throw()
    {
        Rb.isKinematic = false;
        Rb.AddForce(Vector3.back * 750);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var opponent = collision.gameObject.GetComponent<Block>();
        if (opponent == null)
            return;

        if (opponent.Point == Point)
        {
            if (opponent.Id < Id)
            {
                // IDの古い方を残すため、自分のが新しければ破壊フラグをON
                Broken = true;
            }
            else
            {
                // 残す方は、衝突時に飛ばす
                var vec = (transform.position - collision.transform.position).normalized;
                Rb.AddForce(vec * opponent.Rb.velocity.magnitude * opponent.Rb.mass * 10);

                GrowUp();
            }
        }
    }

    public void GrowUp()
    {
        Point++;
        UpdateScore();
    }

    private void UpdateScore()
    {
        var score = 1;
        for (var i = 0; i < Point; i++)
        {
            score *= 2;
        }

        foreach (var scoreText in _scoreTexts)
        {
            scoreText.text = score.ToString();
            scoreText.fontSize = 36 - Point;
        }
    }
}
