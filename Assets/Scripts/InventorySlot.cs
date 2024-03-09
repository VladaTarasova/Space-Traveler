using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {
    private Image itemImage; // Now private, we'll set this in Start or Awake

    void Awake() {
        itemImage = null;
        
        // Check for Image components on children only, excluding the parent GameObject.
        Image[] images = GetComponentsInChildren<Image>(true); // true to include inactive children
        foreach (Image img in images) {
            if (img.transform != transform) { // Check if the found Image component is not on the parent GameObject
                itemImage = img;
                break; // Break after finding the first child Image component
            }
        }

        if (itemImage == null) {
            Debug.LogError("No Image component found on child GameObjects", this);
        }
    }

    // Call this method to update the slot with an item's image
    public void SetItem(Sprite itemSprite) {
        if (itemImage != null) {
            itemImage.sprite = itemSprite;
            itemImage.enabled = itemSprite != null; // Enable or disable based on if we have a sprite
            itemImage.gameObject.SetActive(itemSprite != null);
        }
    }

    // Call this method to clear the slot
    public void ClearSlot() {
        if (itemImage != null) {
            itemImage.sprite = null;
            itemImage.enabled = false;
        }
    }
}
