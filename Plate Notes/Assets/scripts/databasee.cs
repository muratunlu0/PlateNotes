using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Globalization;
using UnityEngine.Networking;
using TMPro;

public class databasee : MonoBehaviour
{
    protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();

    // [Header("degiskenlere bakınca analrsın bea :)")]
    List<string> ulke_plate_code_list = new List<string> { "TR", "A", "AFG", "AL", "AM", "AND", "AUS", "AZ", "B", "BD", "BDS", "BF", "BG", "BH", "BIH", "BOL", "BR", "BRN", "BRU", "BS", "BUR", "BVI", "BW", "BY",
        "C", "CAM", "CDN", "CGO", "CH", "CI", "CL", "CO", "CR", "CY", "CZ", "D", "DK", "DOM", "DY", "DZ", "E", "EAK", "EAT", "EAU", "EAZ", "EC", "EIR", "ER", "ES", "EST", "ET", "ETH", "F",
        "FIN", "FJI", "FL", "FO", "G", "GB", "GBA", "GBG", "GBJ", "GBM", "GBZ", "GCA", "GE", "GH", "GR", "GUY", "H", "HKJ", "HN", "HR", "I", "IL", "IND", "IR", "IRL", "IRQ", "IS", "J", "JA", "K",
        "KS", "KSA", "KWT", "KZ", "L", "LAO", "LAR", "LB", "LS", "LT", "LV", "M", "MA", "MAL", "MC", "MD", "MEX", "MNE", "MNG", "MOC", "MS", "MW", "N", "NA", "NAM", "NAU", "NEP", "NIC", "NL",
        "NMK", "NZ", "P", "PA", "PE", "PK", "PL", "PNG", "PY", "Q", "RA", "RC", "RCA", "RCB", "RCH", "RG", "RH", "RI", "RIM", "RKS", "RL", "RM", "RMM", "RN", "RO", "ROK", "RP", "RSM", "RU", "RUS",
        "RWA", "S", "SD", "SGP", "SK", "SLO", "SME", "SN", "SO", "SRB", "SUD", "SY", "SYR", "T", "TCH, TD", "TG", "TJ", "TM", "TN", "TT", "UA", "UAE", "USA", "UY", "UZ", "V", "VN", "WAG", "WAL",
        "WAN", "WD", "WG", "WL", "WS", "WV", "YAR", "YV", "Z", "ZA", "ZW" };
    List<string> ulke_names = new List<string> { "Türkiye", "Austria", "Afghanistan", "Albania", "Armenia", "Andorra", "Australia", "Azerbaijan", "Belgium", "Bangladesh", "Barbados", "Burkina Faso", "Bulgaria", "Belize", "Bosnia and Herzegovina", "Bolivia", "Brazil", "Bahrain", "Brunei", "Bahamas", "Myanmar", "British Virgin Islands", "Botswana", "Belarus",
        "Cuba", "Cameroon", "Canada", "Democratic Republic of the Congo", "Switzerland", "Ivory Coast", "Sri Lanka", "Colombia", "Costa Rica", "Cyprus", "Czech Republic", "Germany", "Denmark", "Dominican Republic", "Benin", "Algeria", "Spain", "Kenya", "Tanzania", "Uganda", "Zanzibar", "Ecuador", "Ireland", "Eritrea", "El Salvador", "Estonia", "Egypt", "Ethiopia", "France",
        "Finland", "Fiji", "Liechtenstein", "Faroe Islands", "Gabon", "United Kingdom (of Great Britain and Northern Ireland)", "Alderney", "Guernsey", "Jersey", "Isle of Man", "Gibraltar", "Guatemala", "Georgia", "Ghana", "Greece", "Guyana", "Hungary", "Jordan", "Honduras", "Croatia", "Italy", "Israel", "India", "Iran", "Ireland", "Iraq", "Iceland", "Japan", "Jamaica", "Cambodia",
        "Kyrgyzstan", "Saudi Arabia", "Kuwait", "Kazakhstan", "Luxembourg", "Laos", "Libya", "Liberia", "Lesotho", "Lithuania", "Latvia", "Malta", "Morocco", "Malaysia", "Monaco", "Moldova", "Mexico", "Montenegro", "Mongolia", "Mozambique", "Mauritius", "Malawi", "Norway", "Netherlands Antilles", "Namibia", "Nauru", "Nepal", "Nicaragua", "Netherlands",
        "North Macedonia", "New Zealand", "Portugal", "Panama", "Peru", "Pakistan", "Poland", "Papua New Guinea", "Paraguay", "Qatar", "Argentina", "Republic of China (Taiwan)", "Central African Republic ", "Republic of the Congo", "Chile", "Guinea", "Haiti", "Indonesia", "Mauritania", "Kosovo", "Lebanon", "Madagascar", "Mali", "Niger", "Romania", "South Korea", "Philippines", "San Marino", "Burundi", "Russia",
        "Rwanda", "Sweden", "Eswatini", "Singapore", "Slovakia", "Slovenia", "Suriname", "Senegal", "Somalia", "Serbia", "Sudan", "Seychelles", "Syria", "Thailand", "Chad", "Togo", "Tajikistan", "Turkmenistan", "Tunisia", "Trinidad and Tobago", "Ukraine", "United Arab Emirates", "United States", "Uruguay", "Uzbekistan", "Vatican City", "Vietnam", "Gambia", "Sierra Leone",
        "Nigeria", "Dominica", "Grenada", "Saint Lucia", "Samoa", "Saint Vincent and the Grenadines", "Yemen", "Venezuela", "Zambia", "South Africa", "Zimbabwe" };

    [Header("KULLANICI ADI KONTROL PANELİ DEGİSKENLER")]
    public GameObject[] username_status_icons;
    public GameObject username_check_panel;

    [Header("YENİ KAYIT DEGİSKENLER")]
    public int yeni_kayit_mi;

    [Header("STORAGE İCİN DEGİSKENLER")]
    public GameObject profil_image_change_panel_resim_varsa;
    public GameObject profil_resmi_yukle_icon;
    protected string storageLocation = "gs://plate-6d02b.appspot.com";

    [Header("SHUFFLE PANELİ İCİN DEGİSKENLER")]
    public GameObject kesfet_paneli;
    public GameObject content_kesfet_profili;
    public Scrollbar yenile_post_ekle_shuffle;
    public InputField plaka_no_search;
    List<DataSnapshot> posts_kesfet = new List<DataSnapshot>();

    [Header("BİLDİRİMLER PANELİ İCİN DEGİSKENLER")]
    public GameObject bildirimler_paneli;
    public GameObject content_bildirimler;
    public Scrollbar yenile_post_ekle_bildirimler;
    public GameObject bildirimler_button;
    List<DataSnapshot> posts_bildirimler = new List<DataSnapshot>();
    
    [Header("PROFİL PANELİ İCİN DEGİSKENLER")]
    public GameObject profilim_paneli;
    public GameObject profil_buyuk_image_paneli;
    public GameObject yuzsuz_resim_profil;
    public GameObject secenekler_tusu_kisi_profili;
    public Text username_profil;
    string instagram_profil_yazi;
    public GameObject instagram_tusu;
    public Text user_uid;
    string other_user_uid;
    ArrayList my_follow_plakalar_list = new ArrayList();

    [Header("MY_PROFİLE PANELİ İCİN DEGİSKENLER")]
    public GameObject my_profile_paneli;
    public Scrollbar scrollbar_my_profile;
    public GameObject content_my_profile;
    List<DataSnapshot> posts_my_profile  = new List<DataSnapshot>();

    [Header("POST PAYLAS PANELİ İCİN DEGİSKENLER")]
    public GameObject post_paylas_paneli;
    public Text kullanici_takma_ad_post_paylas_yazi;
    public TMP_InputField mesaj;
    public Text plaka_no;
    public GameObject profil_Resmi_post_paylas;
    public GameObject yuzsuz_resim_post_paylas;

