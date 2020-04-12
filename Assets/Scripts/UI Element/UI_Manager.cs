using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : Singleton<UI_Manager>
{
    // needed for singleton
    protected UI_Manager() {}

    // RectTransform objects
    RectTransform ParentRT;
    RectTransform RandomPipeGridRT;
    RectTransform PipeGridRT;
    RectTransform ScoreBoardRT;

    // sprites
    public Sprite PipeStraight;
    public Sprite PipeCurve;
    public Sprite PipeCross;
    public Sprite PipeStart;
    
    // scale, width and height
    float parentScale;
    Vector2 parentWidthHeight;
    Vector2 randomPipeGridWidthHeight;
    Vector2 pipeGridWidthHeight;

    // constant value
    const int RANDOM_PIPE_GRID_ROW = 5;
    const int RANDOM_PIPE_GRID_COL = 1;
    const int PIPE_GRID_ROW = 7;
    const int PIPE_GRID_COL = 11;
    const float TILE_SIZE = 1f;

    void Start()
    {
        // objects declaration
        ParentRT = GetComponent<RectTransform>();
        RandomPipeGridRT = transform.Find("Random Pipe Grid").GetComponent<RectTransform>();
        PipeGridRT = transform.Find("Pipe Grid").GetComponent<RectTransform>();
        ScoreBoardRT = transform.Find("Score Board").GetComponent<RectTransform>();

        parentScale = transform.localScale.x;
        parentWidthHeight = new Vector2(ParentRT.rect.width,
                                        ParentRT.rect.height);
        randomPipeGridWidthHeight = new Vector2(-RandomPipeGridRT.offsetMax.x,
                                                RandomPipeGridRT.offsetMin.y);
        pipeGridWidthHeight = new Vector2(PipeGridRT.offsetMin.x,
                                          -PipeGridRT.offsetMax.y);
    }

    // accessors
    // random pipe grid
    public Vector2 get_random_pipe_grid_top_left_coords()
    {
        Vector2 starting_coords = new Vector2(0f, 0f);

        // formula application
        starting_coords.x = - parentScale * ((randomPipeGridWidthHeight.x) / 2);
        starting_coords.y = parentScale * ((randomPipeGridWidthHeight.y) / 2) -
                                             (int)Math.Floor((double)(RANDOM_PIPE_GRID_ROW / 2));

        return starting_coords;
    }

    public Vector2 get_random_pipe_grid_dimension()
    {
        return new Vector2(RANDOM_PIPE_GRID_ROW, RANDOM_PIPE_GRID_COL);
    }

    // pipe grid
    public Vector2 get_pipe_grid_top_left_coords() {
        Vector2 starting_coords = new Vector2(0f, 0f);

        // formula application
        starting_coords.x = parentScale * ((pipeGridWidthHeight.x) / 2) -
                                           (int)Math.Floor((double)(PIPE_GRID_COL / 2));
        starting_coords.y = - parentScale * ((pipeGridWidthHeight.y) / 2) +
                                             (int)Math.Floor((double)(PIPE_GRID_ROW / 2));

        return starting_coords;
    }

    public Vector2 get_pipe_grid_dimension() {
        return new Vector2(PIPE_GRID_ROW, PIPE_GRID_COL);
    }

    public Vector2 get_pipe_grid_x_mouse_domain() {
        Vector2 starting_coords = get_pipe_grid_top_left_coords();
        return new Vector2(starting_coords.x - TILE_SIZE / 2,
                           starting_coords.x + PIPE_GRID_COL - TILE_SIZE / 2);
    }

    public Vector2 get_pipe_grid_y_mouse_range() {
        Vector2 starting_coords = get_pipe_grid_top_left_coords();
        return new Vector2(starting_coords.y + TILE_SIZE / 2,
                           starting_coords.y - PIPE_GRID_ROW + TILE_SIZE / 2);
    }

    // sprites
    public Sprite[] get_empty_pipe_sprites() {
        Sprite[] PipeSprite = new Sprite[4];
        // append to sprite array
        PipeSprite[0] = PipeStraight;
        PipeSprite[1] = PipeCurve;
        PipeSprite[2] = PipeCross;
        PipeSprite[3] = PipeStart;

        return PipeSprite;
    }
}
