using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
    public string inventoryItemName;

    public void Collect() {
        FindObjectOfType<Main>().CollectResource(inventoryItemName);
        Destroy(gameObject);
    }
}
