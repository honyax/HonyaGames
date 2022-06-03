using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Stone _stonePrefab;

    [SerializeField]
    private Transform _stoneBase;

    private Stone[][] _stones;

    private void Start()
    {
        _stones = new Stone[8][];
        for (var z = 0; z < 8; z++)
        {
            _stones[z] = new Stone[8];
            for (var x = 0; x < 8; x++)
            {
                var stone = Instantiate(_stonePrefab, _stoneBase);
                var t = stone.transform;
                t.localPosition = new Vector3(x * 10, 0, z * 10);
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                _stones[z][x] = stone;
                stone.SetActive(false, Stone.Color.Black);
            }
        }

        _stones[3][3].SetActive(true, Stone.Color.Black);
        _stones[3][4].SetActive(true, Stone.Color.White);
        _stones[4][3].SetActive(true, Stone.Color.White);
        _stones[4][4].SetActive(true, Stone.Color.Black);
    }
}
