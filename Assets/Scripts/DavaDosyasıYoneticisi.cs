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

    [Header("Ses Ayarlarý")]
    public AudioSource sayfaCevirmeAudioSource; 
    public AudioClip sayfaCevirmeSoundClip;    

    [Header("Diðer Sistemlerle Entegrasyon")]
    public ArkaPlanYonetimi arkaPlanYoneticisi;
    public CanvasGroup arkaPlanCanvasGroup;

    void Start()
    {
        // Butonlarýn týklama olaylarýný baðladým
        if (davaDosyasiButon != null)
            davaDosyasiButon.onClick.AddListener(DavaDosyasiPaneliAc);

        if (davaDosyasiKapatButon != null)
            davaDosyasiKapatButon.onClick.AddListener(DavaDosyasiPaneliKapat);

        // Sonraki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarýný baðladým.
        if (sonrakiSayfaButon != null)
        {
            sonrakiSayfaButon.onClick.AddListener(SonrakiSayfa);
            sonrakiSayfaButon.onClick.AddListener(PlayPageTurnSound); 
        }

        // Önceki sayfa butonuna hem sayfa çevirme hem de ses çalma olaylarýný baðladým.
        if (oncekiSayfaButon != null)
        {
            oncekiSayfaButon.onClick.AddListener(OncekiSayfa);
            oncekiSayfaButon.onClick.AddListener(PlayPageTurnSound);
        }

        // Eðer tasarým ekranýnda panellerden birini açýk unutursam kapalý olmasý için burda iþimi garantiye aldým.
        if (davaDosyasiAnaPanel != null)
            davaDosyasiAnaPanel.SetActive(false);

        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            if (sonrakiSayfaButon != null) sonrakiSayfaButon.interactable = false;
            if (oncekiSayfaButon != null) oncekiSayfaButon.interactable = false;
        }
        else
        {
            SayfayiGoster(0); // Ýlk sayfayý göster
            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = false; // Ýlk sayfadayken önceki buton devre dýþý çünkü ilk sayfadayýz.
            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = davaDosyasiSayfalari.Length > 1; // Tek sayfa varsa sonraki de devre dýþý þeklinde yaptým.
        }
    }

    // Dava dosyasý panelini açar ve arka planý ayarlar
    void DavaDosyasiPaneliAc()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyasý Ana Paneli atanmamýþ!"); // Kontrol için debug ekledim.
            return;
        }

        davaDosyasiAnaPanel.SetActive(true);

        if (arkaPlanYoneticisi != null)
        {
            arkaPlanYoneticisi.BlurluArkaPlaniAyarla(true);
        }

        // Panel açýldýðýnda arka plandaki olan avukat button vs bunlarýn hepsine týklanamaz hale getirdim.
        if (arkaPlanCanvasGroup != null)
        {
            arkaPlanCanvasGroup.interactable = false;
            arkaPlanCanvasGroup.blocksRaycasts = false;
        }
    }

    void DavaDosyasiPaneliKapat()
    {
        if (davaDosyasiAnaPanel == null)
        {
            Debug.LogWarning("Dava Dosyasý Ana Paneli atanmamýþ!");  // Kontrol için yine debug satýrý ekledim.
            return;
        }

        davaDosyasiAnaPanel.SetActive(false);

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
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex < davaDosyasiSayfalari.Length - 1)
        {
            aktifSayfaIndex++;
            SayfayiGoster(aktifSayfaIndex);

            if (oncekiSayfaButon != null)
                oncekiSayfaButon.interactable = true;

            if (sonrakiSayfaButon != null && aktifSayfaIndex == davaDosyasiSayfalari.Length - 1)
                sonrakiSayfaButon.interactable = false;
        }
    }

    public void OncekiSayfa()
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0) return;

        if (aktifSayfaIndex > 0)
        {
            aktifSayfaIndex--;
            SayfayiGoster(aktifSayfaIndex);

            if (sonrakiSayfaButon != null)
                sonrakiSayfaButon.interactable = true;

            if (oncekiSayfaButon != null && aktifSayfaIndex == 0)
                oncekiSayfaButon.interactable = false; 
        }
    }

    void SayfayiGoster(int sayfaIndex)
    {
        if (davaDosyasiSayfalari == null || davaDosyasiSayfalari.Length == 0)
        {
            Debug.LogWarning("Dava Dosyasý sayfalarý atanmamýþ veya boþ!");
            return;
        }

        for (int i = 0; i < davaDosyasiSayfalari.Length; i++)
        {
            if (davaDosyasiSayfalari[i] != null)
            {
                davaDosyasiSayfalari[i].SetActive(false);
            }
        }

        if (sayfaIndex >= 0 && sayfaIndex < davaDosyasiSayfalari.Length && davaDosyasiSayfalari[sayfaIndex] != null)
        {
            davaDosyasiSayfalari[sayfaIndex].SetActive(true);
        }
        
    }

    
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
}