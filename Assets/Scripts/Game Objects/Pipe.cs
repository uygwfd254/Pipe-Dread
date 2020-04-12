using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool cross_flip;
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
        cross_flip = false;
    }
    public Pipe(ref GameObject Pipe, PipeData pipe_data,
        float x_pos, float y_pos)
        : this(ref Pipe, x_pos, y_pos)
    {
        // pipe data handeling
        change_pipe_data(pipe_data);
    }

    public void change_pipe_data(PipeData pipe_data) {
        pipe_type = pipe_data.pipeType;
        PipeSprite.GetComponent<SpriteRenderer>().sprite = pipe_data.PipeSprite;
        PipeSprite.transform.Rotate(Vector3.forward * -90 * pipe_data.rotationTimes);
        pipe_open_side = pipe_data.boolPipeSide;
        rc_index = pipe_data.PipeIndex;
        curved_pipe_side = pipe_data.curvedPipeSide;
        check_adjacent_pipe(pipe_data.isInGrid);

        // after data is inserted
        PipeSprite.GetComponent<Animator>().enabled = false;
    }

    public PipeData get_pipe_data() {
        PipeData pipe_data = new PipeData();

        pipe_data.pipeType = pipe_type;
        pipe_data.PipeSprite = PipeSprite.GetComponent<SpriteRenderer>().sprite;
        pipe_data.rotationTimes = (int)(-(PipeSprite.transform.eulerAngles.z / 90) % 4);
        pipe_data.boolPipeSide = pipe_open_side;
        pipe_data.curvedPipeSide = curved_pipe_side;

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
        string[] restriction_ = new string[2] {"D", "L"};
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
                if (!cross_flip) {
                    foreach(string element in restriction) {
                        if (side == element) {
                            PipeSprite.transform.Rotate(Vector3.forward * 180);
                            cross_flip = true;
                        }
                    }
                } else {
                    foreach(string element in restriction_) {
                        if (side == element) {
                            PipeSprite.transform.Rotate(Vector3.forward * 180);
                        }
                    }
                }
                break;

            default:
                break;
        }
    }

    public bool check_animation_is_finished() {
        return PipeSprite.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1;
    }

    bool check_adjacent_pipe(string side="") {
        if (side == "no")
            return false;

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
        bool is_adjacent = false;
        foreach(KeyValuePair<string, Vector2> element in search_coord_list) {
            if (pipe_open_side.get_bool_side(element.Key) &&
                (rc_index != restriction[i]) &&
                (side == "" || side == element.Key)) {
                is_adjacent = true;
                pipe = (Pipe)Event_Manager.TriggerEvent("get_pipe_with_index",
                    new Vector2(rc_index.x + element.Value.x,
                                rc_index.y + element.Value.y));
                if (pipe.get_pipe_type() != PipeType.Empty) {
                    adjacent_pipes.set_pipe_with_side(element.Key, pipe);
                    if (pipe.adjacent_pipes.get_pipe_with_side(adjacent_pipes.get_opposite_pipe_string(element.Key)) 
                    != this) {
                        if (!pipe.check_adjacent_pipe(adjacent_pipes.get_opposite_pipe_string(element.Key)))
                            adjacent_pipes.set_pipe_with_side(element.Key, null);
                    }
                }
            }
            i++;
        }

        return is_adjacent;
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
