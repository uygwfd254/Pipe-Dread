using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
enum PipeType {
    Straight = 0,
    Curved = 1,
    Cross = 2,
    Start = 3
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
        change_pipe_data(pipe_data);
    }

    public void change_pipe_data(System.Object[] pipe_data) {
        // pipe data : PipeType, Sprite, time_of_rotation.
        int i = 0;

        pipe_type = (PipeType)pipe_data[i++];
        PipeSprite.GetComponent<SpriteRenderer>().sprite = (Sprite)pipe_data[i++];
        PipeSprite.transform.Rotate(Vector3.forward * -90 * (int)pipe_data[i++]);
    }

    public System.Object[] get_pipe_data() {
        System.Object[] pipe_data = new System.Object[10];
        int i = 0;

        pipe_data[i++] = pipe_type;
        pipe_data[i++] = PipeSprite.GetComponent<SpriteRenderer>().sprite;
        pipe_data[i++] = (int)(-(PipeSprite.transform.eulerAngles.z / 90) % 4);
        Debug.Log((int)pipe_data[2]);

        return pipe_data;
    }

    public void set_gameObject_name(string new_name) {
        PipeSprite.name = new_name;
    }

    public Vector2 get_gameObject_position() {
        return PipeSprite.transform.position;
    }

    public void set_gameObject_position(Vector2 pos) {
        PipeSprite.transform.position = new Vector2(pos.x, pos.y);
    }

    public void destroy_gameObject() {
        UnityEngine.MonoBehaviour.Destroy(PipeSprite);
    }
}
