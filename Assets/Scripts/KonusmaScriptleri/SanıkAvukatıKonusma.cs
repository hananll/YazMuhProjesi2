using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// KonusmaMetniData ScriptableObject'i ayrı bir dosyada tanımlı olduğu için,
// bu script içinde tekrar TANIMLANMAMALI.

public class SanıkAvukatıKonusma : MonoBehaviour
{
    // Sanık Avukatı'nın ağız animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu sanikAvukatiAnimationControl;

    public GameObject sanikAvukatiKonusmaPanel;
    public TextMeshProUGUI sanikAvukatiMetinText;
    public TextMeshProUGUI sanikAvukatiAdiText;
    public Button sanikAvukatiKapatButon;
    public Button sanikAvukatiButon; // Kendi diyalogunu başlatan buton
    public Button sanikAvukatiDevamEtButon;


    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullanıyor)
    public List<KonusmaMetniData> sanikAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    // Hangi karakterin animasyonunun o an aktif olduğunu tutmak için
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;


    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public MagdurveHakimKonusma magdurKonusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;


    void Start()
    {
        sanikAvukatiKonusmaPanel.SetActive(false); // Paneli başlangıçta kapat

        // Buton olaylarını dinlemeye başla
        sanikAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        sanikAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        sanikAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        sanikAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu başlangıçta kapat

        // Diğer butonların başlangıç etkileşimini ayarla (genellikle ButonlariAc çağrılır)
        // Eğer bu script'in Start'ında tüm butonların interactable = true olmasını istiyorsanız
        // ButonlariAc() metodunu çağırın. Şu anki kodunuzda bu yoktu, bu yüzden eklemedim.
        // Ancak bu butonların durumunu sadece KonusmayiBasla/Bitir yönetiyorsa sorun yok.
    }

    void KonusmayiBaslat()
    {
        if (sanikAvukatiDiyalogMetinleri == null || sanikAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Sanık Avukatı diyalog listesi boş veya atanmamış!");
            return;
        }

        // Önceki konuşanın animasyonunu durdur (eğer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        sanikAvukatiKonusmaPanel.SetActive(true); // Paneli aktif et

        ButonlariKapat(); // Diğer butonları kapat (kendi butonunu kapatma hariç)

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        // Eğer metin hala yazılıyorsa, yazmayı tamamla
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            KonusmaMetniData currentData = sanikAvukatiDiyalogMetinleri[mevcutMetinIndex];
            sanikAvukatiMetinText.text = currentData.metin; // Metni anında tamamla

            // Hızlı tıklamada metin yazımı tamamlandığında ağzı kapat
            if (currentSpeakerAnimationControl != null)
            {
                sanikAvukatiAnimationControl.KonusmayiBitir();
            }

            // Butonları tekrar aktif et (metin yazma tamamlandı)
            sanikAvukatiDevamEtButon.gameObject.SetActive(true);
            sanikAvukatiDevamEtButon.interactable = true;
            return;
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < sanikAvukatiDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuşmayı bitir.
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
        // Önceki konuşanın animasyonunu durdur (eğer varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        sanikAvukatiDevamEtButon.gameObject.SetActive(false); // Metine Başladığımızda devam et butonuna tıklayamamayı sağladık.

        if (mevcutMetinIndex < sanikAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = sanikAvukatiDiyalogMetinleri[mevcutMetinIndex];

            sanikAvukatiMetinText.text = ""; // Metin alanını temizle
            sanikAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            // Sanık Avukatı animasyonunu başlat
            if (sanikAvukatiAnimationControl != null)
            {
                sanikAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = sanikAvukatiAnimationControl; // Aktif konuşan yap
            }
            // MetniHarfHarfGoster'e devam butonu parametresini ekledik
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(sanikAvukatiMetinText, mevcutKonusma.metin, sanikAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir(); // Tüm metinler gösterildiyse konuşmayı bitir.
        }
    }

    IEnumerator MetniHarfHarfGoster(TextMeshProUGUI metinAlani, string metin, Button devamButonu) // Button parametresi eklendi
    {
        int harfSayisi = 0;
        metinAlani.text = "";
        while (harfSayisi < metin.Length)
        {
            harfSayisi++;
            metinAlani.text = metin.Substring(0, harfSayisi);
            yield return new WaitForSeconds(harfHiz);
        }

        // Metin yazımı tamamlandığında konuşma animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        devamButonu.gameObject.SetActive(true); // Metin tamamlandıktan sonra devam et butonu aktif
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        // Aktif olan son konuşanın animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        sanikAvukatiKonusmaPanel.SetActive(false); // Paneli kapat
        sanikAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu kapat

        ButonlariAc(); // Tüm butonları aç
        mevcutMetinIndex = 0; // İndeksi sıfırla
    }

    void ButonlariKapat()
    {
        // sanikAvukatiButon.interactable = false; // Bu satır yorum satırı yapıldı / silindi

        // Diğer diyalog butonlarını kapat
        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = false;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = false;
        if (magdurKonusma != null && magdurKonusma.magdurButon != null)
            magdurKonusma.magdurButon.interactable = false;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void ButonlariAc()
    {
        // Kendi butonunu aç (eğer ButonlariKapat içinde kapatılmadıysa bu satır gereksiz olabilir ama güvenli bırakıldı)
        sanikAvukatiButon.interactable = true;

        // Diğer diyalog butonlarını aç
        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = true;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = true;
        if (magdurKonusma != null && magdurKonusma.magdurButon != null)
            magdurKonusma.magdurButon.interactable = true;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}