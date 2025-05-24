using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

// KonusmaMetniData ScriptableObject'i ayr� bir dosyada tan�ml� oldu�u i�in,
// bu script i�inde tekrar TANIMLANMAMALI.

public class MagdurAvukatiKonusma : MonoBehaviour
{
    // Ma�dur Avukat�'n�n a��z animasyon kontrol script'ine referans
    public KarakterKonusmaAnimasyonu magdurAvukatiAnimationControl;

    public GameObject magdurAvukatiKonusmaPanel;
    public TextMeshProUGUI magdurAvukatiMetinText;
    public TextMeshProUGUI magdurAvukatiAdiText;
    public Button magdurAvukatiKapatButon;
    public Button magdurAvukatiButon; // Kendi diyalogunu ba�latan buton
    public Button magdurAvukatiDevamEtButon;


    public float harfHiz = 0.05f;

    // Diyalog verileri (KonusmaMetniData ScriptableObject'ini kullan�yor)
    public List<KonusmaMetniData> magdurAvukatiDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;
    // Hangi karakterin animasyonunun o an aktif oldu�unu tutmak i�in
    private KarakterKonusmaAnimasyonu currentSpeakerAnimationControl = null;


    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public San�kAvukat�Konusma san�kAvukat�Konusma;
    public MagdurveHakimKonusma magdurKonusma;
    public TokmakSistemiYoneticisi tokmakSistemiYoneticisi;
    
    void Start()
    {
        magdurAvukatiKonusmaPanel.SetActive(false); // Paneli ba�lang��ta kapat

        // Buton olaylar�n� dinlemeye ba�la
        magdurAvukatiButon.onClick.AddListener(KonusmayiBaslat);
        magdurAvukatiKapatButon.onClick.AddListener(KonusmayiBitir);
        magdurAvukatiDevamEtButon.onClick.AddListener(SonrakiMetniGoster);

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu ba�lang��ta kapat

        // Di�er butonlar�n ba�lang�� etkile�imini ayarla
        // Bu k�s�m genellikle t�m diyalog y�neticileri aras�nda tutarl� olmal�.
        // E�er Start'ta t�m butonlar�n a��k olmas�n� istiyorsan�z, ButonlariAc() �a��rabilirsiniz.
    }

    void KonusmayiBaslat()
    {
        if (magdurAvukatiDiyalogMetinleri == null || magdurAvukatiDiyalogMetinleri.Count == 0)
        {
            Debug.LogWarning("Ma�dur Avukat� diyalog listesi bo� veya atanmam��!");
            return;
        }

        // �nceki konu�an�n animasyonunu durdur (e�er varsa)
        if (currentSpeakerAnimationControl != null)
        {
            currentSpeakerAnimationControl.KonusmayiBitir();
            currentSpeakerAnimationControl = null;
        }

        magdurAvukatiKonusmaPanel.SetActive(true); // Paneli aktif et

        // magdurAvukatiButon'un interactable durumu bu fonksiyonda art�k de�i�tirilmeyecek.
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

            KonusmaMetniData currentData = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];
            magdurAvukatiMetinText.text = currentData.metin; // Metni an�nda tamamla

            // H�zl� t�klamada metin yaz�m� tamamland���nda a�z� kapat
            if (currentSpeakerAnimationControl != null)
            {
                currentSpeakerAnimationControl.KonusmayiBitir();
            }

            // Butonlar� tekrar aktif et (metin yazma tamamland�)
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
            KonusmayiBitir(); // T�m metinler g�sterildiyse konu�may� bitir.
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

        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Metine Ba�lad���m�zda devam et butonuna t�klayamamay� sa�lad�k.

        if (mevcutMetinIndex < magdurAvukatiDiyalogMetinleri.Count)
        {
            KonusmaMetniData mevcutKonusma = magdurAvukatiDiyalogMetinleri[mevcutMetinIndex];

            magdurAvukatiMetinText.text = ""; // Metin alan�n� temizle
            magdurAvukatiAdiText.text = mevcutKonusma.konusmaciAdi;

            // Ma�dur Avukat� animasyonunu ba�lat
            if (magdurAvukatiAnimationControl != null)
            {
                magdurAvukatiAnimationControl.KonusmayaBasla();
                currentSpeakerAnimationControl = magdurAvukatiAnimationControl; // Aktif konu�an yap
            }
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurAvukatiMetinText, mevcutKonusma.metin, magdurAvukatiDevamEtButon));
        }
        else
        {
            KonusmayiBitir(); // T�m metinler g�sterildiyse konu�may� bitir.
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

        devamButonu.gameObject.SetActive(true); // Metin tamamland�ktan sonra devam et butonu aktif
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

        magdurAvukatiKonusmaPanel.SetActive(false); // Paneli kapat
        magdurAvukatiDevamEtButon.gameObject.SetActive(false); // Devam et butonunu kapat

        ButonlariAc(); // T�m butonlar� a�
        mevcutMetinIndex = 0; // �ndeksi s�f�rla
    }

    void ButonlariKapat()
    { 
        // magdurAvukatiButon.interactable = false; // BU SATIR YORUM SATIRI YAPILDI / S�L�ND�

        // Di�er diyalog butonlar�n� kapat
      
            sanikveHakimKonusma.mikrofonButon.interactable = false;
            savciKonusma.savciButon.interactable = false;
            san�kAvukat�Konusma.sanikAvukatiButon.interactable = false;
            magdurKonusma.magdurButon.interactable = false;
           tokmakSistemiYoneticisi.anaTokmakButonu.interactable = false;
        

       

    }


    void ButonlariAc()
    {
        
        // Kendi butonunu a� (e�er ButonlariKapat i�inde kapat�lmad�ysa bu sat�r gereksiz olabilir ama g�venli b�rak�ld�)
        magdurAvukatiButon.interactable = true;

        // Di�er diyalog butonlar�n� a�
            sanikveHakimKonusma.mikrofonButon.interactable = true;
            savciKonusma.savciButon.interactable = true;
            san�kAvukat�Konusma.sanikAvukatiButon.interactable = true;
            magdurKonusma.magdurButon.interactable = true;
        tokmakSistemiYoneticisi.anaTokmakButonu.interactable = true; 





    }
}