    [Header("PLAKA PANELİ İCİN DEGİSKENLER")]
    public GameObject plaka_profili_paneli;
    public GameObject content_plaka_profili;
    public Text plaka_profili_baslik_plaka_no;
    public Text plaka_profili_baslik_ulke_kodu;
    public Text plaka_profili_yorum_sayisi;
    public Text plaka_profili_takipci_sayisi;
    public InputField takma_ad_plaka;
    string gecici_takma_ad_plaka;
    public GameObject hic_yorum_Yok_paneli;
    public GameObject takip_et_tusu;
    public GameObject takip_et_butonu_yukleniyor_obje;
    public Text takip_et_yazisi;
    List<DataSnapshot> posts_plaka_paneli = new List<DataSnapshot>();
    public Color[] takip_et_buton_renk;
    [Header("POST PANELİ İCİN DEGİSKENLER")]
    public GameObject post_paneli;
    public GameObject post_direct;

    [Header("PROFIL - AYARLAR PANELİ İCİN DEGİSKENLER")]
    public GameObject profil_ayarlar_paneli;
    public Text username;
    public InputField instagram_ayarlar_input;
    public GameObject profil_resmi_ayarlar_menu;
    public InputField plaka1_ekle_ayarlar;
    public Text user_ulke;
    public Text[] ulke_kodu_yazi;
    public InputField garaj_plakano;
    
    [Header("PROFİL RESMİ AYARLA PANELİ İCİN DEGİSKENLER")]
    byte[] profil_resmi_upload_buyuk;
    string gecici_profil_resmi_konumu;
    public GameObject profilresmi_onizleme;
    public GameObject profil_resmi_ayarla_paneli;

    [Header("YUKLENİYOR PANELİ İCİN DEGİSKENLER")]
    public GameObject yukleniyor_paneli;

    [Header("BULUNAMADİ PANELİ İCİN DEGİSKENLER")]
    public GameObject bulunamadi_paneli;
    public Text bulunamadi_yazisi;

    [Header("POST İLE İLGİLİ DEGİSKENLER")]
    public GameObject post_harbi;
    public GameObject post_secenekler_paneli;
    public GameObject post_sikayet_et_tusu;
    public GameObject post_sil_ana_tus;
    public Text post_secenekler_post_id_gecici;

    public GameObject havuz_parent;
    public GameObject havuz_parent_bildirimler;

    private SimplePool<GameObject> post_havuzu;
    private SimplePool<GameObject> post_havuzu_bildirimler;

    [Header("BİLDİRİM ATMA İLE İLGİLİ DEGİSKENLER")]
    public Text bildirim_yazisi;
    public int bildirim_suresi = 2;
    public GameObject toast_mesaj_paneli;

    [Header("ZAMANLA İLE İLGİLİ DEGİSKENLER")]
    DateTime oldtime;
    string[] zaman_normal_turkish = { " yıl önce", " hafta önce", " gün önce", " saat önce", " dakika önce", " saniye önce" };
    string[] zaman_mini_turkish = { " y", " h", " g", " s", " d", " sn" };
    string[] time_things;

    [Header("ULKE KODU LİSTESİ İLE İLGİLİ DEGİSKENLER")]
    public Dropdown ulke_kodu_liste_post_paylas;
    int hangi_ulke_kodu_butonu;

    [Header("ULKELER LİSTESİ İLE İLGİLİ DEGİSKENLER")]
    public Dropdown ulkeler_dropdown;

    [Header("KAYIT ETME İLE İLGİLİ DEGİSKENLER")]
    Hashtable takma_ad_plaka_list = new Hashtable();

    [Header("BİLDİRİM ATMA İLE İLGİLİ DEGİSKENLER")]
    public GameObject[] genel_tuslar_status;

