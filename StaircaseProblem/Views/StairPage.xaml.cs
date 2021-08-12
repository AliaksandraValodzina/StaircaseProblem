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

        public StairPage()
        {
            stairViewModel = new StairViewModel();
            stairViewModel.Stair = new StairService().GetStair(-1);
            InitializeComponent();
            nextPathButton.IsEnabled = false;
            goButton.IsEnabled = false;
            BindingContext = stairViewModel;
        }

        async void OnGoButtonClicked(object sender, EventArgs args)
        {
            var numberOfStairs = int.Parse(entry.Text);
            stairViewModel.Stair = new StairService().GetStair(numberOfStairs);

            path.Text = stairViewModel.Stair.WaysToClimb.First();
            DrawStairs(numberOfStairs);
            nextPathButton.IsEnabled = true;

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

                goButton.IsEnabled = true;
            }
            else
            {
                stairViewModel.Stair = new StairService().GetStair(-1);
                nextPathButton.IsEnabled = false;
                goButton.IsEnabled = false;
                canvasView.InvalidateSurface();
            }

            path.Text = string.Empty;
        }

        private void DrawStairs(int numberOfStairs)
        {
            stairViewModel.Stair.Step = new List<Step>();

            if (numberOfStairs == 0)
                return;

            var combination = path.Text.Split(',').ToList().Select(int.Parse).ToList();
            var noStep = new List<int>();
            deviceWidth = (int)(Application.Current.MainPage.Width * 1.7);
            stairViewModel.Stair.StepWidth = deviceWidth / (numberOfStairs + 1);
            stairViewModel.Stair.StepHeight = stairViewModel.Stair.StepWidth;

            for (int i = 1; i <= combination.Count; i++)
            {
                noStep.Add(combination.Take(i).Sum());
            }

            var stepNoList = noStep.OrderByDescending(x => x).ToList();

            for (int i = numberOfStairs; i >= 1; i--)
            {
                var isStepped = stepNoList.Contains(i) ? false : true;
                stairViewModel.Stair = new StairService().AddStepParams(stairViewModel.Stair, i, isStepped);
            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            var stepWidth = stairViewModel.Stair.StepWidth;
            var stepHeight = stairViewModel.Stair.StepHeight;

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
                SKRect rect = new SKRect(step.StartPoint.X, step.StartPoint.Y, step.StartPoint.Y + 2 * stepWidth, step.StartPoint.Y + 2 * stepHeight);

                if (step.IsStepped)
                {
                    rect = new SKRect(step.StartPoint.X - 2 * stepWidth, step.StartPoint.Y - stepHeight, step.StartPoint.X + 2 * stepWidth, step.StartPoint.Y + 3 * stepHeight);
                }

                if ((i != (stairViewModel.Stair.Step.Count - 1) && !stairViewModel.Stair.Step.ElementAt(i + 1).IsStepped) || i == (stairViewModel.Stair.Step.Count - 1))
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