using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int rotationIndex { get; private set; }

    public Animator animator;


    [Header("Controls")]
    public float moveDelay = 0.1f;
    public float move2Delay = 0.25f;
    private float moveTime;

    [Header("Steps and Locks")]
    public bool levelIncrement;
    public bool timeIncrement;

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    [Tooltip("A float between 0 - 100")]
    public float timeStepDecreaseRate;

    private float stepTime;
    private float lockTime;
    private int nextIncrement = 10;
    private int leftMoveCounter;
    private int rightMoveCounter;

    public void SetDefaults()
    {
        stepDelay = .6f;
        lockDelay = .25f;
    }
    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        moveTime = Time.time + moveDelay;
        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        rotationIndex = 0;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        if (!CanMove(Vector2Int.down))
        {
            lockTime += Time.deltaTime;
        }

        if (lockTime >= lockDelay)
        {
            Lock();
        }

        if (timeIncrement)
        {
            if (stepDelay > 0.1f)
            {
                float decreaseAmount = Mathf.Clamp(1000 - (timeStepDecreaseRate * 10), 1f,1000f);
                stepDelay -= Time.deltaTime/decreaseAmount;

                stepDelay = Mathf.Clamp(stepDelay, 0.1f, 0.6f);
            }
        }

        if (GameManager.instance.gameOver || !GameManager.instance.gameStarted)
        {
            return;
        }
        //Rotation
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AudioManager.instance.PlayAudio(AudioManager.instance.rotateClick);

            Rotate(-1);
        }else if (Input.GetKeyDown(KeyCode.E))
        {

            AudioManager.instance.PlayAudio(AudioManager.instance.rotateClick);
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow ))
        {
            AudioManager.instance.PlayAudio(AudioManager.instance.rotateClick);
            Rotate(1);
        }


        //Movement
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            //leftMoveCounter = 0;

            if (Time.time >= moveTime)
            {
                if (Move(Vector2Int.left))
                {
                    AudioManager.instance.PlayAudio(AudioManager.instance.gridClick);

                }
                
                if(rightMoveCounter < 2)
                {
                    moveTime = Time.time + move2Delay;
                    rightMoveCounter++;
                }else
                {
                    moveTime = Time.time + moveDelay;
                }

            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            //rightMoveCounter = 0;

            if (Time.time >= moveTime)
            {
                if (Move(Vector2Int.right))
                {
                    AudioManager.instance.PlayAudio(AudioManager.instance.gridClick);
                }
                if (leftMoveCounter < 2)
                {
                    moveTime = Time.time + move2Delay;
                    leftMoveCounter++;
                }
                else
                {
                    moveTime = Time.time + moveDelay;

                }

            }
        }
        else
        {
            leftMoveCounter = 0;
            rightMoveCounter = 0;
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if(Time.time >= moveTime)
            {
                Move(Vector2Int.down);
                moveTime = Time.time + moveDelay;

            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            HoldPiece();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Backspace))
        {
            HardDrop();
        }

        if (Time.time >= stepTime)
        {
            Step();
        }


        board.Set(this);
    }

    void HoldPiece()
    {
        board.HoldPiece();
    }
    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);
    }
    
    private void Lock()
    {

        board.hasSwapped = false;
        board.UpdateHoldPieceGFX();

        Vector3Int tilePosition = cells[0] + position;

        GameObject go = Instantiate(board.pieceSetPrefab, tilePosition, Quaternion.identity);
        Destroy(go, 2f);

        Tetromino tetromino = data.tetromino;

        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();

        
        AudioManager.instance.PlayAudio(AudioManager.instance.pieceDing);
        AudioManager.instance.PlayAudio(AudioManager.instance.pieceThump);

        animator.SetTrigger("Thump");

        if (GameManager.instance.lines >= nextIncrement)
        {
            if (GameManager.instance.level < 20)
            {
                nextIncrement += 10;
                GameManager.instance.level++;

                if (levelIncrement)
                {
                    stepDelay -= .025f;
                }
            }
        }


        switch (tetromino) 
        {
            case Tetromino.I:
                GameManager.instance.score += 16;
                break;
            case Tetromino.O:
                GameManager.instance.score += 22;
                break;
            case Tetromino.T:
                GameManager.instance.score += 18;
                break;
            case Tetromino.J:
                GameManager.instance.score += 20;
                break;
            case Tetromino.L:
                GameManager.instance.score += 19;
                break;
            case Tetromino.S:
                GameManager.instance.score += 23;
                break;
            case Tetromino.Z:
                GameManager.instance.score += 24;
                break;
            default:
                GameManager.instance.score += 20;
                break;
        }

        GameManager.instance.pieces++;
        GameManager.instance.UpdateText();
    }

    private void HardDrop()
    {
        int counter = 0;
        while (Move(Vector2Int.down))
        {
            counter++;
            continue;
        }

        GameManager.instance.score += Mathf.Clamp(counter * 2, 0, 24);
        
        Lock();
    }
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;
            lockTime = 0f;
        }

        return valid;
    }

    private bool CanMove(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        return valid;
    }

    void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }

    }

    private bool TestWallKicks(int rotationIndex, int direction)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, direction);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }
    
    private int GetWallKickIndex(int rotationIndex, int direction)
    {
        int wallKickIndex = rotationIndex * 2;

        if(direction < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }
    private int Wrap(int input, int min, int max)
    {
        if(input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
}
