using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MathVisualizationManager : MonoBehaviour
{
    public GameObject prefab;

    public int count;
    public List<GameObject> sphereInstances;

    public Vector3 center;

    public float circleRadius;

    private void Awake()
    {
        InitializeArray();
    }


    private void Start()
    {
        if(sphereInstances.Count == 0)
        {
            count = 10;
            InitializeArray();
        }

        PlaceInstances();
    }

    private void InitializeArray()
    {
        var radiusOverCount = 2 * circleRadius / count;
        var smallerScale = new Vector3(radiusOverCount, radiusOverCount, radiusOverCount);
        sphereInstances = new List<GameObject>(count);

        for(int i = 0; i < count; i++)
        {
            var obj = GameObject.Instantiate(prefab);
            obj.transform.localScale = smallerScale;
            obj.SetActive(false);
            sphereInstances.Add(obj);
        }
    }


    private Vector2 GetPointOnUnitCircle(float theta)
    {
        var result = Vector2.zero;

        result.x = Mathf.Cos(theta);
        result.y = Mathf.Sin(theta);

        return result;
    }

    private void PlaceInstances()
    {
        var theta_base = (Mathf.PI * 2) / count;

        var theta = 0f;
        for(int i = 0; i < sphereInstances.Count; i++)
        {
            theta += theta_base;

            var point = GetPointOnUnitCircle(theta);
            var direction = new Vector3(point.x, point.y, 0f).normalized;

            sphereInstances[i].SetActive(true);
            sphereInstances[i].transform.position = center + direction * circleRadius;
            sphereInstances[i].transform.SetParent(this.transform);
        }
    }





}
