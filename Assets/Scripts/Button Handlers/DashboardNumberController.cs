using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DashboardNumberController : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider householdSizeSlider;
    public Button redButton;
    public Button greenButton;
    
    [Header("Dashboard Texts")]
    public TextMeshProUGUI kwhText;           // 600 (Hauptwert)
    public TextMeshProUGUI savingsKWhProcentText; // -25% (unter kWh)
    public TextMeshProUGUI chfText;           // 180 (Hauptwert)
    public TextMeshProUGUI savingsChfText;    // -60 CHF (unter CHF)
    public TextMeshProUGUI solarText;         // 598 (Hauptwert rechts)
    public TextMeshProUGUI savingsSolarText;  // +54 (unter Solar)
    public TextMeshProUGUI waterText;         // 159 (Hauptwert rechts unten)
    public TextMeshProUGUI savingsWaterText;  // +14 (unter Wasser)

    [Header("Kategorie-Auswahl")]
    [SerializeField] private int selectedCategory = 0; //Kategorie-Auswahl im Inspector

    [Header("Datentabellen")]
    [SerializeField] private int[,,] consumptionTables = new int[,,]
    {
        // Tabelle 0: Küche
        {
            { 584, 438, 169, 127, 680, 742, 181, 197, 25, 42, 62, 16 }, // 1 Person
            { 710, 532, 206, 154, 544, 592, 145, 157, 25, 51, 48, 13 }, // 2 Personen  
            { 962, 721, 279, 209, 388, 421, 103, 112, 25, 70, 33, 9 },  // 3 Personen
            { 1088, 816, 316, 237, 340, 368, 90, 98, 25, 79, 29, 8 },   // 4 Personen
            { 1214, 910, 352, 264, 302, 327, 80, 87, 25, 88, 25, 7 }    // 5 Personen
        },

        // Tabelle 1: Gesamt
        {
            { 1752, 1314, 508, 381, 680, 907, 181, 241, 25, 127, 227, 60 }, // 1 Person
            { 2190, 1643, 635, 476, 544, 725, 145, 193, 25, 159, 181, 48 }, // 2 Personen  
            { 3067, 2300, 889, 667, 388, 518, 103, 138, 25, 222, 129, 34 }, // 3 Personen
            { 3506, 2629, 1017, 762, 340, 453, 90, 120, 25, 254, 113, 30 }, // 4 Personen
            { 3944, 2958, 1144, 858, 302, 403, 80, 107, 25, 286, 101, 27 }  // 5 Personen
        }
        // Hier können weitere Tabellen für andere Kategorien hinzugefügt werden
    };

    private bool showAfterSavings = false;

    void Start()
    {
        SetupSliders();
        SetupToggleButtons();
        UpdateDisplay();
    }

    void SetupSliders()
    {
        householdSizeSlider.minValue = 1;
        householdSizeSlider.maxValue = 5;
        householdSizeSlider.wholeNumbers = true;
        householdSizeSlider.SetValueWithoutNotify(2);
        
        householdSizeSlider.onValueChanged.AddListener(_ => UpdateDisplay());
    }

    void SetupToggleButtons()
    {
       if (redButton != null)
            redButton.onClick.AddListener(OnRedButtonClicked);
        
       if (greenButton != null)
            greenButton.onClick.AddListener(OnGreenButtonClicked);
    }

    void OnRedButtonClicked()
    {
        showAfterSavings = true;
        UpdateButtonStates();
        UpdateDisplay();
    }

    void OnGreenButtonClicked()
    {
        showAfterSavings = false;
        UpdateButtonStates();
        UpdateDisplay();
    }

    void UpdateButtonStates()
    {
        if (redButton != null)
        {
            redButton.gameObject.SetActive(!showAfterSavings);
        }
        
        if (greenButton != null)
        {
            greenButton.gameObject.SetActive(showAfterSavings);
        }
    }

    void UpdateDisplay()
    {
        int household = (int)householdSizeSlider.value - 1;
        int s = showAfterSavings ? 1 : 0;
        
        if (household >= 0 && household < consumptionTables.GetLength(1) && 
            selectedCategory >= 0 && selectedCategory < consumptionTables.GetLength(0))
        {
            // Hauptwerte (wechseln vor/nach bei Button-Klick)
            if (kwhText != null) 
                kwhText.text = consumptionTables[selectedCategory, household, 0 + s].ToString();
            if (chfText != null) 
                chfText.text = consumptionTables[selectedCategory, household, 2 + s].ToString();
            if (solarText != null) 
                solarText.text = consumptionTables[selectedCategory, household, 4 + s].ToString();
            if (waterText != null) 
                waterText.text = consumptionTables[selectedCategory, household, 6 + s].ToString();
            
            // Ersparnis-Werte (bleiben konstant, zeigen Differenz)
            if (savingsKWhProcentText != null) 
                savingsKWhProcentText.text = "-" + consumptionTables[selectedCategory, household, 8] + "%";
            if (savingsChfText != null) 
                savingsChfText.text = "-" + consumptionTables[selectedCategory, household, 9] + " CHF";
            if (savingsSolarText != null) 
                savingsSolarText.text = "+" + consumptionTables[selectedCategory, household, 10].ToString();
            if (savingsWaterText != null) 
                savingsWaterText.text = "+" + consumptionTables[selectedCategory, household, 11].ToString();
        }
    }
}
