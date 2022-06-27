using UnityEngine;
using UnityEngine.InputSystem;

public class Game : SingletonMonoBehaviour<Game>
{
    [SerializeField]
    private Field _field;

    [SerializeField]
    private Player _player;

    [SerializeField]
    private Follower[] _followers;

    [SerializeField]
    private float _fieldSpeed = 40.0f;

    private Transform _fieldTransform;

    public GameState State { get; private set; } = GameState.None;

    public enum GameState
    {
        None,
        Initialize,
        Playing,
        GameClear,
        GameOver,
    }

    private void Start()
    {
        _fieldTransform = _field.transform;

        State = GameState.Initialize;
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.Initialize:
                _fieldTransform.position = new Vector3(0, 0, 500);
                _fieldTransform.rotation = Quaternion.identity;
                _field.Initialize();
                _player.Initialize();
                foreach (var follower in _followers)
                {
                    follower.Initialize(_player);
                }
                State = GameState.Playing;
                break;

            case GameState.Playing:
                UpdatePlaying();
                _player.UpdatePlaying();
                break;

            case GameState.GameClear:
                UpdateGameFinished();
                break;

            case GameState.GameOver:
                UpdateGameFinished();
                break;
        }
    }

    private void UpdatePlaying()
    {
        var deltaZ = _fieldSpeed * Time.deltaTime;
        var pos = _fieldTransform.position;
        pos.z = pos.z - deltaZ;
        if (pos.z <= -500)
        {
            pos.z = -500;
            State = GameState.GameClear;
        }
        _fieldTransform.position = pos;
    }

    private void UpdateGameFinished()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (kb.enterKey.wasPressedThisFrame || mouse.leftButton.wasPressedThisFrame)
        {
            State = GameState.Initialize;
        }
    }

    public void GameOver()
    {
        State = GameState.GameOver;
    }
}
