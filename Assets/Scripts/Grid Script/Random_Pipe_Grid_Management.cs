using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Random_Pipe_Grid_Management : MonoBehaviour
{
    private int NUM_OF_PIPE_DISPLAY;
    private Vector2 DIMESION;
    private Vector2 STARTING_COORDS;

    // pipe objects act like queue
    Pipe[] pipes;
    private Sprite[] PipeEmptySprite;

    // setup listener to send first pipe
    private Func<System.Object, System.Object> RandPipeGridListener;

    void Awake() {
        RandPipeGridListener = new Func<System.Object, System.Object>(send_front_pipe);
    }

    void OnEnable()
    {
        Event_Manager.StartListening("send_front_pipe", RandPipeGridListener);
    }

    void OnDisable()
    {
        Event_Manager.StopListening("send_front_pipe", RandPipeGridListener);
    }


    // Start is called before the first frame update
    IEnumerator Start()
    {
        // delay for values to load
        yield return new WaitForSeconds(0.1f);

        DIMESION = UI_Manager.Instance.get_random_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_random_pipe_grid_top_left_coords();
        PipeEmptySprite = UI_Manager.Instance.get_empty_pipe_sprites();

        NUM_OF_PIPE_DISPLAY = (int)DIMESION.x;
        pipes = new Pipe[NUM_OF_PIPE_DISPLAY];

        generate_empty_grid_with_coords();
    }

    // Update is called once per frame
    void Update()
    {
        check_first_pipe_is_destroyed();
    }

    void generate_empty_grid_with_coords() {
        //load empty grid
        GameObject RefTile = (GameObject)Instantiate(Resources.Load("Pipe Sprite"));

        for (int r = 0; r < NUM_OF_PIPE_DISPLAY; r++) {
            GameObject Pipe = (GameObject)Instantiate(RefTile, transform);
            
            Pipe.name = "Random Pipe R" + r.ToString();
            pipes[r] = new Pipe(ref Pipe, 
                                generate_random_pipe_data(),
                                STARTING_COORDS.x,
                                STARTING_COORDS.y + r);
        }

        Destroy(RefTile);
    }

    PipeData generate_random_pipe_data() {
        PipeData pipe_data = new PipeData();

        // generate pipe type
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
        PipeType pipe_type = (PipeType)rnd.Next(0, 3);
        pipe_data.pipeType = pipe_type;
        
        // generate image path
        int pipe_type_num = (int)pipe_type;
        pipe_data.PipeSprite = PipeEmptySprite[pipe_type_num];

        // generate rotation data
        int num_of_rotation = 0;
        switch(pipe_type) {
            case PipeType.Straight:
                num_of_rotation = rnd.Next(0, 2);
                break;
            
            case PipeType.Curved:
                num_of_rotation = rnd.Next(0, 4);
                break;
            
            default:
                break;
        }
        pipe_data.rotationTimes = num_of_rotation;

        
        // generate open side base on rotation
        pipe_data.curvedPipeSide = "";
        bool[] sides = new bool[4];
        switch(pipe_type) {
            case PipeType.Straight:
                if (num_of_rotation == 1)
                    sides = new bool[4] {true, false, true, false};
                else
                    sides = new bool[4] {false, true, false, true};

                break;
            
            case PipeType.Curved:
                string[] possible_sides = new string[4] {"R", "D", "L", "U"};
                string open_side = "";
                sides = new bool[4] {true, true, false, false};

                bool tmp;
                // array rotate to right
                for (int k = 0; k < num_of_rotation; k++) {
                    tmp = sides[3];
                    for (int l = 3; l > 0; l--)
                        sides[l] = sides[l - 1];
                    sides[0] = tmp;
                }
                int m = 0;
                foreach (string elem in possible_sides) {
                    if (sides[m])
                        open_side += elem;
                    m++;
                }
                pipe_data.curvedPipeSide = open_side;
                break;
            
            case PipeType.Cross:
                sides = new bool[4] {true, true, true, true};
                
                break;

            default:
                break;
        }
        pipe_data.boolPipeSide = new BoolPipeSide(sides);

        pipe_data.isInGrid = "no";
        pipe_data.PipeIndex = new Vector2(0, 0);

        return pipe_data;
    }
    System.Object send_front_pipe(System.Object p) {
        Pipe first_pipe = pipes[0];
        // first destroy gameObject
        pipes[0].destroy_gameObject();

        return first_pipe;
    }

    void check_first_pipe_is_destroyed() {
        try {
            if (pipes[0].get_pipe_state() == PipeState.Destroyed)
                pop_front_pipe();
        } catch {}
    }

    void pop_front_pipe() {
        
        for (int i = 0; i < NUM_OF_PIPE_DISPLAY - 1; i++) {
            pipes[i] = pipes[i + 1]; // shift pipe array down
            pipes[i].set_gameObject_name("Random Pipe R" + i.ToString());
        }

        GameObject RefTile = (GameObject)Instantiate(Resources.Load("Pipe Sprite"));
        GameObject Pipe = (GameObject)Instantiate(RefTile, transform);

        int pipe_index = NUM_OF_PIPE_DISPLAY - 1;

        Pipe.name = "Random Pipe R" + pipe_index.ToString();
        pipes[pipe_index] = new Pipe(ref Pipe, 
                            generate_random_pipe_data(),
                            STARTING_COORDS.x,
                            STARTING_COORDS.y + pipe_index);

        Destroy(RefTile);

        // shift the position of pipes down
        for (int i = 0; i < NUM_OF_PIPE_DISPLAY - 1; i++) {
            Vector2 pipe_pos = pipes[i].get_gameObject_position();
            pipe_pos.y -= 1;

            pipes[i].set_gameObject_position(pipe_pos);
        }
    }
}
