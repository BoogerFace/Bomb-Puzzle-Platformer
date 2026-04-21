using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Linq;

public class MakerBlockManager : MonoBehaviour
{
    public int worldSize = 256;
    private GameObject[,,] blockGrid = null;
    public Vector3 currentGrid = new Vector3(0,0,0);
    // private int blockSize = 4;

    private InputActionMap makerInputMap;
    private InputAction confirmAction;
    private InputAction backAction;

    [SerializeField] private GameObject world;
    [SerializeField] private GameObject blockSpawnPivot;
    [SerializeField] private GameObject tempBlockToSpawn;
    
    public GameObject currentBlock;
    
    private bool isPlacing = false;
    private bool isDeleting = false;
    
    private InputAction rotateX;
    private InputAction rotateY;
    private InputAction rotateZ;
    private float angleX = 0;
    private float angleY = 0;
    private float angleZ = 0;
    private Quaternion blockRotation;
    private bool isRotating = false;
    
    [SerializeField] private GameObject rotateDisplayX;
    [SerializeField] private GameObject rotateDisplayY;
    [SerializeField] private GameObject rotateDisplayZ;
    
    [SerializeField] private GameObject[] displayLines;
    
    private bool isMovingUI = false;

    private InputAction tabAction;
    private bool isInventoryOpen = false;
    private bool isMovingInv = false;

    [Header("Inventory Panel")]
    [SerializeField] private RectTransform inventoryPanel;
    private Vector3 invStartPos;
    private Vector3 invEndPos;
    [SerializeField] private Vector3 invPosChange;
    
    private InputAction enterAction;
    private bool isPropertyOpen = false;
    private bool isMovingProp = false;
    private bool hasProps = false;

    [Header("Properties Panel")]
    [SerializeField] private RectTransform propertyPanel;
    private Vector3 propStartPos;
    private Vector3 propEndPos;
    [SerializeField] private Vector3 propPosChange;

    private MakerController makerController;
    private bool canMove = true;
    private MakerInventoryManager inventoryManager;
    
    [SerializeField] private string componentTags;
    [SerializeField] private LayerMask componentLayers;


    void Start()
    {
        blockGrid = new GameObject[worldSize,worldSize,worldSize];

        makerController = transform.GetComponent<MakerController>();
        inventoryManager = transform.GetComponent<MakerInventoryManager>();

        makerInputMap = InputSystem.actions.FindActionMap("LevelMaker");
        confirmAction = makerInputMap.FindAction("Confirm");
        backAction = makerInputMap.FindAction("Back");
        rotateX = makerInputMap.FindAction("RotateX");
        rotateY = makerInputMap.FindAction("RotateY");
        rotateZ = makerInputMap.FindAction("RotateZ");
        tabAction = makerInputMap.FindAction("Tab");
        enterAction = makerInputMap.FindAction("Enter");

        // Bind functions to button press
        confirmAction.performed += PlaceBlock;
        backAction.performed += DeleteBlock;
        rotateX.performed += RotateBlock;
        rotateY.performed += RotateBlock;
        rotateZ.performed += RotateBlock;
        tabAction.performed += ToggleInventory;
        enterAction.performed += ToggleProperties;

        InstantiateCurrentBlock(tempBlockToSpawn);

        invStartPos = inventoryPanel.localPosition;
        invEndPos = inventoryPanel.localPosition + invPosChange;

        propStartPos = propertyPanel.localPosition;
        propEndPos = propertyPanel.localPosition + propPosChange;
    }


    void Update()
    {
        if (confirmAction.WasPressedThisFrame() && canMove)
        {
            isPlacing = true;
            isDeleting = false;
            ChangeBlockDisplayColour(Color.green);
        }
        else if (confirmAction.WasReleasedThisFrame() && canMove)
        {
            if (isPlacing)
            {
                ChangeBlockDisplayColour(Color.white);
            }
            isPlacing = false;
        }

        if (backAction.WasPressedThisFrame() && canMove)
        {
            isDeleting = true;
            isPlacing = false;
            ChangeBlockDisplayColour(Color.red);
        }
        else if (backAction.WasReleasedThisFrame() && canMove)
        {
            if (isDeleting)
            {
                ChangeBlockDisplayColour(Color.white);
            }
            isDeleting = false;
        }
    }
    

