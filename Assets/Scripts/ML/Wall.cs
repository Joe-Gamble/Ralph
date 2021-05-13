using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lerp = Vector3.Lerp(Vector3.right, Vector3.up, Counter.percentage_1000 / 100.0f);
        Vector4 color = new Vector4(lerp.x, lerp.y, lerp.z, mat.color.a);
        mat.color = color;
    }
}
