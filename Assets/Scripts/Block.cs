using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ˆÚ“®—p
    void Move(Vector3 move_direction)
    {
        transform.position += move_direction;
    }

    // ˆÚ“®ŠÖ”
    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }
    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }
    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }
    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }

    // ‰ñ“]ŠÖ”
    public void RotateRight()
    {
        transform.Rotate(0, 0, -90);
        RotateChild(new Vector3(0, 0, -90));
    }
    public void RotateLeft()
    {
        transform.Rotate(0, 0, 90);
        RotateChild(new Vector3(0, 0, 90));
    }

    // q‹Ÿ•ª‚Ì‰ñ“]
    private void RotateChild(Vector3 rotate)
    {
        for (int child_count = 0; child_count < transform.childCount; child_count++)
        {
            Transform chiled = transform.GetChild(child_count);

            chiled.Rotate(rotate.x, rotate.y, rotate.z * -1);
        }
    }
}
