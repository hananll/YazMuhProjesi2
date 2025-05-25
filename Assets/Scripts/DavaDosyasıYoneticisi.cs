using UnityEngine;
using UnityEngine.UI;

public class DavaDosyasiYoneticisi : MonoBehaviour
{
    [Header("Ana Panel ve Butonlar")]
    public Button davaDosyasiButon;
    public GameObject davaDosyasiAnaPanel;
    public Button davaDosyasiKapatButon;

    [Header("Sayfa Yönetimi")]
    public GameObject[] davaDosyasiSayfalari;
    public Button sonrakiSayfaButon;
    public Button oncekiSayfaButon;
    private int aktifSayfaIndex = 0;

    // --- SES ÝÇÝN EKLENEN KISIM ---
    [Header("Ses Ayarlarý")]
    public AudioSource sayfaCevirmeAudioSource; // Sayfa çevirme sesini çalacak AudioSource
    public AudioClip sayfaCevirmeSoundClip;     // Sayfa çevirme ses dosyanýz
    // --- SES ÝÇÝN EKLENEN KISIM SONU ---

    [Header("Diðer Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
        // Butonlarýn týklama olaylarýný baðla
        if (davaDosyasiButon != null)
            davaDosyasiButon.onClick.AddListener(DavaDosyasiPaneliAc);

        if (davaDosyasiKapatButon != null)
            davaDosyasiKapatButon.onClick.AddListener(DavaDosyasiPaneliKapat);

        // Sonraki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarýný baðla
        if (sonrakiSayfaButon != null)
        {
            sonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
            sonrakiSayfaButon.onClick.AddListener(PlayPageTurnSound); // SES ÝÇÝN EKLENDÝ
        }

        // Önceki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarýný baðla
        if (oncekiSayfaButon != null)
        {
            oncekiSayfaButon.onClick.AddListener(OncekiSayfa);
            oncekiSayfaButon.onClick.AddListener(PlayPageTurnSound); // SES ÝÇÝN EKLENDÝ
        }

        // Paneli baþlangýçta gizle
        if (davaDosyasiAnaPanel != null)
            davaDosyasiAnaPanel.SetActive(false);

        // Sayfa navigasyon butonlarýnýn baþlangýç durumunu ayarla
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            if (sonrakiSayfaButon != null) sonrakiSayfaButon.interactable = false;
            if (oncekiSayfaButon != null) oncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // Ýlk sayfayý göster
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = false; // Ýlk sayfadayken önceki buton devre dýþý
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = davaDosyasiSayfalari.Length > 1; // Tek sayfa varsa sonraki de devre dýþý
        }
    }

    // Dava dosyasý panelini açar ve arka planý ayarlar
    void DavaDosyasiPaneliAc()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyasý Ana Paneli atanmamýþ!");
            return;
        }

        davaDosyasiAnaPanel.SetActive(true);

        // Arka plan yönetimi entegrasyonu
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }

        // Arka plandaki UI etkileþimini devre dýþý býrak
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
        }
    }

    // Dava dosyasý panelini kapatýr ve arka planý eski haline getirir
    void DavaDosyasiPaneliKapat()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyasý Ana Paneli atanmamýþ!");
            return;
        }

        davaDosyasiAnaPanel.SetActive(false);

        // Arka plan yönetimi entegrasyonu
        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(false);
        }

        // Arka plandaki UI etkileþimini tekrar aktif et
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = true;
            arkaPlanCanvasGroup.blocksRaycasts = true;
        }
    }

    // Sonraki sayfaya geçiþi saðlar
    public void SonrakiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < davaDosyasiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            // Butonlarýn etkileþim durumunu güncelle
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = true;

            if (sonrakiSayfaButon != null && aktifSayfaIndex == davaDosyasiSayfalari.Length - 1)
                sonrakiSayfaButon.interactable = false; // Son sayfadaysa sonraki buton devre dýþý
        }
    }

    // Önceki sayfaya geçiþi saðlar
    public void OncekiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            // Butonlarýn etkileþim durumunu güncelle
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = true;

            if (oncekiSayfaButon != null && aktifSayfaIndex == 0)
                oncekiSayfaButon.interactable = false; // Ýlk sayfadaysa önceki buton devre dýþý
        }
    }

    // Belirtilen indeksteki sayfayý gösterir ve diðerlerini gizler
    void SayfayiGoster(int sayfaIndex)
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            Debug.LogWarning("Dava Dosyasý sayfalarý atanmamýþ veya boþ!");
            return;
        }

        // Tüm sayfalarý gizle
        for (int i = 0; i < davaDosyasiSayfalari.Length; i++)
        {
            if (davaDosyasiSayfalari[i] != null)
            {
                davaDosyasiSayfalari[i].SetActive(false);
            }
        }

        // Sadece istenen sayfayý aktif et
        if (sayfaIndex >= 0 && sayfaIndex < davaDosyasiSayfalari.Length && davaDosyasiSayfalari[sayfaIndex] != null)
        {
            davaDosyasiSayfalari[sayfaIndex].SetActive(true);
        }
        else
        {
            Debug.LogError($"Geçersiz sayfa indeksi: {sayfaIndex}. Toplam sayfa sayýsý: {davaDosyasiSayfalari.Length}");
        }
    }

    // --- SES ÝÇÝN EKLENEN KISIM ---
    // Sayfa çevirme sesini çalan fonksiyon
    private void PlayPageTurnSound()
    {
        if (sayfaCevirmeAudioSource != null && sayfaCevirmeSoundClip != null)
        {
            sayfaCevirmeAudioSource.PlayOneShot(sayfaCevirmeSoundClip);
        }
        else
        {
            Debug.LogWarning("Sayfa çevirme AudioSource veya AudioClip atanmamýþ! Lütfen Inspector'dan atayýn.");
        }
    }
    // --- SES ÝÇÝN EKLENEN KISIM SONU ---
}