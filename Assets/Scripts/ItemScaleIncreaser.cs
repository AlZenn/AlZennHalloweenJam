using UnityEngine;

/// <summary>
/// Bu script, "Player" etiketli bir nesne taraf�ndan al�nd���nda (trigger edildi�inde)
/// o nesnenin scale'ini (boyutunu) art�ran bir item'� kontrol eder.
/// Bu script'in item prefab'�na eklenmesi gerekir.
/// </summary>
[RequireComponent(typeof(Collider2D))] // �al��mas� i�in bir Collider2D zorunludur.
public class ItemScaleIncreaser : MonoBehaviour
{
    [Header("B�y�me Ayarlar�")]
    [Tooltip("Bu item al�nd���nda Player'�n scale'ine eklenecek de�er (X ve Y).")]
    [SerializeField]
    private Vector2 scaleToAdd = new Vector2(0.1f, 0.1f);

    [Header("Toplanma Ayarlar�")]
    [Tooltip("Item topland�ktan sonra yok olsun mu? (False yaparsan�z object pooling i�in pasif hale getirir)")]
    [SerializeField]
    private bool destroyOnPickup = true;

    private void Awake()
    {
        // Bu script'in tetikleyici (trigger) olarak �al��mas� �artt�r.
        // Inspector'dan "Is Trigger" kutusunu i�aretlemeyi unutsan�z bile
        // bu kod, script'in ba�l� oldu�u Collider2D'yi otomatik olarak trigger yapar.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"'{gameObject.name}' �zerindeki Collider2D 'Is Trigger' olarak ayarland�.", this);
            col.isTrigger = true;
        }
    }

    /// <summary>
    /// Bu fonksiyon, ba�ka bir Collider2D bu nesnenin trigger alan�na girdi�inde
    /// Unity taraf�ndan otomatik olarak �a�r�l�r.
    /// </summary>
    /// <param name="other">Bize �arpan di�er nesnenin Collider2D bile�eni.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kontrol: Bize �arpan nesnenin etiketi "Player" m�?
        // CompareTag kullanmak, 'other.tag == "Player"' yazmaktan daha performansl�d�r.
        if (other.CompareTag("Player"))
        {
            // 2. ��lem: Player'�n scale'ini art�r.
            ApplyScale(other.transform);

            // 3. ��lem: Item'� ortadan kald�r.
            HandlePickup();
        }
    }

    /// <summary>
    /// Verilen transform'un localScale'ini 'scaleToAdd' de�eri kadar art�r�r.
    /// </summary>
    /// <param name="playerTransform">B�y�t�lecek nesnenin (Player) Transform'u.</param>
    private void ApplyScale(Transform playerTransform)
    {
        // Player'�n mevcut scale'ini al�yoruz
        Vector3 currentScale = playerTransform.localScale;

        // Yeni scale'i hesapl�yoruz.
        // 'scaleToAdd' (Vector2) de�erini Vector3 olarak ekliyoruz (Z ekseni de�i�mesin).
        Vector3 newScale = new Vector3(
            currentScale.x + scaleToAdd.x,
            currentScale.y + scaleToAdd.y,
            currentScale.z // 2D oyunda Z scale'ini korumak genellikle en iyisidir.
        );

        // Player'�n scale'ini yeni hesaplanan de�erle g�ncelliyoruz.
        playerTransform.localScale = newScale;
    }

    /// <summary>
    /// Item'�n toplanma i�lemini (yok etme veya pasif hale getirme) y�netir.
    /// </summary>
    private void HandlePickup()
    {
        if (destroyOnPickup)
        {
            // Item'� sahneden kal�c� olarak yok et.
            Destroy(gameObject);
        }
        else
        {
            // Item'� sadece pasif hale getir.
            // (Bu, 'Object Pooling' gibi sistemler kullan�yorsan�z tercih edilir)
            gameObject.SetActive(false);
        }
    }
}