using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagdurveHakimKonusma : MonoBehaviour
{
    public GameObject hakimKonusmaPanel;
    public TextMeshProUGUI hakimMetinText;
    public TextMeshProUGUI hakimAdiText;
    public Button hakimDevamEtButon;

    public GameObject magdurKonusmaPanel;
    public TextMeshProUGUI magdurMetinText;
    public TextMeshProUGUI magdurAdiText;
    public Button magdurDevamEtButon;
    public Button magdurKapatButon;
    public Button magdurButon;

    public float harfHiz = 0.05f;

    public List<KonusmaMetniData> magdurDiyalogMetinleri;

    private int mevcutMetinIndex = 0;
    private Coroutine mevcutMetinAnimasyonu;

    public SanikveHakimKonusma sanikveHakimKonusma;
    public SavciKonusma savciKonusma;
    public SanýkAvukatýKonusma sanýkAvukatýKonusma;
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
            Debug.LogWarning("Maðdur diyalog listesi boþ!");
            return;
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);

        ButonlariKapat();

        mevcutMetinIndex = 0;
        MevcutMetniGoster();
    }

    void SonrakiMetniGoster()
    {
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
        if (mevcutMetinAnimasyonu != null)
        {
            StopCoroutine(mevcutMetinAnimasyonu);
        }

        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);
        magdurDevamEtButon.gameObject.SetActive(false);
        hakimDevamEtButon.gameObject.SetActive(false);

        KonusmaMetniData mevcutKonusma = magdurDiyalogMetinleri[mevcutMetinIndex];

        if (mevcutKonusma.konusmaciAdi == "Maðdur")
        {
            magdurKonusmaPanel.SetActive(true);
            magdurAdiText.text = mevcutKonusma.konusmaciAdi;
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(magdurMetinText, mevcutKonusma.metin, magdurDevamEtButon));
        }
        else if (mevcutKonusma.konusmaciAdi == "Hakim")
        {
            hakimKonusmaPanel.SetActive(true);
            hakimAdiText.text = mevcutKonusma.konusmaciAdi;
            mevcutMetinAnimasyonu = StartCoroutine(MetniHarfHarfGoster(hakimMetinText, mevcutKonusma.metin, hakimDevamEtButon));
        }
        else
        {
            Debug.LogWarning("Bilinmeyen konuþmacý: " + mevcutKonusma.konusmaciAdi);
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

        devamButonu.gameObject.SetActive(true);
        devamButonu.interactable = true;
    }

    void KonusmayiBitir()
    {
        magdurKonusmaPanel.SetActive(false);
        hakimKonusmaPanel.SetActive(false);
        ButonlariAc();
        mevcutMetinIndex = 0;
    }

    void ButonlariKapat()
    {
        magdurButon.interactable = true;
        sanikveHakimKonusma.mikrofonButon.interactable = false;
        savciKonusma.savciButon.interactable = false;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = false;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = false;
    }

    void ButonlariAc()
    {
        magdurButon.interactable = true;
        sanikveHakimKonusma.mikrofonButon.interactable = true;
        savciKonusma.savciButon.interactable = true;
        sanýkAvukatýKonusma.sanikAvukatiButon.interactable = true;
        magdurAvukatiKonusma.magdurAvukatiButon.interactable = true;
    }
}
