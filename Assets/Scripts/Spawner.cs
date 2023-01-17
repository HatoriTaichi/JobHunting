using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // ��������u���b�N
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

    // �����_���ȃu���b�N
    Block GetRandomBlock()
    {
        // ��ނ����߂�
        int type = Random.Range(0, blocks.Length);

        // ��ނ���������
        if (blocks[type])
        {
            return blocks[type];
        }
        // ����ȊO
        else
        {
            return null;
        }
    }

    // �I�������u���b�N�𐶐�
    public Block SpawnBlock()
    {
        // ����
        Block block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity);

        // ��������Ă�����
        if (block)
        {
            return block;
        }
        // ����ȊO
        else
        {
            return null;
        }
    }
}
