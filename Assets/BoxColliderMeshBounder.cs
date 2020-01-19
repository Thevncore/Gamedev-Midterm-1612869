using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderMeshBounder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var bc = GetComponent<BoxCollider2D>();
        var rt = GetComponent<RectTransform>();

        bc.size = rt.rect.size;
        bc.offset = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
