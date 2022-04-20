using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    const int FieldXLength = 10;
    const int FieldYLength = 20;

    [SerializeField]
    private GameObject _field;

    [SerializeField]
    private SpriteRenderer _blockPrefab;

    private SpriteRenderer[,] _blockObjects;

    private void Start()
    {
        _blockObjects = new SpriteRenderer[FieldXLength, FieldYLength];
        for (var y = 0; y < FieldYLength; y++)
        {
            for (var x = 0; x < FieldXLength; x++)
            {
                var block = Instantiate(_blockPrefab, _field.transform);
                block.transform.localPosition = new Vector3(x, y, 0);
                block.transform.localRotation = Quaternion.identity;
                block.transform.localScale = Vector3.one;
                block.color = Color.black;
                _blockObjects[x, y] = block;
            }
        }
    }
}
