using System.Collections.Generic;
using SkiaSharp;
using StaircaseProblem.Models;

namespace StaircaseProblem.Services
{
    public class StairService
    {
        private List<string> resultPath;

        public Stair GetStaircase(int numberOfStairs)
        {
            var stair = new Stair();
            stair.NumberOfSteps = numberOfStairs;

            if (numberOfStairs == -1)
            {
                stair.WaysToReachTheTop = "?";
            }
            else
            {
                stair.WaysToReachTheTop = CountWays(numberOfStairs);
                WaysToClimb(numberOfStairs, new List<int>());
                stair.WaysToClimb = resultPath;
            }

            return stair;
        }

        public Stair AddStepParams(Stair stair, int stepNo, bool isStepped)
        {
            var step = new Step
            {
                IsStepped = isStepped,
                StartPoint = new SKPoint((stair.NumberOfSteps - stepNo) * stair.StepWidth, (stair.NumberOfSteps - stepNo) * stair.StepHeight),
                EndPoint = new SKPoint((stair.NumberOfSteps - stepNo + 1) * stair.StepWidth, (stair.NumberOfSteps - stepNo) * stair.StepHeight)
            };

            stair.Step.Add(step);

            return stair;
        }

        private void WaysToClimb(int n, List<int> path)
        {
            if (n == 0)
            {
                var res = string.Empty;

                foreach (var i in path)
                {
                    res += i + ",";
                }

                res = res.TrimEnd(',');

                if (resultPath == null)
                {
                    resultPath = new List<string>();
                }

                resultPath.Add(res);
            }
            else if (n == 1)
            {
                List<int> newPath = new List<int>(path);
                newPath.Add(1);
                WaysToClimb(n - 1, newPath);
            }
            else if (n > 1)
            {
                List<int> newPath1 = new List<int>(path);
                newPath1.Add(1);
                WaysToClimb(n - 1, newPath1);

                List<int> newPath2 = new List<int>(path);
                newPath2.Add(2);
                WaysToClimb(n - 2, newPath2);
            }
        }

        // Returns number of ways to reach n'th stair
        private string CountWays(int s, int m = 2)
        {
            return CountWaysUtil(s + 1, m).ToString();
        }

        private int CountWaysUtil(int n, int m)
        {
            int[] res = new int[n];

            if (n == 1 || n == 0)
                return 1;

            res[0] = 1;
            res[1] = 1;

            for (int i = 2; i < n; i++)
            {
                res[i] = 0;

                for (int j = 1; j <= m && j <= i; j++)
                    res[i] += res[i - j];
            }

            return res[n - 1];
        }
    }
}
