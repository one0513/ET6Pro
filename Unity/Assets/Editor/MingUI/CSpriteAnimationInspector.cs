using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (CSpriteAnimation))]
public class CSpriteAnimationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        CSpriteAnimation anim = target as CSpriteAnimation;
        if (anim == null) return;

        int fps = EditorGUILayout.IntField("Framerate", anim.FramesPerSecond);
        fps = Mathf.Clamp(fps, 0, 60);
        if (anim.FramesPerSecond != fps)
        {
            anim.FramesPerSecond = fps;
            EditorUtility.SetDirty(anim);
        }

        string namePrefix = EditorGUILayout.TextField("Name Prefix", anim.NamePrefix ?? "");

        if (anim.NamePrefix != namePrefix)
        {
            anim.NamePrefix = namePrefix;
            EditorUtility.SetDirty(anim);
        }

        bool loop = EditorGUILayout.Toggle("Loop", anim.Loop);

        if (anim.Loop != loop)
        {
            anim.Loop = loop;
            EditorUtility.SetDirty(anim);
        }

        bool iregular = EditorGUILayout.Toggle("_irregularSprite", anim.IsIrregularSprite);

        if (anim.IsIrregularSprite != iregular) {
            anim.IsIrregularSprite = iregular;
            EditorUtility.SetDirty(anim);
        }

        bool isFlipX = EditorGUILayout.Toggle("isFlipX", anim.IsFlipX);

        if (anim.IsFlipX != isFlipX) {
            anim.IsFlipX = isFlipX;
            EditorUtility.SetDirty(anim);
        }
    }
}