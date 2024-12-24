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
        scannerBtn.text = (isEnglish ? "SCANNER" : "ΣΑΡΩΤΗΣ");
        favoritesBtn.text = (isEnglish ? "FAVORITES" : "ΑΓΑΠΗΜΕΝΑ");
        infoBtn.text = (isEnglish ? "INFO" : "ΠΛΗΡΟΦΟΡΙΕΣ");
        aboutUsBtn.text = (isEnglish ? "ABOUT US" : "ΣΧΕΤΙΚΑ ΜΕ ΕΜΑΣ");
        qrScannerBtn.text = (isEnglish ? "QR SCANNER" : "QR ΣΑΡΩΤΗΣ");
        quitBtn.text = (isEnglish ? "QUIT" : "ΕΞΟΔΟΣ");

        geoparkSitesBtn.text = (isEnglish ? "Geopark Sites" : "Σημεία Γεωπάρκου");
        mapBtn.text = (isEnglish ? "Map" : "Χάρτης");

        detailsInfoBtn.text = (isEnglish ? "Info" : "Πληροφορίες");
        galleryBtn.text = (isEnglish ? "Gallery" : "Εικόνες");
        videosBtn.text = (isEnglish ? "Videos" : "Βίντεο");

        servicesLabel.text = (isEnglish ? "Services" : "Υπηρεσίες");

        geoparkInfoTitle.text = (isEnglish ? "About Geopark Grevena - Kozani" : "Σχετικά με το Γεωπάρκο Γρεβενών - Κοζάνης");
        geoparkInfo.text = (isEnglish ? "Welcome to Grevena–Kozani UNESCO Global Geopark, within the heart of West Macedonia. Here, you can retrace the steps of early researchers contributing towards the development of plate tectonic theory. You will see landforms created by the intersecting actions of plate tectonics, climate change and life.  The geopark includes the oldest dated rocks of Greece, dating to nearly a billion years in age. The mountains contain the lost Pangaean world of ~250 million years ago that split apart to give birth to Africa, Europe, and the ancient Tethyan Ocean. This long geologic evolution produced an environment where mammoths and ancient elephants flourished. The landscape is polished to a rare beauty by glaciation that created a highland of rugged mountains, cataclysmic canyons and rock formations of strange shapes and mysterious figures. This unique geologic and environmental setting is home to traditional villages where residents maintain time-honored folkways of the Greek countryside.\r\n\r\nOur figurehead is Tethys herself, the Titan of Greek mythology who was mother to all the waters of the Earth. She is the namesake of the ancient ocean which gave birth to our unique geological terrain. Tethys was said to have given birth to the Aliakmon River, as told by the poet Hesiod (~700 BC).\r\n\r\nAs you explore Geopark Grevena-Kozani, you will travel through time to the land of Tethys, the birth of West Macedonia and the heritage of a billion years of Earth history.\r\n\r\nGeopark activities have been held in the area for over forty years: these include international earth science educational projects, regional and international geoscience conferences, and the promoting of geotouristic goals. Geopark Grevena - Kozani has been in operation since 2014, and was inducted to the UNESCO Global Geoparks Network in 2021. It is managed by the Regional Development Agency of West Macedonia, and scientific support is provided by Geowonders Greece SCE. The geopark includes parts of five townships of the Regional Government of West Macedonia (Grevena, Deskati, Voio, Kozani, and Servia).\r\n\r\nThe Geopark hosts more than 119 geosites of unique geologic interest, and 18 geoheritage areas where tourists can enjoy hours or days in appreciation of the geo-environment and cultural legacy of the parkland.\r\n\r\nFor more information, visit our Geo-Information Centers in Grevena, Siatista, Deskati .\r\n\r\nEmail us at: <color=\"orange\">geoparkgrevenakozani@anko.gr & geowonders@gmail.com</color>, \r\n\r\nContact us by phone at <color=\"orange\">(+30)6949439053 or (+30)2461024022\r\n</color>\r\n\r\nVisit our website at <color=\"orange\">www.geoparkgrevenakozani.com</color> or \r\n\r\n<color=\"orange\">Facebook at Geopark Grevena-Kozani.</color>\r\n" : "Καλώς ήλθατε Παγκόσμιο Γεωπάρκο Γρεβενών – Κοζάνης αναγνωρισμένο από την UNESCO, στη καρδιά της Δυτικής Μακεδονίας. Εδώ, με την βοήθεια της επιστήμης μπορείτε να ακολουθήσετε βήμα βήμα, την πορεία των τεκτονικών πλακών, να θαυμάσετε τα ανάγλυφα τοπία που δημιουργούνται από την κίνηση των πλακών αυτών και να απολαύσετε τη σπάνια βιοποικιλότητα τους. Φιλοξενούμε τα παλαιότερα πετρώματα της Ελλάδος που χρονολογούνται σχεδόν ένα δισεκατομμύριο χρόνια. \r\n\r\nΣτα βουνά μας αποτυπώνεται ο απολεσθέντας κόσμο της Παγγαίας από όπου, πριν από 250 εκατομμύρια χρόνια, χωρίστηκε για να δημιουργήσει την Αφρική, την Ευρώπη και τον αρχαίο ωκεανό της Τηθύος. Αυτή η μακρά γεωλογική εξέλιξη παρήγαγε ένα περιβάλλον, όπου ευδοκιμούσαν μαμούθ και αρχαίοι ελέφαντες. Το τοπίο είναι σμιλευμένο και από παγετώνες, δημιουργώντας μια σπάνια και ιδιαίτερη ομορφιά. Μια οροσειρά με τραχιά βουνά, κατακλυσμικά φαράγγια και βραχώδεις περίεργους σχηματισμούς.\r\n\r\nΤο Γεωπάρκο Γρεβενών – Κοζάνης φιλοξενεί παραδοσιακούς οικισμούς, όπου οι κάτοικοι διατηρούν μέχρι σήμερα την παραδοσιακή τους ταυτότητα μέσα από τα ήθη κι έθιμα της ελληνικής υπαίθρου, σχετιζόμενα με την μοναδική γεωλογία και το φυσικό περιβάλλον της περιοχής. \r\n\r\nΤο λογότυπό μας είναι η ίδια η Tηθύς, μία από τις Τιτανίδες της Ελληνικής Μυθολογίας, μητέρα όλων των υδάτων της Γης. Το όνομα της δόθηκε στον αρχαίο ωκεανό που γέννησε το μοναδικό μας γεωλογικό υπόβαθρο, καθώς και τον ποταμό Αλιάκμονα, όπως αναφέρει ο ίδιος ο Ησίοδος (~ 700 π.Χ.). \r\n\r\nΕλπίζουμε να γνωρίσετε τη γη της Τηθύος μέσα από το Γεωπάρκο Γρεβενών-Κοζάνης, που βρίσκεται στην καρδιά της Δυτικής Μακεδονίας και να γίνετε αποδέκτες της κληρονομιάς ενός δισεκατομμυρίου ετών παγκόσμιας ιστορίας.\r\n\r\nΓια περισσότερα από σαράντα χρόνια, διεξάγονται στην περιοχή ποικίλες δραστηριότητες του γεωπάρκου με στόχο τη ανάπτυξη και προώθηση του γεωτουρισμού, περιλαμβάνοντας διεθνή γεωλογικά εκπαιδευτικά προγράμματα και περιφερειακά & διεθνή συνέδρια γεωεπιστημών. Το Γεωπάρκο Γρεβενών – Κοζάνης χρονολογείται από το 2014. Συντονιστής φορέας είναι η ANKO Δυτικής Μακεδονίας Α.Ε. (Αναπτυξιακός Οργανισμός Τοπικής Αυτοδιοίκησης), υποστηρίζεται επιστημονικά από την ΚοινΣΕπ «Γεωθαύματα Ελλάς» και αποτελεί μέλος του Παγκόσμιου Δικτύου Γεωπάρκων της UNESCO. Στην έκτασή του περιλαμβάνονται τμήματα πέντε Δήμων της Περιφέρειας Δυτικής Μακεδονίας (Γρεβενών, Δεσκάτης, Βοΐου, Κοζάνης και Σερβίων), καθώς εντοπίζονται περισσότεροι από 119 γεώτοποι μοναδικού γεωλογικού ενδιαφέροντος και 18 περιοχές γεωκληρονομιάς. Οι επισκέπτες μπορούν να γνωρίσουν και να μελετήσουν τη γεωπεριβαλλοντική και πολιτιστική κληρονομιά του Γεωπάρκου Γρεβενών – Κοζάνης, μέσα από εκπαιδευτικά και τουριστικά προγράμματα.  \r\n\r\nΓια περισσότερες πληροφορίες, επισκεφθείτε τα κέντρα πληροφόρησης Γρεβενών, Σιάτιστας και Δεσκάτης. \r\n\r\nΕπίσης, μπορείτε να επικοινωνήστε μαζί μας με <color=\"orange\">email: geoparkgrevenakozani@anko.gr & geowonders@gmail.com</color>, ή τηλεφωνικώς στους αριθμούς <color=\"orange\">6949439053 ή 2461024022\r\n</color>\r\n\r\n ή να επισκεφθείτε την ιστοσελίδα μας <color=\"orange\">www.geoparkgrevenakozani.com</color>, \r\n\r\nαλλά και το <color=\"orange\">Facebook στο Geopark Grevena – Kozani.\r\n</color>\r\n");

        servicesInfoTitle.text = (isEnglish ? "Geopark & Services Info" : "Πληροφορίες Υπηρεσιών Γεωπάρκου");

        accomodationService.text = (isEnglish ? "Accomodation" : "Διαμονή");
        foodService.text = (isEnglish ? "Food" : "Φαγητό");
        parkingService.text = (isEnglish ? "Parking" : "Χώρος Στάθμευσης");
        walkingService.text = (isEnglish ? "Walking" : "Περπάτημα");

        startingPanelTitle.text = (isEnglish ? "<b>Geopark</b>\nGrevena - Kozani" : "<b>Γεωπάρκο</b>\nΓρεβενών - Κοζάνης");
        startingPanelText.text = (isEnglish ? "Please enable <b>GPS</b> and <b>Bluetooth</b> in order for the beacon scanner to work." : "Παρακαλώ ανοίξτε το <b>GPS</b> και τα <b>Bluetooth</b> για να λειτουργεί ο σαρωτής πομπών.");

#if !UNITY_EDITOR
        warningText.text = (isEnglish ? "Please enable <b>Bluetooth</b> & <b>GPS</b> in order for the beacon scanner to work." : "Παρακαλώ ανοίξτε το <b>GPS</b> και τα <b>Bluetooth</b>, για να λειτουργήσει ο σαρωτής πομπών.");
        openSettingsButtonText.text = (isEnglish ? "Open Settings" : "Άνοιγμα Ρυθμίσεων");
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
