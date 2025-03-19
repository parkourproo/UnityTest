﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanelMain : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnMoves;
    
    [SerializeField] private Button btnTimer;

    [SerializeField] private Button btnAutoPlay;
    
    [SerializeField] private Button btnAutoLose;

    private UIMainManager m_mngr;

    private void Awake()
    {
        btnMoves.onClick.AddListener(OnClickMoves);
        btnTimer.onClick.AddListener(OnClickTimer);
        btnAutoPlay.onClick.AddListener(OnClickAutoPlay);
        btnAutoLose.onClick.AddListener(OnClickAutoLose);
    }

    private void OnDestroy()
    {
        if (btnMoves) btnMoves.onClick.RemoveAllListeners();
        if (btnTimer) btnTimer.onClick.RemoveAllListeners();
        if (btnAutoPlay) btnAutoPlay.onClick.RemoveAllListeners();
        if (btnAutoLose) btnAutoLose.onClick.RemoveAllListeners();
    }

    public void Setup(UIMainManager mngr)
    {
        m_mngr = mngr;
    }
    private void OnClickMoves()
    {
        m_mngr.LoadLevelMoves();
    }

    private void OnClickTimer()
    {
        m_mngr.LoadLevelTimer();
    }


    private void OnClickAutoPlay()
    {
        m_mngr.LoadLevelAutoPlay();
    }

    private void OnClickAutoLose()
    {
        m_mngr.LoadLevelAutoLose();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
