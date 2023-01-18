using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManeger : MonoBehaviour
{
    // ゲームオーバーパネル
    [SerializeField]
    private GameObject game_over_panel;
    // ホールドパネル
    [SerializeField]
    private GameObject hold_panel;
    // 次のミノパネル
    [SerializeField]
    private GameObject next_panel;
    // オーディオクリップ
    [SerializeField]
    private AudioClip restart_se;
    [SerializeField]
    private AudioClip hold_se;
    [SerializeField]
    private AudioClip move_se;
    // 落下スピード
    [SerializeField]
    private float down_time;
    // インターバル
    [SerializeField]
    private float move_interval, rotate_interval;
    // 感度
    [SerializeField]
    private float touch_sensitivity;
    // タップ時間
    [SerializeField]
    private float tap_time_max;

    private AudioSource game_se;    // オーディオ
    private Transform start_trans;  // 初期データ
    private Spawner spawner;    // スポナー
    private Board board;    // ボード
    private Block active_block;    // 生成されたブロック格納
    private Block next_block;    // 次のブロック格納
    private Block hold_block;    // ホールドブロック格納
    private float current_time = 0.0f;  // タイマー
    private float tap_time = 0.0f;
    private float move_timer, rotate_timer;     // 入力受付タイマー
    private bool is_game_over = false; // ゲームオーバーか
    private bool is_tap_time_start = false; // タップ時間計測してるか
    private bool is_tap = false;    // タップしてるか

    // Start is called before the first frame update
    void Start()
    {
        // パネルを非表示
        if (game_over_panel.activeInHierarchy)
        {
            game_over_panel.SetActive(false);
        }

        // タイマーの初期化
        move_timer = Time.time + move_interval;
        rotate_timer = Time.time + rotate_interval;

        // コンポーネント取得
        board = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner> ();
        game_se = GetComponent<AudioSource>();

        // ブロック生成関数で変数を保存
        if (!active_block)
        {
            // 生成
            active_block = spawner.SpawnBlock();
            next_block = spawner.SpawnBlock();

            // 初期データの保存
            start_trans = active_block.transform;

            // 次のミノの調整
            next_block.transform.position = next_panel.transform.position;
            next_block.transform.localScale *= 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ゲームオーバーなら
        if (is_game_over)
        {
            return;
        }

        // プレイヤーの操作
        PlayerInput();
    }

    // プレイヤーの操作
    void PlayerInput()
    {
        // エディターじゃなかったら
        if (Application.isEditor)
        {
            // タッチしてたら
            if (Input.touchCount > 0)
            {
                // 移動していたら
                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    float x, y; // 感度計算用

                    // 感度を計算
                    x = Input.touches[0].deltaPosition.x * touch_sensitivity;
                    y = Input.touches[0].deltaPosition.y * touch_sensitivity;

                    // 右に移動したら
                    if (x > 1.0f && (Time.time > move_timer))
                    {
                        // 右に動かす
                        active_block.MoveRight();

                        game_se.clip = move_se;
                        game_se.Play();

                        // タイマー更新
                        move_timer = Time.time + move_interval;

                        // ボードから出てたら
                        if (!board.CheckPosition(active_block))
                        {
                            // 戻す
                            active_block.MoveLeft();
                        }
                    }
                    // 左に移動したら
                    else if (x < -1.0f && (Time.time > move_timer))
                    {
                        // 左に動かす
                        active_block.MoveLeft();

                        game_se.clip = move_se;
                        game_se.Play();

                        // タイマー更新
                        move_timer = Time.time + move_interval;

                        // ボードから出てたら
                        if (!board.CheckPosition(active_block))
                        {
                            // 戻す
                            active_block.MoveRight();
                        }
                    }
                    // 下に移動したら
                    else if (y < -1.5f)
                    {
                        // 落下
                        active_block.MoveDown();

                        game_se.clip = move_se;
                        game_se.Play();

                        // タイマー更新
                        move_timer = Time.time + move_interval;

                        if (!board.CheckPosition(active_block))
                        {
                            // 戻す
                            active_block.MoveUp();
                        }
                    }
                }

                // 触れたら
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    is_tap_time_start = true;
                }

                // 時間内に指を離したら
                if (is_tap_time_start &&
                    tap_time >= 0.0f &&
                    tap_time <= tap_time_max &&
                    Input.touches[0].phase == TouchPhase.Ended)
                {
                    is_tap = true;
                }
            }

            // 計測してたら
            if (is_tap_time_start)
            {
                tap_time += Time.deltaTime;
            }
            // 計測してなかったら
            else if (!is_tap_time_start)
            {
                tap_time = 0.0f;
            }

            // 時間を超えたら
            if (tap_time > tap_time_max)
            {
                is_tap_time_start = false;
                is_tap = false;
            }

            // タップしたら
            if (is_tap)
            {
                // 右回転
                active_block.RotateRight();

                // タイマー更新
                rotate_timer = Time.time + rotate_interval;

                // ボードから出てたら
                if (!board.CheckPosition(active_block))
                {
                    // 戻す
                    active_block.RotateLeft();
                }
                is_tap = false;
            }
        }

        // エディターだったら
        else if (Application.isEditor)
        {
            // Dを押したら
            if (Input.GetKeyDown(KeyCode.D))
            {
                // 右に動かす
                active_block.MoveRight();

                // タイマー更新
                move_timer = Time.time + move_interval;

                // ボードから出てたら
                if (!board.CheckPosition(active_block))
                {
                    // 戻す
                    active_block.MoveLeft();
                }
            }
            // Aを押したら
            else if (Input.GetKeyDown(KeyCode.A))
            {
                // 左に動かす
                active_block.MoveLeft();

                // タイマー更新
                move_timer = Time.time + move_interval;

                // ボードから出てたら
                if (!board.CheckPosition(active_block))
                {
                    // 戻す
                    active_block.MoveRight();
                }
            }
            // Eを押したら
            if (Input.GetKeyDown(KeyCode.E))
            {
                // 右回転
                active_block.RotateRight();

                // タイマー更新
                rotate_timer = Time.time + rotate_interval;

                // ボードから出てたら
                if (!board.CheckPosition(active_block))
                {
                    // 戻す
                    active_block.RotateLeft();
                }
            }
        }

        current_time += Time.deltaTime;

        // 指定した秒数に一回
        if (current_time > down_time)
        {
            // ログ
            Debug.Log(down_time + "経過");

            // 下に落ちるように関数呼び出し
            if (active_block)
            {
                // 落下
                active_block.MoveDown();

                // Boardクラスの関数を呼び出してボードからはみ出ていないか確認
                if (!board.CheckPosition(active_block))
                {
                    // 上からはみ出てるか確認
                    if (board.OverLimit(active_block))
                    {
                        GameOver();
                    }
                    // それ以外
                    else
                    {
                        BottomBoard();
                    }
                }
            }
            current_time = 0.0f;
        }
    }


    // ボードの底についた時に次のブロックを生成
    void BottomBoard()
    {
        GameObject buf_block;    // 一時保存

        active_block.MoveUp();

        // ボードに情報を保存
        board.SaveBlockInGrid(active_block);

        // 倍率を元に戻す
        next_block.transform.localScale *= 0.5f;

        // 一時保存
        buf_block = Instantiate(next_block.gameObject);

        // 次のミノを破棄
        Destroy(next_block.gameObject);
        next_block = null;

        // 現在のミノを一時保存したミノにする
        active_block = buf_block.GetComponent<Block>();
        active_block.transform.position = spawner.transform.position;
        active_block.transform.localScale = start_trans.localScale;

        // 次のミノを生成
        next_block = spawner.SpawnBlock();

        // 次のミノの調整
        next_block.transform.position = next_panel.transform.position;
        next_block.transform.localScale *= 2;

        // タイマー更新
        move_timer = Time.time;
        rotate_timer = Time.time;
        board.ClearAllRows();
    }

    // パネル表示
    void GameOver()
    {
        active_block.MoveUp();

        // 非表示なら
        if (!game_over_panel.activeInHierarchy)
        {
            game_over_panel.SetActive(true);
        }
        is_game_over = true;
    }

    // ホールド
    public void Hold()
    {
        game_se.clip = hold_se;
        game_se.Play();

        // ホールドされていたら
        if (hold_block)
        {
            GameObject buf_block;    // 一時保存
            Transform buf_bloc_trans;   // 情報の一時保存

            // 一時保存
            buf_block = Instantiate(hold_block.gameObject);

            // ホールドしてるやつを破棄
            Destroy(hold_block.gameObject);
            hold_block = null;

            // 現在のミノの情報を保存
            buf_bloc_trans = active_block.transform;

            // 現在のミノをホールド
            hold_block = active_block;

            // 現在のミノを一時保存したミノにする
            active_block = buf_block.GetComponent<Block>();
            active_block.transform.position = buf_bloc_trans.position;
            active_block.transform.localScale = buf_bloc_trans.localScale;
        }
        // ホールドされていなかったら
        else if (!hold_block)
        {
            // ホールドする
            hold_block = active_block;

            // 新しいミノを出す
            active_block = spawner.SpawnBlock();

            // ホールドミノの調整
            hold_block.transform.localScale *= 2;
        }

        // ホールドミノの調整
        hold_block.transform.position = hold_panel.transform.position;

        move_timer = Time.time;
        rotate_timer = Time.time;
    }

    // リプレイ
    public void Replay()
    {
        game_se.clip = restart_se;
        game_se.Play();
        SceneManager.LoadScene(0);
    }
}