using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum PipeType {
    Straight = 1,
    Curved = 2,
    Cross = 3,
    Start = 4
}

enum PipeState {
    Empty = 0,
    Filling = 1,
    HalfFilled = 2,
    Filled = 3
}

[System.Serializable] //public defintion of class
public class Pipe
{
    // data member
    private Vector2 xy_position;
    private GameObject PipeSprite;
    PipeType pipe_type;

    // equivalent to start()
    public Pipe(ref GameObject Pipe, float x_pos, float y_pos) {
        xy_position = new Vector2(x_pos, y_pos);

        PipeSprite = Pipe;
        PipeSprite.transform.position = new Vector2(xy_position.x, xy_position.y);
    }
    public Pipe(ref GameObject Pipe, System.Object[] pipe_data, float x_pos, float y_pos)
        : this(ref Pipe, x_pos, y_pos)
    {
        // pipe data handeling
        pipe_type = (PipeType)pipe_data[0];
        PipeSprite.GetComponent<SpriteRenderer>().sprite = (Sprite)pipe_data[1];
        PipeSprite.transform.Rotate(Vector3.forward * -90 * (int)pipe_data[2]);
    }
}
