using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SinkWashInteraction : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    public float interactionDistance = 3f;   
    public Camera playerCamera;              
    public TMP_Text hintText;                    
    public Image fadeImage;                  
    public AudioSource waterSound;           
    public float fadeDuration = 1.5f;        
    public float washDuration = 3f;          

    private bool isNearSink = false;
    private bool isWashing = false;

    void Start()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0;
            fadeImage.color = c;
        }
    }

    void Update()
    {
        if (!isWashing)
        {
            CheckSink();
            HandleInput();
        }
    }

    void CheckSink()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Sink"))
            {
                if (!isNearSink)
                {
                    isNearSink = true;
                    hintText.gameObject.SetActive(true);
                    hintText.text = "Нажмите E";
                }
                return;
            }
        }

        if (isNearSink)
        {
            isNearSink = false;
            hintText.gameObject.SetActive(false);
        }
    }

    void HandleInput()
    {
        if (isNearSink && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(WashSequence());
        }
    }

    System.Collections.IEnumerator WashSequence()
    {
        isWashing = true;
        hintText.gameObject.SetActive(false);

        
        if (waterSound != null)
            waterSound.Play();

        
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0, 1, t / fadeDuration);
                fadeImage.color = c;
            }
            yield return null;
        }

        
        yield return new WaitForSeconds(washDuration);

        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(1, 0, t / fadeDuration);
                fadeImage.color = c;
            }

            
            if (waterSound != null)
            {
                waterSound.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
            }

            yield return null;
        }

        if (waterSound != null)
        {
            waterSound.Stop();
            waterSound.volume = 1f;
        }
    }
}