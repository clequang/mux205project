using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    int nbResourceCollected = 0;

    public Text textResourceNumber;

    private void Start()
    {
        nbResourceCollected = 0;
    }

    public void Collect(string tag)
    {
        switch(tag)
        {
            case "resource":
                nbResourceCollected += 1;
                GetComponent<AudioSource>().Play();
                textResourceNumber.text = $"x {nbResourceCollected:00000000}";
                break;
        }
    }
}
