using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Sağlık Ayarları")]
    public int maxHealth = 100; // Karakterin maksimum canı
    private int currentHealth;  // Karakterin o anki canı

    void Start()
    {
        // Oyun başladığında canı maksimum seviyeye ayarla
        currentHealth = maxHealth;
        print("Selam");
        int abe = 4;
    }

    // Hasar alma fonksiyonu (Başka scriptlerden çağrılabilir)
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // Gelen hasarı candan düş

        Debug.Log(gameObject.name + " " + damageAmount + " hasar aldı! Kalan can: " + currentHealth);

        // Can sıfıra veya sıfırın altına düştüyse ölme fonksiyonunu çağır
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Ölme fonksiyonu
    void Die()
    {
        Debug.Log(gameObject.name + " öldü!");

        // Karakteri/Objeyi sahneden yok et
        Destroy(gameObject);

        // Not: Burada yok etmek yerine ölüm animasyonu oynatabilir 
        // veya Game Over ekranı çağırabilirsiniz.
    }
}using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Sağlık Ayarları")]
    public int maxHealth = 100; // Karakterin maksimum canı
    private int currentHealth;  // Karakterin o anki canı

    void Start()
    {
        // Oyun başladığında canı maksimum seviyeye ayarla
        currentHealth = maxHealth;
    }

    // Hasar alma fonksiyonu (Başka scriptlerden çağrılabilir)
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; // Gelen hasarı candan düş

        Debug.Log(gameObject.name + " " + damageAmount + " hasar aldı! Kalan can: " + currentHealth);

        // Can sıfıra veya sıfırın altına düştüyse ölme fonksiyonunu çağır
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Ölme fonksiyonu
    void Die()
    {
        Debug.Log(gameObject.name + " öldü!");

        // Karakteri/Objeyi sahneden yok et
        Destroy(gameObject);

        // Not: Burada yok etmek yerine ölüm animasyonu oynatabilir 
        // veya Game Over ekranı çağırabilirsiniz.
    }
}