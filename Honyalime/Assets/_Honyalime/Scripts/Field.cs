using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _wallPrefabs;

    private List<GameObject> _walls = new List<GameObject>();

    public void Initialize()
    {
        CreateRandomWalls();
    }

    private void CreateRandomWalls()
    {
        foreach (var wall in _walls)
        {
            Destroy(wall);
        }
        _walls.Clear();

        var random = new System.Random();
        var parent = transform;
        for (var z = -460; z <= 460; z += 40)
        {
            var wall = Instantiate(_wallPrefabs[random.Next(_wallPrefabs.Length)], parent);
            var pos = wall.transform.localPosition;
            pos.z = z;
            wall.transform.localPosition = pos;
            _walls.Add(wall);
        }
    }
}
