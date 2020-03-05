using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : Singleton<UI_Manager>
{
    // needed for singleton
    protected UI_Manager() {}

    // objects
    RectTransform ParentRectT;
    RectTransform RandomPipeGridRectT;
    RectTransform PipeGridRectT;
    RectTransform ScoreBoardRectT;
    
    // object values
    private float parent_scale = 0f;
    private Vector2 parent_width_height;
    private Vector2 random_pipe_grid_width_height;

    // const value
    private const int RANDOM_PIPE_GRID_ROW = 5;
    private const int RANDOM_PIPE_GRID_COL = 1;

    void Start()
    {
        // objects declaration
        ParentRectT = GetComponent<RectTransform>();
        RandomPipeGridRectT = transform.Find("Random Pipe Grid").GetComponent<RectTransform>();
        PipeGridRectT = transform.Find("Pipe Grid").GetComponent<RectTransform>();
        ScoreBoardRectT = transform.Find("Score Board").GetComponent<RectTransform>();

        parent_scale = transform.localScale.x;
    }

    void OnGUI()
    {
        parent_width_height = new Vector2(ParentRectT.rect.width,
                                          ParentRectT.rect.height);

        random_pipe_grid_width_height = new Vector2(RandomPipeGridRectT.rect.width,
                                                    RandomPipeGridRectT.rect.height);
    }

    Vector2 get_starting_random_pipe_grid_coords() {
        Vector2 starting_coords = new Vector2(0f, 0f);

        // formula application
        starting_coords.x = - (random_pipe_grid_width_height.x / 2);
        starting_coords.y = (4 * parent_width_height.y - 
                             random_pipe_grid_width_height.y) / 10;
        
        return starting_coords;
    }
}
