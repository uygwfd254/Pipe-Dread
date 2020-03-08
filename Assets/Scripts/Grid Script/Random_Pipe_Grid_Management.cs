using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_Pipe_Grid_Management : MonoBehaviour
{
    private int NUM_OF_PIPE_DISPLAY;
    private Vector2 DIMESION;
    private Vector2 STARTING_COORDS;
    private float TILE_SIZE;

    // pipe objects act like queue
    Pipe[] pipes;
    private Sprite[] PipeEmptySprite;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // delay for values to load
        yield return new WaitForSeconds(0.1f);

        DIMESION = UI_Manager.Instance.get_random_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_starting_random_pipe_grid_coords();
        PipeEmptySprite = UI_Manager.Instance.get_empty_pipe_sprites();

        TILE_SIZE = UI_Manager.Instance.tile_size;
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
                                STARTING_COORDS.y + (r * TILE_SIZE));
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
        PipeType pipe_type = (PipeType)rnd.Next(1, 4);
        pipe_data[i++] = pipe_type;
        
        // generate image path
        int pipe_type_num = (int)pipe_type;
        pipe_data[i++] = PipeEmptySprite[pipe_type_num - 1];

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

}
