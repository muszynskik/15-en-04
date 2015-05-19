using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace client
{

    class Algorithm
    {
        List<Store> storeList;
        double timeout;

        public Algorithm(List<Store> _storeList, double _timeout)
        {
            storeList = _storeList;
            timeout = _timeout;
        }

        public double SolveTsp()
        {
            int[] storeIdList = new int[storeList.Count];

            int i = 0;
            foreach (Store rec in storeList)
            {
                storeIdList[i] = rec.id;
                i++;
            }
            double minLength = 999999;
            do
            {
                double length = 0d;
                if (!CheckRouteConstraints(storeIdList, out length, timeout))
                    continue;
                if (length < minLength)
                {
                    minLength = length;
                }
            } while (!NextPermutation(storeIdList));
            return minLength;
        }

        private bool CheckRouteConstraints(int[] storeIdList, out double length, double timeout)
        {
            length = 0;
            Point depot = new Point(0, 0);
            length = calculateDistance(depot, storeList[0].coordinate);
            length += storeList[0].duration;
            for (int i = 0; i < storeIdList.Count() - 1; i++)
            {
                length += calculateDistance(storeList[i].coordinate, storeList[i + 1].coordinate);
                length += storeList[i].duration;
            }

            length += calculateDistance(storeList[storeIdList.Count() - 1].coordinate, depot);
            length += storeList[storeIdList.Count() - 1].duration;
            if (length <= timeout)
                return true;
            else return false;
        }

        private double calculateDistance(Point x, Point y)
        {
            //double result = Math.Sqrt(Math.Pow((x.X - y.X), 2) - Math.Pow((x.Y - y.Y), 2));
            double result = Math.Sqrt(Math.Pow((y.X - x.X), 2) + Math.Pow((y.Y - x.Y), 2));
            return result;
        }

        private static bool NextPermutation(int[] numList)
        {
            /*
             Knuths
             1. Find the largest index j such that a[j] < a[j + 1]. If no such index exists, the permutation is the last permutation.
             2. Find the largest index l such that a[j] < a[l]. Since j + 1 is such an index, l is well defined and satisfies j < l.
             3. Swap a[j] with a[l].
             4. Reverse the sequence from a[j + 1] up to and including the final element a[n].

             */
            var largestIndex = -1;
            for (var i = numList.Length - 2; i >= 0; i--)
            {
                if (numList[i] < numList[i + 1])
                {
                    largestIndex = i;
                    break;
                }
            }

            if (largestIndex < 0) return false;

            var largestIndex2 = -1;
            for (var i = numList.Length - 1; i >= 0; i--)
            {
                if (numList[largestIndex] < numList[i])
                {
                    largestIndex2 = i;
                    break;
                }
            }

            var tmp = numList[largestIndex];
            numList[largestIndex] = numList[largestIndex2];
            numList[largestIndex2] = tmp;

            for (int i = largestIndex + 1, j = numList.Length - 1; i < j; i++, j--)
            {
                tmp = numList[i];
                numList[i] = numList[j];
                numList[j] = tmp;
            }

            return true;
        }

    }

    class Store
    {
        public Point coordinate;
        public int id;
        public double duration;
        public double demand;
    }

}

