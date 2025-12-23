using UnityEngine;

public class CollectibleItemFive : MonoBehaviour
{
    [Header("Настройки предмета")]
    [SerializeField] private string itemName = "Gear1";
    [SerializeField] private GameObject pickupEffect; // Опциональный эффект при подборе
    [SerializeField] private AudioClip pickupSound;   // Опциональный звук при подборе

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
                    // Воспроизводим звук
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    // Создаем эффект
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    // Уничтожаем предмет
                    Destroy(gameObject);
                }
            }
        }
    }

    // Альтернативный метод для взаимодействия по нажатию E
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
                    // Воспроизводим звук
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                    }

                    // Создаем эффект
                    if (pickupEffect != null)
                    {
                        Instantiate(pickupEffect, transform.position, Quaternion.identity);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    // Для визуализации в редакторе
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}