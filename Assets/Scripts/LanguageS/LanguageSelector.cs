using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    public TMP_Dropdown languageDropdown; // Reference to the dropdown
    public bool isGreek; // This will hold the state based on selection

    void Start()
    {
        // Ensure Dropdown is assigned
        if (languageDropdown == null)
            languageDropdown = GetComponent<TMP_Dropdown>();

        // Set up the listener for when the dropdown value changes
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        // Initialize the dropdown
        InitializeDropdown();
    }

    private void InitializeDropdown()
    {
        // Set default value (e.g., English)
        languageDropdown.value = 1; // Assuming English is at index 1
        isGreek = false; // Default is English
    }

    // Callback for when the dropdown value changes
    private void OnLanguageChanged(int index)
    {
        if (index == 0) // Assuming "Greek" is at index 0
        {
            isGreek = true;
            Debug.Log("Language set to Greek");
        }
        else if (index == 1) // Assuming "English" is at index 1
        {
            isGreek = false;
            Debug.Log("Language set to English");
        }
    }
}
