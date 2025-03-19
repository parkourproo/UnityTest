using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using System;

public class BufferController : MonoBehaviour
{
    public event Action<int> OnBufferSizeChange = delegate { };

    private const int BUFFER_SIZE = 5;
    private Cell[] bufferCells;
    private int[] identicalIndexItem = new int[2];
    private GameObject[] identicalItemsList = new GameObject[3];
    [SerializeField] private float spacing = 1.15f;
    //private GameSettings m_gameSettings;
    private int numberOfItemInBuffer = 0;

    private BoardController m_boardController;

    public void Setup(BoardController boardController)
    {
        m_boardController = boardController;
    }
    public void CreateBufferCells(GameSettings gameSettings)
    {
        //m_gameSettings = gameSettings;
        float totalWidth = (BUFFER_SIZE - 1) * spacing;
        Vector3 origin = new Vector3(
            -totalWidth * 0.5f,
            -gameSettings.BoardSizeY * 0.5f - 1f, // Đặt phía dưới board
            0f
        );

        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        bufferCells = new Cell[BUFFER_SIZE];

        for (int i = 0; i < BUFFER_SIZE; i++)
        {
            GameObject go = Instantiate(prefabBG);
            go.transform.position = origin + new Vector3(i * spacing, 0, 0f);
            go.transform.SetParent(this.transform);

            Cell cell = go.GetComponent<Cell>();
            cell.Setup(i, -1);
            bufferCells[i] = cell;
        }
    }
    public void AddToBufferCells(Item item)
    {
        if (CheckThreeMatch(item))
        {
            numberOfItemInBuffer -= 2;
            StartCoroutine(DesTroyItem());
        }
        else
        {
            bufferCells[numberOfItemInBuffer].Assign(item);
            numberOfItemInBuffer++;
        }
        //Debug.Log(numberOfItemInBuffer);
        //if(numberOfItemInBuffer == BUFFER_SIZE)
        //{
            //Debug.Log("lose");
            //m_boardController.OnFullBuffer();
            OnBufferSizeChange(numberOfItemInBuffer);
        //}
    }

    IEnumerator DesTroyItem()
    {
        bufferCells[identicalIndexItem[0]].Clear();
        bufferCells[identicalIndexItem[1]].Clear();
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(RemoveFromBufferCells());
    }
    IEnumerator RemoveFromBufferCells()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 3; i++)
        {
            Transform itemTransform = identicalItemsList[i].transform;
            StartCoroutine(ScaleDownItem(itemTransform, 0.1f, i * 0.05f, i));
        }

        StartCoroutine(LineUpItemList());
    }

    IEnumerator ScaleDownItem(Transform itemTransform, float duration, float delay, int id)
    {
        //Debug.Log(id);
        // các item biến mất cách nhau 1 thời gian bằng delay giây
        yield return new WaitForSeconds(delay);

        Vector3 originalScale = itemTransform.localScale;
        float elapsedTime = 0f;

        // Thực hiện thu nhỏ đối tượng
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            itemTransform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        itemTransform.localScale = Vector3.zero;
        Destroy(identicalItemsList[id]);
        
    }


    IEnumerator LineUpItemList()
    {
        yield return new WaitForSeconds(0.3f);
        int startId = 0;

        for (int i = 0; i < BUFFER_SIZE; i++)
        {
            if (bufferCells[i].Item != null)
            {
                if (i != startId)
                {
                    bufferCells[i].Item.View.DOMove(bufferCells[startId].transform.position, 0.1f).SetEase(Ease.InOutQuad);
                    bufferCells[startId].Assign(bufferCells[i].Item);
                    bufferCells[i].Clear();
                }
                startId++;
            }
        }
    }

    public bool CheckThreeMatch(Item item)
    {
        if (numberOfItemInBuffer < 2)
        {
            return false;
        }
        int sameCount = 0;
        for (int i = 0; i < BUFFER_SIZE; i++)
        {
            if(bufferCells[i].Item == null)
            {
                //Debug.Log("null" + i);
                continue;
            }
            if (bufferCells[i].Item.IsSameType(item))
            {
                identicalIndexItem[sameCount] = i;
                identicalItemsList[sameCount] = bufferCells[i].Item.View.gameObject;
                sameCount++;
            }

        }
        //Debug.Log(sameCount);
        if (sameCount == 2)
        {
            identicalItemsList[sameCount] = item.View.gameObject;
            return true;
        }
        return false;
    }

    public Transform GetBufferCellTransform(int id)
    {
        return bufferCells[id].transform;
    }

    public int GetNumberOfItemInBuffer()
    {
        return numberOfItemInBuffer;
    }
}