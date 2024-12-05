using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageSwitch : MonoBehaviour
{
    public BeaconManager _beaconManager;

    [Header("\nSide Panel Button Texts")]
    public TMP_Text scannerBtn;
    public TMP_Text favoritesBtn;
    public TMP_Text infoBtn;
    public TMP_Text aboutUsBtn;
    public TMP_Text qrScannerBtn;
    public TMP_Text quitBtn;

    [Header("\nMain Menu Tabs Texts")]
    public TMP_Text geoparkSitesBtn;
    public TMP_Text mapBtn;

    [Header("\nDetails Panel Tabs Texts")]
    public TMP_Text detailsInfoBtn;
    public TMP_Text galleryBtn;
    public TMP_Text videosBtn;

    public TMP_Text servicesLabel;

    [Header("\nVariables to change button and assign the bool value")]
    public Image languageButton;
    public Sprite greekFlagSprite;
    public Sprite englishFlagSprite;


    // Start is called before the first frame update
    void Start()
    {
        ChangeTextsLanguage(false);
    }


    public void ChangeTextsLanguage(bool  isEnglish)
    {
        scannerBtn.text = (isEnglish ? "SCANNER" : "�������");
        favoritesBtn.text = (isEnglish ? "FAVORITES" : "���������");
        infoBtn.text = (isEnglish ? "INFO" : "�����������");
        aboutUsBtn.text = (isEnglish ? "ABOUT US" : "������� �� ����");
        qrScannerBtn.text = (isEnglish ? "QR SCANNER" : "QR �������");
        quitBtn.text = (isEnglish ? "QUIT" : "������");

        geoparkSitesBtn.text = (isEnglish ? "Geopark Sites" : "������ ���������");
        mapBtn.text = (isEnglish ? "Map" : "������");

        detailsInfoBtn.text = (isEnglish ? "Info" : "�����������");
        galleryBtn.text = (isEnglish ? "Gallery" : "�������");
        videosBtn.text = (isEnglish ? "Videos" : "������");

        servicesLabel.text = (isEnglish ? "Services" : "���������");
    }




    public void SwitchLanguage()
    {
        _beaconManager.isEnglish = !_beaconManager.isEnglish;

        if (_beaconManager.isEnglish)
        {
            languageButton.sprite = greekFlagSprite;            
        }            
        else
        {
            languageButton.sprite = englishFlagSprite;
        }

        ChangeTextsLanguage(_beaconManager.isEnglish);

    }
}
