using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.InputSystem;
using static System.Enum;
using System.Collections.Generic;
using System.Linq;

public class MakerInventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject invPanel;
    
    private Vector2 invSize = new Vector2(1560, 720);
    private int invRows = 5;
    private int invCols = 8;
    private int currentRow = 1;
    private int currentCol = 1;
    private float cellSizeX;
    private float cellSizeY;
    
    private Object[] iconList = null;

    enum ColourOptions
    {
        blue,
        green,
        red,
        yellow
    }
    private ColourOptions currentColour = ColourOptions.blue;
    private static Color32 colourBlue = new Color32(0,126,189,255);
    private static Color32 colourGreen = new Color32(0,132,84,255);
    private static Color32 colourRed = new Color32(210,34,39,255);
    private static Color32 colourYellow = new Color32(249,170,74,255);
    private Color32[] colourList = {colourBlue,colourGreen,colourRed,colourYellow};
    [SerializeField] private Image colourDisplay;
    [SerializeField] private GameObject colourSelectionDisplay;

    [SerializeField] private Toggle[] toggleList;
    
    private List<GameObject> invCells = new List<GameObject>();
    [SerializeField] private GameObject cellPrefab;
    
    private MakerController makerController;
    
    public bool isOpen = false;

    enum BlockCategories
    {
        Colour,
        Neutral,
        Prototype,
        Components
    }
    private BlockCategories currentCategory;
    private Dictionary<BlockCategories, Object[]> blockListDict = new Dictionary<BlockCategories, Object[]>
    {
        [BlockCategories.Colour] = null,
        [BlockCategories.Neutral] = null,
        [BlockCategories.Prototype] = null,
        [BlockCategories.Components] = null
    };

    private InputActionMap makerInputMap;
    private InputAction actionUD;
    private InputAction actionLR;
    private InputAction actionCat;
    private InputAction actionCol;
    private InputAction actionConfirm;
    private InputAction actionHotbar;
    
    private int currentCell = 0;
    
    private int currentHotbar = 0;
    [SerializeField] private GameObject hotbarObject;
    private List<GameObject> hotbarCells = new List<GameObject>();

    private (BlockCategories Category, ColourOptions Colour, int Index)[]
    hotbarBlocks = new (BlockCategories Category, ColourOptions Colour, int Index)[9];
    
    private MakerBlockManager blockManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blockListDict[BlockCategories.Colour] = Resources.LoadAll("Blocks/BlocksColour", typeof(GameObject));
        blockListDict[BlockCategories.Neutral] = Resources.LoadAll("Blocks/BlocksNeutral", typeof(GameObject));
        blockListDict[BlockCategories.Prototype] = Resources.LoadAll("Blocks/BlocksPrototype", typeof(GameObject));
        blockListDict[BlockCategories.Components] = Resources.LoadAll("Blocks/BlocksComponents", typeof(GameObject));

        iconList = Resources.LoadAll("Textures/BlockIcons", typeof(Texture2D));

        makerInputMap = InputSystem.actions.FindActionMap("LevelMaker");
        actionUD = makerInputMap.FindAction("UI-UpDown");
        actionLR = makerInputMap.FindAction("UI-LeftRight");
        actionCat = makerInputMap.FindAction("UI-Category");
        actionCol = makerInputMap.FindAction("UI-Colour");
        actionHotbar = makerInputMap.FindAction("Hotbar");
        actionConfirm = makerInputMap.FindAction("Confirm");
        
        actionUD.performed += MoveSelectionY;
        actionLR.performed += MoveSelectionX;
        actionCat.performed += NextCategory;
        actionCol.performed += SwitchColour;
        actionConfirm.performed += AddToHotbar;
        actionHotbar.performed += SelectHotbar;

        blockManager = transform.GetComponent<MakerBlockManager>();

        foreach (Transform child in hotbarObject.transform)
        {
            hotbarCells.Add(child.gameObject);
        }
        // SelectHotbar(currentHotbar);
        
        cellSizeX = invSize.x / invCols;
        cellSizeY = invSize.y / invRows;

        for (int i = 0; i < (invRows*invCols); i++)
        {
            GameObject newCell = Instantiate(cellPrefab, invPanel.transform, false);
            newCell.name = "InvCell("+currentRow+","+currentCol+")";
            newCell.GetComponent<Toggle>().group = invPanel.GetComponent<ToggleGroup>();
            newCell.transform.localPosition = new Vector2(cellSizeX * currentCol - (cellSizeX / 2), cellSizeY * -currentRow + (cellSizeY / 2));
            newCell.GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                UpdateSelection(newCell.GetComponent<Toggle>());
            });
            invCells.Add(newCell);
                        
            currentCol += 1;
            if (currentCol > invCols)
            {
                currentCol = 1;
                currentRow += 1;
            }
        }
        invCells[0].GetComponent<Toggle>().isOn = true;
        UpdateCells(blockListDict[BlockCategories.Colour]);
    }


    // Update is called once per frame
    // void Update()
    // {
        
    // }
    
    
    private void UpdateCells(Object[] blockList)
    {
        for (int i = 0; i < invCells.Count; i++)
        {
            GameObject cell = invCells[i];
            
            if (i < blockList.Length)
            {
                cell.SetActive(true);

                GameObject block = blockList[i] as GameObject;
                string iconName;

                if (currentCategory == BlockCategories.Colour)
                {
                    iconName = block.name + "_" + currentColour.ToString();
                }
                else
                {
                    iconName = block.name;
                }

                Texture2D iconTex;
                foreach (Texture2D icon in iconList)
                {
                    if (icon.name == iconName)
                    {
                        iconTex = icon;
                        cell.transform.Find("Icon").GetComponent<RawImage>().texture = iconTex;
                        break;
                    }
                }
            }
            else
            {
                cell.SetActive(false);
            }
        }
    }
    
    
    public void ChangeCategory(Toggle m_toggle)
    {
        if (isOpen)
        {
            print(m_toggle.name + " switched to " + m_toggle.isOn);
            if (m_toggle.isOn) // When switched TO a category
            {
                if (currentCategory.ToString() != m_toggle.name)
                {
                    currentCategory = (BlockCategories) System.Enum.Parse(typeof(BlockCategories), m_toggle.name);
                    print("Current Category is " + currentCategory);

                    UpdateCells(blockListDict[currentCategory]);

                    if (m_toggle.name == "Colour")
                    {
                        colourSelectionDisplay.SetActive(true);
                    }
                    else
                    {
                        colourSelectionDisplay.SetActive(false);
                    }

                    while (!invCells[currentCell].activeSelf)
                    {
                        currentCell -= 1;
                    }

                    Toggle cellToggle = invCells[currentCell].GetComponent<Toggle>();
                    cellToggle.isOn = true;
                }
            }
            else // When switched FROM a category
            {
                
            }
        }
    }
    
    
    private void NextCategory(InputAction.CallbackContext ctx)
    {
        if (isOpen)
        {
            int value = (int)actionCat.ReadValue<float>();
            NextCategory(value);
        }
    }
    
    
    public void NextCategory(int value)
    {
        if (isOpen)
        {
            BlockCategories nextCat = currentCategory+value;
            
            nextCat = (BlockCategories) ((int)nextCat % GetValues(typeof(BlockCategories)).Length);
            if (nextCat < 0) nextCat += GetValues(typeof(BlockCategories)).Length;

            // toggleList[(int)(currentCategory)].isOn = false;
            toggleList[(int)(nextCat)].isOn = true;
        }
    }
    
    
    private void SwitchColour(InputAction.CallbackContext ctx)
    {
        if (isOpen && currentCategory == BlockCategories.Colour)
        {
            int value = (int)actionCol.ReadValue<float>();
            SwitchColour(value);
        }
    }
    
    
    public void SwitchColour(int value)
    {
        if (isOpen && currentCategory == BlockCategories.Colour)
        {
            currentColour += value;

            currentColour = (ColourOptions) ((int)currentColour % GetValues(typeof(ColourOptions)).Length);
            if (currentColour < 0) currentColour += GetValues(typeof(ColourOptions)).Length;
            
            colourDisplay.color = colourList[(int)currentColour];
            
            print(currentColour);

            UpdateCells(blockListDict[BlockCategories.Colour]);
        }
    }
    
    
    private void MoveSelectionX(InputAction.CallbackContext ctx)
    {
        if (isOpen)
        {
            int value = (int)actionLR.ReadValue<float>();

            currentCell += value;

            currentCell = currentCell % (invRows*invCols);

            // If go left from start, move to last active cell
            if (currentCell < 0)
            {
                currentCell += (invRows*invCols);

                while (!invCells[currentCell].activeSelf)
                {
                    currentCell -= 1;
                }
            }
            // If go right to non active cell, go back to start
            else if (!invCells[currentCell].activeSelf) currentCell = 0;

            Toggle cellToggle = invCells[currentCell].GetComponent<Toggle>();
            cellToggle.isOn = true;
        }
    }
    
    
    private void MoveSelectionY(InputAction.CallbackContext ctx)
    {
        if (isOpen)
        {
            int value = (int)actionUD.ReadValue<float>();

            currentCell += value * invCols;

            currentCell = currentCell % (invRows*invCols);

            // If go up, move to bottom active cell
            if (currentCell < 0)
            {
                currentCell += (invRows*invCols);

                while (!invCells[currentCell].activeSelf)
                {
                    currentCell -= invCols;
                }
            }
            // If go down to non active cell, go to top cell
            else if (!invCells[currentCell].activeSelf) currentCell = currentCell % invCols;

            Toggle cellToggle = invCells[currentCell].GetComponent<Toggle>();
            cellToggle.isOn = true;
        }
    }
    
    
    public void UpdateSelection(Toggle m_toggle)
    {
        if (isOpen)
        {
            if (m_toggle.isOn) // When switched TO a cell
            {
                currentCell = invCells.IndexOf(m_toggle.gameObject);
                print(currentCell);
            }
        }
    }
    
    
    private void SelectHotbar(InputAction.CallbackContext ctx)
    {
        int slotIndex = (int)(actionHotbar.ReadValue<float>()) - 1;

        GameObject cell = hotbarCells[slotIndex];
        Toggle cellToggle = cell.GetComponent<Toggle>();
        cellToggle.isOn = true;
    }


    public void UpdateHotbar(Toggle m_toggle)
    {
        if (m_toggle.isOn) // When switched TO a slot
        {
            // untoggle previous slot 
            GameObject cell = hotbarCells[currentHotbar];
            Toggle cellToggle = cell.GetComponent<Toggle>();
            cell.transform.localPosition = new Vector3(cell.transform.localPosition.x, 64, cell.transform.localPosition.z);

            // update to current slot
            int.TryParse(m_toggle.name, out int slot);
            currentHotbar = slot;
            // toggle current selected slot
            cell = hotbarCells[currentHotbar];
            cellToggle = cell.GetComponent<Toggle>();
            cell.transform.localPosition = new Vector3(cell.transform.localPosition.x, 90, cell.transform.localPosition.z);
            
            if (cell.transform.Find("Icon").GetComponent<RawImage>().enabled)
            {
                (BlockCategories Category, ColourOptions Colour, int Index)
                blockToAdd = hotbarBlocks[currentHotbar];
                UpdateSelectedBlock((GameObject)blockListDict[blockToAdd.Category][blockToAdd.Index]);
            }
            else
            {
                UpdateSelectedBlock(null);
            }
        }
    }


    private void AddToHotbar(InputAction.CallbackContext ctx)
    {
        if (isOpen)
        {
            (BlockCategories Category, ColourOptions Colour, int Index)
             blockToAdd = (currentCategory, currentColour, currentCell);

            hotbarBlocks[currentHotbar] = blockToAdd;
            
            string blockName;
            if (blockToAdd.Category == BlockCategories.Colour)
            {
                blockName = blockListDict[blockToAdd.Category][blockToAdd.Index].name + "_" + blockToAdd.Colour.ToString();
            }
            else
            {
                blockName = blockListDict[blockToAdd.Category][blockToAdd.Index].name;
            }
            
            Texture2D iconTex;
            foreach (Texture2D icon in iconList)
            {
                if (icon.name == blockName)
                {
                    iconTex = icon;
                    RawImage image = hotbarCells[currentHotbar].transform.Find("Icon").GetComponent<RawImage>();
                    image.texture = iconTex;
                    image.enabled = true;
                    break;
                }
            }
            
            print((GameObject)blockListDict[blockToAdd.Category][blockToAdd.Index]);
            UpdateSelectedBlock((GameObject)blockListDict[blockToAdd.Category][blockToAdd.Index]);
        }
    }


    private void UpdateSelectedBlock(GameObject block)
    {
        blockManager.InstantiateCurrentBlock(block);
    }
}
