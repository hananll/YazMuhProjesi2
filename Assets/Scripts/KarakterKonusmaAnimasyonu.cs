using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class KarakterKonusmaAnimasyonu : MonoBehaviour
{
    private Animator animator;
    private readonly int konusuyorMuParamID = Animator.StringToHash("KonusuyorMu"); // Parametre ID'sini cache'lemek daha performansl�d�r

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Bu objeye atanm�� bir Animator component'i bulunamad�!");
            enabled = false; // Script'i devre d��� b�rak
            return;
        }
    }

    // Bu fonksiyonu diyalog sisteminizden konu�ma ba�lad���nda �a��r�n
    public void KonusmayaBasla()
    {
        if (animator != null)
        {
            animator.SetBool(konusuyorMuParamID, true);
            Debug.Log(gameObject.name + " konu�maya ba�lad� animasyonu.");
        }
    }

    // Bu fonksiyonu diyalog sisteminizden konu�ma bitti�inde �a��r�n
    public void KonusmayiBitir()
    {
        if (animator != null)
        {
            animator.SetBool(konusuyorMuParamID, false);
            Debug.Log(gameObject.name + " konu�may� bitirdi animasyonu.");
        }
    }
}
