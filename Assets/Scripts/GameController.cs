using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int cols, rows, mines;
    int[,] board;
    bool[,] flag, visited;

    List<int> minesList, flagsList;

    Board boardUI;
    Camera cam;

    bool isTimerRunning;
    float timeInSecs;

    bool firstClick;

    private float timeTouchBegan;
    private float holdTime = 0.3f;

    int ini_val, r, c, flagsLeft, unrevealedCells;

    [SerializeField] Text timeText, flagsLeftText;

    public bool pauseMode;
    bool isGameOver;

    [SerializeField] Transform finishLabel;

    [SerializeField] GameObject confetti;

    int[] drow = { -1, 0, 1, 0, 1, 1, -1, -1 };
    int[] dcol = { 0, 1, 0, -1, 1, -1, -1, 1 };

    public enum GameMode
    {
        Beginner = 0,
        Intermediate = 1,
        Expert = 2
    }
    public static GameMode currentMode;

    void Start()
    {
        boardUI = FindObjectOfType<Board>();
        cam = Camera.main;

        NewGame();
    }

    void Update()
    {
        if(!pauseMode)
        {
            if (isTimerRunning)
            {
                timeInSecs += Time.deltaTime % 60;
                timeText.text = ((int)timeInSecs).ToString();
            }
            if (!isGameOver)
            {
                HandleInput();
            }
        }
    }

    public void NewGame()
    {
        currentMode = (GameMode)SceneController.GetParameters();
        SetParameters(currentMode);

        // CreateBoard();
        boardUI.CreateBoardUI();

        isGameOver = false;
        firstClick = false;
        pauseMode = false;

        isTimerRunning = true;
        timeInSecs = 0;

        flagsLeft = mines;
        unrevealedCells = cols * rows;
        flagsLeftText.text = flagsLeft.ToString();

        finishLabel.GetChild(0).gameObject.SetActive(false);
        finishLabel.GetChild(1).gameObject.SetActive(false);
        finishLabel.gameObject.SetActive(false);
        confetti.gameObject.SetActive(false);
    }

    void GameOver()
    {
        isGameOver = true;
        isTimerRunning = false;

        finishLabel.gameObject.SetActive(true);
    }

    void GameWon()
    {
        GameOver();
        Debug.Log("Game Won!");
        finishLabel.GetChild(1).gameObject.SetActive(false);
        finishLabel.GetChild(0).gameObject.SetActive(true);
        confetti.gameObject.SetActive(true);
    }

    void GameFailed()
    {
        GameOver();

        minesList.Remove(r * cols + c);
        foreach (int mine in minesList)
        {
            r = mine / cols;
            c = mine % cols;
            if (!flag[r, c])
                boardUI.UpdateCell(r, c, 10);
            else
                flagsList.Remove(mine);
        }
        foreach(int flag in flagsList)
        {
            r = flag / cols;
            c = flag % cols;
            if(!visited[r, c])
                boardUI.UpdateCell(r, c, 11);
        }

        Debug.Log("Game Failed!");

        finishLabel.GetChild(0).gameObject.SetActive(false);
        finishLabel.GetChild(1).gameObject.SetActive(true);
    }

    void HandleInput()
    {
        // Mobile
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            HandleTouch(touch);
        }
            

        // Windows
        //Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        //if ((-(cols / 4f) < pos.x) && (pos.x < (cols / 4f)) && (-(rows / 4f) < pos.y) && (pos.y < (rows / 4f)))
        //{
        //    HandleLeftRightClick(pos);
        //}
    }

    void HandleTouch(Touch touch)
    {
        Vector2 pos = cam.ScreenToWorldPoint(touch.position);
        Debug.Log("Touch: " + pos);
        if ((-(cols / 4f) < pos.x) && (pos.x < (cols / 4f)) && (-(rows / 4f) < pos.y) && (pos.y < (rows / 4f)))
        {
            if(touch.phase == TouchPhase.Began)
            {
                timeTouchBegan = Time.time;
            }
            if (touch.phase == TouchPhase.Ended)
            {
                float dTime = Time.time - timeTouchBegan;
                Debug.Log("Time: " + timeTouchBegan + " " + Time.time);
                if (dTime < holdTime)
                {
                    SingleClick(pos);
                }
                else if (dTime > holdTime)
                {
                    FlagClick(pos);
                }
            }
        }
    }

    void HandleLeftRightClick(Vector2 pos)
    {
        if (Input.GetMouseButtonDown(0))
        {
            SingleClick(pos);
        }
        if (Input.GetMouseButtonDown(1))
        {
            FlagClick(pos);
        }
    }

    void SingleClick(Vector2 pos)
    {
        GetSquareUnderMouse(pos, out r, out c);
        // Debug.Log(r + " " + c);

        if (!firstClick)
        {
            firstClick = true;
            ini_val = r * cols + c;
            Debug.Log("First Click: " + ini_val);
            CreateBoard();
        }

        if (!flag[r, c])
            RevealCell();
    }

    void FlagClick(Vector2 pos)
    {
        GetSquareUnderMouse(pos, out r, out c);
        // Debug.Log(r + " " + c);

        if (!visited[r, c] && !flag[r, c])
        {
            FlagCell();
        }
        else if (flag[r, c])
            UnFlagCell();
    }

    void GetSquareUnderMouse(Vector2 pos, out int r, out int c)
    {
        r = (int)(rows * 0.5f - pos.y * 2f);
        c = (int)(pos.x * 2f + cols * 0.5f);
        // Debug.Log(r + ", " + c);
    }

    void RevealCell()
    {
        if (board[r, c] == -1)
        {
            RevealTheMine();
        }
        else if (board[r, c] == 0 && !visited[r, c])
        {
            RevealTheEmpty();
        }
        else
        {
            RevealTheCell();
        }
        Debug.Log("Final: " + unrevealedCells);
        if (mines == unrevealedCells)
        {
            GameWon();
        }
    }

    void RevealTheMine()
    {
        boardUI.UpdateCell(r, c, board[r, c]);

        GameFailed();
        
    }

    void RevealTheEmpty() {
        var stack = new Stack<KeyValuePair<int, int>>();
        stack.Push(new KeyValuePair<int, int>(r, c));

        while (stack.Count > 0)
        {
            KeyValuePair<int, int> curr = stack.Pop();

            r = curr.Key;
            c = curr.Value;
            
            if (!visited[r, c])
            {
                boardUI.UpdateCell(r, c, board[r, c]);
                visited[r, c] = true;
                unrevealedCells--;
                Debug.Log(r + ", " + c + " revealed " + board[r, c] + " Empty: " + unrevealedCells);
            }
            BFS(stack, r, c, drow, dcol);
        }
    }

    void RevealTheCell()
    {
        if (!visited[r, c])
        {
            boardUI.UpdateCell(r, c, board[r, c]);
            visited[r, c] = true;
            unrevealedCells--;
            Debug.Log(r + ", " + c + " revealed " + board[r, c] + " Others: " + unrevealedCells);
        }
    }

    void FlagCell()
    {
        flag[r, c] = true;
        if (flagsLeft > 0)
        {
            flagsList.Add(r * cols + c);

            flagsLeft--;
            flagsLeftText.text = flagsLeft.ToString();
        }
        boardUI.UpdateCell(r, c, 9);
        Debug.Log(r + ", " + c + " Flagged");
    }

    void UnFlagCell()
    {
        flag[r, c] = false;
        flagsLeft++;
        flagsLeftText.text = flagsLeft.ToString();

        boardUI.UpdateCell(r, c, 12);
        Debug.Log(r + ", " + c + " UnFlagged");
    }

    void BFS(Stack<KeyValuePair<int, int>> stack, int row, int col, int[] drow, int[] dcol)
    {
        // Debug.Log(row + "," + col);

        int nrow, ncol;

        for (int i = 0; i < 8; i++)
        {
            nrow = row + drow[i];
            ncol = col + dcol[i];

            if ((nrow >= 0 && nrow < rows) && (ncol >= 0 && ncol < cols) && visited[nrow, ncol] == false)
            {
                if (board[nrow, ncol] == 0 && !stack.Contains(new KeyValuePair<int, int>(nrow, ncol)))
                {
                    stack.Push(new KeyValuePair<int, int>(nrow, ncol));
                    Debug.Log(nrow + ", " + ncol + " pushed");
                }
                else
                {
                    if (!visited[nrow, ncol])
                    {
                        boardUI.UpdateCell(nrow, ncol, board[nrow, ncol]);
                        visited[nrow, ncol] = true;
                        unrevealedCells--;
                        Debug.Log(nrow + ", " + ncol + " revealed " + board[nrow, ncol] + " In BFS: " + unrevealedCells);
                    }
                }
            }
        }
    }

    void CreateBoard()
    {
        board = new int[rows, cols];
        flag = new bool[rows, cols];
        visited = new bool[rows, cols];
        // Debug.Log(cols + " " + rows);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                board[i, j] = 0;
                flag[i, j] = false;
                visited[i, j] = false;
            }
        }

        GenerateMines();
        
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (board[i, j] != -1)
                {
                    board[i, j] = CalculateNeighbours(i, j);
                }
            }
        }

        flagsList = new List<int>();
    }

    void GenerateMines()
    {
        minesList = new List<int>();
        minesList.Add(Random.Range(0, (cols * rows) - 1));
        while (minesList.Count != mines)
        {
            int mine = Random.Range(0, (cols * rows) - 1);
            if (!minesList.Contains(mine) && mine != ini_val)
            {
                minesList.Add(mine);
            }
        }
        foreach (int mine in minesList)
        {
            //Debug.Log(mine.ToString() + ", " + cols.ToString() + " :" + (mine / cols).ToString() + " " + (mine % cols).ToString());
            board[mine / cols, mine % cols] = -1;
            // cells[mine / x, mine % x].transform.GetChild(0).GetComponent<Text>().text = "-1";
        }
    }

    int CalculateNeighbours(int row, int col)
    {
        int neighbours = 0;

        for (int i = 0; i < 8; i++)
        {
            int nrow = row + drow[i];
            int ncol = col + dcol[i];
            if ((nrow >= 0 && nrow < rows) && (ncol >= 0 && ncol < cols) && board[nrow, ncol] == -1)
                neighbours += 1;
        }
        return neighbours;
    }

    void DisplayBoard()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Debug.Log(board[i, j].ToString() + " ");
            }
            Debug.Log("\n");
        }
    }

    public void SetParameters(GameMode mode)
    {
        currentMode = mode;
        switch (mode)
        {
            case GameMode.Beginner:
                cols = 8;
                rows = 8;
                mines = 10;
                break;
            case GameMode.Intermediate:
                cols = 16;
                rows = 16;
                mines = 40;
                break;
            case GameMode.Expert:
                cols = 30;
                rows = 16;
                mines = 99;
                break;
        }
        boardUI.height = rows / 2;
        boardUI.width = cols / 2;
        flagsLeft = mines;
        unrevealedCells = cols * rows;
    }

    public void Pause()
    {
        pauseMode = true;
    }

    public void UnPause()
    {
        pauseMode = false;
    }
}
