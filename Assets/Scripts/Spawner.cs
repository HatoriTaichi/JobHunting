using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 生成するブロック
    [SerializeField]
    Block[] blocks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ランダムなブロック
    Block GetRandomBlock()
    {
        // 種類を決める
        int type = Random.Range(0, blocks.Length);

        // 種類があったら
        if (blocks[type])
        {
            return blocks[type];
        }
        // それ以外
        else
        {
            return null;
        }
    }

    // 選択したブロックを生成
    public Block SpawnBlock()
    {
        // 生成
        Block block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity);

        // 生成されていたら
        if (block)
        {
            return block;
        }
        // それ以外
        else
        {
            return null;
        }
    }
}
