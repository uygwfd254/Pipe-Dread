using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pipe
{
    private Vector2 xy_position;
    public GameObject PipeSprite;

    // equivalent to start()
    public Pipe(ref GameObject Pipe, float x_pos, float y_pos)
    {
        xy_position = new Vector2(x_pos, y_pos);
        PipeSprite = Pipe;

        // initialization
        PipeSprite.transform.position = new Vector2(xy_position.x, xy_position.y);
    }
}
