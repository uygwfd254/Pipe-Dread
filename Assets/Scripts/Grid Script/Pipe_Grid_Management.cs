using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pipe_Grid_Management : MonoBehaviour
{
    private Vector2 DIMESION;
    private Vector2 STARTING_COORDS;
    private Vector2 MOUSE_X_RESTRICTION;
    private Vector2 MOUSE_Y_RESTRICTION;

    private Sprite StartPipe;
    private Vector2 StartPipeCoord;

    // pipe objects
    private Pipe[,] pipes;

    private int current_time = 0; // start at 0

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // delay for values to load
        yield return new WaitForSeconds(0.1f);

        DIMESION = UI_Manager.Instance.get_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_starting_pipe_grid_coords();
        MOUSE_X_RESTRICTION = UI_Manager.Instance.get_pipe_grid_x_mouse_domain();
        MOUSE_Y_RESTRICTION = UI_Manager.Instance.get_pipe_grid_y_mouse_range();

        StartPipe = UI_Manager.Instance.get_empty_pipe_sprites()[3]; 

        pipes = new Pipe[(int)DIMESION.x, (int)DIMESION.y];

        generate_empty_grid_with_coords();
        generate_starting_pipe();
        
    }

    // Update is called once per frame
    void Update()
    {
        detect_mouse_click_and_position();
        update_time();
        is_it_time_to_start();
    }

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
        System.Object[] pipe_data;
        pipe_data = new System.Object[10];
        int i = 0;

        // state
        pipe_data[i++] = PipeType.Start;

        // sprite
        pipe_data[i++] = StartPipe;

        // generate rotation data
        int[] possible_rotation = new int[4];
        int j = 0;
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
                possible_rotation[j++] = element.Value;
            }
        }

        int rnd_index = rnd.Next(0, j);
        pipe_data[i++] = possible_rotation[rnd_index];

        // place the pipe
        pipes[start_row, start_col].change_pipe_data(pipe_data);
    }
    
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
                (int)Math.Floor((double)(mouse_pos.x - MOUSE_X_RESTRICTION.x)),
                (int)Math.Floor((double)(MOUSE_Y_RESTRICTION.x - mouse_pos.y))
            );

            if (pipes[(int)pipe_pos.y, (int)pipe_pos.x].get_pipe_type() == PipeType.Empty) {
                place_down_pipe(pipe_pos);
            }
        }

        if (is_in_pipe_grid && has_right_click) {
            Vector2 pipe_pos;
            pipe_pos = new Vector2(
                (int)Math.Floor((double)(mouse_pos.x - MOUSE_X_RESTRICTION.x)),
                (int)Math.Floor((double)(MOUSE_Y_RESTRICTION.x - mouse_pos.y))
            );

            delete_pipe();
        }
    }

    void update_time() {
        int time = (int)Event_Manager.TriggerEvent("get_time");
        current_time = time;
    }

    System.Object place_down_pipe(Vector2 pipe_pos) {
        Pipe first_random_grid_pipe =
            (Pipe)Event_Manager.TriggerEvent("send_front_pipe");

        // extract pipe data from front pipe
        System.Object[] pipe_data = new System.Object[10];
        pipe_data = first_random_grid_pipe.get_pipe_data();

        pipes[(int)pipe_pos.y, (int)pipe_pos.x].change_pipe_data(pipe_data);

        return null;
    }

    void is_it_time_to_start() {
        if (current_time == 10) {
            pipes[(int)StartPipeCoord.x, (int)StartPipeCoord.y].start_filling();
        }
    }

    void delete_pipe() {
        
    }
}
