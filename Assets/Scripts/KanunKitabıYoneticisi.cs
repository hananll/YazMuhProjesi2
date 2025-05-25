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

    // --- SES İÇİN EKLENEN KISIM ---
    [Header("Ses Ayarları")]
    public AudioSource sayfaCevirmeAudioSource; // Sayfa çevirme sesini çalacak AudioSource
    public AudioClip sayfaCevirmeSoundClip;     // Sayfa çevirme ses dosyanız
    // --- SES İÇİN EKLENEN KISIM SONU ---

    [Header("Diğer Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
        // Butonların tıklama olaylarını bağla
        if (kanunKitabiButon != null)
            kanunKitabiButon.onClick.AddListener(KanunKitabiPaneliAc);

        if (kanunKitabiKapatButon != null)
            kanunKitabiKapatButon.onClick.AddListener(KanunKitabiPaneliKapat);

        // Sonraki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarını bağla
        if (kanunSonrakiSayfaButon != null)
        {
            kanunSonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
            kanunSonrakiSayfaButon.onClick.AddListener(PlayPageTurnSound); // SES İÇİN EKLENDİ
        }

        // Önceki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarını bağla
        if (kanunOncekiSayfaButon != null)
        {
            kanunOncekiSayfaButon.onClick.AddListener(OncekiSayfa);
            kanunOncekiSayfaButon.onClick.AddListener(PlayPageTurnSound); // SES İÇİN EKLENDİ
        }

        // Paneli başlangıçta gizle
        if (kanunKitabiPanel != null)
            kanunKitabiPanel.SetActive(false);

        // Sayfa navigasyon butonlarının başlangıç durumunu ayarla
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0)
        {
            if (kanunSonrakiSayfaButon != null) kanunSonrakiSayfaButon.interactable = false;
            if (kanunOncekiSayfaButon != null) kanunOncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // İlk sayfayı göster
            if (kanunOncekiSayfaButon != null)
                kanunOncekiSayfaButon.interactable = false; // İlk sayfadayken önceki buton devre dışı
            if (kanunSonrakiSayfaButon != null)
                kanunSonrakiSayfaButon.interactable = kanunKitabiSayfalari.Length > 1; // Tek sayfa varsa sonraki de devre dışı
        }
    }

    // Kanun kitabı panelini açar ve arka planı ayarlar
    void KanunKitabiPaneliAc()
    {
        if (kanunKitabiPanel == null)
        {
            Debug.LogWarning("Kanun Kitabı Paneli atanmamış!");
            return;
        }

        kanunKitabiPanel.SetActive(true);

        // Arka plan yönetimi entegrasyonu
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }

        // Arka plandaki UI etkileşimini devre dışı bırak
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
        }
    }

    // Kanun kitabı panelini kapatır ve arka planı eski haline getirir
    void KanunKitabiPaneliKapat()
    {
        if (kanunKitabiPanel == null)
        {
            Debug.LogWarning("Kanun Kitabı Paneli atanmamış!");
            return;
        }

        kanunKitabiPanel.SetActive(false);

        // Arka plan yönetimi entegrasyonu
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(false);
        }

        // Arka plandaki UI etkileşimini tekrar aktif et
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = true;
            arkaPlanCanvasGroup.blocksRaycasts = true;
        }
    }

    // Sonraki sayfaya geçişi sağlar
    public void SonrakiSayfa()
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < kanunKitabiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            // Butonların etkileşim durumunu güncelle
            if (kanunOncekiSayfaButon != null)
                kanunOncekiSayfaButon.interactable = true;

            if (kanunSonrakiSayfaButon != null && aktifSayfaIndex == kanunKitabiSayfalari.Length - 1)
                kanunSonrakiSayfaButon.interactable = false; // Son sayfadaysa sonraki buton devre dışı
        }
    }

    // Önceki sayfaya geçişi sağlar
    public void OncekiSayfa()
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            // Butonların etkileşim durumunu güncelle
            if (kanunSonrakiSayfaButon != null)
                kanunSonrakiSayfaButon.interactable = true;

            if (kanunOncekiSayfaButon != null && aktifSayfaIndex == 0)
                kanunOncekiSayfaButon.interactable = false; // İlk sayfadaysa önceki buton devre dışı
        }
    }

    // Belirtilen indeksteki sayfayı gösterir ve diğerlerini gizler
    void SayfayiGoster(int sayfaIndex)
    {
        if (kanunKitabiSayfalari == null || kanunKitabiSayfalari.Length == 0)
        {
            Debug.LogWarning("Kanun Kitabı sayfaları atanmamış veya boş!");
            return;
        }

        // Tüm sayfaları gizle
        for (int i = 0; i < kanunKitabiSayfalari.Length; i++)
        {
            if (kanunKitabiSayfalari[i] != null)
            {
                kanunKitabiSayfalari[i].SetActive(false);
            }
        }

        // Sadece istenen sayfayı aktif et
        if (sayfaIndex >= 0 && sayfaIndex < kanunKitabiSayfalari.Length && kanunKitabiSayfalari[sayfaIndex] != null)
        {
            kanunKitabiSayfalari[sayfaIndex].SetActive(true);
        }
        else
        {
            Debug.LogError($"Geçersiz sayfa indeksi: {sayfaIndex}. Toplam sayfa sayısı: {kanunKitabiSayfalari.Length}");
        }
    }

    // --- SES İÇİN EKLENEN KISIM ---
    // Sayfa çevirme sesini çalan fonksiyon
    private void PlayPageTurnSound()
    {
        if (sayfaCevirmeAudioSource != null && sayfaCevirmeSoundClip != null)
        {
            sayfaCevirmeAudioSource.PlayOneShot(sayfaCevirmeSoundClip);
        }
        else
        {
            Debug.LogWarning("Sayfa çevirme AudioSource veya AudioClip atanmamış! Lütfen Inspector'dan atayın.");
        }
    }
    // --- SES İÇİN EKLENEN KISIM SONU ---
}