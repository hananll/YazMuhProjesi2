using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TokmakSistemiYoneticisi : MonoBehaviour
{
    [Header("Ana Açılış Butonu")]
    [Tooltip("Oyun sahnesindeki ana 'Tokmak Vur' veya 'Karar Aşamasına Geç' butonu.")]
    public Button anaTokmakButonu;

    [Header("Paneller")]
    [Tooltip("Tokmağa tıklandığında açılacak olan ana seçenekler paneli")]
    public GameObject tokmakKararPanel;
    [Tooltip("Erteleme seçeneklerini veya mesajını gösteren panel")]
    public GameObject erteleMesajPanel;

    [Header("Tokmak Karar Paneli İçindeki Butonlar")] 
    [Tooltip("'tokmakKararPanel' içindeki 'Davayı Ertele' butonu")]
    public Button secenekErteleButonu;
    [Tooltip("'tokmakKararPanel' içindeki 'Davayı Düşür' butonu")]
    public Button secenekDusurButonu;
    [Tooltip("'tokmakKararPanel' içindeki 'Karar Ver' (giriş panelini açacak) butonu")]
    public Button secenekKararVerButonu;

    [Header("Erteleme Paneli Butonları")]
    public Button ertelemeKapatButton;
    public Button ertelemeTutukluDevamKararButonu;
    public Button ertelemeTutuksuzYargilamaKararButonu;

    [Header("Karar Giriş Paneli Entegrasyonu")]
    public KararGirisPaneliYoneticisi kararGirisPaneli;

    [Header("Bağlantılı Sistemler")]
    public BarYoneticisi barYoneticisi;
   
    [Header("Aktif Dava Verisi")]
    public DavaVerisiSO aktifDava;

    void Start()
    {


        if (anaTokmakButonu != null)
        {
            anaTokmakButonu.onClick.AddListener(TokmakSecenekPaneliAc);
        }
       

       
        if (secenekErteleButonu != null)
            secenekErteleButonu.onClick.AddListener(ErteleButonunaTiklandi);
        if (secenekDusurButonu != null)
            secenekDusurButonu.onClick.AddListener(DusurButonunaTiklandi);
        if (secenekKararVerButonu != null)
            secenekKararVerButonu.onClick.AddListener(KararVerSecenegiTiklandi);

        if (ertelemeKapatButton != null) ertelemeKapatButton.onClick.AddListener(ErtelemeMesajPaneliKapat);
        if (ertelemeTutukluDevamKararButonu != null) ertelemeTutukluDevamKararButonu.onClick.AddListener(ErtelemeKarari_TutukluDevam);
        if (ertelemeTutuksuzYargilamaKararButonu != null) ertelemeTutuksuzYargilamaKararButonu.onClick.AddListener(ErtelemeKarari_TutuksuzYargilama);

        if (tokmakKararPanel != null) tokmakKararPanel.SetActive(false);
        if (erteleMesajPanel != null) erteleMesajPanel.SetActive(false);
    }

    public void AktifDavayiAyarla(DavaVerisiSO yeniDava)
    {
        aktifDava = yeniDava;
        
    }

    public void TokmakSecenekPaneliAc()
    {
        if (aktifDava == null || tokmakKararPanel == null)
        {
           
            return;
        }
        tokmakKararPanel.SetActive(true);
        ArkaPlanVeDigerUIAyarla(true);
    }

    public void ErteleButonunaTiklandi()
    {
        if (erteleMesajPanel == null || tokmakKararPanel == null) return;
        erteleMesajPanel.SetActive(true);
        tokmakKararPanel.SetActive(false);
        

    }

    public void ErtelemeKarari_TutukluDevam()
    {
        if (barYoneticisi == null || aktifDava == null) return;
        barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.ertelemeTutuklu_KamuoyuEtkisi);
        barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.ertelemeTutuklu_HukukEtkisi);
        KapatVeGenelDurumuNormaleDonder(erteleMesajPanel);
        SceneManager.LoadScene(1);

    }

    public void ErtelemeKarari_TutuksuzYargilama()
    {
        if (barYoneticisi == null || aktifDava == null) return;
        barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.ertelemeTutuksuz_KamuoyuEtkisi);
        barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.ertelemeTutuksuz_HukukEtkisi);
        KapatVeGenelDurumuNormaleDonder(erteleMesajPanel);
        SceneManager.LoadScene(1);

    }

    public void DusurButonunaTiklandi()
    {
        if (barYoneticisi == null || aktifDava == null) return;
        if (aktifDava.detaylar.davayiDusurmekGecerliMi)
        {
            barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.davayiDusurGecerli_KamuoyuEtkisi);
            barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.davayiDusurGecerli_HukukEtkisi);
        }
        else
        {
            barYoneticisi.DegistirKamuoyuGuven(aktifDava.detaylar.davayiDusurGecersiz_KamuoyuEtkisi);
            barYoneticisi.DegistirHukukGuven(aktifDava.detaylar.davayiDusurGecersiz_HukukEtkisi);
        }
        KapatVeGenelDurumuNormaleDonder(tokmakKararPanel);
        SceneManager.LoadScene(1);

    }

    public void KararVerSecenegiTiklandi()
    {
        if (kararGirisPaneli == null || aktifDava == null)
        {
            return;
        }
        kararGirisPaneli.KararPaneliniGoster(
            aktifDava.detaylar.idealHapisGun,
            aktifDava.detaylar.idealParaCezasi,
            aktifDava.detaylar.idealBeraatOlmalydi
        );
        if (tokmakKararPanel != null)
            tokmakKararPanel.SetActive(false);
    }

    public void ErtelemeMesajPaneliKapat()
    {
        if (erteleMesajPanel != null) erteleMesajPanel.SetActive(false);
        if (tokmakKararPanel != null) tokmakKararPanel.SetActive(true);
    }

    void KapatVeGenelDurumuNormaleDonder(GameObject kapatilacakPanel)
    {
        if (kapatilacakPanel != null) kapatilacakPanel.SetActive(false);
        ArkaPlanVeDigerUIAyarla(false);
    }

    void ArkaPlanVeDigerUIAyarla(bool modalAcikMi)
    {
    
    }

    public void PaneliKapat()
    {
        tokmakKararPanel.SetActive(false);
    }
}