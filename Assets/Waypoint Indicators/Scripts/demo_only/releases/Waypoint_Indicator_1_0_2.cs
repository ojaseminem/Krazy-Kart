using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Waypoint_Indicator_1_0_2 : MonoBehaviour
{
    //Version 1.0.2
    //1. Arrows track smoothly along edges and never snap or jump to other side
    //2. Off screen icons can be rotated even with point to off-screen is enabled
    //3. Text UI no longer disappears after a wpParent.z distance beyond 1,000 or so


    //CANVAS - By default the script will look for "Canvas". If you have labled it something else, update the variable below to match
    //!! Make sure your Canvas > Reference Resolution is the same aspect ratio as your Play Screen or else Waypoint boundary edges may not align exactly
    //See Documentation for more info
    private string canvas_name = "Canvas";
    private Canvas mainCanvas;
    private float canvasScaleFactor;
    private Vector2 canvasRefRez;

    //CAMERA - By default the script will look for Camera.main.
    private Camera mainCamera; //If you'd like to target the Camera by a tag, look at line 137.




    [Header("Parent")]
    private bool isDynamic = false; //Make this public to allow for external scripts to send Text Description copy here as "description"
    public bool showBoundaryBox = false;
    public Color boundaryBoxColor; // = new Color32(255, 0, 0, 255);
    public Vector2Int parentSize = new Vector2Int(50, 50);
    private float parentPaddingX;
    private float parentPaddingY;
    private float onScreenSnapOffset_X;
    private float onScreenSnapOffset_Y;
    public float displayRange = 25f;





    [Header("Icon Global")]
    public bool enableIcon = true;
    public bool offScreenRotates = false;

    [Header("Icon On-Screen")]
    public Sprite onScreenSprite;
    public float onScreenSize = 1f;
    public Vector2 onScreenIconOffset = new Vector2(0f, 0f);
    [Range(0, 360)]
    public float onScreenRotation = 0f;
    public bool onScreenFadeWithRange;
    public bool onScreenScaleWithRange;
    public bool onScreenHide = false;

    [Header("Icon Off-Screen")]
    public Sprite offScreenSprite;
    public float offScreenSize = 1f;
    public Vector2 offScreenIconOffset = new Vector2(0f, 0f);
    [Range(0, 360)]
    public float offScreenRotation = 0f;
    public bool offScreenFadeWithRange;
    public bool offScreenScaleWithRange;
    public bool offScreenHide = false;




    [Header("Text Global")]
    public bool enableText = true;
    public string textDescription;
    public TMP_FontAsset textFont;
    public static string description; //Text pulled from Quest scripts (if marked as dynamic)
    public float textSize = 15;
    public Color textColor = new Color32(0, 0, 0, 255);
    public enum textAlignValue
    {
        Left,
        Center,
        Right
    }
    public textAlignValue textAlign = textAlignValue.Center;
    public float textLineSpacing = 20f;
    public float textRectWidth = 80f;
    public float textRectHeight = 50f;

    [Header("Text On-Screen")]
    public bool onScreenHideDesc = false;
    public bool onScreenHideDist = false;
    public Vector2 onScreenTextOffset = new Vector2(0f, 0f);

    [Header("Text Off-Screen")]
    public bool offScreenHideDesc = false;
    public bool offScreenHideDist = false;
    public Vector2 offScreenTextOfffset = new Vector2(0f, 0f);




    //WP Parent Vars
    private GameObject wpParentGameObject;
    private RectTransform wpParent;
    private Image wpParentImage;
    private bool waypointParentCreated = false;


    //WP Icon Vars
    private GameObject iconGameObject;
    private RectTransform icon;
    private Image iconImage;
    private bool waypointIconCreated = false;

    //WP Text Vars
    private GameObject textGameObject;
    private TextMeshProUGUI textField;
    private bool waypointTextCreated = false;
    private Vector2 onScreenSpriteOriginalSize;
    private Vector2 offScreenSpriteOriginalSize;
    private Vector2 newOnScreenSize;
    private Vector2 newOffScreenSize;

    //Setup Vars
    private Vector3 screenCenter;
    private Vector3 wpParentPos;
    private Color iconColor;
    private float angle;
    private float waypointDist;
    private int iIcon = 0;
    private int iText = 0;
    private int iScreenCheck;
    private bool parentOnScreen;
    private float iconAlphaValue;
    private float scaleValueX;
    private float scaleValueY;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private GameObject waypointParent;

    private void Start()
    {

    }

    void OnEnable()
    {
        mainCamera = Camera.main;
        //mainCamera = GameObject.FindGameObjectWithTag("PlayerCam").GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.Log("The reference to your Main Camera is missing, you may have renamed or deleted it. Try referencing using a tag. See line 135 of this script.");
        }


        mainCanvas = GameObject.Find(canvas_name).GetComponent<Canvas>();
        if (mainCanvas == null)
        {
            Debug.Log("The reference to your Canvas (" + canvas_name + ") is missing, you may have renamed or deleted it. Alos, make sure there is only one Canvas in this scene named: " + canvas_name);
        }



        if (mainCamera && mainCanvas)
        {
            //Assign to a layer
            //gameObject.layer = 5;

            InstantiateWaypointParent();


            if (textDescription == "")
            {
                textDescription = gameObject.name;
                //Debug.Log("There's no text");
            }

            //Find this game object's Waypoint UI by name so we can target it later
            waypointParent = GameObject.Find(wpParent.name);

            iIcon = 0;
            iText = 0;


            //Get the original size of the Sprite before alterations
            onScreenSpriteOriginalSize.x = onScreenSprite.bounds.size.x;
            onScreenSpriteOriginalSize.y = onScreenSprite.bounds.size.y;

            offScreenSpriteOriginalSize.x = offScreenSprite.bounds.size.x;
            offScreenSpriteOriginalSize.y = offScreenSprite.bounds.size.y;

            canvasRefRez = mainCanvas.GetComponent<CanvasScaler>().referenceResolution;
        }

    }

    void OnDisable()
    {
        waypointParentCreated = false;
        waypointIconCreated = false;
        waypointTextCreated = false;

        //Destroy wp ui using find from OnEnable() above
        if (waypointParent != null)
        {
            Destroy(waypointParent);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("RR: " + canvasRefRez);
        //Debug.Log("Scale Factor: " + canvasScaleFactor);
        //Debug.Log("Screen W: " + Screen.width);
        //canvasRefRez = mainCanvas.GetComponent<CanvasScaler>().referenceResolution;

        if (mainCamera && mainCanvas)
        {
            //If true, other scripts can send text description copy via: WaypointIndicator.description = "My Text";
            if (isDynamic)
            {
                textDescription = description;
            }
            //Finsh Remove



            #region CHECK TOGGLE STATES
            //ICON
            if (enableIcon) //ENABLE
            {
                if (iIcon == 0)
                {
                    //Debug.Log("Display Icon");
                    InstantiateWaypointIcon();
                    iIcon++;
                }
            }
            if (!enableIcon) //DISABLE
            {
                if (iIcon == 1)
                {
                    //Debug.Log("Hide Icon");
                    DestroyWaypointIcon();
                    iIcon--;
                }
            }


            //TEXT
            if (enableText) //ENABLE
            {
                if (iText == 0)
                {
                    //Debug.Log("Display Text Field");
                    InstantiateWaypointText();
                    iText++;
                }
            }
            if (!enableText) //DISABLE
            {
                if (iText == 1)
                {
                    //Debug.Log("Hide Text Field");
                    DestroyWaypointText();
                    iText--;
                }
            }
            #endregion

            //Determine Distance from Game Object (that this script is attached to) Distance is from Camera
            waypointDist = (int)Vector3.Distance(mainCamera.transform.position, transform.position);



            #region IF ENABLE PARENT (enabled on Start)
            if (waypointParentCreated && wpParent != null)
            {
                //Reset Scale to 1
                wpParent.localScale = new Vector3(1f, 1f, 1f);

                //Show/Hide
                if (showBoundaryBox)
                {
                    wpParentImage.enabled = true;
                }
                else
                {
                    wpParentImage.enabled = false;
                }

                //Match the PARENT UI pos to 3D MESH position (the object this script is attached to) via WorldToScreenPoint
                if (mainCamera != null)
                {
                    wpParentPos = mainCamera.WorldToScreenPoint(transform.position);
                    //This keeps wpParentPos.z value from going above 1 causing dissapearing if above 1000 or so
                    if (wpParentPos.z > 1)
                    {
                        wpParentPos.z = 1f;
                    }
                    else
                    {
                        wpParentPos.z = mainCamera.WorldToScreenPoint(transform.position).z;
                    }

                    canvasScaleFactor = mainCanvas.scaleFactor;
                }


                //ONSCREEN SNAP TO EDGE OFFSET
                //This ensures the on screen icon boundary edge will trigger offscreen exactly when it touches screen edge
                //Otherwise, the on screen icon will snap to edge before reaching them
                //As long as Canvas > Reference Resolution is the same aspect ratio as the play mode screen, this will always align
                onScreenSnapOffset_X = 22 - (Screen.width * .024f);
                //onScreenSnapOffset_Y = 22 - (Screen.height * .024f);
                onScreenSnapOffset_Y = 0f;



                //Onscreen
                if (wpParentPos.z > 0f &&
                    (wpParentPos.x / canvasScaleFactor) > (wpParent.sizeDelta.x / 2) && wpParentPos.x < (Screen.width - (wpParent.sizeDelta.x / 2)) + onScreenSnapOffset_X &&
                    wpParentPos.y / canvasScaleFactor > (wpParent.sizeDelta.y / 2) && wpParentPos.y < (Screen.height - (wpParent.sizeDelta.y / 2)) + onScreenSnapOffset_Y)
                {
                    //Debug.Log("On");
                    if (iScreenCheck == 1)
                    {
                        //Debug.Log("On Screen");
                        parentOnScreen = true;
                        iScreenCheck--;
                    }

                    wpParent.transform.position = wpParentPos;
                }
                else //(Offscreen)
                {
                    //Debug.Log("Off");
                    if (iScreenCheck == 0)
                    {
                        //Debug.Log("Off Screen");
                        parentOnScreen = false;
                        iScreenCheck++;
                    }


                    if (wpParentPos.z < 0)
                    {
                        //Flip coordinates when things are behind
                        wpParentPos *= -1;
                    }

                    //Find center of screen
                    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0f) / 2;

                    //Set 0,0 DEAD CENTER from lower left
                    wpParentPos -= screenCenter;

                    //Find angle from center of screen to mouse pos
                    angle = Mathf.Atan2(wpParentPos.y, wpParentPos.x);
                    angle -= 90f * Mathf.Deg2Rad;

                    float cos = Mathf.Cos(angle);
                    float sin = Mathf.Sin(angle);

                    //wpParentPos = screenCenter + new Vector3(sin * 150f, cos * 150f, 0f);

                    //y = mx+b format
                    float m = cos / sin;

                    Vector3 screenBounds = screenCenter;


                    //Get Offscreen Padding Offset: (1 / Reference Resolution)
                    //This positions the boundary right along the edge once its gone offscreen
                    //This stops the bouncing back and passing beyond edges when icon reaches screen edge
                    //As long as Canvas > Reference Resolution is the same aspect ratio as the play mode screen, this will always align
                    parentPaddingX = 1 / canvasRefRez.x;
                    parentPaddingY = 1 / canvasRefRez.y;

                    screenBounds.x = screenCenter.x * (.999f - (parentSize.x * parentPaddingX));
                    screenBounds.y = screenCenter.y * (.999f - (parentSize.y * parentPaddingY));


                    //Check up and down first
                    if (cos > 0f)
                    {
                        //up
                        //Debug.Log("TOP area");
                        wpParentPos = new Vector3(-screenBounds.y / m, screenBounds.y, 0f);
                    }
                    else
                    {
                        //down
                        //Debug.Log("BOT area");
                        wpParentPos = new Vector3(screenBounds.y / m, -screenBounds.y, 0f);
                    }

                    //If out of bounds, get point on appropriate side
                    if (wpParentPos.x > screenBounds.x) //Out of bounds! Must be on the right
                    {
                        //Debug.Log("LEFT area");
                        wpParentPos = new Vector3(screenBounds.x, -screenBounds.x * m, 0f);
                    }
                    else if (wpParentPos.x < -screenBounds.x) //Out of bounds! Must be on the left
                    {
                        //Debug.Log("RIGHT area");
                        wpParentPos = new Vector3(-screenBounds.x, screenBounds.x * m, 0);
                    }

                    //Remove coordinate translation
                    wpParentPos += screenCenter;


                    wpParent.transform.position = wpParentPos;
                    //Arrow point offscreen
                    //wpParent.transform.localRotation = Quaternion.Euler(0f, 0f, angle*Mathf.Rad2Deg);
                }




                //Size
                wpParent.sizeDelta = parentSize;

                //Color
                wpParentGameObject.GetComponent<Image>().color = boundaryBoxColor;

            }
            #endregion

            #region IF ENABLE ICON
            if (enableIcon)
            {
                //Set Size in real time
                newOnScreenSize.x = onScreenSpriteOriginalSize.x * onScreenSize;
                newOnScreenSize.y = onScreenSpriteOriginalSize.y * onScreenSize;

                newOffScreenSize.x = offScreenSpriteOriginalSize.x * offScreenSize;
                newOffScreenSize.y = offScreenSpriteOriginalSize.y * offScreenSize;

                if (waypointIconCreated && icon != null)
                {
                    if (waypointDist < displayRange) //ICON IN RANGE
                    {
                        //ICON ON-SCREEN -------------------------------------------------------------
                        if (parentOnScreen) //ICON ON-SCREEN
                        {
                            //Sprite
                            iconImage.sprite = onScreenSprite;

                            //Position
                            icon.position = new Vector2(wpParent.position.x + onScreenIconOffset.x, wpParent.position.y + onScreenIconOffset.y);

                            //Rotation
                            icon.transform.eulerAngles = new Vector3(0f, 0f, onScreenRotation);

                            //Size
                            if (onScreenHide) //HIDE
                            {
                                iconImage.rectTransform.localScale = new Vector2(0, 0);
                            }
                            else //SHOW
                            {
                                //FADE WITH RANGE
                                if (onScreenFadeWithRange)
                                {
                                    FadeWithRange();
                                }
                                else
                                {
                                    //Alpha set to 0 - We use SCALE to "Hide" icons, so no need to do any adjustments here
                                    iconAlphaValue = 1;
                                    iconColor.a = iconAlphaValue;
                                    iconImage.color = iconColor;
                                }

                                //SCALE WITH RANGE
                                if (onScreenScaleWithRange)
                                {
                                    ScaleWithRange(newOnScreenSize);
                                }
                                else //Use user Scale
                                {
                                    iconImage.rectTransform.localScale = new Vector2(newOnScreenSize.x, newOnScreenSize.y);
                                }

                            }
                        }

                        //ICON OFF-SCREEN -------------------------------------------------------------
                        if (!parentOnScreen)
                        {
                            //Sprite
                            iconImage.sprite = offScreenSprite;

                            //Position
                            icon.position = new Vector2(wpParent.position.x + offScreenIconOffset.x, wpParent.position.y + offScreenIconOffset.y);

                            //Rotation
                            if (offScreenRotates)
                            {
                                icon.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg + offScreenRotation);
                            }
                            else
                            {
                                icon.transform.eulerAngles = new Vector3(0f, 0f, offScreenRotation);
                            }

                            //Size
                            if (offScreenHide) //HIDE
                            {
                                iconImage.rectTransform.localScale = new Vector2(0, 0);
                            }
                            else //SHOW
                            {
                                //FADE WITH RANGE
                                if (offScreenFadeWithRange)
                                {
                                    FadeWithRange();
                                }
                                else
                                {
                                    //Alpha set to 0 - We use SCALE to "Hide" icons, so no need to do any adjustments here
                                    iconAlphaValue = 1;
                                    iconColor.a = iconAlphaValue;
                                    iconImage.color = iconColor;
                                }

                                //SCALE WITH RANGE
                                if (offScreenScaleWithRange)
                                {
                                    ScaleWithRange(newOffScreenSize);
                                }
                                else //Use user Scale
                                {
                                    iconImage.rectTransform.localScale = new Vector2(newOffScreenSize.x, newOffScreenSize.y);
                                }
                            }
                        }
                    }
                    else //ICON OUT OF RANGE
                    {
                        iconImage.rectTransform.localScale = new Vector2(0, 0);
                    }


                }
            }
            #endregion

            #region IF ENABLE TEXT
            if (enableText)
            {
                if (waypointTextCreated && textGameObject != null)
                {
                    if (waypointDist < displayRange) //TEXT IN RANGE
                    {
                        //GLOBAL-----
                        //FONT
                        textField.font = textFont;
                        //SIZE
                        textField.fontSize = textSize;
                        //COLOR
                        textField.color = textColor;
                        //ALIGN
                        switch (textAlign)
                        {
                            case textAlignValue.Left:
                                //Debug.Log("Left was chosen.");
                                textField.alignment = TextAlignmentOptions.Left;
                                break;
                            case textAlignValue.Center:
                                //Debug.Log("Center was chosen.");
                                textField.alignment = TextAlignmentOptions.Center;
                                break;
                            case textAlignValue.Right:
                                //Debug.Log("Right was chosen.");
                                textField.alignment = TextAlignmentOptions.Right;
                                break;
                        }
                        //LINE SPACING
                        textField.lineSpacing = textLineSpacing;
                        //SIZE
                        textField.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textRectWidth);
                        textField.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textRectHeight);


                        // TEXT ONSCREEN ---------------------------------------------------
                        if (parentOnScreen) //ICON ON-SCREEN
                        {
                            //POSITION
                            textGameObject.transform.localPosition = onScreenTextOffset;

                            //DESCRIPTION / DISTANCE
                            if (!onScreenHideDesc && !onScreenHideDist) //SHOW BOTH
                            {
                                textField.text = textDescription + "\n" + waypointDist.ToString() + "m";
                            }
                            if (onScreenHideDesc && !onScreenHideDist) //SHOW DISTNACE ONLY
                            {
                                textField.text = waypointDist.ToString() + "m";
                            }
                            if (!onScreenHideDesc && onScreenHideDist) //SHOW DESCRIPTION ONLY
                            {
                                textField.text = textDescription;
                            }
                            if (onScreenHideDesc && onScreenHideDist) //SHOW NO TEXT
                            {
                                textField.text = "";
                            }
                        }

                        // TEXT OFFSCREEN ---------------------------------------------------
                        if (!parentOnScreen)
                        {
                            //POSITION
                            textGameObject.transform.localPosition = offScreenTextOfffset;

                            //DESCRIPTION / DISTANCE
                            if (!offScreenHideDesc && !offScreenHideDist) //SHOW BOTH
                            {
                                textField.text = textDescription + "\n" + waypointDist.ToString() + "m";
                            }
                            if (offScreenHideDesc && !offScreenHideDist) //SHOW DISTNACE ONLY
                            {
                                textField.text = waypointDist.ToString() + "m";
                            }
                            if (!offScreenHideDesc && offScreenHideDist) //SHOW DESCRIPTION ONLY
                            {
                                textField.text = textDescription;
                            }
                            if (offScreenHideDesc && offScreenHideDist) //SHOW NO TEXT
                            {
                                textField.text = "";
                            }
                        }
                    }
                    else  //TEXT OUT OF RANGE
                    {
                        textField.text = "";
                    }


                }
            }
            #endregion
        }

    }



    #region Functions
    //CREATE PARENT UI 
    void InstantiateWaypointParent()
    {
        if (mainCanvas != null)
        {
            wpParentGameObject = new GameObject();
            wpParentGameObject.layer = 5;
            wpParent = wpParentGameObject.AddComponent<RectTransform>();
            wpParent.transform.SetParent(mainCanvas.transform);
            wpParent.name = gameObject.name + GetInstanceID() + ": wpParent";
            wpParentImage = wpParent.gameObject.AddComponent<Image>();
            wpParentImage.color = boundaryBoxColor;


            if (wpParentPos.z > 0f &&
            wpParentPos.x > (wpParent.sizeDelta.x / 2) && wpParentPos.x < Screen.width - (wpParent.sizeDelta.x / 2) &&
            wpParentPos.y > (wpParent.sizeDelta.x / 2) && wpParentPos.y < Screen.height - (wpParent.sizeDelta.x / 2))
            {
                //On screen
                if (iScreenCheck == 1)
                {
                    //Debug.Log("On Screen");
                    parentOnScreen = true;
                    iScreenCheck--;
                }
            }
            else
            {
                //Off screen
                if (iScreenCheck == 0)
                {
                    //Debug.Log("Off Screen");
                    parentOnScreen = false;
                    iScreenCheck++;
                }
            }


            waypointParentCreated = true;
        }

    }


    //CREATE ICON UI 
    void InstantiateWaypointIcon()
    {
        if (onScreenSprite != null)
        {
            iconGameObject = new GameObject();
            iconGameObject.layer = 5;
            icon = iconGameObject.AddComponent<RectTransform>();
            icon.position = wpParent.position;
            iconGameObject.transform.SetParent(wpParentGameObject.transform);
            icon.name = gameObject.name + ": wpIcon";
            iconImage = icon.gameObject.AddComponent<Image>();
            iconImage.sprite = onScreenSprite;
            iconImage.rectTransform.localScale = new Vector2(newOnScreenSize.x, newOnScreenSize.y); //user set scale

            //Set the iconColorarency of an image component that conatains sprites/textures
            iconColor = iconImage.color;
            iconColor.a = 1f; //Default is transparent on spawn
            iconImage.color = iconColor;

            waypointIconCreated = true;
        }
        else
        {
            Debug.LogWarning("Waypoint sprite missing on game object: " + gameObject.name + "\nYou might have Enable Icon checked, uncheck to remove this warning and not display an icon. Otherwise, to add a sprite, go to: " + gameObject.name + "/Inspector/Waypoint_Indicator (Script)/On Screen Image.");
        }

    }


    //CREATE TEXT UI 
    void InstantiateWaypointText()
    {
        textGameObject = new GameObject();
        textGameObject.layer = 5;
        textGameObject.transform.SetParent(wpParentGameObject.transform);
        textField = textGameObject.AddComponent<TextMeshProUGUI>();
        textField.transform.position = wpParent.position;
        textField.name = gameObject.name + ": wpText";
        textField.fontSize = textSize;
        textField.font = textFont;
        textField.color = textColor;
        textField.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textRectWidth); //!!! MAKE THIS WIDTH FIT THE TEXT CHAR LENGTH
        textField.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textRectHeight);
        textField.rectTransform.localScale = new Vector2(1f, 1f);

        waypointTextCreated = true;
    }








    void FadeWithRange()
    {

        if (waypointDist < displayRange)
        {
            //Alpha in real time based off dist
            iconAlphaValue = 1 - (waypointDist / displayRange); //Derrived from "Waypoint Math" on Google Sheets

            //Adjust Alpha
            iconColor.a = iconAlphaValue;
            iconImage.color = iconColor;
        }
        else //Waypoint is beyond the player preset, so set scale
        {
            //Alpha set to 0
            iconAlphaValue = 0;
            iconColor.a = iconAlphaValue;
            iconImage.color = iconColor;
        }

    }




    void ScaleWithRange(Vector2 OnOffScreenSize) //This should take onScreenSize and offScreenSize arguments
    {
        if (waypointDist < displayRange)
        {
            //Scale in real time based off dist
            scaleValueX = OnOffScreenSize.x - ((OnOffScreenSize.x / displayRange) * waypointDist); //Derrived from "Waypoint Math" on Google Sheets
            scaleValueY = OnOffScreenSize.y - ((OnOffScreenSize.y / displayRange) * waypointDist);

            //icon.localScale = new Vector2(scaleValueX, scaleValueY);
            iconImage.rectTransform.localScale = new Vector2(scaleValueX, scaleValueY);
        }
        else //Waypoint is beyond the player preset, so set scale and alpha to 0
        {
            //Scale set to 0
            iconImage.rectTransform.localScale = new Vector2(0f, 0f);
        }
    }




    //DESTROY WAYPOINT PARENT
    void DestroyWaypointParent()
    {
        GameObject waypointParent = GameObject.Find(wpParent.name);
        if (waypointParent != null)
        {
            Destroy(waypointParent);
        }
    }

    //DESTROY WAYPOINT ICON
    void DestroyWaypointIcon()
    {
        GameObject waypointIcon = GameObject.Find(gameObject.name + ": wpIcon");
        if (waypointIcon != null)
        {
            Destroy(waypointIcon);
        }
    }

    //DESTROY WAYPOINT TEXT
    void DestroyWaypointText()
    {
        GameObject waypointText = GameObject.Find(gameObject.name + ": wpText");
        if (waypointText != null)
        {
            Destroy(waypointText);
        }
    }
    #endregion
}
