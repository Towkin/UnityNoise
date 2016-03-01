using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerateGlobe))]
public class GeneratorGlobeInspector : Editor {

    private GenerateGlobe m_Generator;
    private GenerateGlobe Generator
    {
        get { return m_Generator; }
        set { m_Generator = value; }
    }

    private void OnEnable()
    {
        Generator = target as GenerateGlobe;
        Undo.undoRedoPerformed += RefreshCreator;
    }
    private void OnDisable()
    {
        Undo.undoRedoPerformed -= RefreshCreator;
    }
    private void RefreshCreator()
    {
        if (Application.isPlaying)
        {
            Generator.Generate();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck() && Application.isPlaying)
        {
            RefreshCreator();
        }
    }
}
