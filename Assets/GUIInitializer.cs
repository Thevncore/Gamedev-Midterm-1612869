using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIInitializer : MonoBehaviour
{
    // Start is called before the first frame update

    public GUISkin _theSkin = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (_theSkin != null)
        {
            GUI.skin = _theSkin;
        }
    }
}
