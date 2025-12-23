using UnityEngine;

public class CollectibleItemFive : MonoBehaviour
{
    [Header("Настройки предмета")]
    [SerializeField] private string itemName = "Gear1";
    [SerializeField] private GameObject pickupEffect; 
    [SerializeField] private AudioClip pickupSound;   

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            QuestControllerFiveItems quest = FindObjectOfType<QuestControllerFiveItems>();
            if (quest != null)
            {
                bool collected = quest.CollectItem(itemName);

                if (collected)
                {
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            QuestControllerFiveItems quest = FindObjectOfType<QuestControllerFiveItems>();
            if (quest != null)
            {
                bool collected = quest.CollectItem(itemName);

                if (collected)
                {
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}