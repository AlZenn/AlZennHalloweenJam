using UnityEngine;
using System.Collections.Generic; // List kullanabilmek için

/// <summary>
/// Oyuncunun etrafýnda, belirlenen bir yarýçaptaki çemberin kenarýnda
/// rastgele prefab'ler spawn eder.
/// Bu script'in Oyuncu (Player) GameObject'ine eklenmesi gerekir.
/// </summary>
public class PlayerAreaSpawner : MonoBehaviour
{
    [Header("Spawn Edilecekler")]
    [Tooltip("Spawn edilebilecek prefab'lerin listesi. Bu listeden rastgele biri seçilir.")]
    [SerializeField]
    private List<GameObject> prefabList;

    [Header("Spawn Ayarlarý")]
    [Tooltip("Prefab'lerin spawn edileceði çemberin yarýçapý (oyuncudan uzaklýðý).")]
    [SerializeField]
    private float spawnRadius = 10f;

    [Tooltip("Her spawn iþlemi arasýndaki bekleme süresi (saniye cinsinden).")]
    [SerializeField]
    private float spawnInterval = 1.5f;

    [Header("Engel Kontrolü (OverlapCircle)")]
    [Tooltip("Spawn iþlemini engelleyecek katmanlar (örn: Duvar, Engel).")]
    [SerializeField]
    private LayerMask obstacleLayer;

    [Tooltip("Hesaplanan spawn noktasýnýn boþ olup olmadýðýný kontrol etmek için kullanýlacak küçük yarýçap. (0.1 gibi küçük bir deðer olabilir)")]
    [SerializeField]
    private float spawnPointCheckRadius = 0.5f;

    // Spawn için zamanlayýcý
    private float spawnTimer;

    // Oyuncunun Transform bileþenini cache'lemek (performans için)
    // Bu script oyuncuda olduðu için 'transform'u doðrudan kullanabiliriz.
    private Transform playerTransform;

    private void Awake()
    {
        // Script'in eklendiði nesnenin (Oyuncu) transform'unu al
        playerTransform = transform;

        // Zamanlayýcýyý ilk spawn için hazýrla
        spawnTimer = spawnInterval;
    }

    private void Update()
    {
        // Zamanlayýcýyý her saniye azalt
        spawnTimer -= Time.deltaTime;

        // Zamanlayýcý sýfýra ulaþtýysa veya altýna indiyse
        if (spawnTimer <= 0f)
        {
            // Spawn fonksiyonunu çaðýr
            SpawnOnCircleEdge();

            // Zamanlayýcýyý yeniden ayarla
            spawnTimer = spawnInterval;
        }
    }

    /// <summary>
    /// Belirlenen 'spawnRadius' yarýçaplý çemberin kenarýnda rastgele bir noktada prefab spawn eder.
    /// </summary>
    private void SpawnOnCircleEdge()
    {
        // 1. Kontrol: Listede spawn edilecek bir þey var mý?
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogWarning("Spawn listesi (Prefab List) boþ. Lütfen Inspector'dan prefab ekleyin.");
            return; // Fonksiyondan çýk, spawn yapma
        }

        // 2. Rastgele Açý Belirle
        // 0 ile 360 derece arasýnda (radyan cinsinden 0 ile 2*PI) rastgele bir açý seç
        float randomAngle = Random.Range(0f, 2f * Mathf.PI);

        // 3. Spawn Pozisyonunu Hesapla
        // Seçilen açýyý kullanarak bir yön vektörü (Vector2) oluþtur
        // Cos(açý) = X, Sin(açý) = Y (Birim çember)
        Vector2 direction = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));

        // Oyuncunun mevcut pozisyonunu al
        Vector2 playerPosition = playerTransform.position;

        // Son spawn noktasýný hesapla: Oyuncu Pozisyonu + (Yön * Yarýçap)
        Vector2 spawnPoint = playerPosition + (direction * spawnRadius);

        // 4. Engel Kontrolü (Ýstediðiniz OverlapCircle burada kullanýlýyor)
        // Hesaplanan spawn noktasýnda (spawnPoint) 'obstacleLayer' katmanýndan bir þey var mý?
        // 'spawnPointCheckRadius' kadar küçük bir alaný kontrol ediyoruz.
        Collider2D hit = Physics2D.OverlapCircle(spawnPoint, spawnPointCheckRadius, obstacleLayer);

        // Eðer 'hit' null ise, yani çember hiçbir þeye çarpmadýysa (alan boþsa)
        if (hit == null)
        {
            // 5. Prefab'ý Seç ve Spawn Et
            // Listeden rastgele bir prefab seç (Listenin 0. elemaný ile son elemaný arasýnda)
            GameObject prefabToSpawn = prefabList[Random.Range(0, prefabList.Count)];

            // Seçilen prefab'ý, hesaplanan 'spawnPoint' noktasýnda,
            // 'Quaternion.identity' (dönüþ açýsý olmadan, varsayýlan rotasyon) ile spawn et
            Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);
        }
        // else
        // {
        //   Eðer spawn noktasý doluysa (bir duvara veya engele denk geldiyse), bu seferlik spawn yapma.
        //   Bir sonraki 'spawnInterval' dolduðunda tekrar denenecek.
        // }
    }

    /// <summary>
    // (Opsiyonel) Sahne (Scene) ekranýnda spawn çemberini görselleþtirmek için.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Bu script'in baðlý olduðu nesnenin (Oyuncu) pozisyonunu merkez al
        Gizmos.color = Color.green; // Çizgi rengini yeþil yap
        Gizmos.DrawWireSphere(transform.position, spawnRadius); // 'spawnRadius' yarýçapýnda bir tel çember çiz
    }
}