using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptchaPerlinNoise : MonoBehaviour
{
    public RawImage noiseTextureImage;

    private int noiseWidth = 256;
    private int noiseHeight = 256;

    private float scale = 50f;

    public Gradient gradient;

    private void Start()
    {
        //將Imgae的Texture改成噪聲
        noiseTextureImage.material.mainTexture = GenerateTexture();
    }

    Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(noiseWidth, noiseHeight);

        //將每個像素的值替換成噪聲
        for (int x = 0; x < noiseWidth; x++)
        {
            for (int y = 0; y < noiseHeight; y++)
            {
                //調整噪聲密度
                float xCoord = (float)x / noiseWidth * scale;
                float yCoord = (float)y / noiseHeight * scale;

                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                //調整噪聲顏色
                Color color = gradient.Evaluate(sample);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }
}
