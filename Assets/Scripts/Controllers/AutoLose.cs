using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoLose : LevelCondition
{
    private int m_moves;
    private int bufferSize = 5;
    private BoardController m_board;
    private BufferController m_bufferController;
    public override void Setup(float value, Text txt, BoardController board, BufferController bufferController)
    {
        base.Setup(value, txt);
        m_bufferController = bufferController;
        m_moves = (int)value;
        m_board = board;

        m_bufferController.OnBufferSizeChange += OnBufferSizeChange;

        m_board.OnMoveEvent += OnMove;

        UpdateText();
    }

    void Start()
    {
        m_board.StartAutoSelect();
    }

private void OnMove()
    {
        if (m_conditionCompleted) return;

        m_moves--;
        //Debug.Log(m_moves);
        UpdateText();

        if (m_moves <= 0)
        {
            OnWinGame();
        }
    }

    private void OnBufferSizeChange(int num)
    {
        if (num == bufferSize)
        {
            OnEndGame();
        }
    }

    protected override void UpdateText()
    {
        m_txt.text = string.Format("ITEM LEFT:\n{0}", m_moves);
    }

    protected override void OnDestroy()
    {
        if (m_board != null) m_board.OnMoveEvent -= OnMove;

        base.OnDestroy();
    }
}
