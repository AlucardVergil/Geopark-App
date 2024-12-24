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

    public TMP_Text geoparkInfoTitle;
    public TMP_Text geoparkInfo;

    [Header("\nServices Info Texts")]
    public TMP_Text servicesInfoTitle;
    public TMP_Text servicesInfo;

    public TMP_Text accomodationService;
    public TMP_Text foodService;
    public TMP_Text parkingService;
    public TMP_Text walkingService;

    [Header("Starting Panels Texts")]
    public TMP_Text startingPanelTitle;
    public TMP_Text startingPanelText;

    [Header("Bluetooth & GPS Disabled Warning Panel")]
    public TMP_Text warningText;
    public TMP_Text openSettingsButtonText;

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

        geoparkInfoTitle.text = (isEnglish ? "About Geopark Grevena - Kozani" : "������� �� �� �������� �������� - �������");
        geoparkInfo.text = (isEnglish ? "Welcome to Grevena�Kozani UNESCO Global Geopark, within the heart of West Macedonia. Here, you can retrace the steps of early researchers contributing towards the development of plate tectonic theory. You will see landforms created by the intersecting actions of plate tectonics, climate change and life.  The geopark includes the oldest dated rocks of Greece, dating to nearly a billion years in age. The mountains contain the lost Pangaean world of ~250 million years ago that split apart to give birth to Africa, Europe, and the ancient Tethyan Ocean. This long geologic evolution produced an environment where mammoths and ancient elephants flourished. The landscape is polished to a rare beauty by glaciation that created a highland of rugged mountains, cataclysmic canyons and rock formations of strange shapes and mysterious figures. This unique geologic and environmental setting is home to traditional villages where residents maintain time-honored folkways of the Greek countryside.\r\n\r\nOur figurehead is Tethys herself, the Titan of Greek mythology who was mother to all the waters of the Earth. She is the namesake of the ancient ocean which gave birth to our unique geological terrain. Tethys was said to have given birth to the Aliakmon River, as told by the poet Hesiod (~700 BC).\r\n\r\nAs you explore Geopark Grevena-Kozani, you will travel through time to the land of Tethys, the birth of West Macedonia and the heritage of a billion years of Earth history.\r\n\r\nGeopark activities have been held in the area for over forty years: these include international earth science educational projects, regional and international geoscience conferences, and the promoting of geotouristic goals. Geopark Grevena - Kozani has been in operation since 2014, and was inducted to the UNESCO Global Geoparks Network in 2021. It is managed by the Regional Development Agency of West Macedonia, and scientific support is provided by Geowonders Greece SCE. The geopark includes parts of five townships of the Regional Government of West Macedonia (Grevena, Deskati, Voio, Kozani, and Servia).\r\n\r\nThe Geopark hosts more than 119 geosites of unique geologic interest, and 18 geoheritage areas where tourists can enjoy hours or days in appreciation of the geo-environment and cultural legacy of the parkland.\r\n\r\nFor more information, visit our Geo-Information Centers in Grevena, Siatista, Deskati .\r\n\r\nEmail us at: <color=\"orange\">geoparkgrevenakozani@anko.gr & geowonders@gmail.com</color>, \r\n\r\nContact us by phone at <color=\"orange\">(+30)6949439053 or (+30)2461024022\r\n</color>\r\n\r\nVisit our website at <color=\"orange\">www.geoparkgrevenakozani.com</color> or \r\n\r\n<color=\"orange\">Facebook at Geopark Grevena-Kozani.</color>\r\n" : "����� ������ ��������� �������� �������� � ������� ������������� ��� ��� UNESCO, ��� ������ ��� ������� ����������. ���, �� ��� ������� ��� ��������� �������� �� ������������ ���� ����, ��� ������ ��� ���������� ������, �� ��������� �� �������� ����� ��� �������������� ��� ��� ������ ��� ������ ����� ��� �� ���������� �� ������ �������������� ����. ����������� �� ���������� ��������� ��� ������� ��� �������������� ������ ��� �������������� ������. \r\n\r\n��� ����� ��� ������������ � ������������ ����� ��� �������� ��� ����, ���� ��� 250 ����������� ������, ��������� ��� �� ������������ ��� ������, ��� ������ ��� ��� ������ ������ ��� ������. ���� � ����� ��������� ������� �������� ��� ����������, ���� ������������ ������ ��� ������� ���������. �� ����� ����� ���������� ��� ��� ���������, ������������� ��� ������ ��� ��������� �������. ��� �������� �� ������ �����, ������������ �������� ��� ��������� ���������� ������������.\r\n\r\n�� �������� �������� � ������� ��������� ������������� ���������, ���� �� �������� ��������� ����� ������ ��� ����������� ���� ��������� ���� ��� �� ��� �� ����� ��� ��������� ��������, ����������� �� ��� �������� �������� ��� �� ������ ���������� ��� ��������. \r\n\r\n�� �������� ��� ����� � ���� � T����, ��� ��� ��� ��������� ��� ��������� ����������, ������ ���� ��� ������ ��� ���. �� ����� ��� ������ ���� ������ ������ ��� ������� �� �������� ��� ��������� ��������, ����� ��� ��� ������ ���������, ���� �������� � ����� � ������� (~ 700 �.�.). \r\n\r\n��������� �� ��������� �� �� ��� ������ ���� ��� �� �������� ��������-�������, ��� ��������� ���� ������ ��� ������� ���������� ��� �� ������ ��������� ��� ����������� ���� ��������������� ���� ���������� ��������.\r\n\r\n��� ����������� ��� ������� ������, ����������� ���� ������� �������� �������������� ��� ��������� �� ����� �� �������� ��� �������� ��� ������������, ��������������� ������ ��������� ������������ ����������� ��� ������������ & ������ �������� ������������. �� �������� �������� � ������� ������������� ��� �� 2014. ����������� ������ ����� � ANKO ������� ���������� �.�. (������������ ���������� ������� �������������), ������������� ������������ ��� ��� ������� ����������� ����� ��� �������� ����� ��� ���������� ������� ��������� ��� UNESCO. ���� ������ ��� ��������������� ������� ����� ����� ��� ����������� ������� ���������� (��������, ��������, �����, ������� ��� �������), ����� ������������ ������������ ��� 119 �������� ��������� ���������� ������������� ��� 18 �������� ��������������. �� ���������� ������� �� ��������� ��� �� ���������� �� ����������������� ��� ����������� ���������� ��� ��������� �������� � �������, ���� ��� ������������ ��� ���������� �����������.  \r\n\r\n��� ������������ �����������, ������������ �� ������ ������������ ��������, ��������� ��� ��������. \r\n\r\n������, �������� �� ������������� ���� ��� �� <color=\"orange\">email: geoparkgrevenakozani@anko.gr & geowonders@gmail.com</color>, � ����������� ����� �������� <color=\"orange\">6949439053 � 2461024022\r\n</color>\r\n\r\n � �� ������������ ��� ���������� ��� <color=\"orange\">www.geoparkgrevenakozani.com</color>, \r\n\r\n���� ��� �� <color=\"orange\">Facebook ��� Geopark Grevena � Kozani.\r\n</color>\r\n");

        servicesInfoTitle.text = (isEnglish ? "Geopark & Services Info" : "����������� ��������� ���������");

        accomodationService.text = (isEnglish ? "Accomodation" : "�������");
        foodService.text = (isEnglish ? "Food" : "������");
        parkingService.text = (isEnglish ? "Parking" : "����� ����������");
        walkingService.text = (isEnglish ? "Walking" : "���������");

        startingPanelTitle.text = (isEnglish ? "<b>Geopark</b>\nGrevena - Kozani" : "<b>��������</b>\n�������� - �������");
        startingPanelText.text = (isEnglish ? "Please enable <b>GPS</b> and <b>Bluetooth</b> in order for the beacon scanner to work." : "�������� ������� �� <b>GPS</b> ��� �� <b>Bluetooth</b> ��� �� ���������� � ������� ������.");

#if !UNITY_EDITOR
        warningText.text = (isEnglish ? "Please enable <b>Bluetooth</b> & <b>GPS</b> in order for the beacon scanner to work." : "�������� ������� �� <b>GPS</b> ��� �� <b>Bluetooth</b>, ��� �� ������������ � ������� ������.");
        openSettingsButtonText.text = (isEnglish ? "Open Settings" : "������� ���������");
#endif
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


    public void SwitchToGreekLanguage()
    {
        _beaconManager.isEnglish = false;
        languageButton.sprite = englishFlagSprite;        

        ChangeTextsLanguage(_beaconManager.isEnglish);


    }


    public void SwitchToEnglishLanguage()
    {
        _beaconManager.isEnglish = true;
        languageButton.sprite = greekFlagSprite;

        ChangeTextsLanguage(_beaconManager.isEnglish);

    }
}
