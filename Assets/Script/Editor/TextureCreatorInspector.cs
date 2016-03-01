using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureCreator))]
public class TextureCreatorInspector : Editor {

    private TextureCreator m_Creator;
    private TextureCreator Creator
    {
        get { return m_Creator; }
        set { m_Creator = value; }
    }

    private void OnEnable()
    {
        Creator = target as TextureCreator;
        Undo.undoRedoPerformed += RefreshCreator;
    }
    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RefreshCreator;
    }
    private void RefreshCreator()
    {
        if(Application.isPlaying)
        {
            Creator.FillTexture();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if(EditorGUI.EndChangeCheck() && Application.isPlaying)
        {
            RefreshCreator();
        }
    }
}
