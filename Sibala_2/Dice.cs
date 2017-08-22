﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sibala_2
{
    internal interface IDiceResultHandler
    {
        void Handle(Dice dice);
    }

    public class Dice
    {
        public readonly ReadOnlyCollection<int> _dices;

        private Dictionary<int, string> _outputLookup = new Dictionary<int, string>
        {
            {12, "18La" },
            {3,  "BG"}
        };

        private Dictionary<int, IDiceResultHandler> handlersLookup = new Dictionary<int, IDiceResultHandler>()
        {
            {4, new SameDiceResultHandler()},
            {2, new PointsDiceResultHandler()},
            {3, new NoPointDiceResultHandler()},
            {1, new NoPointDiceResultHandler()},
        };

        public Dice(int[] inputString)
        {
            _dices = new ReadOnlyCollection<int>(inputString);
            this.Calculate();
        }

        public int MaxPoint { get; set; }

        public string Output { get; set; }

        public int Points { get; set; }

        public DiceType Type { get; set; }

        private void Calculate()
        {
            SetResult();

            SetOutput();
        }
        private string GetOutputWhenPoints()
        {
            var isSpecialOutput = this._outputLookup.ContainsKey(this.Points);
            return isSpecialOutput ? this._outputLookup[this.Points] : this.Points + "Point";
        }

        private void SetOutput()
        {
            var isTypePoints = this.Type == DiceType.Points;
            this.Output = isTypePoints ? GetOutputWhenPoints() : this.Type.ToString();
        }
        private void SetResult()
        {
            var maxCountOfSameDice = _dices.GroupBy(x => x).Max(x => x.Count());
            var handler = this.handlersLookup[maxCountOfSameDice];
            handler.Handle(this);
        }
    }

    internal class NoPointDiceResultHandler : IDiceResultHandler
    {
        public void Handle(Dice dice)
        {
            dice.Type = DiceType.NoPoint;
        }
    }

    internal class PointsDiceResultHandler : IDiceResultHandler
    {
        public void Handle(Dice dice)
        {
            dice.Type = DiceType.Points;
            var diceGrouping = dice._dices.GroupBy(x => x);
            if (diceGrouping.Count() == 2)
            {
                var maxPoint = dice._dices.Max();
                dice.Points = maxPoint * 2;
                dice.MaxPoint = maxPoint;
            }
            else
            {
                var duplicatePoint = diceGrouping.First(x => x.Count() == 2).Key;
                var dicesOfPoints = dice._dices.Where(x => x != duplicatePoint);
                dice.Points = dicesOfPoints.Sum();
                dice.MaxPoint = dicesOfPoints.Max();
            }
        }
    }
    internal class SameDiceResultHandler : IDiceResultHandler
    {
        public void Handle(Dice dice)
        {
            dice.Type = DiceType.Same;
            dice.Points = dice._dices.Sum();
            dice.MaxPoint = dice._dices.First();
        }
    }
}