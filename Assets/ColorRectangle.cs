using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRectangle : MonoBehaviour {

    private static Texture2D _staticRectTexture;
    private static GUIStyle _staticRectStyle;

    void Awake()
    {
        _staticRectTexture = new Texture2D(1, 1);
        _staticRectStyle = new GUIStyle();
    }

    public static void GUIDrawRect(Rect position, Color color)
    {

        _staticRectTexture.SetPixel(0, 0, color);
        _staticRectTexture.Apply();

        _staticRectStyle.normal.background = _staticRectTexture;

        GUI.Box(position, GUIContent.none, _staticRectStyle);

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        //GUIDrawRect(new Rect(100, 100, 10, 10), Color.red);
    }
}
