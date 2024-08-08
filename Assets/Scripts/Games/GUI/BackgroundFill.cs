using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFill : MonoBehaviour
{
    public Camera c1;
    public GameObject go1;

    void Start()
    {
        ResizeSpriteToScreen(go1, c1, 1, 1);
    }

    // If fitToScreenWidth is set to 1 then the width fits the screen width.
    // If it is set to anyt$$anonymous$$ng over 1 then the sprite will not fit the screen width, it will be divided by that number.
    // If it is set to 0 then the sprite will not resize in that dimension.
    void ResizeSpriteToScreen(GameObject theSprite, Camera theCamera, int fitToScreenWidth, int fitToScreenHeight)
    {
        //SpriteRenderer sr = theSprite.GetComponent<SpriteRenderer>();

        theSprite.transform.localScale = new Vector3(1, 1, 1);

        //float width = sr.sprite.bounds.size.x;
        //float height = sr.sprite.bounds.size.y;

        //1920*1080
        float width = 19.2f;
        float height = 10.8f;

        float worldScreenHeight = (float)(theCamera.orthographicSize * 2.0);
        float worldScreenWidth = (float)(worldScreenHeight / Screen.height * Screen.width);

        if (fitToScreenWidth != 0)
        {
            Vector2 sizeX = new Vector2(worldScreenWidth / width / fitToScreenWidth, theSprite.transform.localScale.y);
            theSprite.transform.localScale = sizeX;
            theSprite.transform.localPosition = new Vector3(theSprite.transform.localPosition.x * sizeX.x, theSprite.transform.localPosition.y, theSprite.transform.localPosition.z);
        }

        if (fitToScreenHeight != 0)
        {
            Vector2 sizeY = new Vector2(theSprite.transform.localScale.x, worldScreenHeight / height / fitToScreenHeight);
            theSprite.transform.localScale = sizeY;
            theSprite.transform.localPosition = new Vector3(theSprite.transform.localPosition.x, theSprite.transform.localPosition.y*sizeY.y, theSprite.transform.localPosition.z);
        }
    }
}
