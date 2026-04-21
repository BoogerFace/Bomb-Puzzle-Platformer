using UnityEngine;
using UnityEngine.InputSystem;

public class MakerController : MonoBehaviour
{
    [SerializeField] private GameObject camPivot;
    [SerializeField] private GameObject camObject;

    private InputActionMap makerInputMap;
    
    private InputAction moveActionZ;
    private InputAction moveActionX;
    private InputAction moveActionY;
    private float moveStep = 4f;
    private float moveValue;
    private Vector3 movePosition = new Vector3(0,0,0);
    
    private InputAction camRotLR;
    private InputAction camRotUD;
    private float camRotValue;
    private Vector3 camPivotRotation = new Vector3(15f,0,0);
    private Vector3 camPosition = new Vector3(0,3f,-15f);
    private InputAction zoomAction;
    private float zoomValue;
    private int zoomLevel = 0;
    private int maxZoomLevel = 1;
    private int minZoomLevel = -2;
    private float zoomStep = 5;

    private float[] pivotRotX = { -90f, -43.5f, -15f, 0, 15, 46.5f, 90f };
    private float[] camPosY = { 0f, 1f, 3f, 2f, 3f, 1f, 0f };
    private float[] camPosZ = { -20f, -17f, -15f, -12f, -15f, -17f, -20f };
    private int currentCamState = 4;
    private int minCamState = 0;
    private int maxCamState = 6;

    //   ^
    //   0  
    // 1-+-3
    //   2
    private int controllerDirection = 0;
    private int minControlDir = 0;
    private int maxControlDir = 3;
    private Vector3 currentZDirection = new Vector3(0,0,1);
    private Vector3 currentXDirection = new Vector3(1,0,0);
    private float pivotRotY = 0f;
    
    private MakerBlockManager blockManager;

    [SerializeField] private GameObject borderDisplay;
    private GameObject borderTop;
    private GameObject borderBottom;
    private GameObject borderLeft;
    private GameObject borderRight;
    private GameObject borderFront;
    private GameObject borderBack;

    public bool canMove = true;
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject makerUI;
    [SerializeField] private GameObject fakePlayer;
    private InputAction startAction;
    public bool isPlaying = false;


    void Start()
    {
        makerInputMap = InputSystem.actions.FindActionMap("LevelMaker");

        moveActionZ = makerInputMap.FindAction("MoveZ");
        moveActionX = makerInputMap.FindAction("MoveX");
        moveActionY = makerInputMap.FindAction("MoveY");

        camRotLR = makerInputMap.FindAction("CamRotLR");
        camRotUD = makerInputMap.FindAction("CamRotUD");
        zoomAction = makerInputMap.FindAction("Zoom");

        startAction = makerInputMap.FindAction("StartLevel");

        // Bind functions to button press
        moveActionZ.performed += OnMove;
        moveActionX.performed += OnMove;
        moveActionY.performed += OnMove;
        camRotLR.performed += OnCamRotate;
        camRotUD.performed += OnCamRotate;
        zoomAction.performed += OnZoom;
        startAction.performed += StartLevel;

        movePosition = transform.localPosition;

        blockManager = transform.GetComponent<MakerBlockManager>();

        // GameObject[] loads = Resources.LoadAll<GameObject>("Blocks");
        // print(Resources.LoadAll<GameObject>("Blocks")[0]);
        // Instantiate(Resources.LoadAll<GameObject>("Blocks")[0], transform.position, transform.rotation);
         
        borderTop = borderDisplay.transform.Find("Top").gameObject;
        borderBottom = borderDisplay.transform.Find("Bottom").gameObject;
        borderLeft = borderDisplay.transform.Find("Left").gameObject;
        borderRight = borderDisplay.transform.Find("Right").gameObject;
        borderFront = borderDisplay.transform.Find("Front").gameObject;
        borderBack = borderDisplay.transform.Find("Back").gameObject;

        CheckBorder();
    }


    // void Update()
    // {
        
    // }
    

