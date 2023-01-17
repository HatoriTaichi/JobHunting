using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    // �{�[�h��՗p�̎l�p�g�i�[�p�ATransform�֎l�p�g���i�[����
    [SerializeField]
    private Transform empty_sprite;
    // �X�R�A�e�L�X�g
    [SerializeField]
    private TextMeshProUGUI score_text;

    private int height = 30, width = 10, header = 11;    // �{�[�h�̍���,�{�[�h�̕�,�{�[�h�̍��������p���l
    private Transform[,] grid;    // 2�����z��̍쐬
    private int score = 0;  // �X�R�A

    // Start is called before the first frame update
    void Start()
    {
        // �e�L�X�g�\��
        score_text.text = "Score : " + score;

        // �{�[�h�̐���
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�{�[�h�쐬
    void CreateBoard()
    {
        // ��񂪂�������
        if (empty_sprite)
        {
            // �c�����̃��[�v
            for (int y = 0; y < height - header; y++)
            {
                // �������̃��[�v
                for (int x = 0; x < width; x++)
                {
                    // ����
                    Transform clone = Instantiate(empty_sprite, new Vector3(x, y, 0), Quaternion.identity);

                    // �e�q�֌W��t����
                    clone.transform.parent = transform;
                }
            }
        }
    }

    // �u���b�N���g���ɂ���̂�����
    public bool CheckPosition(Block block)
    {
        // �������ԂɌ��Ă���
        foreach (Transform item in block.transform)
        {
            // �����_���ۂ߂�
            Vector2 pos = Rounding.Round(item.position);

            // �g���ɂ��邩����
            if (!BoardOutCheck((int)pos.x, (int)pos.y))
            {
                return false;
            }

            // �ړ���Ƀu���b�N���Ȃ�������
            if (BlockCheck((int)pos.x, (int)pos.y, block))
            {
                return false;
            }
        }
        return true;
    }

    // �g���ɂ��邩����
    bool BoardOutCheck(int x, int y)
    {
        // �g���ɂ�����
        if ((x >= 0 && x < width && y >= 0))
        {
            return true;
        }
        // ����ȊO
        else
        {
            return false;
        }
    }

    // �ړ���Ƀu���b�N���Ȃ�������
    bool BlockCheck(int x, int y, Block block)
    {
        // �u���b�N������
        if ((grid[x, y] != null && grid[x, y].parent != block.transform))
        {
            return true;
        }
        // ����ȊO
        else
        {
            return false;
        }
    }

    // �u���b�N���������|�W�V�������L�^
    public void SaveBlockInGrid(Block block)
    {
        // �������ԂɌ��Ă���
        foreach (Transform item in block.transform)
        {
            // �����_���ۂ߂�
            Vector2 pos = Rounding.Round(item.position);

            // �L�^
            grid[(int)pos.x, (int)pos.y] = item;
        }
    }

    // ���ׂĂ̍s���`�F�b�N
    bool IsComplete(int y)
    {
        // �������̃��[�v
        for (int x =0; x < width; x++) 
        {
            // �Ȃ������������
            if (grid[x, y]== null)
            {
                return false;
            }
        }
        return true;
    }

    // ����
    void ClearRow(int y)
    {
        // �������̃��[�v
        for (int x =0; x < width; x++) 
        {
            // ��������Ă�����
            if (grid[x, y] != null)
            {
                // �j��
                Destroy(grid[x, y].gameObject);
            }
            grid[x, y] = null;
        }
    }

    //��ɂ���u���b�N��������
    void ShiftRowsDown(int start_y)
    {
        // �c�����̃��[�v
        for (int y = start_y; y < height; y++)
        {
            // �������̃��[�v
            for (int x = 0; x < width; x++)
            {
                // ��������Ă�����
                if (grid[x, y] != null)
                {
                    // ������i������
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += new Vector3(0,-1,0);
                }
            }
        }
    }


    // ���ׂĂ̍s���`�F�b�N
    public void ClearAllRows()
    {
        int clear_line = 0; // �����C����������

        // �c�����̃��[�v
        for(int y = 0; y < height; y++)
        {
            // �����ɏ�񂪓����Ă�����
            if(IsComplete(y))
            {
                // �j��
                 ClearRow(y);

                // �X�R�A���Z
                score += 100;

                // ��i������
                ShiftRowsDown(y + 1);
                clear_line++;
                y--;
            }
        }

        // ���C���{�[�i�X
        if (clear_line >= 2)
        {
            score += (200 * (clear_line - 1));
        }

        // �e�L�X�g�ύX
        score_text.text = "Score : " + score;
    }

    // �O���b�h�̐���
    private void Awake()
    {
        grid = new Transform[width, height];
    }

    // �͂ݏo�Ă邩
    public bool OverLimit(Block block)
    {
        // �������ԂɌ��Ă���
        foreach (Transform item in block.transform)
        {
            // �͂ݏo�Ă���
            if (item.transform.position.y >= height - header)
            {
                return true;
            }
        }
        return false;
    }
}