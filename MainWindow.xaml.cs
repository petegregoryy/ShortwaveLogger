﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace SWLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow settingsWindow = new SettingsWindow();        
        Schedule scheduleE;
        List<Schedule> schEntries = new List<Schedule>();
        List<Schedule> onAirEntries = new List<Schedule>();
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        DispatcherTimer minuteTick = new System.Windows.Threading.DispatcherTimer();
        string dataPath;
        string historyPath;
        string[] schedEntry;
        DateTime UTC = DateTime.UtcNow;
        public MainWindow()
        {
            InitializeComponent();

            

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimer.Start();

            minuteTick.Tick += new EventHandler(minuteTick_Tick);
            minuteTick.Interval = new TimeSpan(0, 1, 0);
            minuteTick.Start();

            dataPath = settingsWindow.dataPath;
            historyPath = settingsWindow.historyPath;
            schedEntry = File.ReadAllLines(dataPath);
            DrawLive();
        }

        private void minuteTick_Tick(object sender, EventArgs e)
        {
            DrawLive();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            UTC = DateTime.UtcNow;
            ClockUTC.Content = UTC.TimeOfDay.ToString();
        }

        private void OnAirGrid_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Application.Current.Shutdown();
        }

        private void DrawLive()
        {
            
            TimeSpan now = UTC.TimeOfDay;
            foreach (string entry in schedEntry)
            {                
                string[] split = entry.Split(';');
                scheduleE = new Schedule(split);
                schEntries.Add(scheduleE);
                if((now > scheduleE.StartTime) && (now < scheduleE.EndTime))
                {
                    onAirEntries.Add(scheduleE);
                    OnAirGrid.Items.Add(scheduleE);
                }
                
            }
            
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow.ShowDialog();
        }

        private void OnAirGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ContextMenu m = new ContextMenu();
            m.Items.Add("Quick Add");
            m.Items.Add("Add with notes");
            m.IsOpen = true;

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(OnAirGrid, e.GetPosition(OnAirGrid));
            DataGridRow dataGridRow = hitTestResult.VisualHit.GetParentOfType<DataGridRow>();
            int index = dataGridRow.GetIndex();
        }
    }
    public static class Extensions
    {
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            Type type = typeof(T);
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null && ((FrameworkElement)element).Parent is DependencyObject) parent = ((FrameworkElement)element).Parent;
            if (parent == null) return null;
            else if (parent.GetType() == type || parent.GetType().IsSubclassOf(type)) return parent as T;
            return GetParentOfType<T>(parent);
        }
    }
}

