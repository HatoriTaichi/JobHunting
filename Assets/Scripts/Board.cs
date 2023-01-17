using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Board : MonoBehaviour
{
    // ボード基盤用の四角枠格納用、Transformへ四角枠を格納する
    [SerializeField]
    private Transform empty_sprite;
    // スコアテキスト
    [SerializeField]
    private TextMeshProUGUI score_text;

    private int height = 30, width = 10, header = 11;    // ボードの高さ,ボードの幅,ボードの高さ調整用数値
    private Transform[,] grid;    // 2次元配列の作成
    private int score = 0;  // スコア

    // Start is called before the first frame update
    void Start()
    {
        // テキスト表示
        score_text.text = "Score : " + score;

        // ボードの生成
        CreateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ボード作成
    void CreateBoard()
    {
        // 情報があったら
        if (empty_sprite)
        {
            // 縦幅分のループ
            for (int y = 0; y < height - header; y++)
            {
                // 横幅分のループ
                for (int x = 0; x < width; x++)
                {
                    // 生成
                    Transform clone = Instantiate(empty_sprite, new Vector3(x, y, 0), Quaternion.identity);

                    // 親子関係を付ける
                    clone.transform.parent = transform;
                }
            }
        }
    }

    // ブロックが枠内にあるのか判定
    public bool CheckPosition(Block block)
    {
        // 情報を順番に見ていく
        foreach (Transform item in block.transform)
        {
            // 小数点を丸める
            Vector2 pos = Rounding.Round(item.position);

            // 枠内にあるか判定
            if (!BoardOutCheck((int)pos.x, (int)pos.y))
            {
                return false;
            }

            // 移動先にブロックがないか判定
            if (BlockCheck((int)pos.x, (int)pos.y, block))
            {
                return false;
            }
        }
        return true;
    }

    // 枠内にあるか判定
    bool BoardOutCheck(int x, int y)
    {
        // 枠内にいたら
        if ((x >= 0 && x < width && y >= 0))
        {
            return true;
        }
        // それ以外
        else
        {
            return false;
        }
    }

    // 移動先にブロックがないか判定
    bool BlockCheck(int x, int y, Block block)
    {
        // ブロックがある
        if ((grid[x, y] != null && grid[x, y].parent != block.transform))
        {
            return true;
        }
        // それ以外
        else
        {
            return false;
        }
    }

    // ブロックが落ちたポジションを記録
    public void SaveBlockInGrid(Block block)
    {
        // 情報を順番に見ていく
        foreach (Transform item in block.transform)
        {
            // 小数点を丸める
            Vector2 pos = Rounding.Round(item.position);

            // 記録
            grid[(int)pos.x, (int)pos.y] = item;
        }
    }

    // すべての行をチェック
    bool IsComplete(int y)
    {
        // 横幅分のループ
        for (int x =0; x < width; x++) 
        {
            // なんも無かったら
            if (grid[x, y]== null)
            {
                return false;
            }
        }
        return true;
    }

    // 消去
    void ClearRow(int y)
    {
        // 横幅分のループ
        for (int x =0; x < width; x++) 
        {
            // 生成されていたら
            if (grid[x, y] != null)
            {
                // 破棄
                Destroy(grid[x, y].gameObject);
            }
            grid[x, y] = null;
        }
    }

    //上にあるブロックを下げる
    void ShiftRowsDown(int start_y)
    {
        // 縦幅分のループ
        for (int y = start_y; y < height; y++)
        {
            // 横幅分のループ
            for (int x = 0; x < width; x++)
            {
                // 生成されていたら
                if (grid[x, y] != null)
                {
                    // 情報を一段下げる
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += new Vector3(0,-1,0);
                }
            }
        }
    }


    // すべての行をチェック
    public void ClearAllRows()
    {
        int clear_line = 0; // 何ライン消したか

        // 縦幅分のループ
        for(int y = 0; y < height; y++)
        {
            // そこに情報が入っていたら
            if(IsComplete(y))
            {
                // 破棄
                 ClearRow(y);

                // スコア加算
                score += 100;

                // 一段下げる
                ShiftRowsDown(y + 1);
                clear_line++;
                y--;
            }
        }

        // ラインボーナス
        if (clear_line >= 2)
        {
            score += (200 * (clear_line - 1));
        }

        // テキスト変更
        score_text.text = "Score : " + score;
    }

    // グリッドの生成
    private void Awake()
    {
        grid = new Transform[width, height];
    }

    // はみ出てるか
    public bool OverLimit(Block block)
    {
        // 情報を順番に見ていく
        foreach (Transform item in block.transform)
        {
            // はみ出てたら
            if (item.transform.position.y >= height - header)
            {
                return true;
            }
        }
        return false;
    }
}