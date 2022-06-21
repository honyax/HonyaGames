using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Transform _player;

    [SerializeField]
    private Honyagdoll[] _honyagdollPrefabs;

    [SerializeField]
    private ParticleSystem _explosionEffect;

    [SerializeField]
    private AudioSource _seAudioSource;

    [SerializeField]
    private AudioClip _spawnSe;

    [SerializeField]
    private AudioClip _explosionSe;

    private System.Random _random = new System.Random();

    private List<Honyagdoll> _honyagdolls = new List<Honyagdoll>();

    private void Update()
    {
        var kb = Keyboard.current;
        var mouse = Mouse.current;
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            var ps = Instantiate(_explosionEffect, _player);
            var t = ps.transform;
            t.localPosition = new Vector3(0, 1, 0);
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one * 2;
            foreach (var honyagdoll in _honyagdolls)
            {
                honyagdoll.MainRigidbody.AddExplosionForce(1000, _player.position, 100, 5, ForceMode.Impulse);
            }
            _seAudioSource.PlayOneShot(_explosionSe);
        }
        else if (mouse.rightButton.wasReleasedThisFrame)
        {
            var honyagdoll = Instantiate(_honyagdollPrefabs[_random.Next(_honyagdollPrefabs.Length)]);
            var t = honyagdoll.transform;
            t.position = _player.position + _player.forward * 5 + Vector3.up * 5;
            t.rotation = Quaternion.identity;
            _honyagdolls.Add(honyagdoll);
            _seAudioSource.PlayOneShot(_spawnSe);
        }
    }
}
