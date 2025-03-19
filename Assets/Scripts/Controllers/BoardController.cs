using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };
    //public Action OnFullBuffer = delegate { };

    public bool IsBusy { get; private set; }

    private Board m_board;

    private GameManager m_gameManager;

    private bool m_isDragging;

    private Camera m_cam;

    private Collider2D m_hitCollider;

    private GameSettings m_gameSettings;

    private bool m_gameOver;

    // my variables

    private BufferController m_bufferController;

    private Coroutine autoSelectCoroutine;
    private Coroutine autoGroupCoroutine;
    private int id;
    private bool isAutoSelecting = false;
    public void StartGame(GameManager gameManager, GameSettings gameSettings, BufferController bufferController)
    {
        m_gameManager = gameManager;
        m_gameSettings = gameSettings;
        m_bufferController = bufferController;
        m_gameManager.StateChangedAction += OnGameStateChange;
        m_cam = Camera.main;
        m_board = new Board(this.transform, gameSettings);
        Fill();
    }

    private void Fill()
    {
        m_board.Fill();
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }


    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if (isAutoSelecting) return;

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Cell cell = hit.collider.GetComponent<Cell>();
                if (cell != null && cell.Item != null)
                {
                    Item item = cell.Item;
                    cell.Clear();
                    id = m_bufferController.GetNumberOfItemInBuffer();
                    m_bufferController.AddToBufferCells(item);
                    StartCoroutine(MoveItemToBufferCells(item));
                    OnMoveEvent();
                }
            }
        }
    }

    //my code
    IEnumerator MoveItemToBufferCells(Item item)
    {
        Transform targetTransform = m_bufferController.GetBufferCellTransform(id);
        float duration = 0.2f;

        if (item.View != null)
        {
            item.View.DOMove(targetTransform.position, duration).SetEase(Ease.InOutQuad);

            yield return new WaitForSeconds(duration);
        }
        else
        {
            yield break;
        }
    }
    //----------------------------------------------------------------------------

    // tự động chọn thua
    public void StartAutoSelect()
    {
        isAutoSelecting = true;
        if (autoSelectCoroutine == null)
        {
            autoSelectCoroutine = StartCoroutine(AutoSelectRoutine());
        }
    }

    public void StopAutoSelect()
    {
        if (autoSelectCoroutine != null)
        {
            StopCoroutine(autoSelectCoroutine);
            autoSelectCoroutine = null;
        }
    }

    private IEnumerator AutoSelectRoutine()
    {
        yield return new WaitForSeconds(0.8f);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (IsBusy || m_gameOver) continue;

            List<Cell> availableCells = m_board.GetCellsWithItems();
            if (availableCells.Count == 0) yield break;

            int randomIndex = UnityEngine.Random.Range(0, availableCells.Count);
            Cell selectedCell = availableCells[randomIndex];

            HandleCellSelection(selectedCell);
        }
    }

    private void HandleCellSelection(Cell cell)
    {
        if (cell == null || cell.Item == null) return;

        Item item = cell.Item;
        cell.Clear();
        int bufferIndex = m_bufferController.GetNumberOfItemInBuffer();
        id = bufferIndex;
        m_bufferController.AddToBufferCells(item);
        StartCoroutine(MoveItemToBufferCells(item));
        OnMoveEvent();
    }

    internal void Clear()
    {
        m_board.Clear();
    }

    //----------------------------------------------------------------------------
    // tự động chọn thắng
    public void StartAutoSelectSet()
    {
        isAutoSelecting = true;

        if (autoGroupCoroutine == null)
        {
            autoGroupCoroutine = StartCoroutine(AutoSelectSetRoutinePeriodic());
        }
    }

    public void StopAutoSelectSet()
    {
        if (autoGroupCoroutine != null)
        {
            StopCoroutine(autoGroupCoroutine);
            autoGroupCoroutine = null;
        }
    }

    private IEnumerator AutoSelectSetRoutinePeriodic()
    {
        yield return new WaitForSeconds(0.8f);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (IsBusy || m_gameOver) continue;

            AutoSelectSet();
        }
    }

    public void AutoSelectSet()
    {
        if (IsBusy || m_gameOver) return;

        List<Cell> set = m_board.FindSetOfThree();
        if (set != null)
        {
            StartCoroutine(AutoSelectSetRoutine(set));
        }
    }

    private IEnumerator AutoSelectSetRoutine(List<Cell> set)
    {
        IsBusy = true;

        foreach (Cell cell in set)
        {
            Item item = cell.Item;
            cell.Clear(); // Xóa item khỏi ô trên bảng
            id = m_bufferController.GetNumberOfItemInBuffer();
            m_bufferController.AddToBufferCells(item);
            StartCoroutine(MoveItemToBufferCells(item));
            yield return new WaitForSeconds(0.5f);
            OnMoveEvent();
        }

        IsBusy = false;
    }

}
