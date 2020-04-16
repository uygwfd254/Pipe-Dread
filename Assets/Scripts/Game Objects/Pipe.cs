using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe
{
    // data member
    Vector2 XyPosition;
    Vector2 RcIndex;
    GameObject PipeSprite;
    PipeType pipeType;
    PipeState pipeState;
    CrossPipeFillState crossPipeState;
    string curvedPipeSide;
    bool crossFlip;
    BoolPipeSide pipeOpenSide;
    ObjectPipeSide adjacentPipes;

    // equivalent to start()
    public Pipe(ref GameObject Pipe, float x_pos, float y_pos) {
        XyPosition = new Vector2(x_pos, y_pos);

        PipeSprite = Pipe;
        PipeSprite.transform.position = new Vector2(XyPosition.x, XyPosition.y);
        pipeType = PipeType.Empty;
        crossPipeState = CrossPipeFillState.Empty;

        adjacentPipes = new ObjectPipeSide();
        crossFlip = false;
    }
    public Pipe(ref GameObject Pipe, PipeData pipe_data,
        float x_pos, float y_pos)
        : this(ref Pipe, x_pos, y_pos)
    {
        // pipe data handeling
        change_pipe_data(pipe_data);
    }

    public void change_pipe_data(PipeData pipe_data) {
        pipeType = pipe_data.pipeType;
        PipeSprite.GetComponent<SpriteRenderer>().sprite = pipe_data.PipeSprite;
        PipeSprite.transform.Rotate(Vector3.forward * -90 * pipe_data.rotationTimes);
        pipeOpenSide = pipe_data.boolPipeSide;
        RcIndex = pipe_data.PipeIndex;
        curvedPipeSide = pipe_data.curvedPipeSide;
        check_adjacent_pipe(pipe_data.isInGrid);

        // after data is inserted
        PipeSprite.GetComponent<Animator>().enabled = false;
    }

    public PipeData get_pipe_data() {
        PipeData pipe_data = new PipeData();

        pipe_data.pipeType = pipeType;
        pipe_data.PipeSprite = PipeSprite.GetComponent<SpriteRenderer>().sprite;
        pipe_data.rotationTimes = (int)(-(PipeSprite.transform.eulerAngles.z / 90) % 4);
        pipe_data.boolPipeSide = pipeOpenSide;
        pipe_data.curvedPipeSide = curvedPipeSide;

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
        return pipeType;
    }

    public PipeState get_pipe_state() {
        return pipeState;
    }

    public void destroy_gameObject() {
        UnityEngine.MonoBehaviour.Destroy(PipeSprite);
        pipeState = PipeState.Destroyed;
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
            pipeOpenSide.set_bool_side(side, false);

            if (pipeType == PipeType.Cross) {
                string side_;
                if (pipeState == PipeState.Empty) {
                    side_ = pipeOpenSide.pipe_bool_search(false);
                } else {
                    side_ = pipeOpenSide.pipe_bool_search(true);
                }

                foreach(KeyValuePair<string, CrossPipeFillState> element in cross_state_match) {
                    if (side_ == element.Key)
                        crossPipeState = element.Value;
                }
            }
        }

        // handels animation
        PipeSprite.GetComponent<Animator>().enabled = true;
        if (pipeType == PipeType.Cross) {
            PipeSprite.GetComponent<Animator>().SetTrigger(pipeType.ToString() + 
                                                           pipeState.ToString() + 
                                                           crossPipeState.ToString());
            if (pipeState == PipeState.Empty) {
                pipeState = PipeState.HalfFilling;
            } else {
                pipeState = PipeState.Filling;
            }
        } else {
            PipeSprite.GetComponent<Animator>().SetTrigger(pipeType.ToString());
            pipeState = PipeState.Filling;
        }

        // handels rotation
        string[] restriction = new string[2] {"U", "R"};
        string[] restriction_ = new string[2] {"D", "L"};
        string[] curve_restrict = new string[4] {"RD", "DL", "LU", "UR"};
        switch(pipeType) {
            case PipeType.Start:
                return; // skips
            
            case PipeType.Curved:
                string out_ = pipeOpenSide.pipe_bool_search(true);
                string in_ = "";
                if (out_ == curvedPipeSide[0].ToString())
                    in_ = curvedPipeSide[1].ToString();
                else
                    in_ = curvedPipeSide[0].ToString();
                
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
                if (!crossFlip) {
                    foreach(string element in restriction) {
                        if (side == element) {
                            PipeSprite.transform.Rotate(Vector3.forward * 180);
                            crossFlip = true;
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
        PipeUtil util = new PipeUtil();
        Vector2[] restriction = new Vector2[4] {
            new Vector2(RcIndex.x, 10),
            new Vector2(6, RcIndex.y),
            new Vector2(RcIndex.x, 0),
            new Vector2(0, RcIndex.y)
        };

        int i = 0;
        bool is_adjacent = false;
        foreach(KeyValuePair<string, Vector2> element in util.search_coord_list) {
            if (pipeOpenSide.get_bool_side(element.Key) &&
                (RcIndex != restriction[i]) &&
                (side == "" || side == element.Key)) {
                is_adjacent = true;
                pipe = (Pipe)Event_Manager.TriggerEvent("get_pipe_with_index",
                    new Vector2(RcIndex.x + element.Value.x,
                                RcIndex.y + element.Value.y));
                if (pipe.get_pipe_type() != PipeType.Empty) {
                    adjacentPipes.set_pipe_with_side(element.Key, pipe);
                    if (pipe.adjacentPipes.get_pipe_with_side(adjacentPipes.get_opposite_pipe_string(element.Key)) 
                    != this) {
                        if (!pipe.check_adjacent_pipe(adjacentPipes.get_opposite_pipe_string(element.Key)))
                            adjacentPipes.set_pipe_with_side(element.Key, null);
                    }
                }
            }
            i++;
        }

        return is_adjacent;
    }

    public Pipe next_pipe() {
        if (pipeType == PipeType.Cross &&
            (pipeState == PipeState.HalfFilling ||
             pipeState == PipeState.HalfFilled)) {
            return adjacentPipes.get_pipe_with_side(
                    adjacentPipes.get_opposite_pipe_string(
                        pipeOpenSide.pipe_bool_search(false)
                    )
                );
        } else {
            return adjacentPipes.get_pipe_with_side(
                    pipeOpenSide.pipe_bool_search(true)
                );
        }
    }

    public void finished_animation() {
        if (pipeType == PipeType.Cross &&
            pipeState == PipeState.HalfFilling) {
            pipeState = PipeState.HalfFilled;

            if (crossPipeState == CrossPipeFillState.Horizontal)
                crossPipeState = CrossPipeFillState.Vertical;
            else
                crossPipeState = CrossPipeFillState.Horizontal;
    
        } else {
            pipeState = PipeState.Filled;
        }
        
        if (pipeType == PipeType.Cross &&
            pipeState == PipeState.HalfFilled) {
            next_pipe().start_filling(
                    pipeOpenSide.pipe_bool_search(false)
                );
            pipeOpenSide.set_bool_side(adjacentPipes.get_opposite_pipe_string(
                    pipeOpenSide.pipe_bool_search(false)
                ), false);
        } else {
            next_pipe().start_filling(
                    adjacentPipes.get_opposite_pipe_string(
                        pipeOpenSide.pipe_bool_search(true)
                    )
                );
            pipeOpenSide.set_bool_side(pipeOpenSide.pipe_bool_search(true),
                false);
            
        }

        
    }
}
