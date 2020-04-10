using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PipeType {
    Straight = 0,
    Curved = 1,
    Cross = 2,
    Start = 3,
    Empty = 4
}

[System.Serializable]
public enum PipeState {
    Empty = 0,
    Filling = 1,
    HalfFilling = 2,
    HalfFilled = 3,
    Filled = 4,
    Destroyed = 5
}

[System.Serializable]
public class BoolPipeSide {
    private Dictionary<string, bool> OpenSides;

    // bool r, bool d, bool l, bool u
    public BoolPipeSide(bool[] sides) {
        OpenSides = new Dictionary<string, bool>() {
            {"R", false},
            {"D", false},
            {"L", false},
            {"U", false}
        };
        // update the dictionary with new sides
        int i = 0;
        Dictionary<string, bool> Temp = new Dictionary<string, bool>();
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            Temp.Add(element.Key, sides[i++]);
        }

        OpenSides = Temp;
    }

    public bool get_bool_side(string side) {
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            if (side == element.Key)
                return element.Value;
        }

        return false;
    }

    public void set_bool_side(string side, bool bool_) {
        Dictionary<string, bool> Temp = new Dictionary<string, bool>();
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            if (element.Key == side) {
                Temp.Add(element.Key, bool_);
            } else {
                Temp.Add(element.Key, element.Value);
            }
        }

        OpenSides = Temp;
    }

    public string pipe_bool_search(bool target) {
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            if (element.Value == target)
                return element.Key;
        }

        return "";
    }
}

class ObjectPipeSide {
    private Dictionary<string, Pipe> PipeSides;
    private Dictionary<string, string> opposide_side_list = 
            new Dictionary<string, string>() {
                {"R", "L"},
                {"D", "U"},
                {"L", "R"},
                {"U", "D"}
            };

    public ObjectPipeSide() {
        PipeSides = new Dictionary<string, Pipe>() {
            {"R", null},
            {"D", null},
            {"L", null},
            {"U", null}
        };
    }

    public void set_pipe_with_side(string side, Pipe pipe) {
        Dictionary<string, Pipe> Temp = new Dictionary<string, Pipe>();
        foreach(KeyValuePair<string, Pipe> element in PipeSides) {
            if (element.Key == side) {
                Temp.Add(element.Key, pipe);
            } else {
                Temp.Add(element.Key, element.Value);
            }
        }

        PipeSides = Temp;
    }

    public string get_opposite_pipe_string(string side) {
        foreach(KeyValuePair<string, string> element in opposide_side_list) {
            if (element.Key == side)
                return element.Value;
        }

        return "";
    }

    public Pipe get_pipe_with_side(string side) {
        foreach(KeyValuePair<string, Pipe> element in PipeSides) {
            if (element.Key == side)
                return element.Value;
        }

        return null;
    }
}


enum CrossPipeFillState {
    Empty = 0,
    Vertical = 1,
    Horizontal = 2
}

[System.Serializable] //public defintion of class
public class Pipe
{
    // data member
    private Vector2 xy_position;
    private Vector2 rc_index;
    private GameObject PipeSprite;
    private PipeType pipe_type;
    private PipeState pipe_state;
    private CrossPipeFillState cross_pipe_state;
    private string curved_pipe_side;
    private BoolPipeSide pipe_open_side;
    private ObjectPipeSide adjacent_pipes;
    private Pipe[] connect_pipe;

    // equivalent to start()
    public Pipe(ref GameObject Pipe, float x_pos, float y_pos) {
        xy_position = new Vector2(x_pos, y_pos);

        PipeSprite = Pipe;
        PipeSprite.transform.position = new Vector2(xy_position.x, xy_position.y);
        pipe_type = PipeType.Empty;
        cross_pipe_state = CrossPipeFillState.Empty;

        adjacent_pipes = new ObjectPipeSide();
        //rc_index = new Vector2();
    }
    public Pipe(ref GameObject Pipe, System.Object[] pipe_data,
        float x_pos, float y_pos)
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
        pipe_open_side = (BoolPipeSide)pipe_data[i++];
        rc_index = (Vector2)pipe_data[5];
        curved_pipe_side = (string)pipe_data[6];
        check_adjacent_pipe((string)pipe_data[i++]);

