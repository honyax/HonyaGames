using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HonyagdollPlayer : MonoBehaviour
{
    [SerializeField]
    private Rigidbody[] ragdolls;

    [SerializeField]
    private float explosionForce = 10;

    [SerializeField]
    private float explosionRadius = 10;

    [SerializeField]
    private float upwardsModifier = 0;

    [SerializeField]
    private ForceMode forceMode = ForceMode.Force;

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse.leftButton.wasReleasedThisFrame)
        {
            foreach (var ragdoll in ragdolls)
            {
                ragdoll.AddExplosionForce(explosionForce, gameObject.transform.position, explosionRadius, upwardsModifier, forceMode);
            }
        }
    }
}
