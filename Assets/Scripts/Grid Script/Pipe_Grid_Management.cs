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

    // game states
    bool gameOver;
    bool passedLevel;
    int start_time;
    int pipe_distance;

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
        gameOver = false;
        passedLevel = false;
        start_time = Math.Max(15 - ((int)Event_Manager.TriggerEvent("send_level") / 5), 5);
        pipe_distance = Math.Min(12, 2 + ((int)Event_Manager.TriggerEvent("send_level") / 4));

        generate_empty_grid_with_coords();
        generate_starting_and_ending_pipe();

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
        
        if (is_in_pipe_grid && has_left_click && !gameOver) {
            Vector2 pipe_pos;
            pipe_pos = new Vector2(
                (int)Math.Floor((double)(MOUSE_Y_RESTRICTION.x - mouse_pos.y)),
                (int)Math.Floor((double)(mouse_pos.x - MOUSE_X_RESTRICTION.x))
            );

            if (((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_type() == PipeType.Empty) {
                place_down_pipe(pipe_pos);
                Audio_Manager.Instance.Play("Pipe Place");
            }
        }

        if (is_in_pipe_grid && has_right_click && !gameOver) {
            Vector2 pipe_pos;
            pipe_pos = new Vector2(
                (int)Math.Floor((double)(MOUSE_Y_RESTRICTION.x - mouse_pos.y)),
                (int)Math.Floor((double)(mouse_pos.x - MOUSE_X_RESTRICTION.x))
            );

            if (((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_state() == PipeState.Empty &&
                ((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_type() != PipeType.Start &&
                ((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_type() != PipeType.End &&
                ((Pipe)get_pipe_with_index(pipe_pos)).get_pipe_type() != PipeType.Empty &&
                ((int)Event_Manager.TriggerEvent("send_score") > 0)) {
                delete_pipe(pipe_pos);
                Audio_Manager.Instance.Play("Pipe Break");
            }
        }
    }

    void check_connect_pipe_animaton() {
        int time = (int)Event_Manager.TriggerEvent("get_time");
        try {
            if (time == start_time) {
                pipes[(int)StartPipeCoord.x, (int)StartPipeCoord.y].start_filling();
                Audio_Manager.Instance.Play("Water Flowing");
            }
        } catch {}

        if (gameOver) return;

        if (time > start_time) {
            if (connectedPipe[connectPipeIndex].check_animation_is_finished()) {
                if (connectedPipe[connectPipeIndex].next_pipe() != null) {
                    if (connectedPipe[connectPipeIndex].next_pipe().get_pipe_type() == PipeType.End) {
                        passedLevel = true;
                    }
                    connectedPipe[connectPipeIndex + 1] =
                        connectedPipe[connectPipeIndex].next_pipe();
                    connectedPipe[connectPipeIndex].finished_animation();
                    connectPipeIndex++;
                    Event_Manager.TriggerEvent("add_score_to_scoreboard", 10);
                } else if (passedLevel) {
                    Audio_Manager.Instance.Stop("Water Flowing");
                    Event_Manager.TriggerEvent("go_to_next_level");
                } else {
                    Audio_Manager.Instance.Stop("Water Flowing");
                    gameOver = true;
                    Event_Manager.TriggerEvent("stop_timer");
                    Event_Manager.TriggerEvent("remove_a_live");
                    Level_Manager.Instance.ReloadCurrentScene();
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

    void generate_starting_and_ending_pipe() {
        // generate position
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
        Vector2 EndPipeCoord;
        int start_row, start_col, end_row, end_col;
        do {
            start_row = rnd.Next(0, (int)DIMESION.x);
            start_col = rnd.Next(0, (int)DIMESION.y);
            end_row = rnd.Next(0, (int)DIMESION.x);
            end_col = rnd.Next(0, (int)DIMESION.y);

            // make sure coords are the same
            while(start_row == end_row && start_col == end_col) {
                end_row = rnd.Next(0, (int)DIMESION.x);
                end_col = rnd.Next(0, (int)DIMESION.y);
            }
            StartPipeCoord = new Vector2(start_row, start_col);
            EndPipeCoord = new Vector2(end_row, end_col);
        } while(Vector2.Distance(StartPipeCoord, EndPipeCoord) >= pipe_distance);

        // generate pipe data
        PipeData start_pipe_data = new PipeData();
        PipeData end_pipe_data = new PipeData();

        // state
        start_pipe_data.pipeType = PipeType.Start;
        end_pipe_data.pipeType = PipeType.End;

        // sprite
        start_pipe_data.PipeSprite = StartPipe;
        end_pipe_data.PipeSprite = StartPipe; // they look the same

        // generate rotation data
        int[] start_possible_rotation = new int[4];
        int[] end_possible_rotation = new int[4];
        int i = 0;
        int j = 0;
        PipeUtil util = new PipeUtil();
        Dictionary<string, int> RotateRule = 
            new Dictionary<string, int>() {
                {"r0", 2}, // top restriction
                {"r6", 0}, // bottom
                {"c0", 1}, // left
                {"c10", 3} // right
            };
        
        Dictionary<int, string> ForbidRotateRule =
            new Dictionary<int, string>() {
                {2, "U"}, // top restriction
                {0, "D"}, // bottom
                {1, "L"}, // left
                {3, "R"}  // right
            };

        string start_row_string = "r" + start_row.ToString();
        string start_col_string = "c" + start_col.ToString();
        string end_row_string = "r" + end_row.ToString();
        string end_col_string = "c" + end_col.ToString();
        string start_found_end = "";

        foreach(KeyValuePair<string, Vector2> element in util.search_coord_list) {
            if (start_row + element.Value.x == end_row &&
                start_col + element.Value.y == end_col) {
                start_found_end = element.Key;
            }
        }

        foreach(KeyValuePair<string, int> element in RotateRule) {
            if (element.Key != start_row_string && 
                element.Key != start_col_string &&
                ForbidRotateRule[element.Value] != start_found_end) {
                start_possible_rotation[i++] = element.Value;
            }

            if (element.Key != end_row_string && 
                element.Key != end_col_string &&
                ForbidRotateRule[element.Value] != util.opposite_side_list[start_found_end]) {
                end_possible_rotation[j++] = element.Value;
            }
        }

        int start_rnd_index = rnd.Next(0, i);
        int start_num_of_rotation = start_possible_rotation[start_rnd_index];
        start_pipe_data.rotationTimes = start_num_of_rotation;

        int end_rnd_index = rnd.Next(0, j);
        int end_num_of_rotation = end_possible_rotation[end_rnd_index];
        end_pipe_data.rotationTimes = end_num_of_rotation;

        // generate open sides
        bool[] start_sides = new bool[4] {false, false, false, false};
        start_sides[(start_num_of_rotation + 1) % 4] = true;
        bool[] end_sides = new bool[4] {false, false, false, false};
        end_sides[(end_num_of_rotation + 1) % 4] = true;

        start_pipe_data.boolPipeSide = new BoolPipeSide(start_sides);
        start_pipe_data.PipeIndex = new Vector2(start_row, start_col);
        end_pipe_data.boolPipeSide = new BoolPipeSide(end_sides);
        end_pipe_data.PipeIndex = new Vector2(end_row, end_col);


        // place the pipe
        pipes[start_row, start_col].change_pipe_data(start_pipe_data);
        pipes[end_row, end_col].change_pipe_data(end_pipe_data);
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

        int r = (int)pipe_pos.x;
        int c = (int)pipe_pos.y;
        GameObject Pipe = (GameObject)Instantiate(RefTile, transform);

        Pipe.name = "Pipe R" + r.ToString() +
                    "C" + c.ToString();
        pipes[r, c].destroy_gameObject();
        pipes[r, c] = new Pipe(ref Pipe,
                               STARTING_COORDS.x + c, 
                               STARTING_COORDS.y - r);

        Destroy(RefTile);
        Event_Manager.TriggerEvent("subtract_score_to_scoreboard", 3);
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
