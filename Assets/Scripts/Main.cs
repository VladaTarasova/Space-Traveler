using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    public GameObject mainPage, gameplayPage, inventoryPage, player, worldItems;
    public RectTransform joystickBackground, joystickHandle;

    private Dictionary<string, int> inventory = new Dictionary<string, int>();
    private List<string> inventoryItems = new List<string>();
    private Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();

    private Vector2 joystickInitialPosition;
    private float joystickRadius;

    public GameObject[] inventorySlots;
    public Sprite copperSprite;

    [System.Serializable]
    public class InventoryData 
    {
        public List<string> items;
    }

    void Start() {
        // Initialize joystick
        joystickInitialPosition = joystickBackground.position;
        Debug.Log("Joystick inital position:" + joystickInitialPosition);
        joystickRadius = joystickBackground.sizeDelta.x / 2;
        Debug.Log("joystickRadius:" + joystickRadius);
        itemSprites.Add("Copper", copperSprite);
        LoadInventory();
    }

    void Update() {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) { // Check if this is the beginning of the touch
                DetectResourceTouch(touch);
            }
            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                Vector2 localPoint;

                // Convert the touch position to local point in joystick background space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, touch.position, null, out localPoint);
                Vector2 direction = localPoint;

                // Limit the joystick handle movement within the joystick background
                direction = Vector2.ClampMagnitude(direction, joystickRadius);
                joystickHandle.anchoredPosition = direction+joystickInitialPosition;
                Debug.Log("joystickHandle.anchoredPosition:" + direction);

                // Move world items in the opposite direction to simulate player movement
                Vector3 moveDirection = new Vector3(-direction.x, -direction.y, 0);
                worldItems.transform.position += moveDirection * Time.deltaTime;
            } else if (touch.phase == TouchPhase.Ended) {
                // Reset joystick handle when touch ends
                joystickHandle.anchoredPosition = joystickInitialPosition;
            }
        }
    }

    public void StartGame()
    {
        mainPage.SetActive(false);
        gameplayPage.SetActive(true);
    }

    public void OpenInventory()
    {
        gameplayPage.SetActive(false);
        inventoryPage.SetActive(true);
    }

    public void CollectResource(string itemName) {
        inventoryItems.Add(itemName); // Add new item
        UpdateInventoryUI(); // Update UI
    }


    public void UpdateInventoryUI() {
        // Reset all slots first
        foreach (var slot in inventorySlots) {
            slot.GetComponent<InventorySlot>().ClearSlot();
        }
        
        // Populate slots with items in the inventory
        for (int i = 0; i < inventoryItems.Count && i < inventorySlots.Length; i++) {
            if (itemSprites.TryGetValue(inventoryItems[i], out Sprite sprite)) {
                inventorySlots[i].GetComponent<InventorySlot>().SetItem(sprite);
            }
        }
    }


    private void DetectResourceTouch(Touch touch) 
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
        Vector2 touchPos = touch.position;
        Debug.Log("touchPos:" + touchPos);
        RaycastHit2D hit = Physics2D.Raycast(touchPos, Vector2.zero);
        if (hit.collider != null) {
            Resource resource = hit.collider.gameObject.GetComponent<Resource>();
            if (resource != null) {
                resource.Collect();
            }
        }
    }

    public void SaveInventory() 
    {
        // Convert the inventory list to a JSON string
        string jsonInventory = JsonUtility.ToJson(new InventoryData { items = inventoryItems });
        // Save the JSON string to PlayerPrefs
        PlayerPrefs.SetString("Inventory", jsonInventory);
        PlayerPrefs.Save();
        Debug.Log("Inventory Saved");
    }

    public void LoadInventory() 
    {
        if (PlayerPrefs.HasKey("Inventory")) {
            // Get the JSON string from PlayerPrefs
            string jsonInventory = PlayerPrefs.GetString("Inventory");
            // Deserialize the JSON string back into the inventory list
            InventoryData loadedData = JsonUtility.FromJson<InventoryData>(jsonInventory);
            inventoryItems = loadedData.items;
            Debug.Log("Inventory Loaded");
            UpdateInventoryUI(); // Update UI with loaded inventory
        }
    }
}