using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class TanikCagirici : MonoBehaviour
{
    [Header("Geçiþ Ekraný")]
    public GameObject AliKemalCagirmaGecisEkrani;
    public GameObject AliKemalBitirmeGecisEkrani;
    public GameObject EbruGurCagirmaGecisEkrani;
    public GameObject EbruGurBitirmeGecisEkrani;

    public TextMeshProUGUI gecisYazisi;
    public AudioSource sesOynatici;
    public AudioClip tanikSesi;

    [Header("Sahne Adlarý")]
    public string tanik1SahneAdi;
    public string tanik2SahneAdi;
    public string anaSahneAdi; // ifadesi biten tanýðýn dönüþ sahnesi

    // Tanýk çaðýrma fonksiyonu
    public void Tanik1Cagir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(TanikGecisi("Tanýk Ali Kemal Gezmiþ", tanik1SahneAdi));
        AliKemalCagirmaGecisEkrani.SetActive(true);

    }

    public void Tanik2Cagir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(TanikGecisi("Tanýk Ebru Gür", tanik2SahneAdi));
        EbruGurCagirmaGecisEkrani.SetActive(true);
    }

    // Tanýk ifadesini sonlandýrma fonksiyonlarý
    public void Tanik1IfadesiniSonlandir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(SonlandirmaGecisi("Ali Kemal Gezmiþ’in ifadesi sonlandýrýlýyor...", anaSahneAdi));
        AliKemalBitirmeGecisEkrani.SetActive(true);
    }

    public void Tanik2IfadesiniSonlandir()
    {
        sesOynatici.PlayOneShot(tanikSesi);
        StartCoroutine(SonlandirmaGecisi("Ebru Gür'ün ifadesi sonlandýrýlýyor...", anaSahneAdi));
        EbruGurBitirmeGecisEkrani.SetActive(true);
    }

    // Ortak geçiþ coroutine’leri
    private IEnumerator TanikGecisi(string tanikAdi, string sahneAdi)
    {
        // gecisEkrani.SetActive(true);
        sesOynatici.PlayOneShot(tanikSesi);
        gecisYazisi.text = $"{tanikAdi} çaðrýlýyor...";
        

        yield return new WaitForSeconds(tanikSesi.length + 1f);

        SceneManager.LoadScene(sahneAdi);
    }

    private IEnumerator SonlandirmaGecisi(string mesaj, string sahneAdi)
    {
       // gecisEkrani.SetActive(true);
        gecisYazisi.text = mesaj;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sahneAdi);
    }
}
