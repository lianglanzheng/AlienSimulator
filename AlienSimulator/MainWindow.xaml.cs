using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AlienSimulator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty PrecisionProperty = DependencyProperty.Register("Precision", typeof(int), typeof(MainWindow), new PropertyMetadata(5));
        public static readonly DependencyProperty DaysReproduceProperty = DependencyProperty.Register("DaysReproduce", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static readonly DependencyProperty DaysElapsedProperty = DependencyProperty.Register("DaysElapsed", typeof(int), typeof(MainWindow), new PropertyMetadata(0));
        public static readonly DependencyProperty DeathProbabilityProperty = DependencyProperty.Register("DeathProbability", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ReproduceResultsProperty = DependencyProperty.Register("ReproduceResults", typeof(ObservableCollection<ReproduceResult>), typeof(MainWindow), new PropertyMetadata(new ObservableCollection<ReproduceResult>()));

        /// <summary>
        /// 计算精度。
        /// </summary>
        public int Precision
        {
            get => (int)GetValue(PrecisionProperty);
            set => SetValue(PrecisionProperty, value);
        }

        /// <summary>
        /// 繁殖天数。
        /// </summary>
        public int DaysReproduce
        {
            get => (int)GetValue(DaysReproduceProperty);
            set => SetValue(DaysReproduceProperty, value);
        }

        /// <summary>
        /// 逝去天数。
        /// </summary>
        public int DaysElapsed
        {
            get => (int)GetValue(DaysElapsedProperty);
            set => SetValue(DaysElapsedProperty, value);
        }

        /// <summary>
        /// 灭绝概率。
        /// </summary>
        public double DeathProbability
        {
            get => (double)GetValue(DeathProbabilityProperty);
            set => SetValue(DeathProbabilityProperty, value);
        }

        /// <summary>
        /// 模拟结果。
        /// </summary>
        public ObservableCollection<ReproduceResult> ReproduceResults
        {
            get => (ObservableCollection<ReproduceResult>)GetValue(ReproduceResultsProperty);
            set => SetValue(ReproduceResultsProperty, value);
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public static readonly RoutedUICommand BeginReproduce = new RoutedUICommand("开始繁殖", "BeginReproduce", typeof(MainWindow));

        bool isRunning = false;

        private void BeginReproduce_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !isRunning;
        }

        static double[] current = new double[] { 0, 1 };

        private async void BeginReproduce_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            isRunning = true;
            CalculationCore.Precision = Math.Pow(0.1, Precision);
            while (DaysReproduce > 0)
            {
                DaysReproduce--;
                current = await CalculationCore.GetNextGenerationProbabilityAsync(current);
                DaysElapsed++;
                DeathProbability = current[0];
                ReproduceResults.Add(new ReproduceResult { Days = DaysElapsed, Probability = DeathProbability });
            }
            isRunning = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
