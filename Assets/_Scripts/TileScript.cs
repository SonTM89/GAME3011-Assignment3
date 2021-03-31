using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public GameObject[] dots;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialize()
    {
        int gemTOUse = Random.Range(0, dots.Length);
        GameObject dot = Instantiate(dots[gemTOUse], transform.position, Quaternion.identity);
        dot.transform.parent = this.transform;
        dot.name = this.gameObject.name;
    }
}
