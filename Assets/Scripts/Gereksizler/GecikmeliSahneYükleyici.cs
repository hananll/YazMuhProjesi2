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
        
    }

    void OnEnable()
    {
        if (butonum != null)
        {
            butonum.onClick.AddListener(SahneYavasYukleCoroutineBaslat);
            Debug.Log(gameObject.name + " aktif hale geldi, listener eklendi.");
        }
    }

    void OnDisable()
    {
        if (butonum != null)
        {
            butonum.onClick.RemoveListener(SahneYavasYukleCoroutineBaslat);
            Debug.Log(gameObject.name + " pasif hale geldi, listener kaldırıldı.");
        }
    }

    public void SahneYavasYukleCoroutineBaslat()
    {
        // Burada tekrar kontrol ediyoruz, çünkü bir şekilde bu metod çağrıldığında
        // GameObject'in aktif olmadığı bir an olabiliyor.
        if (!gameObject.activeInHierarchy)
        {
            return; // Eğer aktif değilse işlemi durdur
        }

        Debug.Log(gameObject.name + " butonu tıklandı, Coroutine başlatılıyor...");
        StopAllCoroutines(); 
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
       

        yield return new WaitForSeconds(gecikmeSuresi);

        if (!string.IsNullOrEmpty(yuklenecekSahneAdi))
        {
            Debug.Log("Bekleme süresi bitti, sahne yükleniyor: " + yuklenecekSahneAdi);
            SceneManager.LoadScene(yuklenecekSahneAdi);
        }
        
    }
}