using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using ShatterToolkit;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Transform _lightSaber;

    [SerializeField]
    private CinemachineVirtualCamera _camera;

    [SerializeField]
    private StarterAssets.StarterAssetsInputs _playerInput;

    [SerializeField]
    private AudioSource _seAudioSource;

    [SerializeField]
    private AudioClip _slashSe;

    [SerializeField]
    private AudioClip _spawnSe;

    [SerializeField]
    private AudioClip _explosionSe;

    [SerializeField]
    private ParticleSystem _explosionEffect;

    [SerializeField]
    private float _readyCameraDistance = 1f;

    [SerializeField]
    private Vector3 _readyShoulderOffset = new Vector3(2, 0.2f, 0);

    [SerializeField]
    private Transform _blockParent;

    [SerializeField]
    private GameObject[] _blockPrefabs;

    readonly Quaternion _initialLightSaberRotation = Quaternion.Euler(new Vector3(-75, 0, 0));
    readonly float _lightSaberLength = 1.5f;
    private Cinemachine3rdPersonFollow _followCamera;
    private bool _lookAtMouse = false;

    private RaycastHit[] _results = new RaycastHit[8];
    private ShatterTool _cuttingBlock;
    private Vector3 _cutStartPos;

    private void Start()
    {
        _lightSaber.localPosition = new Vector3(0, 1.2f, 0.3f);
        _lightSaber.localScale = Vector3.one * 0.01f;
        _followCamera = _camera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    private void Update()
    {
        var kbd = Keyboard.current;
        if (kbd.zKey.wasPressedThisFrame)
        {
            _seAudioSource.PlayOneShot(_spawnSe);
            var blockIndex = Random.Range(0, _blockPrefabs.Length);
            var block = Instantiate(_blockPrefabs[blockIndex], _blockParent);
            var playerTransform = _playerInput.transform;
            var pos = playerTransform.position + playerTransform.forward * 5 + Vector3.up * 15;
            block.transform.position = pos;
        } else if (kbd.xKey.wasPressedThisFrame)
        {
            var playerTransform = _playerInput.transform;

            var ps = Instantiate(_explosionEffect, playerTransform);
            var t = ps.transform;
            t.localPosition = new Vector3(0, 1, 0);
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one * 2;

            _seAudioSource.PlayOneShot(_explosionSe);
            foreach (var rb in _blockParent.GetComponentsInChildren<Rigidbody>())
            {
                rb.AddExplosionForce(rb.mass * 10, playerTransform.position, 100, 5, ForceMode.Impulse);
            }
        }

        var mouse = Mouse.current;
        _lookAtMouse = mouse.rightButton.isPressed;
        _playerInput.cursorInputForLook = !_lookAtMouse;
        if (_lookAtMouse)
        {
            _followCamera.CameraDistance = _readyCameraDistance;
            _followCamera.ShoulderOffset = _readyShoulderOffset;
        }
        else
        {
            _followCamera.CameraDistance = 4;
            _followCamera.ShoulderOffset = new Vector3(1, 0, 0);
            _lightSaber.localRotation = _initialLightSaberRotation;
            return;
        }

        var mousePos = mouse.position.ReadValue();
        var ray = Camera.main.ScreenPointToRay(mousePos.ToVec3());
        _lightSaber.LookAt(ray.origin + ray.direction * 5);
        //Debug.DrawRay(ray.origin, ray.direction * 5, Color.red, 0.3f);

        var num = Physics.RaycastNonAlloc(_lightSaber.position, _lightSaber.forward, _results, _lightSaberLength);
        if (num > 0 && _cuttingBlock == null)
        {
            _cuttingBlock = _results[0].collider.GetComponent<ShatterTool>();
            _cutStartPos = _results[0].point;
            if (_cuttingBlock)
            {
                _seAudioSource.PlayOneShot(_slashSe);
            }
        }
        else if (num == 0 && _cuttingBlock != null)
        {
            var plane = new Plane(_cutStartPos, _lightSaber.position, _lightSaber.position + _lightSaber.forward);
            _cuttingBlock.Split(new Plane[] { plane });
            var rb = _cuttingBlock.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("AddForce");
                rb.AddForce((_lightSaber.position - _cutStartPos) * 100000);
            }
            _cuttingBlock = null;

            Debug.DrawLine(_cutStartPos, _lightSaber.position, Color.red, 0.3f);
            Debug.DrawLine(_cutStartPos, _lightSaber.position + _lightSaber.forward, Color.red, 0.3f);
            Debug.DrawLine(_lightSaber.position + _lightSaber.forward, _lightSaber.position, Color.red, 0.3f);
        }
    }
}