        // after data is inserted
        PipeSprite.GetComponent<Animator>().enabled = false;
    }

    public System.Object[] get_pipe_data() {
        System.Object[] pipe_data = new System.Object[10];
        int i = 0;

        pipe_data[i++] = pipe_type;
        pipe_data[i++] = PipeSprite.GetComponent<SpriteRenderer>().sprite;
        pipe_data[i++] = (int)(-(PipeSprite.transform.eulerAngles.z / 90) % 4);
        pipe_data[i++] = pipe_open_side;
        pipe_data[6] = curved_pipe_side;

        return pipe_data;
    }

    public void set_gameObject_name(string new_name) {
        PipeSprite.name = new_name;
    }

    public void set_gameObject_position(Vector2 pos) {
        PipeSprite.transform.position = new Vector2(pos.x, pos.y);
    }


    public Vector2 get_gameObject_position() {
        return PipeSprite.transform.position;
    }

    public PipeType get_pipe_type() {
        return pipe_type;
    }

    public PipeState get_pipe_state() {
        return pipe_state;
    }

    public void destroy_gameObject() {
        UnityEngine.MonoBehaviour.Destroy(PipeSprite);
        pipe_state = PipeState.Destroyed;
    }

    // animations
    public void start_filling(string side="") {
        // delete openside
        Dictionary <string, CrossPipeFillState> cross_state_match =
            new Dictionary<string, CrossPipeFillState>() {
                {"R", CrossPipeFillState.Horizontal},
                {"D", CrossPipeFillState.Vertical},
                {"L", CrossPipeFillState.Horizontal},
                {"U", CrossPipeFillState.Vertical},
            };
        
        if (side != "") {
            pipe_open_side.set_bool_side(side, false);

            if (pipe_type == PipeType.Cross) {
                string side_;
                if (pipe_state == PipeState.Empty) {
                    side_ = pipe_open_side.pipe_bool_search(false);
                } else {
                    side_ = pipe_open_side.pipe_bool_search(true);
                }

                foreach(KeyValuePair<string, CrossPipeFillState> element in cross_state_match) {
                    if (side_ == element.Key)
                        cross_pipe_state = element.Value;
                }
            }
        }

        // handels animation
        PipeSprite.GetComponent<Animator>().enabled = true;
        if (pipe_type == PipeType.Cross) {
            PipeSprite.GetComponent<Animator>().SetTrigger(pipe_type.ToString() + 
                                                           pipe_state.ToString() + 
                                                           cross_pipe_state.ToString());
            if (pipe_state == PipeState.Empty) {
                pipe_state = PipeState.HalfFilling;
            } else {
                pipe_state = PipeState.Filling;
            }
        } else {
            PipeSprite.GetComponent<Animator>().SetTrigger(pipe_type.ToString());
            pipe_state = PipeState.Filling;
        }

        // handels rotation
        string[] restriction = new string[2] {"U", "R"};
        string[] curve_restrict = new string[4] {"RD", "DL", "LU", "UR"};
        switch(pipe_type) {
            case PipeType.Start:
                return; // skips
            
            case PipeType.Curved:
                string out_ = pipe_open_side.pipe_bool_search(true);
                string in_ = "";
                if (out_ == curved_pipe_side[0].ToString())
                    in_ = curved_pipe_side[1].ToString();
                else
                    in_ = curved_pipe_side[0].ToString();
                
                string a = in_ + out_;
                foreach(string element in curve_restrict) {
                    if (a == element) {
                        PipeSprite.transform.Rotate(Vector3.right * 180);
                        PipeSprite.transform.Rotate(Vector3.forward * 90);
                    }
                }  

                break;

            case PipeType.Straight:
                foreach(string element in restriction) {
                    if (side == element)
                        PipeSprite.transform.Rotate(Vector3.right * 180);
                }   
                break;

            case PipeType.Cross:
                foreach(string element in restriction) {
                    if (side == element)
                        PipeSprite.transform.Rotate(Vector3.forward * 180);
                } 
                break;

            default:
                break;
        }
    }

    public bool check_animation_is_finished() {
        return PipeSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }

    void check_adjacent_pipe(string side="") {
        if (side == "no")
            return;

        if (side == "yes")
            side = "";

        Pipe pipe;
        Dictionary<string, Vector2> search_coord_list =
            new Dictionary<string, Vector2>() {
                {"R", new Vector2(0, 1)},
                {"D", new Vector2(1, 0)},
                {"L", new Vector2(0, -1)},
                {"U", new Vector2(-1, 0)},
            };
        Vector2[] restriction = new Vector2[4] {
            new Vector2(rc_index.x, 10),
            new Vector2(6, rc_index.y),
            new Vector2(rc_index.x, 0),
            new Vector2(0, rc_index.y)
        };

        int i = 0;
        foreach(KeyValuePair<string, Vector2> element in search_coord_list) {
            if (pipe_open_side.get_bool_side(element.Key) &&
                (rc_index != restriction[i]) &&
                (side == "" || side == element.Key)) {
                pipe = (Pipe)Event_Manager.TriggerEvent("get_pipe_with_index",
                    new Vector2(rc_index.x + element.Value.x,
                                rc_index.y + element.Value.y));
                if (pipe.get_pipe_type() != PipeType.Empty) {
                    adjacent_pipes.set_pipe_with_side(element.Key, pipe);
                    if (pipe.adjacent_pipes.get_pipe_with_side(adjacent_pipes.get_opposite_pipe_string(element.Key)) 
                    != this) {
                        pipe.check_adjacent_pipe(adjacent_pipes.get_opposite_pipe_string(element.Key));
                    }
                }
            }
            i++;
        }
    }

    public Pipe next_pipe() {
        if (pipe_type == PipeType.Cross &&
            (pipe_state == PipeState.HalfFilling ||
             pipe_state == PipeState.HalfFilled)) {
            return adjacent_pipes.get_pipe_with_side(
                    adjacent_pipes.get_opposite_pipe_string(
                        pipe_open_side.pipe_bool_search(false)
                    )
                );
        } else {
            return adjacent_pipes.get_pipe_with_side(
                    pipe_open_side.pipe_bool_search(true)
                );
        }
    }

    public void finished_animation() {
        if (pipe_type == PipeType.Cross &&
            pipe_state == PipeState.HalfFilling) {
            pipe_state = PipeState.HalfFilled;

            if (cross_pipe_state == CrossPipeFillState.Horizontal)
                cross_pipe_state = CrossPipeFillState.Vertical;
            else
                cross_pipe_state = CrossPipeFillState.Horizontal;
    
        } else {
            pipe_state = PipeState.Filled;
        }
        //Debug.Log(pipe_state);
        
        if (pipe_type == PipeType.Cross &&
            pipe_state == PipeState.HalfFilled) {
            next_pipe().start_filling(
                    pipe_open_side.pipe_bool_search(false)
                );
            pipe_open_side.set_bool_side(adjacent_pipes.get_opposite_pipe_string(
                    pipe_open_side.pipe_bool_search(false)
                ), false);
        } else {
            next_pipe().start_filling(
                    adjacent_pipes.get_opposite_pipe_string(
                        pipe_open_side.pipe_bool_search(true)
                    )
                );
            pipe_open_side.set_bool_side(pipe_open_side.pipe_bool_search(true),
                false);
            
        }

        
    }
}
