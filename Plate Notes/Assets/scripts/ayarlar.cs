using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
public class ayarlar : MonoBehaviour
{
    public Button gonder_butonu;
    public TMP_InputField post_yazisi;
    public InputField arama_motoru;    
    public InputField plaka_no1_ayarlar;
     void Start()
     {
        //Screen.fullScreen = false;
        ApplicationChrome.navigationBarColor = ApplicationChrome.statusBarColor = 0xff000000;
        ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
    }
    //public void dimmed()
    //{
    //    ApplicationChrome.dimmed = !ApplicationChrome.dimmed;
    //}
    //public void color()
    //{
    //    ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xffff3300;
    //}
    //public void color_normal()
    //{
    //    ApplicationChrome.statusBarColor = ApplicationChrome.navigationBarColor = 0xff000000;
    //}
    //public void visible()
    //{
    //    ApplicationChrome.statusBarState =  ApplicationChrome.States.Visible;
    //}
    //public void visibleover()
    //{
    //    ApplicationChrome.statusBarState = ApplicationChrome.States.VisibleOverContent;
    //}
    //public void translucentover()
    //{
    //    ApplicationChrome.statusBarState = ApplicationChrome.States.TranslucentOverContent;
    //}
    //public void hidden()
    //{
    //    ApplicationChrome.statusBarState = ApplicationChrome.States.Hidden;
    //}
    /// /////////////////////////////////////////////
    /// </summary>
    public void harfleri_buyut_plaka_1_ayarlar()
    {
        plaka_no1_ayarlar.text = plaka_no1_ayarlar.text.ToUpper();

        plaka_no1_ayarlar.text = plaka_no1_ayarlar.text.Replace(" ", string.Empty);        
    }
    public void plaka_check_ayarlar_final()
    {
        if (plaka_no1_ayarlar.text.Length < 3)
        {
            GameObject.Find("firebase-message").GetComponent<databasee>().bildirim_create("Daha uzun bir plaka yazmalısın");
        }
    }
    /// //////////////////////////////////////////////////////////////
    public void harfleri_buyut_arama_motoru()
    {
        arama_motoru.text = arama_motoru.text.ToUpper();
        arama_motoru.text = arama_motoru.text.Replace("\n", string.Empty);
        arama_motoru.text = arama_motoru.text.Replace(" ", string.Empty);
    }
    public void instagram_input_edit()
    {         
        GameObject.Find("firebase-message").GetComponent<databasee>().instagram_ayarlar_input.text = 
            GameObject.Find("firebase-message").GetComponent<databasee>().instagram_ayarlar_input.text.Replace("@", string.Empty);
        GameObject.Find("firebase-message").GetComponent<databasee>().instagram_ayarlar_input.text =
           GameObject.Find("firebase-message").GetComponent<databasee>().instagram_ayarlar_input.text.Replace("\n", string.Empty);
        GameObject.Find("firebase-message").GetComponent<databasee>().instagram_ayarlar_input.text =
           GameObject.Find("firebase-message").GetComponent<databasee>().instagram_ayarlar_input.text.Replace(" ", string.Empty);
    }
    public void post_icerik_check_post_paylas()
    {
        if (post_yazisi.text.Length > 0)
        {
            post_yazisi.text = post_yazisi.text.Replace("\n", string.Empty);
            int deneme = -1;
            for (int i = 0; i < post_yazisi.text.Length; i++)
            {
                if (post_yazisi.text.Substring(i, 1) != " ")
                {
                    deneme = i;
                    break;
                }                
            }
            if(deneme != -1)
            {
                if (deneme > 0)
                {
                    post_yazisi.text = post_yazisi.text.Substring(deneme, post_yazisi.text.Length - deneme);
                    gonder_butonu.interactable = true;
                }
                else
                {
                    gonder_butonu.interactable = true;
                }
            }
            else
            {
                post_yazisi.text = "";
            }           
        }
        else
        {
            gonder_butonu.interactable = false;
        }     
    }    
}