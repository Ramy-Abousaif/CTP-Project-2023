using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    Planet planet;
    Editor shapeEditor;
    Editor colourEditor;
    Editor atmosphereEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if(check.changed)
                planet.GeneratePlanet();
        }

        if(GUILayout.Button("Manually Force Planet Update"))
            planet.GeneratePlanet();

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsChange, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsChange, ref planet.colourSettingsFoldout, ref colourEditor);
        DrawSettingsEditor(planet.atmosphereSettings, planet.OnAtmosphereSettingsChange, ref planet.atmosphereSettingsFoldout, ref atmosphereEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsChanged, ref bool foldout, ref Editor editor)
    {
        if (settings == null)
            return;

        foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            if(foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    if (onSettingsChanged != null)
                        onSettingsChanged();
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}
