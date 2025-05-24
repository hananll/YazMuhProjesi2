using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

// KonusmaMetniData ScriptableObject'i ayrý bir dosyada tanýmlý olduðu için,
// bu script içinde tekrar TANIMLANMAMALI.

public class MagdurAvukatiKonusma : MonoBehaviour
{
    // Maðdur Avukatý'nýn aðýz animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu magdurAvukatiAnimationControl;

    public GameObject magdurAvukatiKonusmaPanel;
    public TextMeshProUGUI magdurAvukatiMetinText;
    public TextMeshProUGUI magdurAvukatiAdiText;
    public Button magdurAvukatiKapatButon;
    public Button magdurAvukatiButon; // Kendi diyalogunu baþlatan buton
    public Button magdurAvukatiDevamEtButon;


    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullanýyor)
    public List<KonusmaMetniData> magdurAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    // Hangi karakterin animasyonunun o an aktif olduðunu tutmak için
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;


    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public TokmakSistemiYoneticisi tokmakSistemiYoneticisi;
    
    void Start()
    {
        magdurAvukatiKonusmaPanel.SetActive(false); // Paneli baþlangýçta kapat

        // Buton olaylarýný dinlemeye baþla
        magdurAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        magdurAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu baþlangýçta kapat

        // Diðer butonlarýn baþlangýç etkileþimini ayarla
        // Bu kýsým genellikle tüm diyalog yöneticileri arasýnda tutarlý olmalý.
        // Eðer Start'ta tüm butonlarýn açýk olmasýný istiyorsanýz, ButonlariAc() çaðýrabilirsiniz.
    }

    void KonusmayiBaslat()
    {
        if (magdurAvukatiDiyalogMetinleri == null || magdurAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Maðdur Avukatý diyalog listesi boþ veya atanmamýþ!");
            return;
        }

        // Önceki konuþanýn animasyonunu durdur (eðer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurAvukatiKonusmaPanel.SetActive(true); // Paneli aktif et

        // magdurAvukatiButon'un interactable durumu bu fonksiyonda artýk deðiþtirilmeyecek.
        // Sadece diðer butonlarý kapatacaðýz.
        ButonlariKapat();

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        // Eðer metin hala yazýlýyorsa, yazmayý tamamla
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            KonusmaMetniData currentData = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];
            magdurAvukatiMetinText.text = currentData.metin; // Metni anýnda tamamla

            // Hýzlý týklamada metin yazýmý tamamlandýðýnda aðzý kapat
            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            // Butonlarý tekrar aktif et (metin yazma tamamlandý)
            magdurAvukatiDevamEtButon.gameObject.SetActive(true);
            magdurAvukatiDevamEtButon.interactable = true;
            return;
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuþmayý bitir.
        }
    }

    void MevcutMetniGoster()
    {
        // Önceki metin animasyonunu durdur
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;
        }
        // Önceki konuþanýn animasyonunu durdur (eðer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Metine Baþladýðýmýzda devam et butonuna týklayamamayý saðladýk.

        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];

            magdurAvukatiMetinText.text = ""; // Metin alanýný temizle
            magdurAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            // Maðdur Avukatý animasyonunu baþlat
            if (magdurAvukatiAnimationControl != null)
            {
                magdurAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAvukatiAnimationControl; // Aktif konuþan yap
            }
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurAvukatiMetinText, mevcutKonusma.metin, magdurAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuþmayý bitir.
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin, Button devamButonu)
    {
        int harfSayisi = 0;
        metinAlani.text = "";
        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        // Metin yazýmý tamamlandýðýnda konuþma animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        devamButonu.gameObject.SetActive(true); // Metin tamamlandýktan sonra devam et butonu aktif
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        // Aktif olan son konuþanýn animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurAvukatiKonusmaPanel.SetActive(false); // Paneli kapat
        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu kapat

        ButonlariAc(); // Tüm butonlarý aç
        mevcutMetinIndex = 0; // Ýndeksi sýfýrla
    }

    void ButonlariKapat()
    { 
        // magdurAvukatiButon.interactable = false; // BU SATIR YORUM SATIRI YAPILDI / SÝLÝNDÝ

        // Diðer diyalog butonlarýný kapat
      
            sanikveHakimKonusma.mikrofonButon.interactable = false;
            savciKonusma.savciButon.interactable = false;
            sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
            magdurKonusma.magdurButon.interactable = false;
           tokmakSistemiYoneticisi.anaTokmakButonu.interactable = false;
        

       

    }


    void ButonlariAc()
    {
        
        // Kendi butonunu aç (eðer ButonlariKapat içinde kapatýlmadýysa bu satýr gereksiz olabilir ama güvenli býrakýldý)
        magdurAvukatiButon.interactable = true;

        // Diðer diyalog butonlarýný aç
            sanikveHakimKonusma.mikrofonButon.interactable = true;
            savciKonusma.savciButon.interactable = true;
            sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
            magdurKonusma.magdurButon.interactable = true;
        tokmakSistemiYoneticisi.anaTokmakButonu.interactable = true; 





    }
}