using UnityEngine;
using System.Collections.Generic; // List kullanabilmek i�in

/// <summary>
/// Oyuncunun etraf�nda, belirlenen bir yar��aptaki �emberin kenar�nda
/// rastgele prefab'ler spawn eder.
/// Bu script'in Oyuncu (Player) GameObject'ine eklenmesi gerekir.
/// </summary>
public class PlayerAreaSpawner : MonoBehaviour
{
    [Header("Spawn Edilecekler")]
    [Tooltip("Spawn edilebilecek prefab'lerin listesi. Bu listeden rastgele biri se�ilir.")]
    [SerializeField]
    private List<GameObject> prefabList;

    [Header("Spawn Ayarlar�")]
    [Tooltip("Prefab'lerin spawn edilece�i �emberin yar��ap� (oyuncudan uzakl���).")]
    [SerializeField]
    private float spawnRadius = 10f;

    [Tooltip("Her spawn i�lemi aras�ndaki bekleme s�resi (saniye cinsinden).")]
    [SerializeField]
    private float spawnInterval = 1.5f;

    [Header("Engel Kontrol� (OverlapCircle)")]
    [Tooltip("Spawn i�lemini engelleyecek katmanlar (�rn: Duvar, Engel).")]
    [SerializeField]
    private LayerMask obstacleLayer;

    [Tooltip("Hesaplanan spawn noktas�n�n bo� olup olmad���n� kontrol etmek i�in kullan�lacak k���k yar��ap. (0.1 gibi k���k bir de�er olabilir)")]
    [SerializeField]
    private float spawnPointCheckRadius = 0.5f;

    // Spawn i�in zamanlay�c�
    private float spawnTimer;

    // Oyuncunun Transform bile�enini cache'lemek (performans i�in)
    // Bu script oyuncuda oldu�u i�in 'transform'u do�rudan kullanabiliriz.
    private Transform playerTransform;

    private void Awake()
    {
        // Script'in eklendi�i nesnenin (Oyuncu) transform'unu al
        playerTransform = transform;

        // Zamanlay�c�y� ilk spawn i�in haz�rla
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        // Zamanlay�c�y� her saniye azalt
        spawnTimer -= Time.deltaTime;

        // Zamanlay�c� s�f�ra ula�t�ysa veya alt�na indiyse
        if (spawnTimer <= 0f)
        {
            // Spawn fonksiyonunu �a��r
            SpawnOnCircleEdge();

            // Zamanlay�c�y� yeniden ayarla
            spawnTimer = spawnInterval;
        }
    }

    /// <summary>
    /// Belirlenen 'spawnRadius' yar��apl� �emberin kenar�nda rastgele bir noktada prefab spawn eder.
    /// </summary>
    private void SpawnOnCircleEdge()
    {
        // 1. Kontrol: Listede spawn edilecek bir �ey var m�?
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogWarning("Spawn listesi (Prefab List) bo�. L�tfen Inspector'dan prefab ekleyin.");
            return; // Fonksiyondan ��k, spawn yapma
        }

        // 2. Rastgele A�� Belirle
        // 0 ile 360 derece aras�nda (radyan cinsinden 0 ile 2*PI) rastgele bir a�� se�
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);

        // 3. Spawn Pozisyonunu Hesapla
        // Se�ilen a��y� kullanarak bir y�n vekt�r� (Vector2) olu�tur
        // Cos(a��) = X, Sin(a��) = Y (Birim �ember)
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        // Oyuncunun mevcut pozisyonunu al
        Vector2 playerPosition = playerTransform.position;

        // Son spawn noktas�n� hesapla: Oyuncu Pozisyonu + (Y�n * Yar��ap)
        Vector2 spawnPoint = playerPosition + (direction * spawnRadius);

        // 4. Engel Kontrol� (�stedi�iniz OverlapCircle burada kullan�l�yor)
        // Hesaplanan spawn noktas�nda (spawnPoint) 'obstacleLayer' katman�ndan bir �ey var m�?
        // 'spawnPointCheckRadius' kadar k���k bir alan� kontrol ediyoruz.
        Collider2D hit = Physics2D.OverlapCircle(spawnPoint, spawnPointCheckRadius, obstacleLayer);

        // E�er 'hit' null ise, yani �ember hi�bir �eye �arpmad�ysa (alan bo�sa)
        if (hit == null)
        {
            // 5. Prefab'� Se� ve Spawn Et
            // Listeden rastgele bir prefab se� (Listenin 0. eleman� ile son eleman� aras�nda)
            GameObject prefabToSpawn = prefabList[Random.Range(0, prefabList.Count)];

            // Se�ilen prefab'�, hesaplanan 'spawnPoint' noktas�nda,
            // 'Quaternion.identity' (d�n�� a��s� olmadan, varsay�lan rotasyon) ile spawn et
            Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);
        }
        // else
        // {
        //   E�er spawn noktas� doluysa (bir duvara veya engele denk geldiyse), bu seferlik spawn yapma.
        //   Bir sonraki 'spawnInterval' doldu�unda tekrar denenecek.
        // }
    }

    /// <summary>
    // (Opsiyonel) Sahne (Scene) ekran�nda spawn �emberini g�rselle�tirmek i�in.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Bu script'in ba�l� oldu�u nesnenin (Oyuncu) pozisyonunu merkez al
        Gizmos.color = Color.green; // �izgi rengini ye�il yap
        Gizmos.DrawWireSphere(transform.position, spawnRadius); // 'spawnRadius' yar��ap�nda bir tel �ember �iz
    }
}