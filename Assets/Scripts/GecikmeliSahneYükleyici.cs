using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GecikmeliSahneYükleyici : MonoBehaviour
{
    [Header("Geçiş Ayarları")]
    public AudioClip tiklamaSesi;
    public float gecikmeSuresi = 1.0f;
    public string yuklenecekSahneAdi;

    private AudioSource sesKaynagi;
    private Button butonum;

    void Awake()
    {
        sesKaynagi = GetComponent<AudioSource>();
        if (sesKaynagi == null)
        {
            sesKaynagi = gameObject.AddComponent<AudioSource>();
            sesKaynagi.playOnAwake = false;
            sesKaynagi.loop = false;
        }

        butonum = GetComponent<Button>();
        if (butonum == null)
        {
            Debug.LogWarning("Bu script bir Button üzerinde değil.", this);
        }
    }

    void OnEnable()
    {
        if (butonum != null)
        {
            // Listener'ı sadece GameObject aktifken ekle
            butonum.onClick.AddListener(SahneYavasYukleCoroutineBaslat);
            Debug.Log(gameObject.name + " aktif hale geldi, listener eklendi.");
        }
    }

    void OnDisable()
    {
        if (butonum != null)
        {
            // GameObject pasif hale geldiğinde listener'ı kaldır
            butonum.onClick.RemoveListener(SahneYavasYukleCoroutineBaslat);
            Debug.Log(gameObject.name + " pasif hale geldi, listener kaldırıldı.");
        }
    }

    // Butonun OnClick olayından çağrılacak metod
    public void SahneYavasYukleCoroutineBaslat()
    {
        // Burada tekrar kontrol ediyoruz, çünkü bir şekilde bu metod çağrıldığında
        // GameObject'in aktif olmadığı bir an olabiliyor.
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogError("SahneYavasYukleCoroutineBaslat çağrıldı ama GameObject '" + gameObject.name + "' aktif değil! Coroutine başlatılamadı.", this);
            return; // Eğer aktif değilse işlemi durdur
        }

        Debug.Log(gameObject.name + " butonu tıklandı, Coroutine başlatılıyor...");
        StopAllCoroutines(); // Önceki Coroutine'leri durdur
        StartCoroutine(SahneYavasYukleBekle());
    }

    private IEnumerator SahneYavasYukleBekle()
    {
        Debug.Log("Coroutine başladı: Sahne " + yuklenecekSahneAdi + " için " + gecikmeSuresi + " saniye bekleniyor.");

        if (tiklamaSesi != null && sesKaynagi != null)
        {
            sesKaynagi.clip = tiklamaSesi;
            sesKaynagi.Play();
            Debug.Log("Ses çalıyor: " + tiklamaSesi.name);
        }
        else
        {
            Debug.LogWarning("Ses klibi atanmadı veya AudioSource bulunamadı.");
        }

        yield return new WaitForSeconds(gecikmeSuresi);

        if (!string.IsNullOrEmpty(yuklenecekSahneAdi))
        {
            Debug.Log("Bekleme süresi bitti, sahne yükleniyor: " + yuklenecekSahneAdi);
            SceneManager.LoadScene(yuklenecekSahneAdi);
        }
        else
        {
            Debug.LogWarning("Yüklenecek sahne adı belirtilmedi!");
        }
    }
}