    void FixedUpdate()
    {
        // Move cam to pos
        if (camPivot.transform.localPosition != transform.localPosition)
        {
            camPivot.transform.localPosition = Vector3.Lerp(camPivot.transform.localPosition, transform.localPosition, 10f * Time.deltaTime);
        }
        // Snap cam pos when near
        if (Vector3.Distance(camPivot.transform.localPosition, transform.localPosition) <= .01f)
        {
            camPivot.transform.localPosition = transform.localPosition;
        }

        // Rotate cam
        if (camPivot.transform.localRotation != Quaternion.Euler(camPivotRotation))
        {
            camPivot.transform.localRotation = Quaternion.Lerp(camPivot.transform.localRotation, Quaternion.Euler(camPivotRotation), 10f * Time.deltaTime);
        }
        if (camObject.transform.localPosition != camPosition)
        {
            camObject.transform.localPosition = Vector3.Lerp(camObject.transform.localPosition, camPosition, 10f * Time.deltaTime);
        }
        // Snap cam rotation
        if (Quaternion.Angle(camPivot.transform.localRotation, Quaternion.Euler(camPivotRotation)) <= .1f)
        {
            camPivot.transform.localRotation = Quaternion.Euler(camPivotRotation);
        }
        if (Vector3.Distance(camObject.transform.localPosition, camPosition) <= .01f)
        {
            camObject.transform.localPosition = camPosition;
        }
    }


    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (canMove)
        {
            // Move in corressponding direction
            if (ctx.action.name == "MoveZ")
            {
                moveValue = moveActionZ.ReadValue<float>();
                if (!(blockManager.currentGrid.z + (moveValue * currentZDirection.z) < 0 || blockManager.currentGrid.z + (moveValue * currentZDirection.z) > blockManager.worldSize)
                    && !(blockManager.currentGrid.x + (moveValue * currentZDirection.x) < 0 || blockManager.currentGrid.x + (moveValue * currentZDirection.x) > blockManager.worldSize))
                {
                    transform.localPosition += new Vector3(moveValue * moveStep * currentZDirection.x, 0, moveValue * moveStep * currentZDirection.z);
                    blockManager.MoveCurrentGrid(new Vector3((moveValue * currentZDirection.x),0,(moveValue * currentZDirection.z)));
                }
            }
            else if (ctx.action.name == "MoveX")
            {
                moveValue = moveActionX.ReadValue<float>();
                if (!(blockManager.currentGrid.x + (moveValue * currentXDirection.x) < 0 || blockManager.currentGrid.x + (moveValue * currentXDirection.x) > blockManager.worldSize)
                    && !(blockManager.currentGrid.z + (moveValue * currentXDirection.z) < 0 || blockManager.currentGrid.z + (moveValue * currentXDirection.z) > blockManager.worldSize))
                {
                    transform.localPosition += new Vector3(moveValue * moveStep * currentXDirection.x, 0, moveValue * moveStep* currentXDirection.z);
                    blockManager.MoveCurrentGrid(new Vector3((moveValue * currentXDirection.x),0,(moveValue * currentXDirection.z)));
                }
            }
            else if (ctx.action.name == "MoveY")
            {
                moveValue = moveActionY.ReadValue<float>();
                if (!(blockManager.currentGrid.y + moveValue < 0 || blockManager.currentGrid.y + moveValue > blockManager.worldSize))
                {
                    transform.localPosition += new Vector3(0, moveValue * moveStep, 0);
                    blockManager.MoveCurrentGrid(new Vector3(0,moveValue,0));
                }
            }
            
            CheckBorder();
        }
    }


    private void OnCamRotate(InputAction.CallbackContext ctx)
    {
        if (canMove)
        {
            // Rotate camera Left/Right
            if (ctx.action.name == "CamRotLR")
            {
                camRotValue = camRotLR.ReadValue<float>();

                controllerDirection += (int)camRotValue;
                if (controllerDirection > maxControlDir)
                {
                    controllerDirection = minControlDir;
                }
                else if (controllerDirection < minControlDir)
                {
                    controllerDirection = maxControlDir;
                }

                switch(controllerDirection) 
                {
                    case 0:
                        currentZDirection = new Vector3(0,0,1);
                        currentXDirection = new Vector3(1,0,0);
                        break;
                    case 1:
                        currentZDirection = new Vector3(-1,0,0);
                        currentXDirection = new Vector3(0,0,1);
                        break;
                    case 2:
                        currentZDirection = new Vector3(0,0,-1);
                        currentXDirection = new Vector3(-1,0,0);
                        break;
                    case 3:
                        currentZDirection = new Vector3(1,0,0);
                        currentXDirection = new Vector3(0,0,-1);
                        break;
                }

                pivotRotY = 90 * -controllerDirection;
                camPivotRotation = new Vector3(pivotRotX[(int)currentCamState], pivotRotY, 0);
            }
            // Rotate camera Up/Down
            else
            {
                camRotValue = camRotUD.ReadValue<float>();
                if (!(currentCamState + camRotValue > maxCamState || currentCamState + camRotValue < minCamState))
                {
                    currentCamState += (int)camRotValue;

                    camPivotRotation = new Vector3(pivotRotX[(int)currentCamState], pivotRotY, 0);
                    camPosition = new Vector3(0, camPosY[(int)currentCamState], camPosZ[(int)currentCamState] + (zoomLevel * zoomStep));
                }
            }
        }
    }
    
    
    private void OnZoom(InputAction.CallbackContext ctx)
    {
        if (canMove)
        {
            zoomValue = zoomAction.ReadValue<float>();

            if (!(zoomLevel + zoomValue > maxZoomLevel || zoomLevel + zoomValue < minZoomLevel))
            {
                zoomLevel += (int)zoomValue;
                camPosition = new Vector3(0, camPosY[(int)currentCamState], camPosZ[(int)currentCamState] + (zoomLevel * zoomStep));
            }
        }
    }
    
    
    private void CheckBorder()
    {
        if (blockManager.currentGrid.z == 0)
        {
            borderBack.SetActive(true);
        }
        else
        {
            borderBack.SetActive(false);
        }
        
        if (blockManager.currentGrid.z == blockManager.worldSize)
        {
            borderFront.SetActive(true);
        }
        else
        {
            borderFront.SetActive(false);
        }
        
        if (blockManager.currentGrid.x == 0)
        {
            borderLeft.SetActive(true);
        }
        else
        {
            borderLeft.SetActive(false);
        }
        
        if (blockManager.currentGrid.x == blockManager.worldSize)
        {
            borderRight.SetActive(true);
        }
        else
        {
            borderRight.SetActive(false);
        }
        
        if (blockManager.currentGrid.y == 0)
        {
            borderBottom.SetActive(true);
        }
        else
        {
            borderBottom.SetActive(false);
        }
        
        if (blockManager.currentGrid.y == blockManager.worldSize)
        {
            borderTop.SetActive(true);
        }
        else
        {
            borderTop.SetActive(false);
        }
    }

    private void StartLevel(InputAction.CallbackContext ctx)
    {
        player.SetActive(true);
        player.GetComponent<PlayerController>().isLevelMaker = true;
        playerUI.SetActive(true);

        fakePlayer.SetActive(false);
        makerUI.gameObject.SetActive(false);
        camPivot.gameObject.SetActive(false);

        blockManager.TriggerBlocksOnStart();

        makerInputMap.Disable();

        isPlaying = true;
        transform.gameObject.SetActive(false);
    }
}
