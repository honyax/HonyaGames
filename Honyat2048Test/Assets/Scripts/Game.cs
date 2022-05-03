using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Block _blockPrefab;

    private Block _currentBlock = null;

    void Start()
    {
        SpawnBlock();
    }

    void SpawnBlock()
    {
        var block = Instantiate(_blockPrefab, this.transform);
        _currentBlock = block;
    }

    private void Update()
    {
        if (_currentBlock == null)
            return;

        if (Input.GetMouseButton(0))
        {
            DragBlock();
        }
    }

    private void DragBlock()
    {
        var mousePos = Input.mousePosition;
        var targetPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 6));
        var pos = _currentBlock.transform.localPosition;
        var targetPosX = Mathf.Clamp(targetPos.x, -2.5f, 2.5f);
        _currentBlock.transform.localPosition = new Vector3(targetPosX, pos.y, pos.z);
    }
}
