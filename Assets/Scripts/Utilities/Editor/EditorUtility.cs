using UnityEditor;

public class EditorUtility
{
	[MenuItem("Assets/Create/EnergyData")]
	public static void CreateAsset()
	{
		ScriptableObjectUtility.CreateAsset<EnergyRegulator.EnergyData>();
	}
}
