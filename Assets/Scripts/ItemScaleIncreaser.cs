using UnityEngine;

/// <summary>
/// Bu script, "Player" etiketli bir nesne tarafýndan alýndýðýnda (trigger edildiðinde)
/// o nesnenin scale'ini (boyutunu) artýran bir item'ý kontrol eder.
/// Bu script'in item prefab'ýna eklenmesi gerekir.
/// </summary>
[RequireComponent(typeof(Collider2D))] // Çalýþmasý için bir Collider2D zorunludur.
public class ItemScaleIncreaser : MonoBehaviour
{
    [Header("Büyüme Ayarlarý")]
    [Tooltip("Bu item alýndýðýnda Player'ýn scale'ine eklenecek deðer (X ve Y).")]
    [SerializeField]
    private Vector2 scaleToAdd = new Vector2(0.1f, 0.1f);

    [Header("Toplanma Ayarlarý")]
    [Tooltip("Item toplandýktan sonra yok olsun mu? (False yaparsanýz object pooling için pasif hale getirir)")]
    [SerializeField]
    private bool destroyOnPickup = true;

    private void Awake()
    {
        // Bu script'in tetikleyici (trigger) olarak çalýþmasý þarttýr.
        // Inspector'dan "Is Trigger" kutusunu iþaretlemeyi unutsanýz bile
        // bu kod, script'in baðlý olduðu Collider2D'yi otomatik olarak trigger yapar.
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"'{gameObject.name}' üzerindeki Collider2D 'Is Trigger' olarak ayarlandý.", this);
            col.isTrigger = true;
        }
    }

    /// <summary>
    /// Bu fonksiyon, baþka bir Collider2D bu nesnenin trigger alanýna girdiðinde
    /// Unity tarafýndan otomatik olarak çaðrýlýr.
    /// </summary>
    /// <param name="other">Bize çarpan diðer nesnenin Collider2D bileþeni.</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kontrol: Bize çarpan nesnenin etiketi "Player" mý?
        // CompareTag kullanmak, 'other.tag == "Player"' yazmaktan daha performanslýdýr.
        if (other.CompareTag("Player"))
        {
            // 2. Ýþlem: Player'ýn scale'ini artýr.
            ApplyScale(other.transform);

            // 3. Ýþlem: Item'ý ortadan kaldýr.
            HandlePickup();
        }
    }

    /// <summary>
    /// Verilen transform'un localScale'ini 'scaleToAdd' deðeri kadar artýrýr.
    /// </summary>
    /// <param name="playerTransform">Büyütülecek nesnenin (Player) Transform'u.</param>
    private void ApplyScale(Transform playerTransform)
    {
        // Player'ýn mevcut scale'ini alýyoruz
        Vector3 currentScale = playerTransform.localScale;

        // Yeni scale'i hesaplýyoruz.
        // 'scaleToAdd' (Vector2) deðerini Vector3 olarak ekliyoruz (Z ekseni deðiþmesin).
        Vector3 newScale = new Vector3(
            currentScale.x + scaleToAdd.x,
            currentScale.y + scaleToAdd.y,
            currentScale.z // 2D oyunda Z scale'ini korumak genellikle en iyisidir.
        );

        // Player'ýn scale'ini yeni hesaplanan deðerle güncelliyoruz.
        playerTransform.localScale = newScale;
    }

    /// <summary>
    /// Item'ýn toplanma iþlemini (yok etme veya pasif hale getirme) yönetir.
    /// </summary>
    private void HandlePickup()
    {
        if (destroyOnPickup)
        {
            // Item'ý sahneden kalýcý olarak yok et.
            Destroy(gameObject);
        }
        else
        {
            // Item'ý sadece pasif hale getir.
            // (Bu, 'Object Pooling' gibi sistemler kullanýyorsanýz tercih edilir)
            gameObject.SetActive(false);
        }
    }
}