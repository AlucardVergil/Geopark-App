using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeButtonColor : MonoBehaviour
{
    private string hexColor = "#FFA806"; // Hex color code (including the '#' symbol)

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeColor()
    {
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            // Apply the parsed color to the Image component
            GetComponent<Image>().color = color;
        }
        
    }
}
