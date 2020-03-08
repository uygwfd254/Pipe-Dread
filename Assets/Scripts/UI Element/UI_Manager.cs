using System;
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
    // sprites
    public Sprite PipeStraight;
    public Sprite PipeCurve;
    public Sprite PipeCross;
    public Sprite PipeStart;
    
    // object values
    [HideInInspector]
    public float tile_size = 1f;
    private float parent_scale;
    private Vector2 parent_width_height;
    private Vector2 random_pipe_grid_width_height;
    private Vector2 pipe_grid_width_height;

    // const value
    private const int RANDOM_PIPE_GRID_ROW = 5;
    private const int RANDOM_PIPE_GRID_COL = 1;
    private const int PIPE_GRID_ROW = 7;
    private const int PIPE_GRID_COL = 11;

    void Start()
    {
        // objects declaration
        ParentRectT = GetComponent<RectTransform>();
        RandomPipeGridRectT = transform.Find("Random Pipe Grid").GetComponent<RectTransform>();
        PipeGridRectT = transform.Find("Pipe Grid").GetComponent<RectTransform>();
        ScoreBoardRectT = transform.Find("Score Board").GetComponent<RectTransform>();

        parent_scale = transform.localScale.x;
        parent_width_height = new Vector2(ParentRectT.rect.width,
                                          ParentRectT.rect.height);
        random_pipe_grid_width_height = new Vector2(- RandomPipeGridRectT.offsetMax.x,
                                                    RandomPipeGridRectT.offsetMin.y);
        pipe_grid_width_height = new Vector2(PipeGridRectT.offsetMin.x,
                                             -PipeGridRectT.offsetMax.y);
    }

    public Vector2 get_starting_random_pipe_grid_coords()
    {
        Vector2 starting_coords = new Vector2(0f, 0f);

        // formula application
        starting_coords.x = - parent_scale * ((random_pipe_grid_width_height.x) / 2);
        
        starting_coords.y = parent_scale * ((random_pipe_grid_width_height.y) / 2) -
                                             (int)Math.Floor((double)(RANDOM_PIPE_GRID_ROW / 2));

        return starting_coords;
    }

    public Vector2 get_random_pipe_grid_dimension()
    {
        return new Vector2(RANDOM_PIPE_GRID_ROW, RANDOM_PIPE_GRID_COL);
    }

    public Vector2 get_starting_pipe_grid_coords() {
        Vector2 starting_coords = new Vector2(0f, 0f);

        // formula application
        starting_coords.x = parent_scale * ((pipe_grid_width_height.x) / 2) -
                                               (int)Math.Floor((double)(PIPE_GRID_COL / 2));
        
        starting_coords.y = - parent_scale * ((pipe_grid_width_height.y) / 2) +
                                             (int)Math.Floor((double)(PIPE_GRID_ROW / 2));

        Debug.Log(starting_coords);
        return starting_coords;
    }

    public Vector2 get_pipe_grid_dimension() {
        return new Vector2(PIPE_GRID_ROW, PIPE_GRID_COL);
    }

    public Sprite[] get_empty_pipe_sprites() {
        Sprite[] PipeSprite = new Sprite[3];
        // append to sprite array
        PipeSprite[0] = PipeStraight;
        PipeSprite[1] = PipeCurve;
        PipeSprite[2] = PipeCross;

        return PipeSprite;
    }
}
