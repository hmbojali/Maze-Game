using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageMaker : MonoBehaviour
{
    public Material material;

    // The resolution of the RenderTexture and the image file
    public int width = 1024;
    public int height = 1024;

    // The path and name of the image file
    public string filePath = "Assets/MaterialImage.png";

    // The format of the image file (PNG or JPG)
    public TextureFormat format = TextureFormat.RGBA32;

    // A temporary RenderTexture to render the material
    private RenderTexture renderTexture;

    // A temporary Texture2D to store the pixel data
    private Texture2D texture2D;

    void Start()
    {
        // Create a new RenderTexture with the specified resolution
        renderTexture = new RenderTexture(width, height, 0);

        // Create a new Texture2D with the same resolution and format
        texture2D = new Texture2D(width, height, format, false);

        // Set the active RenderTexture to the one we created
        RenderTexture.active = renderTexture;

        // Draw a full-screen quad with the material
        Graphics.Blit(null, renderTexture, material);

        // Read the pixel data from the RenderTexture into the Texture2D
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();

        // Encode the Texture2D as an image file (PNG or JPG)
        byte[] bytes;
        if (format == TextureFormat.RGB24 || format == TextureFormat.RGBA32)
        {
            bytes = texture2D.EncodeToPNG();
        }
        else
        {
            bytes = texture2D.EncodeToJPG();
        }

        // Write the image file to disk
        File.WriteAllBytes(filePath, bytes);

        // Release the temporary RenderTexture and Texture2D
        RenderTexture.active = null;
        renderTexture.Release();
        Destroy(renderTexture);
        Destroy(texture2D);

        // Log a message to confirm that the image file is saved
        Debug.Log("Saved material as image file: " + filePath);
    }
}
