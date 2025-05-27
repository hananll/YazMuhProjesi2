using UnityEngine;
using UnityEngine.UI;

public class KanunKitabiYoneticisi : MonoBehaviour
{
    [Header("Ana Panel ve Butonlar")]
    public Button kanunKitabiButon;
    public GameObject kanunKitabiPanel;
    public Button kanunKitabiKapatButon;

    [Header("Sayfa Yönetimi")]
    public GameObject[] kanunKitabiSayfalari;
    public Button kanunSonrakiSayfaButon;
    public Button kanunOncekiSayfaButon;
    private int aktifSayfaIndex = 0;

    [Header("Ses Ayarları")]
    public AudioSource sayfaCevirmeAudioSource;
    public AudioClip sayfaCevirmeSoundClip;    
  

    [Header("Diğer Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
        
            kanunKitabiButon.onClick.AddListener(KanunKitabiPaneliAc);

            kanunKitabiKapatButon.onClick.AddListener(KanunKitabiPaneliKapat);

            // Sonraki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarını bağladım
        
            kanunSonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
            kanunSonrakiSayfaButon.onClick.AddListener(PlayPageTurnSound); 
        
            // Önceki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarını bağladım
        
            kanunOncekiSayfaButon.onClick.AddListener(OncekiSayfa);
            kanunOncekiSayfaButon.onClick.AddListener(PlayPageTurnSound);
        

        // Paneli başlangıçta gizledim
        if (kanunKitabiPanel != null)
            kanunKitabiPanel.SetActive(false);

        // Sayfa navigasyon butonlarının başlangıç durumunu ayarladım
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0)
        {
            if (kanunSonrakiSayfaButon != null) kanunSonrakiSayfaButon.interactable = false;
            if (kanunOncekiSayfaButon != null) kanunOncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // İlk sayfayı göster
            if (kanunOncekiSayfaButon != null)
                kanunOncekiSayfaButon.interactable = false; // İlk sayfadayken önceki buton devre dışı bıraktım.
           
            if (kanunSonrakiSayfaButon != null)
                kanunSonrakiSayfaButon.interactable = kanunKitabiSayfalari.Length > 1; // Tek sayfa varsa sonraki de devre dışı bıraktım.
        }
           
    }

    // Bu metot kanun kitabını açar ve arka planın blurlu olarak değişmesini sağlar.
    void KanunKitabiPaneliAc()
    {
        if (kanunKitabiPanel == null)
        {
            Debug.LogWarning("Kanun Kitabı Paneli atanmamış!"); //Kontrol ekledim,çünkü bazen inspector a atamaları unutuyorum.
            return;
        }

        kanunKitabiPanel.SetActive(true);

       
       arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        

        // Arka plandaki UI etkileşimini devre dışı bıraktım.Oyuncu basmasın diye.
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
        }
    }

    
    void KanunKitabiPaneliKapat()
    {
        if (kanunKitabiPanel == null)
        {
            Debug.LogWarning("Kanun Kitabı Paneli atanmamış!");
            return;
        }

        kanunKitabiPanel.SetActive(false);

        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(false);
        }

        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = true;
            arkaPlanCanvasGroup.blocksRaycasts = true;
        }
    }

    public void SonrakiSayfa()
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < kanunKitabiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            if (kanunOncekiSayfaButon != null)
                kanunOncekiSayfaButon.interactable = true;

            if (kanunSonrakiSayfaButon != null && aktifSayfaIndex == kanunKitabiSayfalari.Length - 1)
                kanunSonrakiSayfaButon.interactable = false; // Son sayfaysa buton etkileşimi false olur.
        }
    }

    public void OncekiSayfa()
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            if (kanunSonrakiSayfaButon != null)
                kanunSonrakiSayfaButon.interactable = true;

            if (kanunOncekiSayfaButon != null && aktifSayfaIndex == 0)
                kanunOncekiSayfaButon.interactable = false; 
        }
    }

    void SayfayiGoster(int sayfaIndex)
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0)
        {
            Debug.LogWarning("Kanun Kitabı sayfaları atanmamış veya boş!");
            return;
        }

        for (int i = 0; i < kanunKitabiSayfalari.Length; i++)
        {
            if (kanunKitabiSayfalari[i] != null)
            {
                kanunKitabiSayfalari[i].SetActive(false);
            }
        }

        if (sayfaIndex >= 0 && sayfaIndex < kanunKitabiSayfalari.Length && kanunKitabiSayfalari[sayfaIndex] != null)
        {
            kanunKitabiSayfalari[sayfaIndex].SetActive(true);
        }
        
    }

   
    private void PlayPageTurnSound()
    {
        if (sayfaCevirmeAudioSource != null && sayfaCevirmeSoundClip != null)
        {
            sayfaCevirmeAudioSource.PlayOneShot(sayfaCevirmeSoundClip);
        }
        
    }
}