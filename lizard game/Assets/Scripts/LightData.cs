using System;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class LightData : MonoBehaviour
{
    private Light light;
    private Transform lightTrans;
    [SerializeField] private float lightRange = 5f;

    public Transform LightTrans
    {
        get { return lightTrans; }
    }

    public float LightRange
    {
        get { return Mathf.Abs(lightRange); }
    }

    // Use this for initialization
    void Start()
    {
        light = this.GetComponent<Light>();
        lightTrans = light.transform;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (this != null && lightTrans.position != null && lightTrans != null)
        {
            Gizmos.DrawWireSphere(this.LightTrans.position, this.LightRange);
        }
    }
#endif
}