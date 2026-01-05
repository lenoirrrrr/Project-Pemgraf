using UnityEngine;
using TMPro;

public class ArtifactInspector : MonoBehaviour
{
    public float reachDistance = 20f;
    public LayerMask inspectableLayer;

    [Header("UI Reference")]
    public GameObject inspectionPanel; // Panel khusus info artefak
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descText;

    private bool isInspecting = false;


    void Start() { Debug.Log("KING: Sistem Inspeksi Aktif di Kamera!"); }

    void Update()
    {
        // Jika sedang inspeksi, tekan Escape untuk keluar
        if (isInspecting && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInspection();
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CastRay();
        }
    }

    void CastRay()
{
    Ray ray = new Ray(transform.position, transform.forward);
    RaycastHit hit;

    // Gambar laser di Scene View agar Anda bisa melihat arah bidikan
    Debug.DrawRay(transform.position, transform.forward * reachDistance, Color.red, 2f);

    if (Physics.Raycast(ray, out hit, reachDistance)) // Lepas InspectableLayer dulu untuk testing
    {
        Debug.Log("KING: Laser mengenai objek -> " + hit.collider.name);
        Debug.Log("KING: Layer objek tersebut adalah -> " + LayerMask.LayerToName(hit.collider.gameObject.layer));

        // Cek apakah laser kena layer Inspectable
        if (((1 << hit.collider.gameObject.layer) & inspectableLayer) != 0)
        {
            ArtifactItem item = hit.collider.GetComponentInParent<ArtifactItem>();
            if (item != null)
            {
                ShowInspection(item.data);
            }
            else
            {
                Debug.LogWarning("KING: Kena layer Inspectable, tapi script ArtifactItem tidak ditemukan di " + hit.collider.name + " atau Parentnya!");
            }
        }
    }
    else
    {
        Debug.Log("KING: Laser tidak mengenai apapun. Cobalah lebih dekat!");
    }
}

    void ShowInspection(ArtifactData data)
    {
        isInspecting = true;
        inspectionPanel.SetActive(true);
        nameText.text = data.artifactName;
        descText.text = data.description;

        // Ilmu Mahal: Matikan kontrol pemain agar tidak bisa jalan saat baca
        Time.timeScale = 0f; // Pause game atau matikan script PlayerController
        Cursor.lockState = CursorLockMode.None;
    }

    void CloseInspection()
    {
        isInspecting = false;
        inspectionPanel.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    
}