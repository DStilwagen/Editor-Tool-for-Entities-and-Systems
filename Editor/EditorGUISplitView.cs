using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/// <summary>
/// Originally from https://github.com/miguel12345/EditorGUISplitView
/// </summary>
public class EditorGUISplitView
{

	public enum Direction {
		Horizontal,
		Vertical
	}

	Direction splitDirection;
	float splitNormalizedPosition;
    public float SplitNormalizedPosition
    {
        get
        {
            if (float.IsNaN(splitNormalizedPosition))
                return splitDirection == Direction.Horizontal ? availableRect.width * 0.5f : availableRect.height * 0.5f;
            return splitNormalizedPosition;
        }
    }
	bool resize;
	public Vector2 scrollPosition;
	Rect availableRect;


	public EditorGUISplitView(Direction splitDirection) {
		splitNormalizedPosition = float.NaN;
		this.splitDirection = splitDirection;
	}
	public void BeginSplitView() {
		Rect tempRect;
		if(splitDirection == Direction.Horizontal)
			tempRect = EditorGUILayout.BeginHorizontal (GUILayout.ExpandWidth(true));
		else 
			tempRect = EditorGUILayout.BeginVertical (GUILayout.ExpandHeight(true));
		
		if (tempRect.width > 0.0f) {
            availableRect = tempRect;
        }
		if(splitDirection == Direction.Horizontal)
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(SplitNormalizedPosition));
		else
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(SplitNormalizedPosition));
	}

	public void Split() {
		GUILayout.EndScrollView();
		ResizeSplitFirstView ();
	}

	public void EndSplitView() {

		if(splitDirection == Direction.Horizontal)
			EditorGUILayout.EndHorizontal ();
		else 
			EditorGUILayout.EndVertical ();
	}
    private static void DrawEmpty (Rect rect) {
        //Color col = EditorGUIUtility.isProSkin
                        //? (Color) new Color32 (56, 56, 56, 255)
                        //: (Color) new Color32 (194, 194, 194, 255);
        var col = (Color) new Color32(255, 255, 255, 50); 
        Texture2D tex = new Texture2D (1, 1);
        tex.SetPixel (0, 0, col);
        tex.Apply ();
        GUI.DrawTexture(rect, tex);
    }

	private void ResizeSplitFirstView(){

		Rect resizeHandleRect;
        var lastRect = GUILayoutUtility.GetLastRect();
        if (splitDirection == Direction.Horizontal)
            resizeHandleRect = new Rect(lastRect.width, lastRect.y, 2f, lastRect.height);
        else
            resizeHandleRect = new Rect(lastRect.x, lastRect.height + lastRect.y, lastRect.width, 2f);
        
        DrawEmpty(resizeHandleRect);

		if(splitDirection == Direction.Horizontal)
			EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeHorizontal);
		else
			EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeVertical);

		if( Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition)){
			resize = true;
		}
		if(resize)
        {
            if (splitDirection == Direction.Horizontal)
                splitNormalizedPosition = Event.current.mousePosition.x - lastRect.x;
            else
                splitNormalizedPosition = Event.current.mousePosition.y - lastRect.y;
        }
		if(Event.current.type == EventType.MouseUp)
			resize = false;        
	}
}

