using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pipe_Grid_Management : MonoBehaviour
{
    // constant values
    Vector2 DIMESION;
    Vector2 STARTING_COORDS;
    Vector2 MOUSE_X_RESTRICTION;
    Vector2 MOUSE_Y_RESTRICTION;

    // starting pipe
    Sprite StartPipe;
    Vector2 StartPipeCoord;

    // pipe objects
    Pipe[,] pipes;
    Pipe[] connectedPipe;
    int connectPipeIndex;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // delay for values to load
        yield return new WaitForSeconds(0.1f);

        DIMESION = UI_Manager.Instance.get_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_pipe_grid_top_left_coords();
        MOUSE_X_RESTRICTION = UI_Manager.Instance.get_pipe_grid_x_mouse_domain();
        MOUSE_Y_RESTRICTION = UI_Manager.Instance.get_pipe_grid_y_mouse_range();
        StartPipe = UI_Manager.Instance.get_empty_pipe_sprites()[3];

        pipes = new Pipe[(int)DIMESION.x, (int)DIMESION.y];
        connectedPipe = new Pipe[(int)DIMESION.x * (int)DIMESION.y];
        connectPipeIndex = 0;

        generate_empty_grid_with_coords();
        generate_starting_pipe();

        // start timer
        Event_Manager.TriggerEvent("start_timer");
    }

    // Update is called once per frame
    void Update()
    {
        detect_mouse_click_and_position();
        check_connect_pipe_animaton();
    }

    // update functions
    void detect_mouse_click_and_position() {
        // check if left mouse clicked
        Vector2 mouse_pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool is_in_pipe_grid = false;
        bool has_left_click = false;
        bool has_right_click = false;

        // checking mouse click and position
        if (Input.GetMouseButtonDown(0)) {
            has_left_click = true;
        }

        if (Input.GetMouseButtonDown(1)) {
            has_right_click = true;
        }

        if ((MOUSE_X_RESTRICTION.x <= mouse_pos.x &&
             mouse_pos.x <= MOUSE_X_RESTRICTION.y) &&
            (MOUSE_Y_RESTRICTION.x >= mouse_pos.y &&
             mouse_pos.y >= MOUSE_Y_RESTRICTION.y))
        {
            is_in_pipe_grid = true;
        }
        
        if (is_in_pipe_grid && has_left_click) {
            Vector2 pipe_pos;
            pipe_pos = new Vector2(
                (int)Math.Floor((double)(MOUSE_Y_RESTRICTION.x - mouse_pos.y)),
                (int)Math.Floor((double)(mouse_pos.x - MOUSE_X_RESTRICTION.x))
            );

            if (((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_type() == PipeType.Empty) {
                place_down_pipe(pipe_pos);
            }
        }

        if (is_in_pipe_grid && has_right_click) {
            Vector2 pipe_pos;
            pipe_pos = new Vector2(
                (int)Math.Floor((double)(MOUSE_Y_RESTRICTION.x - mouse_pos.y)),
                (int)Math.Floor((double)(mouse_pos.x - MOUSE_X_RESTRICTION.x))
            );

            if (((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_state() == PipeState.Empty &&
                ((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_type() != PipeType.Start) {
                delete_pipe(pipe_pos);
            }
        }
    }

    void check_connect_pipe_animaton() {
        int time = (int)Event_Manager.TriggerEvent("get_time");

        if (time == 10) {
            pipes[(int)StartPipeCoord.x, (int)StartPipeCoord.y].start_filling();
        }

        if (time > 10) {
            if (connectedPipe[connectPipeIndex].check_animation_is_finished()) {
                if (connectedPipe[connectPipeIndex].next_pipe() != null) {
                    connectedPipe[connectPipeIndex + 1] =
                        connectedPipe[connectPipeIndex].next_pipe();
                    connectedPipe[connectPipeIndex].finished_animation();
                    connectPipeIndex++;
                } else {
                    Time.timeScale = 0;
                }
            }
        }
    }

    // generation
    void generate_empty_grid_with_coords() {
        //load empty grid
        GameObject RefTile = (GameObject)Instantiate(Resources.Load("Pipe Sprite"));

        for (int r = 0; r < DIMESION.x; r++) {
            for (int c = 0; c < DIMESION.y; c++) {
                GameObject Pipe = (GameObject)Instantiate(RefTile, transform);

                Pipe.name = "Pipe R" + r.ToString() +
                            "C" + c.ToString();
                pipes[r, c] = new Pipe(ref Pipe,
                                       STARTING_COORDS.x + c, 
                                       STARTING_COORDS.y - r);
            }
        }

        Destroy(RefTile);
    }

    void generate_starting_pipe() {
        // generate position
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
        int start_row = rnd.Next(0, (int)DIMESION.x);
        int start_col = rnd.Next(0, (int)DIMESION.y);
        StartPipeCoord = new Vector2(start_row, start_col);

        // generate pipe data
        PipeData pipe_data = new PipeData();

        // state
        pipe_data.pipeType = PipeType.Start;

        // sprite
        pipe_data.PipeSprite = StartPipe;

        // generate rotation data
        int[] possible_rotation = new int[4];
        int i = 0;
        Dictionary<string, int> StartRotateRule = 
                    new Dictionary<string, int>() {
                        {"r0", 2},
                        {"r6", 0},
                        {"c0", 1},
                        {"c10", 3}
                    };

        string start_row_string = "r" + start_row.ToString();
        string start_col_string = "c" + start_col.ToString();

        foreach(KeyValuePair<string, int> element in StartRotateRule) {
            if (element.Key != start_row_string && 
                element.Key != start_col_string) {
                possible_rotation[i++] = element.Value;
            }
        }

        int rnd_index = rnd.Next(0, i);
        int num_of_rotation = possible_rotation[rnd_index];
        pipe_data.rotationTimes = num_of_rotation;

        // generate open sides
        bool[] sides = new bool[4] {false, false, false, false};
        sides[(num_of_rotation + 1) % 4] = true;

        pipe_data.boolPipeSide = new BoolPipeSide(sides);
        pipe_data.PipeIndex = new Vector2(start_row, start_col);
        

        // place the pipe
        pipes[start_row, start_col].change_pipe_data(pipe_data);
        connectedPipe[connectPipeIndex] = pipes[start_row, start_col];
    }

    System.Object place_down_pipe(Vector2 pipe_pos) {
        Pipe first_random_grid_pipe =
            (Pipe)Event_Manager.TriggerEvent("send_front_pipe");

        // extract pipe data from front pipe
        PipeData pipe_data = new PipeData();

        pipe_data = first_random_grid_pipe.get_pipe_data();
        pipe_data.isInGrid = "yes";
        pipe_data.PipeIndex = pipe_pos;

        ((Pipe)get_pipe_with_index(pipe_pos)).change_pipe_data(pipe_data);

        return null;
    }

    void delete_pipe(Vector2 pipe_pos) {
        GameObject RefTile = (GameObject)Instantiate(Resources.Load("Pipe Sprite"));

        int r = (int)pipe_pos.y;
        int c = (int)pipe_pos.x;
        GameObject Pipe = (GameObject)Instantiate(RefTile, transform);

        Pipe.name = "Pipe R" + r.ToString() +
                    "C" + c.ToString();
        pipes[r, c].destroy_gameObject();
        pipes[r, c] = new Pipe(ref Pipe,
                               STARTING_COORDS.x + c, 
                               STARTING_COORDS.y - r);

        Destroy(RefTile);
    }

    // set up listener for communication
    Func<System.Object, System.Object> PipeGridListener;

    void Awake() {
        PipeGridListener = new Func<System.Object, System.Object>(get_pipe_with_index);
    }

    void OnEnable()
    {
        Event_Manager.StartListening("get_pipe_with_index", PipeGridListener);
    }

    void OnDisable()
    {
        Event_Manager.StopListening("get_pipe_with_index", PipeGridListener);
    }

    // listener function
    public System.Object get_pipe_with_index(System.Object index) {
        Vector2 _index = (Vector2)index;
        return pipes[(int)_index.x, (int)_index.y];
    }
}