    protected virtual void Start()
    {
        post_havuzu = new SimplePool<GameObject>(post_harbi);
        post_havuzu_bildirimler = new SimplePool<GameObject>(bildirimler_button);
    }
    public void intcagir_database()
    {
        InitializeFirebase();
    }
    protected virtual void InitializeFirebase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://plate-6d02b.firebaseio.com/");
        if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
        yukleniyor_paneli.SetActive(false);
        bulunamadi_paneli.SetActive(false);
        username_exist_check();
    }
    public void username_exist_check()
    {
        FirebaseDatabase.DefaultInstance
        .GetReference("users/public/").Child(user_uid.text).Child("username")
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_yap_kayit_ol_paneli.SetActive(true);
                GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_ekrani_paneli.SetActive(false);
                bildirim_create("Akış yenilenemedi");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    auth_user_data();
                }
                else
                {
                    username_check_panel.SetActive(true);
                    GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_yap_kayit_ol_paneli.SetActive(false);
                    GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_ekrani_paneli.SetActive(false);
                    kesfet_paneli.SetActive(false);
                    profil_ayarlar_paneli.SetActive(true);
                }
            }
        });
    }
    public void plaka_info_check()
    {
        if (plaka_profili_baslik_plaka_no.text != "")
        {
            FirebaseDatabase.DefaultInstance
             .GetReference("/counts/plaka_info_count/" + plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text)
             .GetValueAsync().ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     if (snapshot.Exists)
                     {
                         if (snapshot.Child("follow_count").Exists)
                         {
                             plaka_profili_takipci_sayisi.text = snapshot.Child("follow_count").Value.ToString();
                         }
                         if (snapshot.Child("post_count").Exists)
                         {
                             if(Convert.ToInt32(snapshot.Child("post_count").Value.ToString())<=30)
                             {
                                 plaka_profili_yorum_sayisi.text = snapshot.Child("post_count").Value.ToString();
                             }
                             else
                             {
                                 plaka_profili_yorum_sayisi.text = "30+";
                             }
                         }
                     }
                     Debug.Log("plaka_info_check tamamlandı");
                 }
             });
        }
    }
    public void post_sil()
    {
        DatabaseReference bana_ait_postlar = FirebaseDatabase.DefaultInstance.GetReference("/posts/").Child(post_secenekler_post_id_gecici.text);
        bana_ait_postlar.RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                bildirim_create("İşlem tamamlanamadı");
            }
            else if (task.IsCompleted)
            {
                bildirim_create("Gönderi silindi");
            }
        });
    }
    public void post_options(string post_id)
    {
        if (post_id != "" && post_id != null)
        {
            post_secenekler_post_id_gecici.text = post_id;
            FirebaseDatabase.DefaultInstance
             .GetReference("/posts/" + post_id)
             .GetValueAsync().ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     bildirim_create("Akış yenilenemedi");
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     if(snapshot.Exists)
                     {
                         post_secenekler_paneli.SetActive(true);
                         if (snapshot.Child("uid").Value.ToString() == user_uid.text)
                         {
                             post_sikayet_et_tusu.SetActive(false);
                             post_sil_ana_tus.SetActive(true);
                         }
                         else
                         {
                             post_sikayet_et_tusu.SetActive(true);
                             post_sil_ana_tus.SetActive(false);
                         }
                     }
                     else
                     {
                         bildirim_create("Gönderi silinmiş olabilir");
                     }
                 }
             });
        }
    }
    public void username_tamam(InputField usernamee)
    {
        FirebaseDatabase.DefaultInstance.GetReference("/takenUsernames/").Child(usernamee.text).Child(user_uid.text).SetValueAsync(1)
                         .ContinueWith(task =>
                         {
                             if (task.IsFaulted)
                             {
                                 bildirim_create("Kullanıcı adı az önce alınmış");
                                 username_status_icons[0].SetActive(false);
                                 username_status_icons[1].SetActive(false);
                                 username_status_icons[2].SetActive(true);
                             }
                             else if (task.IsCompleted)
                             {
                                 username_status_icons[0].SetActive(false);
                                 username_status_icons[1].SetActive(false);
                                 username_status_icons[2].SetActive(false);
                                 username.text = "";
                                 username_check_panel.SetActive(false);

                                 username.text = kullanici_takma_ad_post_paylas_yazi.text = usernamee.text;
                             }
                         });
    }
    public void username_character_analiz(InputField username)
    {
        username.text = username.text.ToLower();
        username.text = username.text.Replace("\n", string.Empty);
        username.text = username.text.Replace(" ", string.Empty);
    }
    public void username_query(InputField username)
    {
        username_status_icons[0].SetActive(true);
        username_status_icons[1].SetActive(false);
        username_status_icons[2].SetActive(false);

        if (username.text.Length < 3)
        {
            bildirim_create("Minimum 3 karakterden oluşmalıdır.");
            username_status_icons[0].SetActive(false);
            username_status_icons[1].SetActive(false);
            username_status_icons[2].SetActive(true);
        }
        else
        {
            Debug.Log("1");
            FirebaseDatabase.DefaultInstance
             .GetReference("/takenUsernames/").Child(username.text.Replace(".", ","))
             .GetValueAsync().ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     username_status_icons[0].SetActive(false);
                     username_status_icons[1].SetActive(false);
                     username_status_icons[2].SetActive(true);
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     Debug.Log("2");
                     if (snapshot.Exists)
                     {
                         bildirim_create("Kullanıcı adı alınamıyor");
                         username_status_icons[0].SetActive(false);
                         username_status_icons[1].SetActive(false);
                         username_status_icons[2].SetActive(true);
                     }
                     else
                     {
                         username_status_icons[0].SetActive(false);
                         username_status_icons[1].SetActive(true);
                         username_status_icons[2].SetActive(false);
                     }
                     Debug.Log("3");
                 }
             });
        }
    }
    public void plaka_takma_ad_kontrol()
    {
        if (plaka_profili_baslik_plaka_no.text != "" && plaka_profili_baslik_ulke_kodu.text != "")
        {
            if (takma_ad_plaka_list.Contains(plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text))
            {
                string kayitli_plaka_takma_adi_gecici =(string)takma_ad_plaka_list[plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text];

                if (kayitli_plaka_takma_adi_gecici != null && kayitli_plaka_takma_adi_gecici != "" && kayitli_plaka_takma_adi_gecici != " ")
                {
                    takma_ad_plaka.text = (string)takma_ad_plaka_list[plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text];
                    gecici_takma_ad_plaka = takma_ad_plaka.text;
                }
            }
            else
            {
                takma_ad_plaka.text = "";
                gecici_takma_ad_plaka = takma_ad_plaka.text;
            }
        }
    }
    public void galeriden_profil_resmi_al()
    {
        gecici_profil_resmi_konumu = "";
        NativeGallery.Permission izin = NativeGallery.GetImageFromGallery((konum) =>
            {
                Debug.Log("Seçilen resmin konumu: " + konum);
                if (konum != null)
                {
                    Texture2D texture = NativeGallery.LoadImageAtPath(konum, 512);
                    if (texture == null)
                    {
                        bildirim_create("Resim seçilemedi");
                        return;
                    }
                    profilresmi_onizleme.GetComponent<RawImage>().texture = texture;
                    profilresmi_onizleme.GetComponent<RectTransform>().localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);
                    
                    profil_resmi_ayarla_paneli.SetActive(true);
                    gecici_profil_resmi_konumu = konum;
                }
            }, "Bir resim seçin", "image/png", 512);
        Debug.Log("İzin durumu: " + izin);
    }
    public void convert_to_byte(Texture profilresmi_buyuk)
    {
        RenderTexture tmp = RenderTexture.GetTemporary(profilresmi_buyuk.width, profilresmi_buyuk.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(profilresmi_buyuk, tmp);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = tmp;
        Texture2D myTexture2D = new Texture2D(profilresmi_buyuk.width, profilresmi_buyuk.height);
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(tmp);
        profil_resmi_upload_buyuk = myTexture2D.EncodeToPNG();

        yukle_profil_resmi(gecici_profil_resmi_konumu, user_uid.text);
    }
    public void profil_resmi_yukleme_func()
    {
        yukleniyor_paneli.SetActive(true);
        bulunamadi_paneli.SetActive(false);
        convert_to_byte(profilresmi_onizleme.GetComponent<RawImage>().texture as Texture2D);
    }
    public void takip_et_etme()
    {
        takip_et_butonu_yukleniyor_obje.SetActive(true);
        if (plaka_profili_baslik_plaka_no.text != "")
        {
                     if (takip_et_tusu.GetComponent<Image>().color == takip_et_buton_renk[1])
                     {
                         FirebaseDatabase.DefaultInstance.GetReference("/users/not_public/" + user_uid.text+ "/others_plaka_info/" + plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text).Child("followed").SetValueAsync(null)
                         .ContinueWith(task =>
                         {
                             if (task.IsFaulted)
                             {
                                 takip_et_butonu_yukleniyor_obje.SetActive(false);
                                 yukleniyor_paneli.SetActive(false);
                                 bildirim_create("Akış yenilenemedi");
                             }
                             else if (task.IsCompleted)
                             {
                                 plaka_profili_takipci_sayisi.text = (Convert.ToInt32(plaka_profili_takipci_sayisi.text) - 1).ToString();
                                 takip_et_yazisi.text = "Takip et";
                                 takip_et_tusu.GetComponent<Image>().color = takip_et_buton_renk[0];
                                 
                                 Firebase.Messaging.FirebaseMessaging.Unsubscribe(plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text); /////benim plakam kodlarını ekleyince burası da değişikliğe ugrayacak..
                                 if(my_follow_plakalar_list.IndexOf(plaka_profili_baslik_ulke_kodu.text +"/"+plaka_profili_baslik_plaka_no.text) != -1)
                                 {
                                         my_follow_plakalar_list.Remove(plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text);
                                 }
                                 takip_et_butonu_yukleniyor_obje.SetActive(false);
                             }
                         });
                     }
                     else
                     {
                         FirebaseDatabase.DefaultInstance.GetReference("/users/not_public/" + user_uid.text + "/others_plaka_info/" + plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text).Child("followed").SetValueAsync(1)
                         .ContinueWith(task =>
                         {
                             if (task.IsFaulted)
                             {
                                 takip_et_butonu_yukleniyor_obje.SetActive(false);
                                 yukleniyor_paneli.SetActive(false);
                                 bildirim_create("Akış yenilenemedi");
                             }
                             else if (task.IsCompleted)
                             {
                                 plaka_profili_takipci_sayisi.text = (Convert.ToInt32(plaka_profili_takipci_sayisi.text) + 1).ToString();
                                 takip_et_yazisi.text = "Takip ediyorsun";
                         takip_et_tusu.GetComponent<Image>().color = takip_et_buton_renk[1];
                         
                         Firebase.Messaging.FirebaseMessaging.Subscribe(plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text);

                                 if (my_follow_plakalar_list.IndexOf(plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text) == -1)
                                 {
                                     my_follow_plakalar_list.Add(plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text);
                                 }
                                 takip_et_butonu_yukleniyor_obje.SetActive(false);
                             }
                         });
                     }
        }
    }
    public void plaka_profili_baslık_sifirlama()
    {
        plaka_profili_baslik_plaka_no.text = "";
    }
    public void user_profili_uid_sifirlama()
    {
        if (other_user_uid != "")
        {
            other_user_uid = "";
        }
    }
    public void takma_ad_input_check_plaka()
    {
        if (takma_ad_plaka.text.Length > 0)
        {
            takma_ad_plaka.text = takma_ad_plaka.text.Replace("\n", string.Empty);
            int deneme = -1;
            for (int i = 0; i < takma_ad_plaka.text.Length; i++)
            {
                if (takma_ad_plaka.text.Substring(i, 1) != " ")
                {
                    deneme = i;
                    break;
                }
            }
            if (deneme != -1)
            {
                if (deneme > 0)
                {
                    takma_ad_plaka.text = takma_ad_plaka.text.Substring(deneme, takma_ad_plaka.text.Length - deneme);
                }
            }
            else
            {
                takma_ad_plaka.text = "";
            }
        }
    }
    public void takmaa_ad_ekle_plaka()
    {
        if (gecici_takma_ad_plaka != takma_ad_plaka.text)
        {
            DatabaseReference takma_ad_kullanici_path = FirebaseDatabase.DefaultInstance.GetReference("users/not_public/").Child(user_uid.text).Child("others_plaka_info")
                .Child(plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text).Child("takma_ad");

            if (takma_ad_plaka.text != "")
            {
                takma_ad_kullanici_path.SetValueAsync(takma_ad_plaka.text);
            }
            else
            {
                takma_ad_kullanici_path.SetValueAsync(null);
            }

            if (takma_ad_plaka_list.Contains(plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text))
            {
                if (takma_ad_plaka.text != "")
                {
                    takma_ad_plaka_list[plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text] = takma_ad_plaka.text;
                }
                else
                {
                    takma_ad_plaka_list.Remove(plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text);
                }  
            }
            else
            {
                if (takma_ad_plaka.text != "")
                {
                    takma_ad_plaka_list.Add(plaka_profili_baslik_ulke_kodu.text + plaka_profili_baslik_plaka_no.text, takma_ad_plaka.text);
                }
            }
            gecici_takma_ad_plaka = takma_ad_plaka.text;
        }        
    }
    public void instagram_kullanici_set()
    {
        DatabaseReference takma_ad_kullanici_path = FirebaseDatabase.DefaultInstance.GetReference("users/public/").Child(user_uid.text);

        if (instagram_ayarlar_input.text == "" || instagram_ayarlar_input.text == null)
        {            
            takma_ad_kullanici_path.Child("/instagram/").SetValueAsync(null);
        }
        else
        {
            takma_ad_kullanici_path.Child("/instagram/").SetValueAsync(instagram_ayarlar_input.text);
        }        
    }
    public void geridon_tuslar_status()
    {
        if (genel_tuslar_status[0].active == true)
        {
            kesfet_paneli.SetActive(true);
        }
        else if (genel_tuslar_status[1].active == true)
        {
            bildirimler_paneli.SetActive(true);
        }
    }
    public void instagram_button_profile()
    {
        if (instagram_profil_yazi!=""&& instagram_profil_yazi!=null&& instagram_profil_yazi!="null")
        {
            Application.OpenURL("https://www.instagram.com/" + instagram_profil_yazi);
        }
    }
    public void post_gonder()
    {
        if (string.IsNullOrEmpty(mesaj.text))
        {
            bildirim_create("Yorum kısmı bomboş :/");
        }
        else if(string.IsNullOrEmpty(plaka_no.text))
        {
            bildirim_create("Plaka eklemeyi unuttun :)");
        }
        else
        {
            DatabaseReference post_atma_plaka_profil_tum_yorumlar = FirebaseDatabase.DefaultInstance.GetReference("/posts/").Push();
            string push_key = post_atma_plaka_profil_tum_yorumlar.Key;

                    Dictionary<string, object> post_json = new Dictionary<string, object>();
                    Dictionary<string, object> timestamp = new Dictionary<string, object>();
                    timestamp[".sv"] = "timestamp";

                    post_json["/uid"] = user_uid.text;
                    post_json["/mesaj"] = mesaj.text;
                    post_json["/plaka"] = plaka_no.text;
                    post_json["/user_ulke"] = user_ulke.text;
                    post_json["/plaka_country_code"] = ulke_kodu_yazi[1].text;
                    post_json["/zaman/"] = timestamp;
                    post_json["/username/"] = username.text;

                    post_atma_plaka_profil_tum_yorumlar.UpdateChildrenAsync(post_json).ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            bildirim_create("Gönderi paylaşılamadı");
                        }
                        else if (task.IsCompleted)
                        {
                            sifirla();
                            post_paylas_paneli.SetActive(false);
                            bildirim_create("Gönderi paylaşıldı");
                        }
                    });
        }
    }
    void debug_kapat()
    {
        toast_mesaj_paneli.SetActive(false);
    }
    public void bildirim_create(string mesaj)
    {
        toast_mesaj_paneli.SetActive(true);
        bildirim_yazisi.text = mesaj;
        Invoke("debug_kapat", bildirim_suresi);
    }
    public void bulunamadi_paneli_notification(string mesaj)
    {
        bulunamadi_paneli.SetActive(true);
        bulunamadi_yazisi.text = mesaj;
    }
    public void sifirla()
    {
        mesaj.text = "";
        plaka_no.text = "";
    }
    public void zaman_hesaplama_(GameObject post, string TimeStampp, int status)
    {
        double TimeStamp = Convert.ToDouble(TimeStampp);
        oldtime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        oldtime = oldtime.AddMilliseconds(TimeStamp).ToLocalTime();
        TimeSpan travelTime = System.DateTime.Now - oldtime;
        int years = travelTime.Days / 365;
        int weeks = travelTime.Days / 7;

        
        if (status == 0)
        {
          time_things = zaman_normal_turkish;
        }
        else if(status == 1)
        {
          time_things = zaman_mini_turkish;
        }

        if (years > 0)
        {
            post.transform.Find("zaman").GetComponentInChildren<Text>().text = years + time_things[0];
        }
        else if (weeks > 0)
        {
            post.transform.Find("zaman").GetComponentInChildren<Text>().text = weeks + time_things[1];
        }
        else if (travelTime.Days > 0)
        {
            post.transform.Find("zaman").GetComponentInChildren<Text>().text = travelTime.Days + time_things[2];
        }
        else if (travelTime.Hours > 0)
        {
            post.transform.Find("zaman").GetComponentInChildren<Text>().text = travelTime.Hours + time_things[3];
        }
        else if (travelTime.Minutes > 0)
        {
            post.transform.Find("zaman").GetComponentInChildren<Text>().text = travelTime.Minutes + time_things[4];
        }
        else if (travelTime.Seconds > 0)
        {
            post.transform.Find("zaman").GetComponentInChildren<Text>().text = travelTime.Seconds + time_things[5];
        }
    }
    void auth_user_data()
    {
        FirebaseDatabase.DefaultInstance
         .GetReference("users/public/" + user_uid.text)
         .GetValueAsync().ContinueWith(task =>
         {
             if (task.IsFaulted)
             {
                 GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_yap_kayit_ol_paneli.SetActive(true);
                 GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_ekrani_paneli.SetActive(false);
                 bildirim_create("Akış yenilenemedi");
             }
             else if (task.IsCompleted)
             {
                 DataSnapshot snapshot = task.Result;
                 if (snapshot.Exists)
                 {
                     if (snapshot.Child("user_ulke").Exists)
                     {
                         user_ulke.text = snapshot.Child("user_ulke").Value.ToString();
                         Debug.Log("Kullanici yasadigi ulke alindi = " + user_ulke.text);
                     }
                     if (snapshot.Child("/instagram/").Exists)
                     {
                         instagram_ayarlar_input.text = snapshot.Child("/instagram/").Value.ToString();
                     }
                     else
                     {
                         instagram_ayarlar_input.text = "";
                     }
                     if (snapshot.Child("username").Exists)
                     {
                         username.text = snapshot.Child("username").Value.ToString();
                         username.text = kullanici_takma_ad_post_paylas_yazi.text = username.text.Replace(",", ".");
                         Debug.Log("Kullanici = " + username.text);
                     }
                     my_profile_hide_info();
                 }
             }
         });
    }   
    void my_profile_hide_info()
    {
        my_follow_plakalar_list.Clear();
        takma_ad_plaka_list.Clear();
        FirebaseDatabase.DefaultInstance
            .GetReference("users/not_public/" + user_uid.text)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_yap_kayit_ol_paneli.SetActive(true);
                    GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_ekrani_paneli.SetActive(false);
                    bildirim_create("Akış yenilenemedi");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        if (snapshot.Child("others_plaka_info").Exists)
                        {
                            foreach (var ulke in snapshot.Child("others_plaka_info").Children)
                            {
                                foreach (var plaka in ulke.Children)
                                {
                                    if (plaka.Child("followed").Exists)
                                    {
                                        my_follow_plakalar_list.Add(ulke.Key + "/" + plaka.Key);
                                    }
                                    if (plaka.Child("takma_ad").Exists)
                                    {
                                        takma_ad_plaka_list.Add(ulke.Key + plaka.Key, plaka.Child("takma_ad").Value.ToString());
                                    }
                                }
                            }
                        }
                    }
                    kesfet_shuffle_func();
                    user_auth_profile_images_get(user_uid.text , profil_Resmi_post_paylas);
                    //FirebaseDatabase.DefaultInstance.GoOnline();
                }
            });
    }
    public void my_profile_get_post_func()
    {
        bulunamadi_paneli.SetActive(false);
        yukleniyor_paneli.SetActive(true);
        posts_my_profile.Clear();
        FirebaseDatabase.DefaultInstance
     .GetReference("/posts/").OrderByChild("uid").EqualTo(user_uid.text).LimitToLast(30)
     .GetValueAsync().ContinueWith(task =>
     {
         if (task.IsFaulted)
         {
             yukleniyor_paneli.SetActive(false);
             bildirim_create("Akış yenilenemedi");
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             if (snapshot.Exists)
             {
                 foreach (var Snapshott in snapshot.Children)
                 {
                     posts_my_profile.Add(Snapshott);
                 }
                 for (int i = posts_my_profile.Count - 1; i > -1; i--)
                 {
                     genel_kisaltma(content_my_profile, posts_my_profile[i]);
                 }
                 yukleniyor_paneli.SetActive(false);
             }
             else
             {
                 yukleniyor_paneli.SetActive(false);
                 bulunamadi_paneli_notification("Burada görecek bir şey yok. Henüz...");
             }
         }
     });
    }
    public void kesfet_shuffle()
    {
       if((content_kesfet_profili.transform.childCount <2 && kesfet_paneli.active==false) || kesfet_paneli.active == true)
        {
            bulunamadi_paneli.SetActive(false);
            yukleniyor_paneli.SetActive(true);
            kesfet_paneli.SetActive(true);
            posts_kesfet.Clear();
            kesfet_shuffle_func();
        }
       else
        {
            bulunamadi_paneli.SetActive(false);
            kesfet_paneli.SetActive(true);
        }
    }
    void kesfet_shuffle_func()
    {
        FirebaseDatabase.DefaultInstance
     .GetReference("/posts/").OrderByChild("user_ulke").EqualTo(user_ulke.text).LimitToLast(30)
     .GetValueAsync().ContinueWith(task =>
     {
         if (task.IsFaulted)
         {
             yukleniyor_paneli.SetActive(false);
             bildirim_create("Akış yenilenemedi");
             GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_yap_kayit_ol_paneli.SetActive(false);
             GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_ekrani_paneli.SetActive(false);
         }
         else if (task.IsCompleted)
         {
             DataSnapshot snapshot = task.Result;
             if (snapshot.Exists)
             {
                 panel_content_veri_sifirla(content_kesfet_profili);
                 foreach (var Snapshott in snapshot.Children)
                 {
                     posts_kesfet.Add(Snapshott);
                 }
                 for (int i = posts_kesfet.Count - 1; i > -1; i--)
                 {
                     genel_kisaltma(content_kesfet_profili, posts_kesfet[i]);
                 }
                 yukleniyor_paneli.SetActive(false);
             }
             else
             {
                 panel_content_veri_sifirla(content_kesfet_profili);
                 yukleniyor_paneli.SetActive(false);
                 bulunamadi_paneli_notification("Burada görecek bir şey yok. Henüz...");
             }
             GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_yap_kayit_ol_paneli.SetActive(false);
             GameObject.Find("firebase-message").GetComponent<oturum_ac>().giris_ekrani_paneli.SetActive(false);
         }
     });
    }
    public void profilim_post_getir_func_anabuton()
    {
        profilim_post_getirr(user_uid.text);
    }
    public void profilim_post_getirr(string user_no)
    {       
        user_profili_uid_sifirlama();
        instagram_tusu.SetActive(false);
        yukleniyor_paneli.SetActive(true);
        profilim_paneli.SetActive(true);
        yuzsuz_resim_profil.SetActive(true);
        kesfet_paneli.SetActive(false);
        bildirimler_paneli.SetActive(false);
        username_profil.text = instagram_profil_yazi = "";
        if (user_uid.text == user_no)
        {
            secenekler_tusu_kisi_profili.SetActive(false);
        }
        else
        {
            secenekler_tusu_kisi_profili.SetActive(true);
        }
        other_user_uid = user_no;
        profil_post_read_data_kodu(user_no);
    }
    void profil_post_read_data_kodu(string user_no)
    {
        profil_image_buyuk_view();
        FirebaseDatabase.DefaultInstance
             .GetReference("users/public/").Child(user_no)
             .GetValueAsync().ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     yukleniyor_paneli.SetActive(false);
                     bildirim_create("Akış yenilenemedi");
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     if (snapshot.Exists)
                     {
                         username_profil.text = snapshot.Child("/username/").Value.ToString();
                         if (snapshot.Child("/instagram/").Exists)
                         {
                             instagram_profil_yazi = snapshot.Child("/instagram/").Value.ToString();
                             if (user_uid.text != user_no)
                             {
                                 instagram_tusu.SetActive(true);
                             }
                         }
                     }
                     yukleniyor_paneli.SetActive(false);
                 }
             });
    }
    public void ulke_kodu_select()
    {
        ulke_kodu_yazi[hangi_ulke_kodu_butonu].text = ulke_kodu_liste_post_paylas.options[ulke_kodu_liste_post_paylas.value].text;
    }
    public void ulke_plaka_select_show()
    {
        if (ulke_kodu_liste_post_paylas.options.Count == 0)
        {
            ulke_kodu_liste_post_paylas.AddOptions(ulke_plate_code_list);
            ulke_plate_code_list.Clear();
            for (int i = 0; i < ulke_kodu_liste_post_paylas.options.Count; i++)
            {
                if (ulke_kodu_liste_post_paylas.options[i].text == user_ulke.text)
                {
                    ulke_kodu_liste_post_paylas.value = i;
                }
            }
            ulke_kodu_liste_post_paylas.Show();
        }
        else
        {
            ulke_kodu_liste_post_paylas.Show();

        }
    }
    public void ulke_kodu_hangi_buton(int button)
    {
        hangi_ulke_kodu_butonu = button;
    }
    public void ulke_select_drop_on()
    {
        user_ulke.text = ulkeler_dropdown.options[ulkeler_dropdown.value].text;

        DatabaseReference takma_ad_kullanici_path = FirebaseDatabase.DefaultInstance.GetReference("users/public/").Child(user_uid.text);
        takma_ad_kullanici_path.Child("/user_ulke/").SetValueAsync(user_ulke.text);
    }
    public void ulke_select_liste()
    {
        if (ulkeler_dropdown.options.Count == 0)
        {
            ulkeler_dropdown.AddOptions(ulke_names);
            ulke_names.Clear();
            for (int i = 0; i < ulkeler_dropdown.options.Count; i++)
            {
                if (ulkeler_dropdown.options[i].text == user_ulke.text)
                {
                    ulkeler_dropdown.value = i;
                }
            }
            ulkeler_dropdown.Show();
        }
        else
        {
            ulkeler_dropdown.Show();
        }
    }
    public void panel_content_destroy(GameObject content)
    {
        if (content.transform.childCount != 0)
        {
            int i = 0;
            GameObject[] allChildren = new GameObject[content.transform.childCount];
            foreach (Transform child in content.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }
            foreach (GameObject child in allChildren)
            {
                if (child.transform.name != "plaka profil ust panel (1)")
                {
                    Destroy(child.gameObject, 0);
                }
            }
        }
    }
    public void content_bildirimler_sifirla(GameObject content)
    {
        if (content.transform.childCount != 0)
        {
            int i = 0;
            GameObject[] allChildren = new GameObject[content.transform.childCount];
            foreach (Transform child in content.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }
            foreach (GameObject child in allChildren)
            {
                child.transform.Find("profil mask").Find("yuzsuz_resim").gameObject.SetActive(true);

                child.transform.Find("icerik").GetComponentInChildren<TextMeshProUGUI>().text = 
                child.transform.Find("zaman").GetComponentInChildren<Text>().text = 
                child.transform.Find("title").GetComponentInChildren<Text>().text = "";

                child.transform.SetParent(havuz_parent_bildirimler.transform);
                post_havuzu_bildirimler.Push(child.gameObject);
            }
        }
    }
    public void panel_content_veri_sifirla(GameObject content)
    { if (content.transform.childCount != 0)
        {
            int i = 0;
            GameObject[] allChildren = new GameObject[content.transform.childCount];
            foreach (Transform child in content.transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }
            foreach (GameObject child in allChildren)
            {
                if (child.transform.name != "plaka profil ust panel (1)")
                {
                    child.transform.Find("ust bilgi paneli").GetChild(0).Find("profil mask")
                      .Find("yuzsuz_resim").gameObject.SetActive(true);
                    child.transform.Find("like").Find("on_icon").gameObject.SetActive(false);
                    child.transform.Find("like").GetComponentInChildren<Text>().text = "0";
                    child.transform.SetParent(havuz_parent.transform);
                    post_havuzu.Push(child.gameObject);
                }
            }
        }
    }
    public void content_plaka_sifirla()
    {
        panel_content_veri_sifirla(content_plaka_profili);
        Debug.Log("content_plaka_sifirla tamamlandı");
    }
    public void like_post(Transform gecici_like_tusu, string post_key)
    {
                 gecici_like_tusu.GetComponentInChildren<Button>().interactable = false;
                 if (gecici_like_tusu.Find("on_icon").gameObject.active != false)
                 {
                     FirebaseDatabase.DefaultInstance.GetReference("/posts/").Child(post_key).Child("likes").Child(user_uid.text).SetValueAsync(null).ContinueWith(task =>
                     {
                         if (task.IsFaulted)
                         {
                             gecici_like_tusu.GetComponentInChildren<Button>().interactable = true;
                             bildirim_create("Akış yenilenemedi");
                         }
                         else if (task.IsCompleted)
                         {
                             gecici_like_tusu.Find("on_icon").gameObject.SetActive(false);
                             gecici_like_tusu.GetComponentInChildren<Text>().text = (Convert.ToInt32(gecici_like_tusu.GetComponentInChildren<Text>().text) - 1).ToString();
                             gecici_like_tusu.GetComponentInChildren<Button>().interactable = true;
                         }
                     });
                 }
                 else
                 {
                     FirebaseDatabase.DefaultInstance.GetReference("/posts/").Child(post_key).Child("likes").Child(user_uid.text).SetValueAsync(1).ContinueWith(task =>
                     {
                         if (task.IsFaulted)
                         {
                             gecici_like_tusu.GetComponentInChildren<Button>().interactable = true;
                             bildirim_create("Akış yenilenemedi");
                         }
                         else if (task.IsCompleted)
                         {
                             gecici_like_tusu.GetComponentInChildren<Text>().text = (Convert.ToInt32(gecici_like_tusu.GetComponentInChildren<Text>().text) + 1).ToString();
                             gecici_like_tusu.Find("on_icon").gameObject.SetActive(true);
                             gecici_like_tusu.GetComponentInChildren<Button>().interactable = true;
                         }
                     });
            
                 }
    }
    public void bildirimler_listen_func_on()
    {
        if (content_bildirimler.transform.childCount < 1 && bildirimler_paneli.active == false)
        {
            bildirimler_listen_func();
        }
        else if (bildirimler_paneli.active == true)
        {
            bildirimler_listen_func();
        }
        else
        {
            bulunamadi_paneli.SetActive(false);
            bildirimler_paneli.SetActive(true);
        }
    }
    public void bildirimler_listen_func()
    {
        if (garaj_plakano.text != "" && garaj_plakano.text != null && ulke_kodu_yazi[2].text != "" && ulke_kodu_yazi[2].text != null)
        {
            posts_bildirimler.Clear();
            bildirimler_paneli.SetActive(true);
            yukleniyor_paneli.SetActive(true);
            bulunamadi_paneli.SetActive(false);
            FirebaseDatabase.DefaultInstance
            .GetReference("/posts/").OrderByChild("plaka").EqualTo(garaj_plakano.text).LimitToLast(20)
             .GetValueAsync().ContinueWith(task =>
             {
                 if (task.IsFaulted && task.IsCanceled)
                 {
                     yukleniyor_paneli.SetActive(false);
                     bildirim_create("Akış yenilenemedi");
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     content_bildirimler_sifirla(content_bildirimler);
                     if (snapshot.Exists)
                     {
                         foreach (var childSnapshot in snapshot.Children)
                         {
                                 if (childSnapshot.Child("plaka_country_code").Value.ToString() == ulke_kodu_yazi[2].text)
                                 {
                                     Debug.Log(childSnapshot.Key);
                                     posts_bildirimler.Add(childSnapshot);
                                 }
                         }
                             if(posts_bildirimler.Count > 0)
                             {
                              bildirim_post_listele();
                             }
                             else
                             {
                             bulunamadi_paneli_notification("Henüz gönderi eklenmemiş");
                             yukleniyor_paneli.SetActive(false);
                             }
                     }
                     else
                     {
                         bulunamadi_paneli_notification("Plakana Henüz bir şey yazılmamış");
                         yukleniyor_paneli.SetActive(false);
                     }
                 }
             });
        }
        else
        {
            bulunamadi_paneli_notification("Bildirimini alabileceğin bir plaka eklemelisin");
        }
    } 
    void bildirim_post_listele()
    {
        if (posts_bildirimler.Count > 1)
        {
            for (int i = posts_bildirimler.Count - 1; i > -1; i--)
            {
                GameObject post = post_havuzu_bildirimler.Pop();
                post.transform.SetParent(content_bildirimler.transform);
                post.transform.GetComponent<RectTransform>().localScale = new Vector3(1.03f, 1.03f, 1.03f);
                post.transform.name = posts_bildirimler[i].Key;
                zaman_hesaplama_(post, posts_bildirimler[i].Child("zaman").Value.ToString(), 0);
                post.transform.Find("title").GetComponentInChildren<Text>().text = posts_bildirimler[0].Child("username").Value.ToString() + " <color=#c0c0c0ff>aracına yazdı:</color>";
                profil_resmi_download_bildirimler(posts_bildirimler[i].Child("uid").Value.ToString(), post);
            }
        }
        else
        {
            GameObject post = post_havuzu_bildirimler.Pop();
            post.transform.SetParent(content_bildirimler.transform);
            post.transform.GetComponent<RectTransform>().localScale = new Vector3(1.03f, 1.03f, 1.03f);
            post.transform.name = posts_bildirimler[0].Key;
            zaman_hesaplama_(post, posts_bildirimler[0].Child("zaman").Value.ToString(), 0);
            post.transform.Find("title").GetComponentInChildren<Text>().text = posts_bildirimler[0].Child("username").Value.ToString() + " <color=#c0c0c0ff>aracına yazdı:</color>";
            profil_resmi_download_bildirimler(posts_bildirimler[0].Child("uid").Value.ToString(), post);
        }
        yukleniyor_paneli.SetActive(false);
    }
    
    public void post_view_func(GameObject bildirim_obje)
    {
        yukleniyor_paneli.SetActive(true);
        bildirimler_paneli.SetActive(false);
        kesfet_paneli.SetActive(false);
        FirebaseDatabase.DefaultInstance.GetReference("/posts/").Child(bildirim_obje.transform.name)
            .GetValueAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    yukleniyor_paneli.SetActive(false);
                    bildirim_create("Akış yenilenemedi");
                }
                else if (task.IsCompleted)
                {
                    
                    DataSnapshot Snapshott = task.Result;
                    if (Snapshott.Exists)
                    {
                        post_paneli.SetActive(true);
                        post_info_get(post_direct, Snapshott);
                    }
                    else
                    {
                        bildirim_create("Gönderi silinmiş olabilir");
                    }
                   
                }
            });
    }
    
    public void arama_motoru()
    {
        if(plaka_no_search.text != "")
        {
            if(plaka_no_search.text.Length >= 3)
            {
                plaka_araa(ulke_kodu_yazi[0].text);
            }
            else
            {
                bildirim_create("Daha uzun bir plaka yazmalısın");
            }
        }
    }
    public void plaka_araa(string ulke_kodu)
    {
            if (profilim_paneli.active == true)
            {
                profilim_paneli.SetActive(false);
            }
            if (plaka_profili_paneli.active == false)
            {
                plaka_profili_paneli.SetActive(true);
            }
            if (yukleniyor_paneli.active == false)
            {
                yukleniyor_paneli.SetActive(true);
            }
            if (post_paneli.active == true)
            {
                post_paneli.SetActive(false);
            }
            if (hic_yorum_Yok_paneli.active == true)
            {
                hic_yorum_Yok_paneli.SetActive(false);
            }
            bildirimler_paneli.SetActive(false);
            kesfet_paneli.SetActive(false);

        plaka_no.text = plaka_profili_baslik_plaka_no.text = plaka_no_search.text;
        plaka_profili_yorum_sayisi.text = plaka_profili_takipci_sayisi.text = "0";
        posts_plaka_paneli.Clear();
        takma_ad_plaka.text = plaka_no_search.text = "";
        ulke_kodu_yazi[1].text = plaka_profili_baslik_ulke_kodu.text = ulke_kodu;

        mesaj.text = "";

        takip_et_yazisi.text = "Takip et";
        takip_et_tusu.GetComponent<Image>().color = takip_et_buton_renk[0];
        takip_et_butonu_yukleniyor_obje.SetActive(false);

        if (my_follow_plakalar_list.Count > 0)
        {
            if (my_follow_plakalar_list.IndexOf(plaka_profili_baslik_ulke_kodu.text + "/" + plaka_profili_baslik_plaka_no.text) != -1)
            {
                takip_et_yazisi.text = "Takip ediyorsun";
                takip_et_tusu.GetComponent<Image>().color = takip_et_buton_renk[1];
                takip_et_butonu_yukleniyor_obje.SetActive(false);
            }
        }

        user_profili_uid_sifirlama();
        plaka_takma_ad_kontrol();
        Debug.Log("Plaka profili yükleniyor...");
            FirebaseDatabase.DefaultInstance
             .GetReference("/posts/").OrderByChild("plaka").EqualTo(plaka_profili_baslik_plaka_no.text).LimitToLast(15)
             .GetValueAsync().ContinueWith(task =>
             {
                 if (task.IsFaulted)
                 {
                     yukleniyor_paneli.SetActive(false);
                     bildirim_create("Akış yenilenemedi");
                 }
                 else if (task.IsCompleted)
                 {
                     DataSnapshot snapshot = task.Result;
                     plaka_info_check();
                     if (snapshot.Exists)
                     {
                         foreach (var snap in snapshot.Children)
                         {
                             if (snap.Child("plaka_country_code").Value.ToString() == ulke_kodu)
                             {
                                 posts_plaka_paneli.Add(snap);
                             }
                         }
                         Debug.Log("Toplam post sayisi = " + snapshot.ChildrenCount);
                         if (posts_plaka_paneli.Count >1)
                         {
                             for (int i = posts_plaka_paneli.Count - 1; i > -1; i--)
                             {
                                 genel_kisaltma(content_plaka_profili, posts_plaka_paneli[i]);
                             }
                         }
                         else if (posts_plaka_paneli.Count > 0)
                         {
                             genel_kisaltma(content_plaka_profili, posts_plaka_paneli[0]);
                         }
                         else
                         {
                             hic_yorum_Yok_paneli.SetActive(true);
                         }
                     }
                     else
                     {
                         Debug.Log("Hiç post eklenmemiş");
                         hic_yorum_Yok_paneli.SetActive(true);
                     }
                         yukleniyor_paneli.SetActive(false);
                 }
             });
    }
    void genel_kisaltma(GameObject content, DataSnapshot post_snap)
    {
        GameObject post = post_havuzu.Pop();
        post.transform.SetParent(content.transform);
        post.transform.GetComponent<RectTransform>().localScale = new Vector3(2.3094f, 2.3094f, 2.3094f);
        post_info_get(post, post_snap);
    }
    void post_info_get(GameObject post, DataSnapshot post_snap)
    {
        post.transform.name = post_snap.Key;
        post.transform.Find("post yazisi").GetComponentInChildren<Text>().text = post_snap.Child("mesaj").Value.ToString();
        post.transform.Find("ust bilgi paneli").Find("hangi plakaya yazisi").Find("ulkekodu").Find("Text").GetComponentInChildren<Text>().text = post_snap.Child("plaka_country_code").Value.ToString();
        post.transform.Find("ust bilgi paneli").Find("hangi plakaya yazisi").GetComponentInChildren<Text>().text = post_snap.Child("plaka").Value.ToString();
        zaman_hesaplama_(post, post_snap.Child("zaman").Value.ToString(), 0);
        post.transform.Find("ust bilgi paneli").GetChild(0).name = post_snap.Child("uid").Value.ToString();
        //////////////////////////////////////
        post.transform.Find("like").Find("on_icon").gameObject.SetActive(false);
        if (post_snap.Child("likes").Exists)
        {
            post.transform.Find("like").GetComponentInChildren<Text>().text = post_snap.Child("likes").ChildrenCount.ToString();
            foreach (var like_yapan_user_no in post_snap.Child("likes").Children)
            {
                if (like_yapan_user_no.Key == user_uid.text)
                {
                    post.transform.Find("like").Find("on_icon").gameObject.SetActive(true);
                }
            }
        }
        ///////////////////////////////////////////
        post.transform.Find("ust bilgi paneli").GetChild(0).GetComponentInChildren<Text>().text = post_snap.Child("username").Value.ToString();

        yukleniyor_paneli.SetActive(false);
        profil_resmi_download(post.transform.Find("ust bilgi paneli").GetChild(0).name, post);
    }
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////// BURADAN İTİBAREN TAMAMI STORAGE KODLARIDIR..
    protected StorageReference GetStorageReference() // storage kodu
    {
        if (storageLocation.StartsWith("gs://") ||
            storageLocation.StartsWith("http://") ||
            storageLocation.StartsWith("https://"))
        {
            var storageUri = new Uri(storageLocation);
            var firebaseStorage = FirebaseStorage.GetInstance(
              String.Format("{0}://{1}", storageUri.Scheme, storageUri.Host));
            return firebaseStorage.GetReferenceFromUrl(storageLocation);
        }
        return FirebaseStorage.DefaultInstance.GetReference(storageLocation);
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void profil_resmi_degistir()
    {
        GetStorageReference().Child("/users_public_dosya/").Child(user_uid.text).Child("/Profil_image_klasor/").Child("resized-profil_image.png")
            .GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    profil_image_change_panel_resim_varsa.SetActive(true);
                }
                else
                {
                    profil_image_change_panel_resim_varsa.SetActive(false);
                    galeriden_profil_resmi_al();
                }
            });
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void profil_resmini_sil()
    {
        GetStorageReference().Child("/users_public_dosya/").Child(user_uid.text).Child("/Profil_image_klasor/").Child("orjinal_profil_image.png")
            .DeleteAsync().ContinueWith(task => {
                if (task.IsCompleted)
                {
                    profil_resmi_ayarlar_menu.GetComponentInChildren<RawImage>().texture =
                    profil_Resmi_post_paylas.GetComponentInChildren<RawImage>().texture = null;
                    yuzsuz_resim_post_paylas.SetActive(true);

                    profil_resmi_yukle_icon.SetActive(true);
                    bildirim_create("Başarıyla silindi");
                }
                else
                {
                    profil_resmi_yukle_icon.SetActive(false);
                    bildirim_create("İşlem Başarısız");
                }
            });
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void yukle_profil_resmi(string konum, string user_id)
    {
        yukleniyor_paneli.SetActive(true);
        bulunamadi_paneli.SetActive(false);
        if (profil_resmi_upload_buyuk != null && profil_resmi_upload_buyuk.Length > 0)
        {
            GetStorageReference().Child("/users_public_dosya/").Child(user_id).Child("/Profil_image_klasor/").Child("orjinal_profil_image.png")
            .PutBytesAsync(profil_resmi_upload_buyuk)
      .ContinueWith((Task<StorageMetadata> task) =>
      {
          if (task.IsFaulted || task.IsCanceled)
          {
              profil_resmi_yukle_icon.SetActive(true);
              yukleniyor_paneli.SetActive(false);
              bildirim_create("Resim Yüklenemedi");
          }
          else
          {
              Texture2D texture = NativeGallery.LoadImageAtPath(konum, 128);
              if (texture == null)
              {
                  return;
              }
              profil_resmi_ayarlar_menu.GetComponentInChildren<RawImage>().texture =
                  profil_Resmi_post_paylas.GetComponentInChildren<RawImage>().texture =
              new Texture2D(1, 1, TextureFormat.RGB24, false);
              profil_Resmi_post_paylas
              .GetComponent<RawImage>().texture = texture;
              profil_Resmi_post_paylas
              .GetComponent<RectTransform>().localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

              profil_resmi_ayarlar_menu.GetComponentInChildren<RawImage>().texture =
               profil_Resmi_post_paylas.GetComponentInChildren<RawImage>().texture;

               profil_resmi_ayarlar_menu.GetComponent<RectTransform>().localScale =
               profil_Resmi_post_paylas.GetComponent<RectTransform>().localScale;

              profil_resmi_yukle_icon.SetActive(false);
              yukleniyor_paneli.SetActive(false);
              bildirim_create("Resim başarıyla değiştirildi");
          }
      });
        }
        else
        {
            yukleniyor_paneli.SetActive(false);
            bildirim_create("Resim Yüklenemedi");
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void profil_resmi_download_bildirimler(string user_id, GameObject bildirim_button)
    {
        if (bildirim_button != null && bildirim_button.transform.parent.name != "post_havuz_bildirimler")
        {
            GetStorageReference().Child("/users_public_dosya/").Child(user_id).Child("/Profil_image_klasor/").Child("resized-profil_image.png").GetDownloadUrlAsync().ContinueWith((Task<Uri> task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    string url = String.Format("{0}", task.Result);
                    if (bildirim_button != null && bildirim_button.transform.parent.name != "post_havuz_bildirimler")
                    {
                        StartCoroutine(AccessURLbildirimler(url, bildirim_button));
                    }
                }
            });
        }
    }
    IEnumerator AccessURLbildirimler(string url, GameObject bildirim_button)
    {
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (bildirim_button != null && bildirim_button.transform.parent.name != "post_havuz_bildirimler")
            {
                bildirim_button.transform.Find("profil mask")
               .Find("profil image").GetComponentInChildren<RawImage>().texture = new Texture2D(1, 1, TextureFormat.RGB24, false);

                www.LoadImageIntoTexture((Texture2D)bildirim_button.transform.Find("profil mask")
                .Find("profil image").GetComponentInChildren<RawImage>().texture);

                bildirim_button.transform.Find("profil mask")
               .Find("profil image").GetComponent<RectTransform>().localScale = new Vector3(1f, www.texture.height / (float)www.texture.width, 1f);
                bildirim_button.transform.Find("profil mask")
               .Find("yuzsuz_resim").gameObject.SetActive(false);
                Debug.Log("resim yuklendi");
            }
            www.Dispose();
            www = null;
            Resources.UnloadUnusedAssets();
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void profil_resmi_download(string user_id, GameObject post)  // www ile
    {
        if (post.transform.parent.name != "post_havuz")
        {
            GetStorageReference().Child("/users_public_dosya/").Child(user_id).Child("/Profil_image_klasor/").Child("resized-profil_image.png").GetDownloadUrlAsync().ContinueWith((Task<Uri> task) =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    string url = String.Format("{0}", task.Result);
                    //Debug.Log(String.Format("DownloadUrl = {0}", task.Result));
                    if (post.transform.parent.name != "post_havuz")
                    {
                        StartCoroutine(AccessURL(url, post));
                    }
                }
            });
        }
    }
    IEnumerator AccessURL(string url, GameObject post)
    {
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (post != null && post.transform.parent.name != "post_havuz")
            {
                post.transform.Find("ust bilgi paneli").GetChild(0).Find("profil mask")
               .Find("profil image").GetComponentInChildren<RawImage>().texture = new Texture2D(1, 1, TextureFormat.RGB24, false);

                www.LoadImageIntoTexture((Texture2D)post.transform.Find("ust bilgi paneli").GetChild(0).Find("profil mask")
                .Find("profil image").GetComponentInChildren<RawImage>().texture);

                post.transform.Find("ust bilgi paneli").GetChild(0).Find("profil mask")
               .Find("profil image").GetComponent<RectTransform>().localScale = new Vector3(1f, www.texture.height / (float)www.texture.width, 1f);
                post.transform.Find("ust bilgi paneli").GetChild(0).Find("profil mask")
               .Find("yuzsuz_resim").gameObject.SetActive(false);
               // Debug.Log("resim yuklendi");
            }
            www.Dispose();
            www = null;
            Resources.UnloadUnusedAssets();
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void profil_image_buyuk_view()
    {
        profil_buyuk_image_paneli.GetComponentInChildren<RawImage>().texture = new Texture2D(1, 1, TextureFormat.RGB24, false);
        GetStorageReference().Child("/users_public_dosya/").Child(other_user_uid).Child("/Profil_image_klasor/").Child("orjinal_profil_image.png").GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                string url = String.Format("{0}", task.Result);
               // Debug.Log(String.Format("DownloadUrl = {0}", task.Result));

                StartCoroutine(profil_image_buyuk_view_numerator(url));
            }
            else
            {
                yukleniyor_paneli.SetActive(false);
                bildirim_create("İşlem Başarısız");
            }
        });
    }
    IEnumerator profil_image_buyuk_view_numerator(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (profil_buyuk_image_paneli != null)
            {
                www.LoadImageIntoTexture((Texture2D)profil_buyuk_image_paneli.GetComponentInChildren<RawImage>().texture);

                profil_buyuk_image_paneli.GetComponent<RectTransform>().localScale =
                    new Vector3(1f, profil_buyuk_image_paneli.GetComponentInChildren<RawImage>().texture.height /
                    (float)profil_buyuk_image_paneli.GetComponentInChildren<RawImage>().texture.width, 1f);
                yuzsuz_resim_profil.SetActive(false);
            }
            else
            {
                bildirim_create("İşlem Başarısız");
            }

            www.Dispose();
            www = null;
            Resources.UnloadUnusedAssets();
        }
        else
        {
            bildirim_create("İşlem Başarısız");
        }
        yukleniyor_paneli.SetActive(false);
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void user_auth_profile_images_get(string user_id, GameObject profil_Resmi_post_paylas)
    {
        GetStorageReference().Child("/users_public_dosya/").Child(user_id).Child("/Profil_image_klasor/").Child("resized-profil_image.png").GetDownloadUrlAsync().ContinueWith((Task<Uri> task) => {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                string url = String.Format("{0}", task.Result);
                StartCoroutine(user_auth_first_profil_image_download(url, profil_Resmi_post_paylas));
            }
            else
            {
                yuzsuz_resim_post_paylas.SetActive(true);
                profil_resmi_yukle_icon.SetActive(true);
                Debug.Log("resim linki bulunamadı");
            }
        });
    }
    IEnumerator user_auth_first_profil_image_download(string url, GameObject profil_Resmi_post_paylas)
    {
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            profil_Resmi_post_paylas.GetComponentInChildren<RawImage>().texture =
            profil_resmi_ayarlar_menu.GetComponentInChildren<RawImage>().texture =
            new Texture2D(1, 1, TextureFormat.RGB24, false);

            www.LoadImageIntoTexture((Texture2D)profil_Resmi_post_paylas.GetComponentInChildren<RawImage>().texture);

            profil_Resmi_post_paylas.GetComponent<RectTransform>().localScale
                = new Vector3(1f, www.texture.height / (float)www.texture.width, 1f);

            profil_resmi_ayarlar_menu.GetComponentInChildren<RawImage>().texture =
            profil_Resmi_post_paylas.GetComponentInChildren<RawImage>().texture;
            profil_resmi_ayarlar_menu.GetComponent<RectTransform>().localScale =
            profil_Resmi_post_paylas.GetComponent<RectTransform>().localScale;
            //Debug.Log("auth yapan kullanicinin profil resimleri yuklendi");
            yuzsuz_resim_post_paylas.SetActive(false);
            profil_resmi_yukle_icon.SetActive(false);
            www.Dispose();
            www = null;
            Resources.UnloadUnusedAssets();
        }
        else
        {
            yuzsuz_resim_post_paylas.SetActive(true);
            profil_resmi_yukle_icon.SetActive(true);
            Debug.Log("link bulundu ama indirilemedi");
        }
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}

