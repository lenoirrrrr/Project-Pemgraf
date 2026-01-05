using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Museum/Artifact Data")]
public class ArtifactData : ScriptableObject
{
    public string artifactName;
    [TextArea(5, 10)]
    public string description;
    public GameObject artifactModel; // Versi high-poly untuk inspeksi (opsional)
}

