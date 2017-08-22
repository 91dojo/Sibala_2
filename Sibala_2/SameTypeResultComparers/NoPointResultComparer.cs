using System.Collections.Generic;

namespace Sibala_2.SameTypeResultComparers
{
    public class NoPointResultComparer : IComparer<Dice>
    {
        public int Compare(Dice dice1, Dice dice2)
        {
            return 0;
        }
    }
}