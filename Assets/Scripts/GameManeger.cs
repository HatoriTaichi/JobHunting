using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManeger : MonoBehaviour
{
    // �Q�[���I�[�o�[�p�l��
    [SerializeField]
    private GameObject game_over_panel;
    // �z�[���h�p�l��
    [SerializeField]
    private GameObject hold_panel;
    // ���̃~�m�p�l��
    [SerializeField]
    private GameObject next_panel;
    // �I�[�f�B�I�N���b�v
    [SerializeField]
    private AudioClip restart_se;
    [SerializeField]
    private AudioClip hold_se;
    [SerializeField]
    private AudioClip move_se;
    // �����X�s�[�h
    [SerializeField]
    private float down_time;
    // �C���^�[�o��
    [SerializeField]
    private float move_interval, rotate_interval;
    // ���x
    [SerializeField]
    private float touch_sensitivity;
    // �^�b�v����
    [SerializeField]
    private float tap_time_max;

    private AudioSource game_se;    // �I�[�f�B�I
    private Transform start_trans;  // �����f�[�^
    private Spawner spawner;    // �X�|�i�[
    private Board board;    // �{�[�h
    private Block active_block;    // �������ꂽ�u���b�N�i�[
    private Block next_block;    // ���̃u���b�N�i�[
    private Block hold_block;    // �z�[���h�u���b�N�i�[
    private float current_time = 0.0f;  // �^�C�}�[
    private float tap_time = 0.0f;
    private float move_timer, rotate_timer;     // ���͎�t�^�C�}�[
    private bool is_game_over = false; // �Q�[���I�[�o�[��
    private bool is_tap_time_start = false; // �^�b�v���Ԍv�����Ă邩
    private bool is_tap = false;    // �^�b�v���Ă邩

    // Start is called before the first frame update
    void Start()
    {
        // �p�l�����\��
        if (game_over_panel.activeInHierarchy)
        {
            game_over_panel.SetActive(false);
        }

        // �^�C�}�[�̏�����
        move_timer = Time.time + move_interval;
        rotate_timer = Time.time + rotate_interval;

        // �R���|�[�l���g�擾
        board = GameObject.FindObjectOfType<Board>();
        spawner = GameObject.FindObjectOfType<Spawner> ();
        game_se = GetComponent<AudioSource>();

        // �u���b�N�����֐��ŕϐ���ۑ�
        if (!active_block)
        {
            // ����
            active_block = spawner.SpawnBlock();
            next_block = spawner.SpawnBlock();

            // �����f�[�^�̕ۑ�
            start_trans = active_block.transform;

            // ���̃~�m�̒���
            next_block.transform.position = next_panel.transform.position;
            next_block.transform.localScale *= 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �Q�[���I�[�o�[�Ȃ�
        if (is_game_over)
        {
            return;
        }

        // �v���C���[�̑���
        PlayerInput();
    }

    // �v���C���[�̑���
    void PlayerInput()
    {
        // �G�f�B�^�[����Ȃ�������
        if (Application.isEditor)
        {
            // �^�b�`���Ă���
            if (Input.touchCount > 0)
            {
                // �ړ����Ă�����
                if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    float x, y; // ���x�v�Z�p

                    // ���x���v�Z
                    x = Input.touches[0].deltaPosition.x * touch_sensitivity;
                    y = Input.touches[0].deltaPosition.y * touch_sensitivity;

                    // �E�Ɉړ�������
                    if (x > 1.0f && (Time.time > move_timer))
                    {
                        // �E�ɓ�����
                        active_block.MoveRight();

                        game_se.clip = move_se;
                        game_se.Play();

                        // �^�C�}�[�X�V
                        move_timer = Time.time + move_interval;

                        // �{�[�h����o�Ă���
                        if (!board.CheckPosition(active_block))
                        {
                            // �߂�
                            active_block.MoveLeft();
                        }
                    }
                    // ���Ɉړ�������
                    else if (x < -1.0f && (Time.time > move_timer))
                    {
                        // ���ɓ�����
                        active_block.MoveLeft();

                        game_se.clip = move_se;
                        game_se.Play();

                        // �^�C�}�[�X�V
                        move_timer = Time.time + move_interval;

                        // �{�[�h����o�Ă���
                        if (!board.CheckPosition(active_block))
                        {
                            // �߂�
                            active_block.MoveRight();
                        }
                    }
                    // ���Ɉړ�������
                    else if (y < -1.5f)
                    {
                        // ����
                        active_block.MoveDown();

                        game_se.clip = move_se;
                        game_se.Play();

                        // �^�C�}�[�X�V
                        move_timer = Time.time + move_interval;

                        if (!board.CheckPosition(active_block))
                        {
                            // �߂�
                            active_block.MoveUp();
                        }
                    }
                }

                // �G�ꂽ��
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    is_tap_time_start = true;
                }

                // ���ԓ��Ɏw�𗣂�����
                if (is_tap_time_start &&
                    tap_time >= 0.0f &&
                    tap_time <= tap_time_max &&
                    Input.touches[0].phase == TouchPhase.Ended)
                {
                    is_tap = true;
                }
            }

            // �v�����Ă���
            if (is_tap_time_start)
            {
                tap_time += Time.deltaTime;
            }
            // �v�����ĂȂ�������
            else if (!is_tap_time_start)
            {
                tap_time = 0.0f;
            }

            // ���Ԃ𒴂�����
            if (tap_time > tap_time_max)
            {
                is_tap_time_start = false;
                is_tap = false;
            }

            // �^�b�v������
            if (is_tap)
            {
                // �E��]
                active_block.RotateRight();

                // �^�C�}�[�X�V
                rotate_timer = Time.time + rotate_interval;

                // �{�[�h����o�Ă���
                if (!board.CheckPosition(active_block))
                {
                    // �߂�
                    active_block.RotateLeft();
                }
                is_tap = false;
            }
        }

        // �G�f�B�^�[��������
        else if (Application.isEditor)
        {
            // D����������
            if (Input.GetKeyDown(KeyCode.D))
            {
                // �E�ɓ�����
                active_block.MoveRight();

                // �^�C�}�[�X�V
                move_timer = Time.time + move_interval;

                // �{�[�h����o�Ă���
                if (!board.CheckPosition(active_block))
                {
                    // �߂�
                    active_block.MoveLeft();
                }
            }
            // A����������
            else if (Input.GetKeyDown(KeyCode.A))
            {
                // ���ɓ�����
                active_block.MoveLeft();

                // �^�C�}�[�X�V
                move_timer = Time.time + move_interval;

                // �{�[�h����o�Ă���
                if (!board.CheckPosition(active_block))
                {
                    // �߂�
                    active_block.MoveRight();
                }
            }
            // E����������
            if (Input.GetKeyDown(KeyCode.E))
            {
                // �E��]
                active_block.RotateRight();

                // �^�C�}�[�X�V
                rotate_timer = Time.time + rotate_interval;

                // �{�[�h����o�Ă���
                if (!board.CheckPosition(active_block))
                {
                    // �߂�
                    active_block.RotateLeft();
                }
            }
        }

        current_time += Time.deltaTime;

        // �w�肵���b���Ɉ��
        if (current_time > down_time)
        {
            // ���O
            Debug.Log(down_time + "�o��");

            // ���ɗ�����悤�Ɋ֐��Ăяo��
            if (active_block)
            {
                // ����
                active_block.MoveDown();

                // Board�N���X�̊֐����Ăяo���ă{�[�h����͂ݏo�Ă��Ȃ����m�F
                if (!board.CheckPosition(active_block))
                {
                    // �ォ��͂ݏo�Ă邩�m�F
                    if (board.OverLimit(active_block))
                    {
                        GameOver();
                    }
                    // ����ȊO
                    else
                    {
                        BottomBoard();
                    }
                }
            }
            current_time = 0.0f;
        }
    }


    // �{�[�h�̒�ɂ������Ɏ��̃u���b�N�𐶐�
    void BottomBoard()
    {
        GameObject buf_block;    // �ꎞ�ۑ�

        active_block.MoveUp();

        // �{�[�h�ɏ���ۑ�
        board.SaveBlockInGrid(active_block);

        // �{�������ɖ߂�
        next_block.transform.localScale *= 0.5f;

        // �ꎞ�ۑ�
        buf_block = Instantiate(next_block.gameObject);

        // ���̃~�m��j��
        Destroy(next_block.gameObject);
        next_block = null;

        // ���݂̃~�m���ꎞ�ۑ������~�m�ɂ���
        active_block = buf_block.GetComponent<Block>();
        active_block.transform.position = spawner.transform.position;
        active_block.transform.localScale = start_trans.localScale;

        // ���̃~�m�𐶐�
        next_block = spawner.SpawnBlock();

        // ���̃~�m�̒���
        next_block.transform.position = next_panel.transform.position;
        next_block.transform.localScale *= 2;

        // �^�C�}�[�X�V
        move_timer = Time.time;
        rotate_timer = Time.time;
        board.ClearAllRows();
    }

    // �p�l���\��
    void GameOver()
    {
        active_block.MoveUp();

        // ��\���Ȃ�
        if (!game_over_panel.activeInHierarchy)
        {
            game_over_panel.SetActive(true);
        }
        is_game_over = true;
    }

    // �z�[���h
    public void Hold()
    {
        game_se.clip = hold_se;
        game_se.Play();

        // �z�[���h����Ă�����
        if (hold_block)
        {
            GameObject buf_block;    // �ꎞ�ۑ�
            Transform buf_bloc_trans;   // ���̈ꎞ�ۑ�

            // �ꎞ�ۑ�
            buf_block = Instantiate(hold_block.gameObject);

            // �z�[���h���Ă���j��
            Destroy(hold_block.gameObject);
            hold_block = null;

            // ���݂̃~�m�̏���ۑ�
            buf_bloc_trans = active_block.transform;

            // ���݂̃~�m���z�[���h
            hold_block = active_block;

            // ���݂̃~�m���ꎞ�ۑ������~�m�ɂ���
            active_block = buf_block.GetComponent<Block>();
            active_block.transform.position = buf_bloc_trans.position;
            active_block.transform.localScale = buf_bloc_trans.localScale;
        }
        // �z�[���h����Ă��Ȃ�������
        else if (!hold_block)
        {
            // �z�[���h����
            hold_block = active_block;

            // �V�����~�m���o��
            active_block = spawner.SpawnBlock();

            // �z�[���h�~�m�̒���
            hold_block.transform.localScale *= 2;
        }

        // �z�[���h�~�m�̒���
        hold_block.transform.position = hold_panel.transform.position;

        move_timer = Time.time;
        rotate_timer = Time.time;
    }

    // ���v���C
    public void Replay()
    {
        game_se.clip = restart_se;
        game_se.Play();
        SceneManager.LoadScene(0);
    }
}