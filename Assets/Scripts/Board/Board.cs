using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int boardSizeX;

    private int boardSizeY;

    private Cell[,] m_cells;

    private Transform m_root;

    private int m_matchMin;

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        m_matchMin = gameSettings.MatchesMin;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        //set neighbours
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (y + 1 < boardSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
                if (x + 1 < boardSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
                if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
                if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
            }
        }

    }

    //internal void Fill()
    //{
    //    for (int x = 0; x < boardSizeX; x++)
    //    {
    //        for (int y = 0; y < boardSizeY; y++)
    //        {
    //            Cell cell = m_cells[x, y];
    //            NormalItem item = new NormalItem();

    //            List<NormalItem.eNormalType> types = new List<NormalItem.eNormalType>();
    //            if (cell.NeighbourBottom != null)
    //            {
    //                NormalItem nitem = cell.NeighbourBottom.Item as NormalItem;
    //                if (nitem != null)
    //                {
    //                    types.Add(nitem.ItemType);
    //                }
    //            }

    //            if (cell.NeighbourLeft != null)
    //            {
    //                NormalItem nitem = cell.NeighbourLeft.Item as NormalItem;
    //                if (nitem != null)
    //                {
    //                    types.Add(nitem.ItemType);
    //                }
    //            }

    //            item.SetType(Utils.GetRandomNormalTypeExcept(types.ToArray()));
    //            item.SetView();
    //            item.SetViewRoot(m_root);

    //            cell.Assign(item);
    //            cell.ApplyItemPosition(false);
    //        }
    //    }
    //}

    internal void Fill()
    {
        List<Cell> emptyCells = new List<Cell>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                emptyCells.Add(m_cells[x, y]);
            }
        }

        int groupCount = emptyCells.Count / 3;

        for (int i = 0; i < groupCount; i++)
        {
            NormalItem.eNormalType randomType = Utils.GetRandomNormalType();

            List<Cell> selectedCells = new List<Cell>();
            for (int j = 0; j < 3; j++)
            {
                int index = UnityEngine.Random.Range(0, emptyCells.Count);
                selectedCells.Add(emptyCells[index]);
                emptyCells.RemoveAt(index);
            }

            foreach (Cell cell in selectedCells)
            {
                NormalItem item = new NormalItem();
                item.SetType(randomType);
                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
            }
        }
    }


    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }

    //my code
    public List<Cell> GetCellsWithItems()
    {
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (m_cells[x, y].Item != null)
                {
                    cells.Add(m_cells[x, y]);
                }
            }
        }
        return cells;
    }

    public List<Cell> FindSetOfThree()
    {
        Dictionary<NormalItem.eNormalType, List<Cell>> itemGroups = new Dictionary<NormalItem.eNormalType, List<Cell>>();

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                if (cell.Item != null && cell.Item is NormalItem)
                {
                    NormalItem item = (NormalItem)cell.Item;
                    if (!itemGroups.ContainsKey(item.ItemType))
                    {
                        itemGroups[item.ItemType] = new List<Cell>();
                    }
                    itemGroups[item.ItemType].Add(cell);
                }
            }
        }

        foreach (var group in itemGroups)
        {
            if (group.Value.Count >= 3)
            {
                return group.Value.Take(3).ToList();
            }
        }
        return null;
    }
}
