using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using StaircaseProblem.Models;
using Xamarin.Forms;

namespace StaircaseProblem
{
    public partial class MainPage : ContentPage
    {
        private List<string> resultPath;
        private ObservableCollection<Stair> staircase;
        private int deviceWidth;
        private int stepWidth;
        private int stepHeight;

        public MainPage()
        {
            resultPath = new List<string>();
            staircase = new ObservableCollection<Stair>();
            InitializeComponent();
            nextPathButton.IsEnabled = false;
            goButton.IsEnabled = false;
        }

        /*private int Fib(int n)
        {
            if (n <= 1)
                return n;

            return Fib(n - 1) + Fib(n - 2);
        }*/

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

        // Returns number of ways to reach n'th stair
        private int CountWays(int s, int m = 2)
        {
            return CountWaysUtil(s + 1, m);
            //return Fib(s + 1);
        }

        async void OnGoButtonClicked(object sender, EventArgs args)
        {
            staircase = new ObservableCollection<Stair>();
            var numberOfStairs = int.Parse(entry.Text);
            result.Text = "Number of ways = " + CountWays(numberOfStairs);

            WaysToClimb(numberOfStairs, new List<int>());
            path.Text = resultPath.First();
            DrawStairsSkia(numberOfStairs);

            if (canvasView != null)
            {
                canvasView.InvalidateSurface();
            }
        }

        async void OnNextPathClicked(object sender, EventArgs args)
        {
            if (resultPath.Count != 0 && resultPath.Count != 1)
            {
                var randomRes = new Random().Next(1, resultPath.Count);
                path.Text = resultPath.ElementAt(randomRes);
                staircase = new ObservableCollection<Stair>();

                DrawStairsSkia(int.Parse(entry.Text));

                if (canvasView != null)
                {
                    canvasView.InvalidateSurface();
                }
            }
        }

        public void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            resultPath = new List<string>();
            staircase = new ObservableCollection<Stair>();

            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x)); //Make sure all characters are numbers

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);

                nextPathButton.IsEnabled = true;
                goButton.IsEnabled = true;
            } else
            {
                result.Text = "Number of ways = ?";
                nextPathButton.IsEnabled = false;
                goButton.IsEnabled = false;
            }

            path.Text = string.Empty;
        }

        public void WaysToClimb(int n, List<int> path)
        {
            if (n == 0)
            {
                var res = string.Empty;

                foreach (var i in path)
                {
                    res += i + ",";
                }

                res = res.TrimEnd(',');
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

        private void DrawStairsSkia(int n)
        {
            if (n == 0)
                return;

            var combination = path.Text.Split(',').ToList().Select(int.Parse).ToList();
            var test = new List<int>();
            var strTest = string.Empty; 
            deviceWidth = (int)(Application.Current.MainPage.Width * 1.7);
            stepWidth = deviceWidth / n;
            stepHeight = stepWidth;

            for (int i = 1; i <= combination.Count; i++)
            {
                test.Add(combination.Take(i).Sum());
            }

            var stepNoList = test.OrderByDescending(x => x).ToList();

            for (int i = n; i >= 1; i--)
            {
                var hashes = new string('#', n - i + 1);

                var step = new Stair
                {
                    Text = hashes,
                    Color = SKColors.Black,
                    X = new SKPoint((n - i) * stepWidth, (n - i) * stepHeight),
                    Y = new SKPoint((n - i + 1) * stepWidth, (n - i) * stepHeight)
                };

                if (stepNoList.Contains(i))
                {
                    step.Color = SKColors.Orange;
                }

                staircase.Add(step);
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5
            };

            for (int i = staircase.Count - 1; i >= 0; i--)
            {
                var step = staircase.ElementAt(i);
                paint.Color = step.Color;

                SKRect rect = new SKRect(step.X.X, step.X.Y, step.X.Y + 2 * stepWidth, step.X.Y + 2 * stepHeight);

                if (paint.Color.Equals(SKColors.Black))
                {
                    rect = new SKRect(step.X.X - 2 * stepWidth, step.X.Y - stepHeight, step.X.X + 2 * stepWidth, step.X.Y + 3 * stepHeight);
                }

                if ((i != (staircase.Count - 1) && !staircase.ElementAt(i + 1).Color.Equals(SKColors.Black)) || i == (staircase.Count - 1))
                {
                    using (SKPath path = new SKPath())
                    {
                        paint.Color = SKColors.Orange;
                        path.AddArc(rect, 0, -90);
                        canvas.DrawPath(path, paint);
                    }
                }

                paint.Color = SKColors.Black;
                canvas.DrawLine(step.X, step.Y, paint);

                if (i != 0)
                {
                    canvas.DrawLine(step.X, staircase.ElementAt(i - 1).Y, paint);
                }

                if (i == (staircase.Count - 1))
                {
                    canvas.DrawLine(step.Y, new SKPoint(step.Y.X, step.Y.Y + stepHeight), paint);
                }
            }
        }
    }
}