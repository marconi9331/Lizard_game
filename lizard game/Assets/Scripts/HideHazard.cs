using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideHazard : MonoBehaviour
{
    private GameObject flipFlop;

    // Use this for initialization
    void Start()
    {
        flipFlop = this.gameObject;
    }

    public void show()
    {
        flipFlop.SetActive(true);
    }

    public void Hide()
    {
        flipFlop.SetActive(false);
    }
}