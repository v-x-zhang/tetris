using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public GameObject pieceSetPrefab;
    public GameObject lineClearPrefab;

    int lastPiece = -1;
    public int nextPiece = -1;
    int currentPiece;
    public int currentHeldPiece = -1;
    public bool hasSwapped;

    public GameObject[] previewSprites;
    public GameObject[] heldSprites;
    public GameObject[] ghostHeldSprites;
    public RectInt bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponent<Piece>();
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();

    }

    public void SpawnPiece(int setPiece = -1)
    {
        TetrominoData data = new TetrominoData();

        if(setPiece == -1)
        {
            if (nextPiece == -1) //Normal Locked Next Piece
            {
                int randomInt = Random.Range(0, tetrominoes.Length);

                nextPiece = Random.Range(0, tetrominoes.Length);
                if (nextPiece == randomInt)
                {
                    nextPiece = Random.Range(0, tetrominoes.Length);
                }
                data = tetrominoes[randomInt];

                currentPiece = randomInt;

                for (int i = 0; i < previewSprites.Length; i++)
                {
                    if (i == nextPiece)
                    {
                        previewSprites[i].SetActive(true);
                    }
                    else
                    {
                        previewSprites[i].SetActive(false);
                    }
                }
            }
            else
            {

                data = tetrominoes[nextPiece];
                currentPiece = nextPiece;

                nextPiece = Random.Range(0, tetrominoes.Length);
                if (lastPiece == nextPiece)
                {
                    nextPiece = Random.Range(0, tetrominoes.Length);
                }


                for (int i = 0; i < previewSprites.Length; i++)
                {
                    if (i == nextPiece)
                    {
                        previewSprites[i].SetActive(true);
                    }
                    else
                    {
                        previewSprites[i].SetActive(false);
                    }
                }
            }

            lastPiece = nextPiece;

            activePiece.Initialize(this, spawnPosition, data);


            if (IsValidPosition(activePiece, spawnPosition))
            {
                Set(activePiece);
            }
            else
            {
                GameOver();
            }
        }
        else //Switch Held Piece
        {
            data = tetrominoes[setPiece];

            activePiece.Initialize(this, spawnPosition, data);

            if (IsValidPosition(activePiece, spawnPosition))
            {
                Set(activePiece);
            }
            else
            {
                GameOver();
            }
        }

    }

    public void UpdateHoldPieceGFX()
    {
        if (hasSwapped)
        {
            foreach (GameObject heldSprite in heldSprites)
            {
                heldSprite.SetActive(false);
            }
            for (int i = 0; i < ghostHeldSprites.Length; i++)
            {
                if (i == currentHeldPiece)
                {
                    ghostHeldSprites[i].SetActive(true);
                }
                else
                {
                    ghostHeldSprites[i].SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < heldSprites.Length; i++)
            {
                if (i == currentHeldPiece)
                {
                    heldSprites[i].SetActive(true);
                }
                else
                {
                    heldSprites[i].SetActive(false);
                }
            }
        }

    }
    public void HoldPiece()
    {
        if(hasSwapped) { return; }
      
        hasSwapped = true;


        if (currentHeldPiece == -1)
        {
            currentHeldPiece = currentPiece;
            SpawnPiece();

        }
        else
        {
            int currentPieceTemp = currentPiece;
            SpawnPiece(currentHeldPiece);
            currentPiece = currentHeldPiece;
            currentHeldPiece = currentPieceTemp;
        }

        UpdateHoldPieceGFX();
    }
    void GameOver()
    {
        GameManager.instance.GameOver();
    }
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);

        }
    }

    public void Clear (Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int newPosition)
    {
        RectInt bounds = this.bounds;
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + newPosition;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition)) 
            {
                return false;
            } 
        }

        return true;
    }

    public void ClearLines()
    {
        int lineCounter = 0;
        RectInt bounds = this.bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                lineCounter++;
            }
            else
            {

                row++;
            }
        }

        GameManager.instance.LineClearScore(lineCounter);
    }

    private bool IsLineFull(int row)
    {
        RectInt bounds = this.bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }


        return true;
    }

    private void LineClear(int row)
    {
        
        RectInt bounds = this.bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);

            GameObject go = Instantiate(lineClearPrefab, position, Quaternion.identity);
            Destroy(go, 2f);
        }

        while(row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);


                
            }

            row++;
        }
    }
}
