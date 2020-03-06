using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_Pipe_Grid_Management : MonoBehaviour
{
    private int NUM_OF_PIPE_DISPLAY;
    private Vector2 DIMESION;
    private Vector2 STARTING_COORDS;
    private float TILE_SIZE;

    // pipe objects
    Pipe[] pipes;
    //GameObject[] PipeSprite;
    

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // delay for values to load

        DIMESION = UI_Manager.Instance.get_random_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_starting_random_pipe_grid_coords();

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

        for (int i = 0; i < NUM_OF_PIPE_DISPLAY; i++) {
            GameObject Pipe = (GameObject)Instantiate(RefTile, transform);
            
            pipes[i] = new Pipe(ref Pipe, STARTING_COORDS.x, STARTING_COORDS.y - i * (TILE_SIZE));
        }

        Destroy(RefTile);
    }
}
