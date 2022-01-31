using UnityEngine;
using TMPro;

public class Game : MonoBehaviour
{
    private const float Gravity = 4.0f;

    [SerializeField]
    private Block _blockPrefab;

    [SerializeField]
    private TextMeshPro _totalScoreText;

    [SerializeField]
    private TextMeshPro _gameOverText;

    int _counter = 0;
    Block _currentBlock;
    float _thrownTime = 0;
    int _totalScore = 0;

    bool _running = false;

    void Start()
    {
        Physics.gravity = -Vector3.up * Gravity;
        _running = true;
        _gameOverText.gameObject.SetActive(false);

        SpawnBlock();
    }

    void Update()
    {
        if (!_running)
            return;

        if (_thrownTime < float.Epsilon)
        {
            if (_currentBlock == null || _currentBlock.State == Block.BlockState.Thrown)
            {
                // block�������Ă���A���邢�͓�����ꂽ�ꍇ�͓��������Ԃ��L�^
                _thrownTime = Time.realtimeSinceStartup;
            }
        }
        else
        {
            if (_thrownTime < Time.realtimeSinceStartup - 1.5f)
            {
                // ���łɓ������Ă����莞�Ԍo�߂��Ă���ꍇ��block�𐶐�
                SpawnBlock();
            }
        }
    }

    void SpawnBlock()
    {
        _counter++;
        var point = Random.Range(1, 6);
        _currentBlock = Instantiate(_blockPrefab, Vector3.zero, Quaternion.identity);
        _currentBlock.name = "Block_" + point + "_" + _counter;
        _currentBlock.Initialize(this, _counter, point, -2f, 2f, -3.5f);
        _thrownTime = 0;
        var score = 1;
        for (var i = 0; i < point; i++)
        {
            score *= 2;
        }
        _totalScore += score;
        _totalScoreText.text = _totalScore.ToString();
    }

    public void GameOver()
    {
        _running = false;
        _gameOverText.gameObject.SetActive(true);
    }
}
