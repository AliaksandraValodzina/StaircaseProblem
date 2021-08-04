using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using StaircaseProblem.Models;
using Xamarin.Forms;

namespace StaircaseProblem
{
    public partial class MainPage : ContentPage
    {
        private List<string> resultPath;
        private ObservableCollection<Stair> staircase;

        public MainPage()
        {
            resultPath = new List<string>();
            staircase = new ObservableCollection<Stair>();
            InitializeComponent();
            nextPathButton.IsEnabled = false;
            goButton.IsEnabled = false;
        }

        private int Fib(int n)
        {
            if (n <= 1)
                return n;
            return Fib(n - 1) + Fib(n - 2);
        }

        // Returns number of ways to reach s'th stair
        private int CountWays(int s)
        {
            return Fib(s + 1);
        }

        async void OnGoButtonClicked(object sender, EventArgs args)
        {
            staircase = new ObservableCollection<Stair>();
            var numberOfStairs = int.Parse(entry.Text);
            result.Text = "Number of ways = " + CountWays(numberOfStairs);

            WaysToClimb(numberOfStairs, new List<int>());
            path.Text = resultPath.First();

            DrawStaircase(numberOfStairs);
            listView.ItemsSource = staircase;
        }

        async void OnNextPathClicked(object sender, EventArgs args)
        {
            if (resultPath.Count != 0 && resultPath.Count != 1)
            {
                var randomRes = new Random().Next(1, resultPath.Count);
                path.Text = resultPath.ElementAt(randomRes);

                staircase = new ObservableCollection<Stair>();
                DrawStaircase(int.Parse(entry.Text));
                listView.ItemsSource = staircase;
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
                listView.ItemsSource = staircase;
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

        private void DrawStaircase(int n)
        {
            var combination = path.Text.Split(',').ToList().Select(int.Parse).ToList();
            var test = new List<int>();

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
                    Color = Color.Black
                };

                if (stepNoList.Contains(i))
                {
                    step.Color = Color.Red;
                }
                
                staircase.Add(step);
            }
        }

        private void DrawStairs1(int n)
        {
            var combination = path.Text.Split(',').ToList().Select(int.Parse).ToList();
            var test = new List<int>();
            var strTest = string.Empty;

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
                    Color = Color.Black
                };

                if (stepNoList.Contains(i))
                {
                    step.Color = Color.Red;
                }

                staircase.Add(step);
            }
        }
    }
}

/*
 * 
 *  0
 * /|\
 * / \
 * * * 
     *  0
     * /|\
     * / \
     * * * 
         *
         *
         *
         * * * 
 */
