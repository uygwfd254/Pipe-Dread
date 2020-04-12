using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PipeType {
    // also pipe sprite index
    Straight = 0,
    Curved = 1,
    Cross = 2,
    Start = 3,
    Empty = 4
}

[System.Serializable]
public enum PipeState {
    Empty,
    Filling,
    HalfFilling,
    HalfFilled,
    Filled,
    Destroyed
}

[System.Serializable]
public class BoolPipeSide {
    Dictionary<string, bool> OpenSides;

    // bool r, bool d, bool l, bool u
    public BoolPipeSide(bool[] sides) {
        OpenSides = new Dictionary<string, bool>() {
            {"R", false},
            {"D", false},
            {"L", false},
            {"U", false}
        };
        // update the dictionary with new sides
        int i = 0;
        Dictionary<string, bool> Temp = new Dictionary<string, bool>();
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            Temp.Add(element.Key, sides[i++]);
        }

        OpenSides = Temp;
    }

    public bool get_bool_side(string side) {
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            if (side == element.Key)
                return element.Value;
        }

        return false;
    }

    public void set_bool_side(string side, bool bool_) {
        Dictionary<string, bool> Temp = new Dictionary<string, bool>();
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            if (element.Key == side) {
                Temp.Add(element.Key, bool_);
            } else {
                Temp.Add(element.Key, element.Value);
            }
        }

        OpenSides = Temp;
    }

    public string pipe_bool_search(bool target) {
        foreach(KeyValuePair<string, bool> element in OpenSides) {
            if (element.Value == target)
                return element.Key;
        }

        return "";
    }
}

[System.Serializable]
class ObjectPipeSide {
    Dictionary<string, Pipe> PipeSides;
    Dictionary<string, string> opposide_side_list = 
        new Dictionary<string, string>() {
            {"R", "L"},
            {"D", "U"},
            {"L", "R"},
            {"U", "D"}
        };

    public ObjectPipeSide() {
        PipeSides = new Dictionary<string, Pipe>() {
            {"R", null},
            {"D", null},
            {"L", null},
            {"U", null}
        };
    }

    public void set_pipe_with_side(string side, Pipe pipe) {
        Dictionary<string, Pipe> Temp = new Dictionary<string, Pipe>();
        foreach(KeyValuePair<string, Pipe> element in PipeSides) {
            if (element.Key == side) {
                Temp.Add(element.Key, pipe);
            } else {
                Temp.Add(element.Key, element.Value);
            }
        }

        PipeSides = Temp;
    }

    public string get_opposite_pipe_string(string side) {
        foreach(KeyValuePair<string, string> element in opposide_side_list) {
            if (element.Key == side)
                return element.Value;
        }

        return "";
    }

    public Pipe get_pipe_with_side(string side) {
        foreach(KeyValuePair<string, Pipe> element in PipeSides) {
            if (element.Key == side)
                return element.Value;
        }

        return null;
    }
}

[System.Serializable]
enum CrossPipeFillState {
    Empty,
    Vertical,
    Horizontal
}

[System.Serializable]
public class PipeData {
    public PipeType pipeType;
    public Sprite PipeSprite;
    public int rotationTimes;
    public BoolPipeSide boolPipeSide;
    public string isInGrid;
    public Vector2 PipeIndex;
    public string curvedPipeSide;

    public PipeData() {}
}