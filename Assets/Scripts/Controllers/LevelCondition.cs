using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action<GameManager.eStateGame> ConditionCompleteEvent = delegate { };
    //public event Action ConditionFailedEvent = delegate { };

    protected Text m_txt;

    protected bool m_conditionCompleted = false;

    public virtual void Setup(float value, Text txt)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, GameManager mngr)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, BoardController board)
    {
        m_txt = txt;
    }

    public virtual void Setup(float value, Text txt, BoardController board, BufferController bufferController)
    {
        m_txt = txt;
    }
    protected virtual void UpdateText() { }

    protected void OnWinGame()
    {
        m_conditionCompleted = true;

        ConditionCompleteEvent(GameManager.eStateGame.GAME_WIN);
    }

    protected void OnEndGame()
    {
        m_conditionCompleted = true;
        ConditionCompleteEvent(GameManager.eStateGame.GAME_OVER);


    }
    protected virtual void OnDestroy()
    {

    }
}
