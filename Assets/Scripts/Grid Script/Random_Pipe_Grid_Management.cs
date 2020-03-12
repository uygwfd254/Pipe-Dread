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
    private UnityAction<Event_Params> PipeGridListener;

    void Awake() {
        PipeGridListener = new UnityAction<Event_Params>(send_front_pipe);
    }

    void OnEnable()
    {
        Event_Manager.StartListening("send_front_pipe", PipeGridListener);
    }

    void OnDisable()
    {
        Event_Manager.StopListening("send_front_pipe", PipeGridListener);
    }


    // Start is called before the first frame update
    IEnumerator Start()
    {
        // delay for values to load
        yield return new WaitForSeconds(0.1f);

        DIMESION = UI_Manager.Instance.get_random_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_starting_random_pipe_grid_coords();
        PipeEmptySprite = UI_Manager.Instance.get_empty_pipe_sprites();

        NUM_OF_PIPE_DISPLAY = (int)DIMESION.x;
        pipes = new Pipe[NUM_OF_PIPE_DISPLAY];

        generate_empty_grid_with_coords();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void generate_empty_grid_with_coords() {
        //load empty grid
        GameObject RefTile = (GameObject)Instantiate(Resources.Load("Pipe Sprite"));

        for (int r = 0; r < NUM_OF_PIPE_DISPLAY; r++) {
            GameObject Pipe = (GameObject)Instantiate(RefTile, transform);
            System.Object[] pipe_data = generate_random_pipe_data();
            
            Pipe.name = "Random Pipe R" + r.ToString();
            pipes[r] = new Pipe(ref Pipe, 
                                pipe_data, STARTING_COORDS.x,
                                STARTING_COORDS.y + r);
        }

        Destroy(RefTile);
    }

    System.Object[] generate_random_pipe_data() {
        System.Object[] pipe_data;
        pipe_data = new System.Object[10];
        int i = 0;

        // generate pipe type
        // load better randomizer
        System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
        PipeType pipe_type = (PipeType)rnd.Next(0, 3);
        pipe_data[i++] = pipe_type;
        
        // generate image path
        int pipe_type_num = (int)pipe_type;
        pipe_data[i++] = PipeEmptySprite[pipe_type_num];

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
        pipe_data[i++] = num_of_rotation;

        return pipe_data;
    }
    void send_front_pipe(Event_Params p) {
        if (p.bool_param) {
            Event_Params parameter = new Event_Params();
            parameter.pipe_param = pipes[0];
            Event_Manager.TriggerEvent("get_first_random_grid_pipe", parameter);
            pop_front_pipe();
        }
    }

    void pop_front_pipe() {
        // first destroy gameObject
        pipes[0].destroy_gameObject();
        
        for (int i = 0; i < NUM_OF_PIPE_DISPLAY - 1; i++) {
            pipes[i] = pipes[i + 1]; // shift pipe array down
            pipes[i].set_gameObject_name("Random Pipe R" + i.ToString());
        }

        GameObject RefTile = (GameObject)Instantiate(Resources.Load("Pipe Sprite"));
        GameObject Pipe = (GameObject)Instantiate(RefTile, transform);
        System.Object[] pipe_data = generate_random_pipe_data();

        int pipe_index = NUM_OF_PIPE_DISPLAY - 1;

        Pipe.name = "Random Pipe R" + pipe_index.ToString();
        pipes[pipe_index] = new Pipe(ref Pipe, 
                            pipe_data, STARTING_COORDS.x,
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
