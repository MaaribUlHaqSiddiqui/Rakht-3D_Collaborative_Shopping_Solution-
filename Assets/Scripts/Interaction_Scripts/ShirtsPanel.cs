using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShirtsPanel : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnButtonClick()
    {
        // Deactivate the UI panel when the button is clicked
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            Debug.Log("Really Deactivating Panel");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
