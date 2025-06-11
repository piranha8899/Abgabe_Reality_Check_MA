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
    public TextMeshProUGUI kwhText;
    public TextMeshProUGUI chfText;
    public TextMeshProUGUI solarText;
    public TextMeshProUGUI waterText;

    [Header("Datentabelle")]
    [SerializeField] private int[,] consumptionTable = new int[,]
    {
        // Haushalte 1-4, [vor Ersparnis, nach Ersparnis] für kWh, CHF, Solar, Wasser
        { 800, 600, 240, 180, 400, 544, 100, 145 }, // 1 Person
        { 1200, 800, 360, 240, 600, 544, 180, 145 }, // 2 Personen  
        { 1800, 1200, 540, 360, 800, 700, 250, 200 }, // 3 Personen
        { 2400, 1600, 720, 480, 1000, 900, 320, 250 } // 4 Personen
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
        householdSizeSlider.maxValue = consumptionTable.GetLength(0);
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
            redButton.gameObject.SetActive(!showAfterSavings); // Rot aktiv wenn "vor Ersparnis"
        }
        
        if (greenButton != null)
        {
            greenButton.gameObject.SetActive(showAfterSavings); // Grün aktiv wenn "nach Ersparnis"
        }
    }

    void UpdateDisplay()
    {
        int household = (int)householdSizeSlider.value - 1;
        int savings = showAfterSavings ? 1 : 0;
        
        if (household >= 0 && household < consumptionTable.GetLength(0))
        {
            kwhText.text = consumptionTable[household, savings].ToString();
            chfText.text = consumptionTable[household, 2 + savings].ToString();
            solarText.text = consumptionTable[household, 4 + savings].ToString();
            waterText.text = consumptionTable[household, 6 + savings].ToString();
        }
    }
}
