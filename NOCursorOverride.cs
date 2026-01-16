using System.IO;
using BepInEx;
using UnityEngine;

[BepInPlugin("KopterBuzz.NOCursorOverride", "NOCursorOverride", "0.0.1")]
public class NOCursorOverride : BaseUnityPlugin
{
    private Texture2D cursorTexture;
    private Vector2 cursorSize = new Vector2(32, 32);
    private bool cursorLoaded = false;
    bool drawCursor = false;

    private void Awake()
    {
        LoadCursorTexture();
        //Cursor.visible = false;
        DontDestroyOnLoad(this);
    }

    private void LoadCursorTexture()
    {
        string path = Path.Combine($"{Paths.PluginPath}\\NOCursorOverride\\CursorImages", "DefaultImage.png");

        if (!File.Exists(path))
        {
            Logger.LogError($"Cursor image not found at: {path}");
            return;
        }

        byte[] pngData = File.ReadAllBytes(path);
        cursorTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

        if (!cursorTexture.LoadImage(pngData))
        {
            Logger.LogError("Failed to load cursor PNG.");
            return;
        }

        cursorTexture.filterMode = FilterMode.Point;
        cursorTexture.wrapMode = TextureWrapMode.Clamp;

        Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width / 2, cursorTexture.height / 2), CursorMode.Auto);

        cursorLoaded = true;


        Logger.LogInfo("Custom cursor loaded successfully.");
    }

    private void HideSystemCursor()
    {
        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.None;
    }

    private void OnGUI()
    {
        if (!cursorLoaded || !drawCursor || cursorTexture == null)
            return;

        Vector2 mousePos = Input.mousePosition;

        mousePos.y = Screen.height - mousePos.y;

        //center image around OS cursor point
        mousePos.x -= cursorSize.x / 2;
        mousePos.y -= cursorSize.y / 2;
        Rect cursorRect = new Rect(
            mousePos.x,
            mousePos.y,
            cursorSize.x,
            cursorSize.y
        );

        GUI.DrawTexture(cursorRect, cursorTexture);
    }
    private void Update()
    {
        //Cursor.visible = false;
        if (CursorManager.Visible)
        {
            if (DynamicMap.i != null && DynamicMap.mapMaximized)
            {
                CursorManager.SetFlag(CursorFlags.Map, true);
            }
            drawCursor = true;
        } else
        {
            drawCursor = false;
        }
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }
}
