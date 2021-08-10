using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using StaircaseProblem.Models;
using StaircaseProblem.Services;
using StaircaseProblem.ViewModels;
using Xamarin.Forms;

namespace StaircaseProblem.Views
{
    public partial class StairPage : ContentPage
    {
        private StairViewModel stairViewModel;
        private int deviceWidth;
        private int stepWidth;
        private int stepHeight;

        public StairPage()
        {
            stairViewModel = new StairViewModel();
            stairViewModel.Stair = new StairService().GetStaircase(0);
            stairViewModel.Stair.Step = new List<Step>();
            stairViewModel.Stair.WaysToClimb = new List<string>();
            InitializeComponent();
            nextPathButton.IsEnabled = false;
            goButton.IsEnabled = false;
            BindingContext = stairViewModel;
        }

        async void OnGoButtonClicked(object sender, EventArgs args)
        {
            var numberOfStairs = int.Parse(entry.Text);
            stairViewModel.Stair = new StairService().GetStaircase(numberOfStairs);

            path.Text = stairViewModel.Stair.WaysToClimb.First();
            DrawStairs(numberOfStairs);

            if (canvasView != null)
            {
                canvasView.InvalidateSurface();
            }
        }

        async void OnNextPathClicked(object sender, EventArgs args)
        {
            if (stairViewModel.Stair.WaysToClimb.Count != 0 && stairViewModel.Stair.WaysToClimb.Count != 1)
            {
                var randomRes = new Random().Next(1, stairViewModel.Stair.WaysToClimb.Count);
                path.Text = stairViewModel.Stair.WaysToClimb.ElementAt(randomRes);
                DrawStairs(int.Parse(entry.Text));

                if (canvasView != null)
                {
                    canvasView.InvalidateSurface();
                }
            }
        }

        public void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                bool isValid = args.NewTextValue.ToCharArray().All(x => char.IsDigit(x)); //Make sure all characters are numbers

                ((Entry)sender).Text = isValid ? args.NewTextValue : args.NewTextValue.Remove(args.NewTextValue.Length - 1);

                nextPathButton.IsEnabled = true;
                goButton.IsEnabled = true;
            } else
            {
                nextPathButton.IsEnabled = false;
                goButton.IsEnabled = false;
            }

            path.Text = string.Empty;
        }

        private void DrawStairs(int n)
        {
            stairViewModel.Stair.Step = new List<Step>();
            if (n == 0)
                return;

            var combination = path.Text.Split(',').ToList().Select(int.Parse).ToList();
            var test = new List<int>();
            var strTest = string.Empty; 
            deviceWidth = (int)(Application.Current.MainPage.Width * 1.7);
            stepWidth = deviceWidth / (n + 1);
            stepHeight = stepWidth;

            for (int i = 1; i <= combination.Count; i++)
            {
                test.Add(combination.Take(i).Sum());
            }

            var stepNoList = test.OrderByDescending(x => x).ToList();

            for (int i = n; i >= 1; i--)
            {
                var step = new Step
                {
                    Color = SKColors.Black,
                    StartPoint = new SKPoint((n - i) * stepWidth, (n - i) * stepHeight),
                    EndPoint = new SKPoint((n - i + 1) * stepWidth, (n - i) * stepHeight)
                };

                if (stepNoList.Contains(i))
                {
                    step.Color = SKColors.Orange;
                }

                stairViewModel.Stair.Step.Add(step);
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (stairViewModel.Stair.Step == null)
                return;

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5
            };

            for (int i = stairViewModel.Stair.Step.Count - 1; i >= 0; i--)
            {
                var step = stairViewModel.Stair.Step.ElementAt(i);
                paint.Color = step.Color;

                SKRect rect = new SKRect(step.StartPoint.X, step.StartPoint.Y, step.StartPoint.Y + 2 * stepWidth, step.StartPoint.Y + 2 * stepHeight);

                if (paint.Color.Equals(SKColors.Black))
                {
                    rect = new SKRect(step.StartPoint.X - 2 * stepWidth, step.StartPoint.Y - stepHeight, step.StartPoint.X + 2 * stepWidth, step.StartPoint.Y + 3 * stepHeight);
                }

                if ((i != (stairViewModel.Stair.Step.Count - 1) && !stairViewModel.Stair.Step.ElementAt(i + 1).Color.Equals(SKColors.Black)) || i == (stairViewModel.Stair.Step.Count - 1))
                {
                    using (SKPath path = new SKPath())
                    {
                        paint.Color = SKColors.Orange;
                        path.AddArc(rect, 0, -90);
                        canvas.DrawPath(path, paint);
                    }
                }

                paint.Color = SKColors.Black;
                canvas.DrawLine(step.StartPoint, step.EndPoint, paint);

                if (i != 0)
                {
                    canvas.DrawLine(step.StartPoint, stairViewModel.Stair.Step.ElementAt(i - 1).EndPoint, paint);
                }

                if (i == (stairViewModel.Stair.Step.Count - 1))
                {
                    canvas.DrawLine(step.EndPoint, new SKPoint(step.EndPoint.X, step.EndPoint.Y + stepHeight), paint);
                }
            }
        }
    }
}