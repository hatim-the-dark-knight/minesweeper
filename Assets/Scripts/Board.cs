using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [HideInInspector] public int width, height;
    int rows, cols;

    SpriteRenderer background;
    [SerializeField] Sprite squareSprite;

    GameObject cells;
    SpriteRenderer[,] cellRenderers;

    public CellTheme cellTheme;

    public void CreateBoardUI()
    {
        rows = 2 * width;
        cols = 2 * height;
        CreateBackground();
        SpawnCells();
    }

    void CreateBackground()
    {
        if (transform.childCount != 0)
            Destroy(cells.transform.gameObject);
        cells = new GameObject("Cells");
        cells.transform.parent = transform;

        background = new GameObject("Background").AddComponent<SpriteRenderer>();
        background.transform.parent = cells.transform;
        background.sprite = squareSprite;
        background.transform.position = new Vector3(0, 0, 1);
        background.transform.localScale = new Vector3(width, height, 1);
        // Debug.Log(width + " " + height);
    }

    void SpawnCells()
    {
        // Debug.Log(rows + " " + cols);
        cellRenderers = new SpriteRenderer[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < cols; j++)
            {
                string name = i.ToString() + " " + j.ToString();

                // Create piece sprite renderer for current square
                SpriteRenderer cellRenderer = new GameObject(name).AddComponent<SpriteRenderer>();
                cellRenderer.transform.parent = cells.transform;
                cellRenderer.transform.position = new Vector3((0.5f * i) - (width - 0.5f) / 2f, -(0.5f * j) + (height - 0.5f) / 2f - 0f, 0);
                cellRenderer.transform.localScale = Vector3.one * 0.25f;
                cellRenderer.sprite = cellTheme.GetCellSprite(12);
                cellRenderer.gameObject.AddComponent<Cell>();
                cellRenderers[i, j] = cellRenderer;
            }
        }
    }

    public void UpdateCell(int r, int c, int ind)
    {
        cellRenderers[c, r].sprite = cellTheme.GetCellSprite(ind);
    }
}
