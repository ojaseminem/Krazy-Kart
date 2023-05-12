using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;


#region Custom Inspector
[ExecuteInEditMode] //Runs in real time
[CustomEditor(typeof(Waypoint_Indicator)), CanEditMultipleObjects]
public class Waypoint_Indicator_Editor : Editor
{
    //Reference MonoBehaviour class in this script
    Waypoint_Indicator targetScript;

    string parentOptionsTitle;
    string parentOptionsTitleOpen = "   [-] Parent Options";
    string parentOptionsTitleClosed = "   [+] Parent Options";

    string spriteOptionsTitle;
    string spriteOptionsTitleOpen = "   [-] Sprite Indicator Options";
    string spriteOptionsTitleClosed = "   [+] Sprite Indicator Options";

    string objectOptionsTitle;
    string objectOptionsTitleOpen = "   [-] Prefab Indicator Options";
    string objectOptionsTitleClosed = "   [+] Prefab Indicator Options";

    string textOptionsTitle;
    string textOptionsTitleOpen = "   [-] Text Options";
    string textOptionsTitleClosed = "   [+] Text Options";

    string diameterOptionsTitle;
    string diameterOptionsTitleOpen = "   [-] Diameter Options";
    string diameterOptionsTitleClosed = "   [+] Diameter Options";

    string spriteCenteredOptionsTitle;
    string spriteCenteredOptionsTitleOpen = "   [-] Sprite Options";
    string spriteCenteredOptionsTitleClosed = "   [+] Sprite Options";

    string circleGizmoOptionsTitle;
    string circleGizmoOptionsTitleOpen = "   [-] 2D Circle Gizmo";
    string circleGizmoOptionsTitleClosed = "   [+] 2D Circle Gizmo";


