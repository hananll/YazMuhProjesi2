using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// KonusmaMetniData ScriptableObject'i ayr� bir dosyada tan�ml� oldu�u i�in,
// bu script i�inde tekrar TANIMLANMAMALI.

public class MagdurveHakimKonusma : MonoBehaviour
{
    // Ma�dur'un a��z animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu magdurAnimationControl;
    // Hakim i�in de animasyon kontrol� ekleyecekseniz (e�er ona da a��z yap�s� kurduysan�z):
    // public KarakterKonusmaAnimasyonu hakimAnimationControl;


    // Hakim Konu�ma UI Elemanlar�
    public GameObject hakimKonusmaPanel;
    public TextMeshProUGUI hakimMetinText;
    public TextMeshProUGUI hakimAdiText;
    public Button hakimDevamEtButon;

    // Ma�dur Konu�ma UI Elemanlar�
    public GameObject magdurKonusmaPanel;
    public TextMeshProUGUI magdurMetinText;
    public TextMeshProUGUI magdurAdiText;
    public Button magdurDevamEtButon;
    public Button magdurKapatButon;
    public Button magdurButon; // Ma�dur'un kendi diyalogunu ba�latan buton

    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullan�yor)
    public List<KonusmaMetniData> magdurDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null; // Aktif konu�an�n animasyonunu y�netmek i�in

    // Di�er diyalog y�neticilerine referanslar (buton etkile�imi i�in)
    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public San�kAvukat�Konusma san�kAvukat�Konusma;
    public MagdurAvukatiKonusma magdurAvukatiKonusma;

    void Start()
    {
        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        magdurButon.onClick.AddListener(KonusmayiBaslat);
        magdurKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurDevamEtButon.onClick.AddListener(SonrakiMetniGoster);
        hakimDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);
    }

    void KonusmayiBaslat()
    {
        if (magdurDiyalogMetinleri == null || magdurDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Ma�dur diyalog listesi bo� veya atanmam��!");
            return;
        }

        // �nceki konu�an�n animasyonunu durdur (e�er varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        // magdurButon'un interactable durumu bu fonksiyonda art�k de�i�tirilmeyecek.
        // Sadece di�er butonlar� kapataca��z.
        ButonlariKapat();

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
        // E�er metin hala yaz�l�yorsa, yazmay� tamamla
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;

            // Metni an�nda tamamla
            KonusmaMetniData currentData = magdurDiyalogMetinleri[mevcutMetinIndex];
            if (currentData.konusmaciAdi == "Ma�dur" && magdurMetinText != null) magdurMetinText.text = currentData.metin;
            else if (currentData.konusmaciAdi == "Hakim" && hakimMetinText != null) hakimMetinText.text = currentData.metin;

            // H�zl� t�klamada metin yaz�m� tamamland���nda a�z� kapat
            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            // Butonlar� tekrar aktif et (metin yazma tamamland�)
            magdurDevamEtButon.gameObject.SetActive(true);
            magdurDevamEtButon.interactable = true;
            hakimDevamEtButon.gameObject.SetActive(true);
            hakimDevamEtButon.interactable = true;
            return;
        }

        mevcutMetinIndex++;
        if (mevcutMetinIndex < magdurDiyalogMetinleri.Count)
        {
            MevcutMetniGoster();
        }
        else
        {
            KonusmayiBitir();
        }
    }

    void MevcutMetniGoster()
    {
        // �nceki metin animasyonunu durdur
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
            mevcutMetinAnimasyonu = null;
        }
        // �nceki konu�an�n animasyonunu durdur (e�er varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);

        KonusmaMetniData mevcutKonusma = magdurDiyalogMetinleri[mevcutMetinIndex];

        if (mevcutKonusma.konusmaciAdi == "Ma�dur")
        {
            magdurKonusmaPanel.SetActive(true);
            magdurAdiText.text = mevcutKonusma.konusmaciAdi;

            // Ma�dur animasyonunu ba�lat
            if (magdurAnimationControl != null)
            {
                magdurAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAnimationControl; // Aktif konu�an yap
            }
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurMetinText, mevcutKonusma.metin, magdurDevamEtButon));
        }
        else if (mevcutKonusma.konusmaciAdi == "Hakim")
        {
            hakimKonusmaPanel.SetActive(true);
            hakimAdiText.text = mevcutKonusma.konusmaciAdi;
            // Hakim i�in animasyon kontrol� varsa burada ba�lat�n
            // if (hakimAnimationControl != null)
            // {
            //     hakimAnimationControl.KonusmayaBasla();
            //     currentSpeakerAnimationControl = hakimAnimationControl;
            // }
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(hakimMetinText, mevcutKonusma.metin, hakimDevamEtButon));
        }
        else
        {
            Debug.LogWarning("Bilinmeyen konu�mac�: " + mevcutKonusma.konusmaciAdi + " (Ma�dur Diyalog Scriptinde) �ndeks: " + mevcutMetinIndex);
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

        // Metin yaz�m� tamamland���nda konu�ma animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
        }

        devamButonu.gameObject.SetActive(true);
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        // Aktif olan son konu�an�n animasyonunu durdur
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariAc(); // magdurButon da burada tekrar aktif edilecek.
        mevcutMetinIndex = 0;
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);
    }

    void ButonlariKapat()
    {
        // magdurButon.interactable = false; // BU SATIR YORUM SATIRI YAPILDI / S�L�ND�

        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = false;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = false;
        if (san�kAvukat�Konusma != null && san�kAvukat�Konusma.sanikAvukatiButon != null)
            san�kAvukat�Konusma.sanikAvukatiButon.interactable = false;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void ButonlariAc()
    {
        // magdurButon'u daima true olaca�� i�in sadece di�erlerini a��yoruz.
        // Ancak bu fonksiyon KonusmayiBitir() i�inde �a�r�ld��� i�in
        // ve magdurButon'un durumunu Start'ta veya ba�ka bir yerde y�netiyorsan�z
        // burada de�i�tirmemek daha iyi olabilir.
        // E�er Start'ta magdurButon'u kapatt�ysan�z, burada tekrar a�man�z gerekir.
        // �u anki koda g�re magdurButon'u her zaman true tuttu�umuz i�in bu sat�r gereksiz olabilir.
        magdurButon.interactable = true;

        if (sanikveHakimKonusma != null && sanikveHakimKonusma.mikrofonButon != null)
            sanikveHakimKonusma.mikrofonButon.interactable = true;
        if (savciKonusma != null && savciKonusma.savciButon != null)
            savciKonusma.savciButon.interactable = true;
        if (san�kAvukat�Konusma != null && san�kAvukat�Konusma.sanikAvukatiButon != null)
            san�kAvukat�Konusma.sanikAvukatiButon.interactable = true;
        if (magdurAvukatiKonusma != null && magdurAvukatiKonusma.magdurAvukatiButon != null)
            magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}