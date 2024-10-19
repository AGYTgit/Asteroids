
using System;

public static class Mapping {
    public static int Map(int value, int in_min, int in_max, int out_min, int out_max) {
        if (in_min == in_max) {
            return value;
        }
        int mapped_value = (value - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        return mapped_value;
    }
}