    void FixedUpdate()
    {
        // Rotate block
        if (blockSpawnPivot.transform.rotation != blockRotation && isRotating)
        {
            blockSpawnPivot.transform.rotation = Quaternion.Lerp(blockSpawnPivot.transform.rotation, blockRotation, 20f * Time.deltaTime);
        }
        // Stop & snap block rotation
        if (Quaternion.Angle(blockSpawnPivot.transform.rotation, blockRotation) <= .1f && isRotating)
        {
            isRotating = false;
            blockSpawnPivot.transform.rotation = blockRotation;
            rotateDisplayX.SetActive(false);
            rotateDisplayY.SetActive(false);
            rotateDisplayZ.SetActive(false);
        }

        // Open/close UI
        if (isMovingUI)
        {
            // Inventory Panel
            if (isMovingInv)
            {
                // Inventory Lerp to Open
                if (isInventoryOpen && inventoryPanel.localPosition != invEndPos)
                {
                    inventoryPanel.localPosition = Vector3.Lerp(inventoryPanel.localPosition, invEndPos, 10f * Time.deltaTime);
                }
                // Snap pos when near
                if (isInventoryOpen && Vector3.Distance(inventoryPanel.localPosition, invEndPos) <= Vector3.Distance(invStartPos, invEndPos) * .01f)
                {
                    inventoryPanel.localPosition = invEndPos;
                    isMovingUI = false;
                    isMovingInv = false;
                }
                // Inventory Lerp to Close
                if (!isInventoryOpen && inventoryPanel.localPosition != invStartPos)
                {
                    inventoryPanel.localPosition = Vector3.Lerp(inventoryPanel.localPosition, invStartPos, 10f * Time.deltaTime);
                }
                // Snap pos when near
                if (!isInventoryOpen && Vector3.Distance(inventoryPanel.localPosition, invStartPos) <= Vector3.Distance(invStartPos, invEndPos) * .01f)
                {
                    inventoryPanel.localPosition = invStartPos;
                    isMovingUI = false;
                    isMovingInv = false;
                }
            }
            
            // Properties Panel
            if (isMovingProp)
            {
                // Properties Lerp to Open
                if (isPropertyOpen && propertyPanel.localPosition != propEndPos)
                {
                    propertyPanel.localPosition = Vector3.Lerp(propertyPanel.localPosition, propEndPos, 10f * Time.deltaTime);
                }
                // Snap pos when near
                if (isPropertyOpen && Vector3.Distance(propertyPanel.localPosition, propEndPos) <= Vector3.Distance(propStartPos, propEndPos) * .01f)
                {
                    propertyPanel.localPosition = propEndPos;
                    isMovingUI = false;
                    isMovingProp = false;
                }
                // Properties Lerp to Close
                if (!isPropertyOpen && propertyPanel.localPosition != propStartPos)
                {
                    propertyPanel.localPosition = Vector3.Lerp(propertyPanel.localPosition, propStartPos, 10f * Time.deltaTime);
                }
                // Snap pos when near
                if (!isPropertyOpen && Vector3.Distance(propertyPanel.localPosition, propStartPos) <= Vector3.Distance(propStartPos, propEndPos) * .01f)
                {
                    propertyPanel.localPosition = propStartPos;
                    isMovingUI = false;
                    isMovingProp = false;
                }
            }
        }
    }


    public void MoveCurrentGrid(Vector3 direction)
    {
        currentGrid += direction;

        if (confirmAction.IsPressed() && isPlacing && !isDeleting)
        {
            PlaceBlock();
        }

        if (backAction.IsPressed() && !isPlacing && isDeleting)
        {
            DeleteBlock();
        }
        
        CheckIfComponent();
    }


    public void InstantiateCurrentBlock(GameObject block)
    {
        if (currentBlock != null)
        {
            Destroy(currentBlock);
        }

        if (block != null)
        {
            currentBlock = Instantiate(block, blockSpawnPivot.transform, false);
            currentBlock.transform.localPosition = new Vector3(0,0,0);
            currentBlock.transform.localRotation = Quaternion.Euler(new Vector3(0,0,0));
        }
    }


