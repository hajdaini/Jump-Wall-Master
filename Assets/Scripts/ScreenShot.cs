using UnityEngine;
using System;
using System.Collections;
using System.IO;

public class ScreenShot : MonoBehaviour
{
    [SerializeField] string directoryName = "Screenshots";
    [SerializeField] string screenshotName = "TestImage";
    
    
    [SerializeField] SpriteRenderer eyesBorder;
    [SerializeField] SpriteRenderer eyesWhite;
    [SerializeField] GameObject player;
    [SerializeField] PlayerSkin[] playerSkins;


    void Start(){
        // StartCoroutine(TakeCharacterScreenShot());
    }


    
    [ContextMenu("Take Screen shot")]
    void TakeScreenshot()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string screenshotPath = Path.Combine(documentsPath, Application.productName, directoryName);
        DirectoryInfo screenshotDirectory = Directory.CreateDirectory(screenshotPath);
        string fullpath = Path.Combine(screenshotDirectory.FullName, screenshotName + ".png");
        ScreenCapture.CaptureScreenshot(fullpath, 1);
    }


    IEnumerator TakeCharacterScreenShot()
    {
        foreach (PlayerSkin playerSkin in playerSkins){
            foreach (GameObject skinToActivate in playerSkin.skinsToActivate) skinToActivate.SetActive(true);
            player.transform.localScale = playerSkin.character.size * 6;
            eyesBorder.sprite = playerSkin.character.eyesBorder;
            eyesWhite.sprite =playerSkin.character.eyesWhite;
            ChangePlayerColor(playerSkin.character);
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string screenshotPath = Path.Combine(documentsPath, Application.productName, directoryName);
            DirectoryInfo screenshotDirectory = Directory.CreateDirectory(screenshotPath);
            string fullpath = Path.Combine(screenshotDirectory.FullName, playerSkin.character.label + ".png");
            TakeTransparentScreenshot(Camera.main, Screen.width, Screen.height, fullpath);

            Debug.Log(playerSkin.character.label + ": " + (1 / (playerSkin.character.size.x / 0.75f)) + " | " + (1 / (playerSkin.character.size.y / 2.1f)));

            yield return new WaitForSeconds(1);
            foreach (GameObject skinToActivate in playerSkin.skinsToActivate) skinToActivate.SetActive(false);
        }
    }

    void ChangePlayerColor(Character character){
        Color color = character.color;
        player.transform.GetComponent<SpriteRenderer>().color = color;
        eyesBorder.color = color;
    }

    public static void TakeTransparentScreenshot(Camera cam, int width, int height, string savePath)
    {
        // Depending on your render pipeline, this may not work.
        var bak_cam_targetTexture = cam.targetTexture;
        var bak_cam_clearFlags = cam.clearFlags;
        var bak_RenderTexture_active = RenderTexture.active;

        var tex_transparent = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // Must use 24-bit depth buffer to be able to fill background.
        var render_texture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
        var grab_area = new Rect(0, 0, width, height);

        RenderTexture.active = render_texture;
        cam.targetTexture = render_texture;
        cam.clearFlags = CameraClearFlags.SolidColor;

        // Simple: use a clear background
        cam.backgroundColor = Color.clear;
        cam.Render();
        tex_transparent.ReadPixels(grab_area, 0, 0);
        tex_transparent.Apply();

        // Encode the resulting output texture to a byte array then write to the file
        byte[] pngShot = ImageConversion.EncodeToPNG(tex_transparent);
        File.WriteAllBytes(savePath, pngShot);

        cam.clearFlags = bak_cam_clearFlags;
        cam.targetTexture = bak_cam_targetTexture;
        RenderTexture.active = bak_RenderTexture_active;
        RenderTexture.ReleaseTemporary(render_texture);
        Texture2D.Destroy(tex_transparent);
    }

}