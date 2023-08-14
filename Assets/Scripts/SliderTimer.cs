using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTimer : MonoBehaviour
{
    Slider slTimer;
    public TextMeshProUGUI textTimer;

    public StageController stageController;

    public void SetStageController(StageController controller)
    {
        this.stageController = controller;
    }

    public float GetCurrentTimeValue() // Method to get the current timer value
    {
        return slTimer.value;
    }

    void Start()
    {
        slTimer = GetComponent<Slider>();
    }

    void Update()
    {
        if(slTimer.value > 0.0f)
        {
            slTimer.value -= Time.deltaTime;
            UpdateTimeText();
        }
        else
        {
            Debug.Log("Time is Zero");
            stageController.EndGame();
        }
    }

    public void StopTimer()
    {
        enabled = false;
    }

    void UpdateTimeText()
    {
        int seconds = Mathf.FloorToInt(slTimer.value);
        textTimer.text = seconds.ToString();
    }
}
