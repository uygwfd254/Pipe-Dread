using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe_Grid_Management : MonoBehaviour
{
    private Vector2 DIMESION;
    private Vector2 STARTING_COORDS;
    private float TILE_SIZE;

    // pipe objects
    Pipe[,] pipes;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        // delay for values to load
        yield return new WaitForSeconds(0.1f);

        DIMESION = UI_Manager.Instance.get_pipe_grid_dimension();
        STARTING_COORDS = UI_Manager.Instance.get_starting_pipe_grid_coords();

        TILE_SIZE = UI_Manager.Instance.tile_size;
        pipes = new Pipe[(int)DIMESION.x, (int)DIMESION.y];

        generate_empty_grid_with_coords();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                                       STARTING_COORDS.x + (c * TILE_SIZE), 
                                       STARTING_COORDS.y - (r * TILE_SIZE));
            }
        }

        Destroy(RefTile);
    }
    
}
