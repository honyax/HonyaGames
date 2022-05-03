using UnityEngine;
using TMPro;

public class Block : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro[] _scoreTexts;

    Rigidbody _rb;
    int _point;

    private void Start()
    {
        _rb = this.GetComponent<Rigidbody>();
        _rb.isKinematic = true;
    }

    public void Initialize(int point)
    {
        _point = point;
        var score = 1;
        for (var i = 0; i < _point; i++)
        {
            score *= 2;
        }

        foreach (var scoreText in _scoreTexts)
        {
            scoreText.text = score.ToString();
            scoreText.fontSize = 36 - _point;
        }
    }

    public void Throw()
    {
        _rb.isKinematic = false;
        _rb.AddForce(Vector3.back * 750);
    }
}
