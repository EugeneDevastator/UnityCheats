public class StyleLister : MonoBehaviour { }

[CustomEditor(typeof(EditroTestMono))]
public class EditroTestMonoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var props = typeof(EditorStyles).GetProperties(BindingFlags.Public | BindingFlags.Static);
        foreach (var propertyInfo in props.Where(p => p.PropertyType == typeof(GUIStyle)))
        {
            EditorGUILayout.LabelField(propertyInfo.Name, (GUIStyle)propertyInfo.GetValue(null));
        }
    }
}