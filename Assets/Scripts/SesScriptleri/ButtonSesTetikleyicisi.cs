using UnityEngine;
using UnityEngine.EventSystems; // Fare olaylar�n� (PointerEnter) yakalamak i�in bu aray�z gerekli

public class ButonSesTetikleyici : MonoBehaviour, IPointerEnterHandler // IPointerEnterHandler aray�z�n� implement ediyoruz
{
    // Fare imleci bu UI eleman�n�n �zerine geldi�inde bu fonksiyon otomatik olarak �a�r�l�r
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SesYoneticisi.Ornek != null)
        {
            SesYoneticisi.Ornek.OynatButonUzerineGelmeSesi();
        }
        else
        {
            // Bu uyar�, SesYoneticisi'nin sahnedeki kurulumunda bir sorun varsa g�r�n�r.
            Debug.LogWarning("ButonSesTetikleyici: SesYoneticisi.Ornek bulunamad�! Ses �al�nam�yor.");
        }
    }
}