    public override void OnInspectorGUI()
    {
        //Do this first to make sure you have the latest version
        serializedObject.Update();
        targetScript = (Waypoint_Indicator)target;


        EditorGUILayout.Separator();


        //GUILayout.BeginHorizontal("box");
        //GUILayout.EndHorizontal();
        //GUILayout.BeginVertical("box");
        //GUILayout.EndVertical();
        //EditorGUI.indentLevel++;

        #region Screen Edge Tracking
        //Screen Edge Tracking
        GUILayout.BeginVertical("box");
        serializedObject.FindProperty("enableStandardTracking").boolValue = EditorGUILayout.ToggleLeft(" Standard Tracking", targetScript.enableStandardTracking, EditorStyles.boldLabel);

        //Disaply these elements based off wether or not Standard Tracking is checked or not
        if (targetScript.enableStandardTracking)
        {
            EditorGUILayout.Separator();

            //PARENT OPTIONS


            serializedObject.FindProperty("toggleParentOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleParentOptions, parentOptionsTitle, EditorStyles.largeLabel);

            if (targetScript.toggleParentOptions)
            {
                parentOptionsTitle = parentOptionsTitleOpen;
                EditorGUI.indentLevel++;
                EditorGUILayout.Separator();
                serializedObject.FindProperty("showBoundaryBox").boolValue = EditorGUILayout.Toggle("Show Boundary", targetScript.showBoundaryBox);
                serializedObject.FindProperty("boundaryBoxColor").colorValue = EditorGUILayout.ColorField("Boundary Color", targetScript.boundaryBoxColor);
                serializedObject.FindProperty("parentSize").vector2IntValue = EditorGUILayout.Vector2IntField("Boundary Size", targetScript.parentSize);
                serializedObject.FindProperty("displayRange").floatValue = EditorGUILayout.FloatField("Display Range", targetScript.displayRange);
                EditorGUILayout.Separator();
                EditorGUI.indentLevel--;
            }
            else
            {
                parentOptionsTitle = parentOptionsTitleClosed;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);


            //SPRITE OPTIONS
            serializedObject.FindProperty("toggleSpriteOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleSpriteOptions, spriteOptionsTitle, EditorStyles.largeLabel);
            if (targetScript.toggleSpriteOptions)
            {
                spriteOptionsTitle = spriteOptionsTitleOpen;
                EditorGUILayout.Separator();

                EditorGUI.indentLevel++;
                serializedObject.FindProperty("enableSprite").boolValue = EditorGUILayout.ToggleLeft("Enable Sprite", targetScript.enableSprite);
                
                //Disaply these elements based off Display drop down (onScreenIconType)
                if (targetScript.enableSprite)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    serializedObject.FindProperty("spriteDepth").intValue = EditorGUILayout.IntField("Depth", targetScript.spriteDepth);
                    serializedObject.FindProperty("offScreenSpriteRotates").boolValue = EditorGUILayout.Toggle("Off Screen Rotates", targetScript.offScreenSpriteRotates);
                    EditorGUILayout.Separator();

                    EditorGUILayout.Separator();
                    //ICON ON-SCREEN
                    EditorGUILayout.LabelField("Sprite On-Screen", EditorStyles.boldLabel);


                    serializedObject.FindProperty("onScreenSprite").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Sprite"), targetScript.onScreenSprite, typeof(Sprite), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    serializedObject.FindProperty("onScreenSpriteColor").colorValue = EditorGUILayout.ColorField("Color", targetScript.onScreenSpriteColor);
                    serializedObject.FindProperty("onScreenSpriteSize").floatValue = EditorGUILayout.FloatField("Size", targetScript.onScreenSpriteSize);
                    serializedObject.FindProperty("onScreenSpriteOffset").vector2Value = EditorGUILayout.Vector2Field("Position", targetScript.onScreenSpriteOffset);
                    serializedObject.FindProperty("onScreenSpriteRotation").floatValue = EditorGUILayout.Slider("Rotation", targetScript.onScreenSpriteRotation, 0, 360);
                    serializedObject.FindProperty("onScreenSpriteFadeWithRange").boolValue = EditorGUILayout.Toggle("Fade with Range", targetScript.onScreenSpriteFadeWithRange);
                    serializedObject.FindProperty("onScreenSpriteScaleWithRange").boolValue = EditorGUILayout.Toggle("Scale with Range", targetScript.onScreenSpriteScaleWithRange);
                    serializedObject.FindProperty("onScreenSpriteHide").boolValue = EditorGUILayout.Toggle("Hide", targetScript.onScreenSpriteHide);


                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();



                    //ICON OFF-SCREEN
                    EditorGUILayout.LabelField("Sprite Off-Screen", EditorStyles.boldLabel);


                    serializedObject.FindProperty("offScreenSprite").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Sprite"), targetScript.offScreenSprite, typeof(Sprite), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    serializedObject.FindProperty("offScreenSpriteColor").colorValue = EditorGUILayout.ColorField("Color", targetScript.offScreenSpriteColor);
                    serializedObject.FindProperty("offScreenSpriteSize").floatValue = EditorGUILayout.FloatField("Size", targetScript.offScreenSpriteSize);
                    serializedObject.FindProperty("offScreenSpriteOffset").vector2Value = EditorGUILayout.Vector2Field("Position", targetScript.offScreenSpriteOffset);
                    serializedObject.FindProperty("offScreenSpriteRotation").floatValue = EditorGUILayout.Slider("Rotation", targetScript.offScreenSpriteRotation, 0, 360);
                    serializedObject.FindProperty("offScreenSpriteFadeWithRange").boolValue = EditorGUILayout.Toggle("Fade with Range", targetScript.offScreenSpriteFadeWithRange);
                    serializedObject.FindProperty("offScreenScaleWithRange").boolValue = EditorGUILayout.Toggle("Scale with Range", targetScript.offScreenScaleWithRange);
                    serializedObject.FindProperty("offScreenSpriteHide").boolValue = EditorGUILayout.Toggle("Hide", targetScript.offScreenSpriteHide);


                    EditorGUILayout.Separator();
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.HelpBox("Check Enable Sprite for options.", MessageType.Info);
                }
                EditorGUI.indentLevel--;

            }
            else
            {
                spriteOptionsTitle = spriteOptionsTitleClosed;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.Separator();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);





            //GAME OBJECT OPTIONS (Prefabs)
            serializedObject.FindProperty("toggleGameObjectOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleGameObjectOptions, objectOptionsTitle, EditorStyles.largeLabel);
            if (targetScript.toggleGameObjectOptions)
            {
                objectOptionsTitle = objectOptionsTitleOpen;
                EditorGUILayout.Separator();

                EditorGUI.indentLevel++;
                serializedObject.FindProperty("enableGameObject").boolValue = EditorGUILayout.ToggleLeft("Enable Object", targetScript.enableGameObject);
                //Disaply these elements based off Display drop down (onScreenIconType)
                if (targetScript.enableGameObject)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    serializedObject.FindProperty("gameObjectDepth").intValue = EditorGUILayout.IntField("Depth", targetScript.gameObjectDepth);
                    serializedObject.FindProperty("offScreenObjectRotates").boolValue = EditorGUILayout.Toggle("Off Screen Rotates", targetScript.offScreenObjectRotates);

                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();

                    //OBJECT ON-SCREEN
                    EditorGUILayout.LabelField("Prefab On-Screen", EditorStyles.boldLabel);


                    serializedObject.FindProperty("onScreenGameObject").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Prefab"), targetScript.onScreenGameObject, typeof(GameObject), true);
                    serializedObject.FindProperty("onScreenGameObjectColor").colorValue = EditorGUILayout.ColorField("Color", targetScript.onScreenGameObjectColor);
                    serializedObject.FindProperty("onScreenGameObjectSize").floatValue = EditorGUILayout.FloatField("Size", targetScript.onScreenGameObjectSize);
                    serializedObject.FindProperty("onScreenGameObjectOffset").vector2Value = EditorGUILayout.Vector2Field("Position", targetScript.onScreenGameObjectOffset);
                    serializedObject.FindProperty("onScreenGameObjectRotation").floatValue = EditorGUILayout.Slider("Rotation", targetScript.onScreenGameObjectRotation, 0, 360);
                    serializedObject.FindProperty("onScreenGameObjectFadeWithRange").boolValue = EditorGUILayout.Toggle("Fade with Range", targetScript.onScreenGameObjectFadeWithRange);
                    serializedObject.FindProperty("onScreenGameObjectScaleWithRange").boolValue = EditorGUILayout.Toggle("Scale with Range", targetScript.onScreenGameObjectScaleWithRange);
                    serializedObject.FindProperty("onScreenGameObjectHide").boolValue = EditorGUILayout.Toggle("Hide", targetScript.onScreenGameObjectHide);


                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();


                    //OBJECT OFF-SCREEN
                    EditorGUILayout.LabelField("Prefab Off-Screen", EditorStyles.boldLabel);


                    serializedObject.FindProperty("offScreenGameObject").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Prefab"), targetScript.offScreenGameObject, typeof(GameObject), true);
                    serializedObject.FindProperty("offScreenGameObjectColor").colorValue = EditorGUILayout.ColorField("Color", targetScript.offScreenGameObjectColor);
                    serializedObject.FindProperty("offScreenGameObjectSize").floatValue = EditorGUILayout.FloatField("Size", targetScript.offScreenGameObjectSize);
                    serializedObject.FindProperty("offScreenGameObjectOffset").vector2Value = EditorGUILayout.Vector2Field("Position", targetScript.offScreenGameObjectOffset);
                    serializedObject.FindProperty("offScreenGameObjectRotation").floatValue = EditorGUILayout.Slider("Rotation", targetScript.offScreenGameObjectRotation, 0, 360);
                    serializedObject.FindProperty("offScreenGameObjectFadeWithRange").boolValue = EditorGUILayout.Toggle("Fade with Range", targetScript.offScreenGameObjectFadeWithRange);
                    serializedObject.FindProperty("offScreenGameObjectScaleWithRange").boolValue = EditorGUILayout.Toggle("Scale with Range", targetScript.offScreenGameObjectScaleWithRange);
                    serializedObject.FindProperty("offScreenGameObjectHide").boolValue = EditorGUILayout.Toggle("Hide", targetScript.offScreenGameObjectHide);


                    EditorGUILayout.Separator();
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.HelpBox("Check Enable Object for options.", MessageType.Info);
                }
                EditorGUI.indentLevel--;

            }
            else
            {
                objectOptionsTitle = objectOptionsTitleClosed;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Separator();

            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);





            //Text Options
            serializedObject.FindProperty("toggleTextOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleTextOptions, textOptionsTitle, EditorStyles.largeLabel);
            if (targetScript.toggleTextOptions)
            {
                textOptionsTitle = textOptionsTitleOpen;
                EditorGUILayout.Separator();

                EditorGUI.indentLevel++;
                serializedObject.FindProperty("enableText").boolValue = EditorGUILayout.ToggleLeft("Enable Text", targetScript.enableText);


                //Disaply these elements based off Display drop down (onScreenIconType)
                if (targetScript.enableText)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    serializedObject.FindProperty("textDepth").intValue = EditorGUILayout.IntField("Depth", targetScript.textDepth);
                    serializedObject.FindProperty("textDescription").stringValue = EditorGUILayout.TextField("Description", targetScript.textDescription);
                    serializedObject.FindProperty("textFont").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Font Face"), targetScript.textFont, typeof(TMP_FontAsset), true);
                    serializedObject.FindProperty("textSize").floatValue = EditorGUILayout.FloatField("Font Size", targetScript.textSize);
                    serializedObject.FindProperty("textColor").colorValue = EditorGUILayout.ColorField("Font Color", targetScript.textColor);

                    serializedObject.FindProperty("textAlign").enumValueIndex = (int)(Waypoint_Indicator.textAlignValue)EditorGUILayout.EnumPopup("Text Align", targetScript.textAlign);
                    serializedObject.FindProperty("textLineSpacing").floatValue = EditorGUILayout.FloatField("Line Spacing", targetScript.textLineSpacing);
                    serializedObject.FindProperty("textRectWidth").floatValue = EditorGUILayout.FloatField("Width", targetScript.textRectWidth);
                    serializedObject.FindProperty("textRectHeight").floatValue = EditorGUILayout.FloatField("Height", targetScript.textRectHeight);

                    EditorGUILayout.Separator();

                    EditorGUILayout.LabelField("Text On-Screen", EditorStyles.boldLabel);
                    serializedObject.FindProperty("onScreenSpriteHideDesc").boolValue = EditorGUILayout.Toggle("Hide Desc", targetScript.onScreenSpriteHideDesc);
                    serializedObject.FindProperty("onScreenSpriteHideDist").boolValue = EditorGUILayout.Toggle("Hide Dist", targetScript.onScreenSpriteHideDist);
                    serializedObject.FindProperty("onScreenTextOffset").vector2Value = EditorGUILayout.Vector2Field("Position", targetScript.onScreenTextOffset);

                    EditorGUILayout.Separator();

                    EditorGUILayout.LabelField("Text Off-Screen", EditorStyles.boldLabel);
                    serializedObject.FindProperty("offScreenSpriteHideDesc").boolValue = EditorGUILayout.Toggle("Hide Desc", targetScript.offScreenSpriteHideDesc);
                    serializedObject.FindProperty("offScreenSpriteHideDist").boolValue = EditorGUILayout.Toggle("Hide Dist", targetScript.offScreenSpriteHideDist);
                    serializedObject.FindProperty("offScreenTextOffset").vector2Value = EditorGUILayout.Vector2Field("Position", targetScript.offScreenTextOffset);
                    EditorGUILayout.Separator();
                    EditorGUILayout.Separator();
                    EditorGUI.indentLevel--;
                }
                else
                {
                    EditorGUILayout.HelpBox("Check Enable Text for options.", MessageType.Info);
                }
                EditorGUI.indentLevel--;

            }
            else
            {
                textOptionsTitle = textOptionsTitleClosed;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            //Footer Spacers
            EditorGUILayout.Separator();
            
        }
        else
        {
            EditorGUILayout.HelpBox("Tracks objects in camera view to the edge of the screen.", MessageType.Info);
        }



        EditorGUILayout.EndFoldoutHeaderGroup();
        GUILayout.EndVertical();
        #endregion













        #region Screen Centered Tracking
        //SCREEN CENTERED TRACKING -----------------------------------------------------------------------
        EditorGUILayout.Separator();
        GUILayout.BeginVertical("box");
        serializedObject.FindProperty("enableCenteredTracking").boolValue = EditorGUILayout.ToggleLeft(" Centered Tracking", targetScript.enableCenteredTracking, EditorStyles.boldLabel);

        //Disaply these elements based off wether or not Centered Tracking is checked or not
        if (targetScript.enableCenteredTracking)
        {
            EditorGUILayout.Separator();
            //DIAMETER OPTIONS
            serializedObject.FindProperty("toggleDiameterOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleDiameterOptions, diameterOptionsTitle, EditorStyles.largeLabel);
            if (targetScript.toggleDiameterOptions)
            {
                diameterOptionsTitle = diameterOptionsTitleOpen;
                EditorGUI.indentLevel++;
                EditorGUILayout.Separator();
                serializedObject.FindProperty("showDiameter").boolValue = EditorGUILayout.Toggle("Show Diameter", targetScript.showDiameter);
                serializedObject.FindProperty("diameterColor").colorValue = EditorGUILayout.ColorField("Diameter Color", targetScript.diameterColor);
                serializedObject.FindProperty("diameterSize").floatValue = EditorGUILayout.FloatField("Diameter Size", targetScript.diameterSize);
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUI.indentLevel--;
            }
            else
            {
                diameterOptionsTitle = diameterOptionsTitleClosed;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.Separator();
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);



            //CENTERED SPRITE
            serializedObject.FindProperty("toggleCenteredSpriteOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleCenteredSpriteOptions, spriteCenteredOptionsTitle, EditorStyles.largeLabel);
            if (targetScript.toggleCenteredSpriteOptions)
            {
                spriteCenteredOptionsTitle = spriteCenteredOptionsTitleOpen;
                EditorGUI.indentLevel++;
                EditorGUILayout.Separator();
                serializedObject.FindProperty("onScreenCenteredRange").floatValue = EditorGUILayout.FloatField("Display Range", targetScript.onScreenCenteredRange);
                serializedObject.FindProperty("onScreenCenteredSprite").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Sprite"), targetScript.onScreenCenteredSprite, typeof(Sprite), true, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                serializedObject.FindProperty("onScreenCenteredSpriteColor").colorValue = EditorGUILayout.ColorField("Color", targetScript.onScreenCenteredSpriteColor);
                serializedObject.FindProperty("onScreenCenteredSpriteSize").floatValue = EditorGUILayout.FloatField("Size", targetScript.onScreenCenteredSpriteSize);
                serializedObject.FindProperty("onScreenCenteredSpriteRotation").floatValue = EditorGUILayout.Slider("Rotation", targetScript.onScreenCenteredSpriteRotation, 0, 360);
                serializedObject.FindProperty("onScreenSpriteCenteredFadeWithRange").boolValue = EditorGUILayout.Toggle("Fade with Range", targetScript.onScreenSpriteCenteredFadeWithRange);
                serializedObject.FindProperty("onScreenSpriteCenteredScaleWithRange").boolValue = EditorGUILayout.Toggle("Scale with Range", targetScript.onScreenSpriteCenteredScaleWithRange);
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUI.indentLevel--;
            }
            else
            {
                spriteCenteredOptionsTitle = spriteCenteredOptionsTitleClosed;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.Separator();
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);



            //RADIUS GIZMO OPTIONS
            serializedObject.FindProperty("toggleRadiusGizmoOptions").boolValue = EditorGUILayout.BeginFoldoutHeaderGroup(targetScript.toggleRadiusGizmoOptions, circleGizmoOptionsTitle, EditorStyles.largeLabel);
            if (targetScript.toggleRadiusGizmoOptions)
            {
                circleGizmoOptionsTitle = circleGizmoOptionsTitleOpen;
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Use this tool to align pointers that rotate around the exterior of a circular path. Scene View only. Must have Gizmos and 2D enabled.", MessageType.Info);
                EditorGUILayout.Separator();
                serializedObject.FindProperty("enableRadiusGizmo").boolValue = EditorGUILayout.Toggle("Show Gizmo", targetScript.enableRadiusGizmo);
                serializedObject.FindProperty("radiusGizmoColor").colorValue = EditorGUILayout.ColorField("Gizmo Color", targetScript.radiusGizmoColor);
                serializedObject.FindProperty("radiusGizmoSize").floatValue = EditorGUILayout.FloatField("Gizmo Size", targetScript.radiusGizmoSize);
                EditorGUILayout.Separator();
                EditorGUILayout.Separator();
                EditorGUI.indentLevel--;
            }
            else
            {
                circleGizmoOptionsTitle = circleGizmoOptionsTitleClosed;
            }
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        else
        {
            EditorGUILayout.HelpBox("Tracks objects along a circular perimeter originating from the center of the screen.", MessageType.Info);
        }

        GUILayout.EndVertical();
        #endregion



        //Footer Spacers
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        //do this last!  it will loop over the properties on your object and apply any it needs to, no if necessary!
        serializedObject.ApplyModifiedProperties();
    }
}
#endregion