    private void PlaceBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            PlaceBlock();
        }
    }

    
    public void PlaceBlock()
    {
        if (!isRotating && canMove)
        {
            isPlacing = true;
            isDeleting = false;

            if (currentBlock != null)
            {
                if (blockGrid[(int)currentGrid.x,(int)currentGrid.y,(int)currentGrid.z] != null)
                {
                    DeleteBlock();
                }

                currentBlock.name = currentGrid.ToString();
                currentBlock.transform.SetParent(world.transform, true);
                currentBlock.transform.localRotation = blockSpawnPivot.transform.localRotation;
                blockGrid[(int)currentGrid.x,(int)currentGrid.y,(int)currentGrid.z] = currentBlock;
                print("Placed block at " + currentGrid);

                GameObject replaceBlock = currentBlock;
                currentBlock = null;
                InstantiateCurrentBlock(replaceBlock);
            }
        }
    }


    private void DeleteBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            DeleteBlock();
        }
    }

    
    public void DeleteBlock()
    {
        if (!isRotating && canMove)
        {
            isPlacing = false;
            isDeleting = true;

            if (blockGrid[(int)currentGrid.x,(int)currentGrid.y,(int)currentGrid.z] != null)
            {
                GameObject blockToDelete = blockGrid[(int)currentGrid.x,(int)currentGrid.y,(int)currentGrid.z];
                Destroy(blockToDelete);
                blockGrid[(int)currentGrid.x,(int)currentGrid.y,(int)currentGrid.z] = null;
                
                print("Deleted block at " + currentGrid);
            }
            else
            {
                print("No block in grid to delete");
            }
        }
    }


    // private void RotateBlock(InputAction.CallbackContext ctx)
    // {
    //     if (ctx.performed)
    //     {
    //         RotateBlock();
    //     }
    // }

    
    public void RotateBlock(InputAction.CallbackContext ctx)
    {
        if (!isRotating && canMove)
        {
            isRotating = true;

            // 1. Define the axis (e.g., the reference's Local Y)
            Vector3 customAxis = new Vector3(0,0,0);

            if (ctx.action.name == "RotateX")
            {
                customAxis = world.transform.right;
                angleX += 1;
                angleX = angleX % 4;
                rotateDisplayX.SetActive(true);
            }
            else if (ctx.action.name == "RotateY")
            {
                customAxis = world.transform.up;
                angleY += 1;
                angleY = angleY % 4;
                rotateDisplayY.SetActive(true);
            }
            else if (ctx.action.name == "RotateZ")
            {
                customAxis = world.transform.forward;
                angleZ += 1;
                angleZ = angleZ % 4;
                rotateDisplayZ.SetActive(true);
            }

            // 2. Create an offset rotation around that specific axis
            Quaternion offset = Quaternion.AngleAxis(90f, customAxis);

            // 3. Apply that offset to our current world rotation
            blockRotation = offset * blockSpawnPivot.transform.rotation;

            // "blockSpawnPivot.transform.rotation" will rotate by "offset" (90 degrees around world x/y/z)

        }
    }

    
    private void ChangeBlockDisplayColour(Color newColour)
    {
        newColour.a = .5f;
        foreach (GameObject line in displayLines) 
        {
            LineRenderer lr = line.GetComponent<LineRenderer>();

            lr.startColor = newColour;
            lr.endColor = newColour;
        }
    }


    private void ToggleInventory(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ToggleInventory();
        }
    }

    
    public void ToggleInventory()
    {
        if (!isMovingUI && !isPropertyOpen)
        {
            canMove = !canMove;
            inventoryManager.isOpen = !inventoryManager.isOpen;
            makerController.canMove = !makerController.canMove;

            isInventoryOpen = !isInventoryOpen;
            print("Inventory is "+isInventoryOpen);

            isMovingUI = true;
            isMovingInv = true;
            
            isDeleting = false;
            isPlacing = false;
            ChangeBlockDisplayColour(Color.white);
        }
    }


    private void ToggleProperties(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ToggleProperties();
        }
    }

    
    public void ToggleProperties()
    {
        if (!isMovingUI && !isInventoryOpen && hasProps)
        {
            isPropertyOpen = !isPropertyOpen;
            print("Property is "+isPropertyOpen);

            isMovingUI = true;
            isMovingProp = true;
        }
    }

    
    private void CheckIfComponent()
    {
        GameObject block = blockGrid[(int)currentGrid.x,(int)currentGrid.y,(int)currentGrid.z];
        if (block != null)
        {
            if (block.CompareTag(componentTags) || (componentLayers.value & (1 << block.layer)) != 0)
            {
                hasProps = true;
                propertyPanel.gameObject.SetActive(true);
            }
            else
            {
                hasProps = false;
                propertyPanel.gameObject.SetActive(false);
            }
        }
    }


    private void OnEnable()
    {
        if (blockGrid != null)
        {
            TriggerBlocksOnReset();
        }
    }

    
    private void TriggerBlocksOnReset()
    {
        foreach (GameObject block in blockGrid)
        {
            if (block != null)
            {
                block.SendMessage("OnLevelMakerReset", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    
    public void TriggerBlocksOnStart()
    {
        foreach (GameObject block in blockGrid)
        {
            if (block != null)
            {
                block.SendMessage("OnLevelMakerStart", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
