using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardController : MonoBehaviour
{
    [SerializeField] private GameObject hazard;
    [SerializeField] private Animator hazardAnimator;
    [SerializeField] PlayerController Lizard;
    private float timer = 0;


    // Use this for initialization
    void Start()
    {
        hazardAnimator = GetComponentInChildren<Animator>();
        hazard = hazardAnimator.gameObject;
        hazard.SetActive(false);
        Lizard = FindObjectOfType<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (Lizard.HazardTrigger())
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0f;
            }

            if (timer > 1f)
            {
                hazard.SetActive(true);
                hazardAnimator.SetTrigger("HazardActive");
                timer = 0f;
            }
        }
    }
}