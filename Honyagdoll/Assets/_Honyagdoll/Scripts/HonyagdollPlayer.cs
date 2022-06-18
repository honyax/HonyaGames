using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HonyagdollPlayer : MonoBehaviour
{
    [SerializeField]
    private Honyagdoll[] _honyagdollPrefab;

    [SerializeField]
    private ParticleSystem _explosionEffect;

    private float _explosionForce = 1000;

    private float _explosionRadius = 100;

    private float _upwardsModifier = 3;

    private ForceMode _forceMode = ForceMode.Impulse;

    private List<Honyagdoll> _honyagdolls = new List<Honyagdoll>();

    private System.Random _random = new System.Random();

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            var ps = Instantiate(_explosionEffect, this.transform);
            var t = ps.gameObject.transform;
            t.localPosition = new Vector3(0, 1, 0);
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one * 2;
            foreach (var honyagdoll in _honyagdolls)
            {
                honyagdoll.MainRigidbody.AddExplosionForce(_explosionForce, gameObject.transform.position, _explosionRadius, _upwardsModifier, _forceMode);
            }
        }
        else if (mouse.rightButton.wasReleasedThisFrame)
        {
            var honyagdoll = Instantiate(_honyagdollPrefab[_random.Next(_honyagdollPrefab.Length)]);
            var t = honyagdoll.transform;
            t.position = transform.position + transform.forward * 5 + Vector3.up * 5;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
            _honyagdolls.Add(honyagdoll);
        }
    }
}
