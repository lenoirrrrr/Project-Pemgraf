using UnityEngine;
using TMPro;
using System.Collections;

public class NPCInteraction : MonoBehaviour
{
    [Header("UI Components")]
    public GameObject interactionPanel; 
    public GameObject dialogPanel;      
    public TextMeshProUGUI dialogText;  

    [Header("Dialog Settings")]
    [TextArea(3, 10)]
    public string[] sentences; 
    public float typingSpeed = 0.05f; 

    private bool isPlayerInRange = false;
    private bool isTalking = false;
    private int currentSentenceIndex = 0;
    private Coroutine typingCoroutine;

    void Start()
    {
        // Standar Arsitektur: Pastikan state awal bersih
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
        // SAFETY CHECK: Cegah IndexOutOfRangeException
        if (sentences == null || sentences.Length == 0)
        {
            Debug.LogError("KING, Anda lupa mengisi Sentences di Inspector NPC!");
            return;
        }

        isTalking = true;
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (dialogPanel != null) dialogPanel.SetActive(true);
        
        currentSentenceIndex = 0;
        
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = ""; 
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter; 
            yield return new WaitForSeconds(typingSpeed); 
        }
        typingCoroutine = null; 
    }

    void NextSentence()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogText.text = sentences[currentSentenceIndex];
            typingCoroutine = null;
            return;
        }

        currentSentenceIndex++;

        if (currentSentenceIndex < sentences.Length)
        {
            typingCoroutine = StartCoroutine(TypeSentence(sentences[currentSentenceIndex]));
        }
        else
        {
            EndDialog();
        }
    }

    public void EndDialog()
    {
        isTalking = false;
        if (dialogPanel != null) dialogPanel.SetActive(false);
        
        // LOGIC FIX: Hanya nyalakan prompt jika player MASIH di dalam area
        if (isPlayerInRange && interactionPanel != null) 
        {
            interactionPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isTalking && interactionPanel != null) interactionPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPanel != null) interactionPanel.SetActive(false); // Matikan paksa
            EndDialog(); 
        }
    }

    public bool IsTalking() 
    { 
        return isTalking; 
    }
} // Kurung kurawal penutup class
