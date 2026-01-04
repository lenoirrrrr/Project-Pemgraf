using UnityEngine;
using TMPro; // WAJIB ADA untuk akses TextMeshPro
using System.Collections; // WAJIB ADA untuk Coroutine

public class NPCInteraction : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject interactionPanel; // Panel UI "Press E" (yang lama)
    public GameObject dialogPanel;      // Panel UI baru untuk teks dialog (Opsional, bisa sama)
    public TextMeshProUGUI dialogText;  // Komponen Text TMP untuk dialog

    [Header("Dialog Settings")]
    [TextArea(3, 10)] // Membuat area ketik di inspector jadi lega
    public string[] sentences; // Array untuk menyimpan kalimat dialog
    public float typingSpeed = 0.05f; // Kecepatan ketik (makin kecil makin cepat)

    private bool isPlayerInRange = false;
    private bool isTalking = false;
    private int currentSentenceIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        // Pastikan semua UI mati di awal
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (dialogPanel != null) dialogPanel.SetActive(false);
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalking)
            {
                StartDialog();
            }
            else
            {
                NextSentence();
            }
        }
    }

    void StartDialog()
    {
        isTalking = true;
        interactionPanel.SetActive(false); // Sembunyikan prompt "Press E"
        dialogPanel.SetActive(true); // Munculkan panel dialog
        currentSentenceIndex = 0;
        
        // Mulai mengetik kalimat pertama
        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
        
        // [OPSIONAL] Beritahu script AI untuk berhenti bergerak
        // GetComponent<NPCWanderAI>()?.SetTalkingState(true);
    }

    // --- INI INTI DARI TYPEWRITER EFFECT ---
    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = ""; // Kosongkan teks dulu
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter; // Tambahkan 1 huruf
            yield return new WaitForSeconds(typingSpeed); // Tunggu sebentar
        }
        typingCoroutine = null; // Tandai bahwa mengetik sudah selesai
    }
    // ---------------------------------------

    void NextSentence()
    {
        // Jika sedang mengetik, selesaikan langsung kalimatnya (fitur skip)
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = sentences[currentSentenceIndex];
            typingCoroutine = null;
            return;
        }

        currentSentenceIndex++; // Pindah ke kalimat berikutnya

        if (currentSentenceIndex < sentences.Length)
        {
            // Ketik kalimat berikutnya
            typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            EndDialog(); // Dialog habis
        }
    }

    void EndDialog()
    {
        isTalking = false;
        dialogPanel.SetActive(false);
        interactionPanel.SetActive(true); // Munculkan lagi prompt "Press E" jika masih dekat

        // [OPSIONAL] Beritahu script AI boleh jalan lagi
        // GetComponent<NPCWanderAI>()?.SetTalkingState(false);
    }

    // --- Trigger Detection (Sama seperti sebelumnya) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isTalking) interactionPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            interactionPanel.SetActive(false);
            EndDialog(); // Paksa berhenti jika player menjauh
        }
    }
    // Getter untuk script AI nanti
    public bool IsTalking() { return isTalking; }
}