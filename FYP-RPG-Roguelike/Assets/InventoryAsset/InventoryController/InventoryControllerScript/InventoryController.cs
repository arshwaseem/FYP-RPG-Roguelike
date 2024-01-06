using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private bool iUnderstandTheSetup = false; // Ensure this is the sole instance of InventoryController.

        [Space(10)] // Add some space for better organization.

        [SerializeField]
        private Transform UI;

        [Space(10)] // Add some space/

        [SerializeField]
        public List<ItemInitializer> items; // Accepted items to add to the inventory.

        [Space(10)] // Add some space.

        [Header("========[ Inventory Setup ]========")]
        [Header("NOTE: After initialization, changes here won't take effect.")]
        [Header("Modify the inventory under the UI component.")]
        [Tooltip("Add templates for each inventory to be initialized.")]
        [SerializeField]
        public List<InventoryInitializer> initializeInventory = new List<InventoryInitializer>(); // Information about inventory setup.




        [SerializeField, HideInInspector]
        private List<InventoryInitializer> prevInventoryTracker; // Previously initialized inventories, so they are not initialized again.


        [SerializeField]
        private GameObject inventoryManagerObj; // Prefab for the inventory manager.

        [SerializeField, HideInInspector]
        private List<GameObject> allInventoryUI = new List<GameObject>(); // Holds all inventory UI instances for each inventory created.
        private Dictionary<string, Inventory> inventoryManager = new Dictionary<string, Inventory>(); // Dictionary to map inventory names to their Inventory object.
        private Dictionary<string, GameObject> inventoryUIDict = new Dictionary<string, GameObject>(); // Dictionary to map inventory names to their GameObject.
        private Dictionary<string, InventoryItem> itemManager = new Dictionary<string, InventoryItem>(); // Dictionary to map item names to their objects.
        private Dictionary<string, List<GameObject>> EnableDisableDict = new Dictionary<string, List<GameObject>>(); // Dictionary to map a key press to which inventory it should enable/disable.

        [SerializeField, HideInInspector]
        public static InventoryController instance; // Shared instance of the InventoryController to enforce only one being created.

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                Debug.LogError("There should only be one inventory controller in the scene");
            }
            if (iUnderstandTheSetup)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            if (!TestInstance()) return;
            if (!TestSetup()) return;
            TestChildObject();
            AllignDictionaries();
            InitializeItems();
        }

        private void Start()
        {
            LoadSave();
        }

        private void Update()
        {
            ToggleOnKeyInput();
        }

        public void InitializeInventories()
        {

            if (!TestSetup()) return;
            instance = this;
            AllignDictionaries();
            RemoveDeletedInventories();
            InitializeNewInventories();
            UpdateInventoryTracker();
            InitializeItems();
        }

        private void UpdateInventoryTracker()
        {
            prevInventoryTracker.Clear();
            for (int i = 0; i < initializeInventory.Count; i++)
            {
                InventoryInitializer InitilizerCopy = new InventoryInitializer();
                InitilizerCopy.Copy(initializeInventory[i]);
                prevInventoryTracker.Add(InitilizerCopy);
            }
        }

        private void InitializeNewInventories()
        {
            foreach (InventoryInitializer initializer in initializeInventory)
            {
                if (!prevInventoryTracker.Contains(initializer))
                {
                    initializer.SetInitialized(true);
                    GameObject tempinventoryUI = Instantiate(inventoryManagerObj, transform.position, Quaternion.identity, UI);
                    RectTransform UIRect = UI.GetComponent<RectTransform>();
                    tempinventoryUI.transform.position = new Vector3(Random.Range(0.0f, UIRect.sizeDelta.x), Random.Range(0.0f, UIRect.sizeDelta.y), 0);
                    tempinventoryUI.SetActive(true);
                    tempinventoryUI.name = initializer.GetInventoryName();
                    allInventoryUI.Add(tempinventoryUI);

                    string inventoryName = initializer.GetInventoryName();
                    int InventorySize = initializer.GetRows() * initializer.GetCols();
                    Inventory curInventory = new Inventory(tempinventoryUI, inventoryName, InventorySize);

                    inventoryManager.Add(inventoryName, curInventory);

                    InventoryUIManager inventoryUI = tempinventoryUI.GetComponent<InventoryUIManager>();
                    inventoryUI.SetVarsOnInit();
                    inventoryUI.SetInventory(ref curInventory);
                    inventoryUI.SetRowCol(initializer.GetRows(), initializer.GetCols());
                    inventoryUI.SetInventoryName(initializer.GetInventoryName());
                    inventoryUI.UpdateInventoryUI();
                }
            }
            foreach (GameObject inObjects in allInventoryUI)
            {
                inObjects.GetComponent<InventoryUIManager>().UpdateInventoryUI();
            }
        }

        private void RemoveDeletedInventories()
        {

            List<GameObject> toremove = new List<GameObject>();
            foreach (InventoryInitializer initializer in prevInventoryTracker)
            {
                if (!initializeInventory.Contains(initializer))
                {
                    foreach (GameObject UI in allInventoryUI)
                    {
                        InventoryUIManager UIInstance = UI.GetComponent<InventoryUIManager>();
                        if (UIInstance.GetInventoryName() == initializer.GetInventoryName())
                        {
                            toremove.Add(UI);
                            inventoryManager.Remove(UIInstance.GetInventoryName());
                        }
                    }
                }
            }

            foreach (GameObject remove in toremove)
            {
                allInventoryUI.Remove(remove);
                DestroyImmediate(remove);

            }
        }

        private void InitializeItems()
        {
            itemManager.Clear();
            foreach (ItemInitializer item in items)
            {
                InventoryItem newItem = new InventoryItem(item);
                if(!itemManager.ContainsKey(newItem.GetItemType()))
                {
                    itemManager.Add(item.GetItemType(), newItem);
                }
                else
                {
                    Debug.LogError("There can only be one of each ItemType");
                }
            }
        }

        public void AddItemPos(string inventoryName, InventoryItem itemType, int position)
        {
            if (!(TestInventoryDict(inventoryName)))
            {
                return;
            }
            if (itemType.GetIsNull())
            {
                Debug.LogError("Cannot add null item");
                return;
            }
            Inventory inventory = inventoryManager[inventoryName];
            inventory.AddItemPos(position, itemType);
        }

        public void AddItemPos(string inventoryName, string itemType, int position, int amount = 1)
        {
            if (!(TestInventoryDict(inventoryName) && TestItemDict(itemType)))
            {
                return;
            }
            Inventory inventory = inventoryManager[inventoryName];
            InventoryItem item = new InventoryItem(itemManager[itemType], amount);
            inventory.AddItemPos(position, item);
        }

        public void AddItem(string inventoryName, string itemType, int amount = 1)
        {
            if (!(TestInventoryDict(inventoryName) && TestItemDict(itemType)))
            {
                return;
            }
            Inventory inventory = inventoryManager[inventoryName];
            InventoryItem item = new InventoryItem(itemManager[itemType], amount);
            inventory.AddItemAuto(item, amount);
        }

        public void RemoveItemPos(string inventoryName, int position, int amount)
        {
            if (!TestInventoryDict(inventoryName))
            {
                return;
            }
            Inventory inventory = inventoryManager[inventoryName];
            inventory.RemoveItemInPosition(position, amount);

        }
        public void RemoveItem(string inventoryName,string itemType, int amount)
        {
            if (!(TestInventoryDict(inventoryName) && TestItemDict(itemType)))
            {
                return;
            }
            Inventory inventory = inventoryManager[inventoryName];
            inventory.RemoveItemAuto(itemType, amount);
        }


        public void RemoveItem(string inventoryName, InventoryItem item, int amount = 1)
        {
            if (!(TestInventoryDict(inventoryName)))
            {
                return;
            }
            if (item.GetIsNull())
            {
                Debug.LogError("Cannot remove null item");
                return;
            }

            Inventory inventory = inventoryManager[inventoryName];
            inventory.RemoveItemInPosition(item, amount);
        }

        public bool InventoryFull(string inventoryName, string itemType)
        {
            if (!TestInventoryDict(inventoryName))
            {
                return true;
            }
            Inventory inventory = inventoryManager[inventoryName];
            return inventory.Full(itemType);
        }
        public void InventoryClear(string inventoryName)
        {
            inventoryManager[inventoryName].Clear();
        }

        private bool TestInventoryDict(string inventoryName)
        {
            if (inventoryName == null)
            {
                Debug.LogError("Inventory name is null");
                return false;
            }
            if (inventoryManager.ContainsKey(inventoryName))
            {
                return true;
            }
            else
            {
                Debug.LogError("No existing inventory with name: " + inventoryName);
                return false;
            }
        }

        private bool TestItemDict(string itemType)
        {
            if (itemType == null)
            {
                Debug.LogError("Itemtype is null");
                return false;
            }
            if (itemManager.ContainsKey(itemType))
            {
                return true;
            }
            else
            {
                Debug.LogError("No existing item with name: " + itemType);
                return false;
            }
        }

        public void ResetInventory()
        {
            inventoryManager.Clear();
            itemManager.Clear();
            prevInventoryTracker.Clear();
            foreach (ItemInitializer item in items)
            {
                itemManager.Add(item.GetItemType(), new InventoryItem(item));
            }
            foreach (GameObject obj in allInventoryUI)
            {
                DestroyImmediate(obj);
            }
            allInventoryUI.Clear();
            inventoryUIDict.Clear();
            DeleteSaveInformation();
        }

        public void DeleteSaveInformation()
        {
            InventorySaveSystem.Reset(SceneManager.GetActiveScene().name);
        }

        public void AllignDictionaries()
        {
            inventoryManager.Clear();
            foreach (GameObject InventoryUI in allInventoryUI)
            {
                bool setActive = false;
                if (!InventoryUI.activeSelf)
                {
                    setActive = true;
                    InventoryUI.SetActive(true);
                }
                InventoryUIManager inventoryInstance = InventoryUI.GetComponent<InventoryUIManager>();
                if (!inventoryUIDict.ContainsKey(inventoryInstance.GetInventoryName()))
                {
                    inventoryUIDict.Add(inventoryInstance.GetInventoryName(), InventoryUI);

                }
                inventoryInstance.GetInventory().InitList();
                inventoryManager.Add(inventoryInstance.GetInventoryName(), inventoryInstance.GetInventory());
                foreach (char character in inventoryInstance.GetEnableDisable())
                {
                    if (EnableDisableDict.ContainsKey(character.ToString().ToLower()))
                    {
                        EnableDisableDict[character.ToString().ToLower()].Add(InventoryUI);
                    }
                    else
                    {
                        EnableDisableDict.Add(character.ToString().ToLower(), new List<GameObject>());
                        EnableDisableDict[character.ToString().ToLower()].Add(InventoryUI);
                    }
                }
                if (setActive)
                {
                    InventoryUI.SetActive(false);
                }
            }
        }

        public int CountItems(string inventoryName, string itemType)
        {
            if (!(TestInventoryDict(inventoryName) && TestItemDict(itemType)))
            {
                return 0;
            }
            Inventory inventory = inventoryManager[inventoryName];
            return inventory.Count(itemType);
        }

        public void AddToggleKey(string InventoryName, char character)
        {
            if (EnableDisableDict.ContainsKey(character.ToString().ToLower()))
            {
                if (EnableDisableDict[character.ToString().ToLower()].Contains(inventoryUIDict[InventoryName]))
                {
                    return;
                }
                EnableDisableDict[character.ToString().ToLower()].Add(inventoryUIDict[InventoryName]);
            }
            else
            {
                EnableDisableDict.Add(character.ToString().ToLower(), new List<GameObject>());
                EnableDisableDict[character.ToString().ToLower()].Add(inventoryUIDict[InventoryName]);
            }
        }

        public void RemoveToggleKey(string InventoryName, char character)
        {
            if (EnableDisableDict.ContainsKey(character.ToString().ToLower()))
            {
                if (!EnableDisableDict[character.ToString().ToLower()].Contains(inventoryUIDict[InventoryName]))
                {
                    return;
                }
                EnableDisableDict[character.ToString().ToLower()].Remove(inventoryUIDict[InventoryName]);
            }
        }

        private void ToggleOnKeyInput()
        {
            if (Input.anyKeyDown)
            {
                string input = Input.inputString;
                input = input.ToLower();
                if (EnableDisableDict.ContainsKey(input))
                {
                    List<GameObject> inventoryUIs = EnableDisableDict[input];
                    foreach (GameObject inventoryUI in inventoryUIs)
                    {
                        if (inventoryUI.activeSelf)
                        {
                            inventoryUI.SetActive(false);
                        }
                        else
                        {
                            inventoryUI.SetActive(true);
                        }
                    }
                }
            }
        }

        private void LoadSave()
        {
            InventorySaveSystem.Create(SceneManager.GetActiveScene().name);
            if (InventorySaveSystem.LoadItem(SceneManager.GetActiveScene().name) != null)
            {
                InventoryData itemData = InventorySaveSystem.LoadItem(SceneManager.GetActiveScene().name);
                foreach (var pair in itemData.inventories)
                {
                    if (pair.Key == null)
                        continue;
                    if (!inventoryManager.ContainsKey(pair.Key))
                    {
                        Debug.LogError(pair.Key + "does not exist in inventoryManager. This may be caused by having two scenes with the same name utilizing save mechanism");
                        continue;
                    }
                    Inventory inventory = inventoryManager[pair.Key];

                    if (!inventory.GetSaveInventory())
                    {
                        continue;
                    }
                    List<ItemData> items = pair.Value;
                    foreach (ItemData item in items)
                    {

                        if (item.name != null)
                        {
                            InventoryItem copyItem = itemManager[item.name];
                            InventoryItem newItem = new InventoryItem(copyItem, item.amount);
                            AddItemPos(pair.Key, newItem, item.position);
                        }
                    }
                }
            }
        }

        public void CreateInventory(Transform instantiaterPos, string inventoryName, int row, int col,
            bool highlightable = false, bool draggable = false, bool saveInventory = false, bool isActive = true)
        {
            if (!TestSetup()) return;
            GameObject tempinventoryUI = Instantiate(inventoryManagerObj, instantiaterPos.position, Quaternion.identity, UI);
            tempinventoryUI.SetActive(isActive);

            tempinventoryUI.transform.position = instantiaterPos.position;

            Inventory curInventory = new Inventory(tempinventoryUI, inventoryName, col * row);
            inventoryManager.Add(inventoryName, curInventory);

            InventoryUIManager inventoryUI = tempinventoryUI.GetComponent<InventoryUIManager>();
            inventoryUI.SetVarsOnInit();
            inventoryUI.SetSave(saveInventory);
            inventoryUI.SetInventory(ref curInventory);
            inventoryUI.SetHighlightable(highlightable);
            inventoryUI.SetDraggable(draggable);
            inventoryUI.SetRowCol(row, col);
            inventoryUI.SetInventoryName(inventoryName);
            inventoryUI.UpdateInventoryUI();
        }

        private bool TestSetup()
        {

            return TestIunderstandTheSetup()
                && TestinventoryManagerObjSetup()
                && TestInventoryUI()
                && TestInveInitializerListSetup()
                && TestUISetup();



        }
        private bool TestIunderstandTheSetup()
        {
            if(!iUnderstandTheSetup)
            {
                Debug.LogError("Read instructions and click The I understand The setup bool.");
                return false;
            }
            return true;
        }


        private bool TestInventoryUI()
        {
            if (allInventoryUI.Count == 0 && Application.isPlaying)
            {
                Debug.LogWarning("No InventoryUIManagers detected. Ensure to initialize all inventories in editor mode. If unexpected try unpacking InventoryController");
                return false;
            }
            foreach (GameObject inventories in allInventoryUI)
            {
                if (inventories == null)
                {
                    Debug.LogError("Inventories in allInventoryUI are null, try unpacking InventoryController and hitting \"Delete All Instantiated Objects\" button");
                    return false;
                }
            }
            return true;
        }


        public bool TestinventoryManagerObjSetup()
        {
            if (inventoryManagerObj == null)
            {
                Debug.LogError("Inventory manager object is null");
                return false;
            }
            return true;
        }


        private bool TestInveInitializerListSetup()
        {
            for (int i = 0; i < initializeInventory.Count; i++)
            {
                int countInstance = 0;

                for (int j = 0; j < initializeInventory.Count; j++)
                {
                    if (initializeInventory[i].GetInventoryName().Equals(initializeInventory[j].GetInventoryName()))
                    {
                        countInstance++;
                    }
                    if (countInstance > 1)
                    {
                        Debug.LogError("There can only be one of each Inventory");

                        return false;
                    }
                }

            }
            return true;
        }


        private bool TestUISetup()
        {
            if (UI == null)
            {
                Debug.LogError("UI Canvas Not Set Correctly");
                return false;
            }
            return true;
        }

        private bool TestInstance()
        {
            if (instance == null)
            {
                Debug.LogError("Read instructions and click The I understand The setup bool. InventoryController destroyed");
                return false;
            }
            return true;
        }


        private void TestChildObject()
        {
            InventoryUIManager manager = transform.GetComponentInChildren<InventoryUIManager>();

            if (manager != null)
            {
                if (manager.gameObject.activeSelf)
                {
                    Debug.LogWarning("The Child of Inventory Controller, InventoryUIManager is active. Disabling it now.");
                    manager.gameObject.SetActive(false);
                }
            }
            else
            {
                if (transform.childCount == 0)
                    Debug.LogWarning("Inventory Controller Does Not Have Child Object with InventoryUIManager");
            }
        }
        public bool checkEnabled(string inventoryName)
        {
            if (inventoryUIDict[inventoryName].activeSelf)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool checkUI(GameObject obj)
        {
            if (inventoryUIDict.ContainsValue(obj))
            {
                return true;
            }
            return false;
        }
        public GameObject GetInventoryManagerPrefab()
        {
            return inventoryManagerObj;
        }
        public Inventory GetInventory(string inventoryName)
        {
            return inventoryManager[inventoryName];
        }
        public Transform GetUI()
        {
            return UI;
        }
        public InventoryItem GetItem(string inventoryName, int index)
        {
            return inventoryManager[inventoryName].InventoryGetItem(index);
        }


        public List<ItemInitializer> GetItems()
        {
            return items;
        }

        private void OnDestroy()
        {
            InventorySaveSystem.SaveInventory(inventoryManager, SceneManager.GetActiveScene().name);
        }
    }
}
