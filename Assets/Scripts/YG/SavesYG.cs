using System;
using System.Collections.Generic;

namespace YG
{
    public partial class SavesYG
    {
        public bool[] levels;

        public List<int> levels_id = new();
        public List<bool> levels_status = new(); // 1 - unlocked
        public List<bool> levels_win = new();

        public string[] colors_id;
        public bool[] colors_status;

        public List<float> scores = new();

        public int cups = 0;
        public int gems = 0;

        public bool newPlayer = true;

    }
}