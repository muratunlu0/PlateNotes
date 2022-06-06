using UnityEngine;
using UnityEngine.UI;
public class profilgorme : MonoBehaviour
{
    Transform like_tusu;
    private void Start()
    {
        like_tusu = gameObject.transform.Find("like");
    }
    public void plaka_profil_gorme()
    {       
       if (GameObject.Find("firebase-message").GetComponent<databasee>().
            plaka_profili_baslik_plaka_no.text!= gameObject.transform.Find("ust bilgi paneli").
                Find("hangi plakaya yazisi").GetComponentInChildren<Text>().text)
        {
            plaka_profil_func();
        }
        else if(GameObject.Find("firebase-message").GetComponent<databasee>().
             plaka_profili_baslik_plaka_no.text == gameObject.transform.Find("ust bilgi paneli").
                 Find("hangi plakaya yazisi").GetComponentInChildren<Text>().text && GameObject.Find("firebase-message").GetComponent<databasee>().
            plaka_profili_paneli.active ==false)
        {
            plaka_profil_func();
        }
    }
    void plaka_profil_func()
    {
        GameObject.Find("firebase-message").GetComponent<databasee>().plaka_no_search.text = gameObject.transform.Find("ust bilgi paneli").
               Find("hangi plakaya yazisi").GetComponentInChildren<Text>().text;

        string ulke_kodu = gameObject.transform.Find("ust bilgi paneli").
            Find("hangi plakaya yazisi").Find("ulkekodu").Find("Text").GetComponentInChildren<Text>().text;
        GameObject.Find("firebase-message").GetComponent<databasee>().plaka_araa(ulke_kodu);
    }
    public void user_profile_view()
    {
        if(GameObject.Find("firebase-message").GetComponent<databasee>().profilim_paneli.active!=true)
        {           
            GameObject.Find("firebase-message").GetComponent<databasee>().profilim_post_getirr(gameObject.transform.Find("ust bilgi paneli").GetChild(0).name);
        }
    }
    public void post_like()
    {
        GameObject.Find("firebase-message").GetComponent<databasee>().like_post(like_tusu, gameObject.transform.name);
    }
    public void post_secenekler()
    {
        GameObject.Find("firebase-message").GetComponent<databasee>().post_options(gameObject.transform.name);
    }
}
