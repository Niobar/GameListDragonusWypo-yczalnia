using System;
using System.Collections.Generic;
using System.Text;

namespace DragonusWypożyczalniaLista
{
    public class Game
    {
        public int LendPrice { get; set; }
        public string Title { get; set; }
        public double ScoreBGG { get; set; }
        public int RankBGG { get; set; }
        public int NumberBGG { get; set; }
        public double WeightBGG { get; set; }
        public override string ToString()
        {
            return this.LendPrice.ToString() + " | " + this.Title.ToString() + " | " + this.ScoreBGG.ToString() + " | " + this.RankBGG.ToString() + " | " + this.NumberBGG.ToString() + " | " + this.WeightBGG.ToString();
        }
    }
}
