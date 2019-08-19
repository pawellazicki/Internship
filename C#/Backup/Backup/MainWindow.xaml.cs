﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;
using System.Windows.Forms;
using System.Threading;

namespace Backup
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private long maxLength;
        private long currentLength;
        private List<string> listOfBackupNames;
        private List<string> listOfBackupSourcePaths;
        private List<string> listOfBackupDestPaths;
        private List<string> listOfDates;
        private string ProgramDataPath = @"kopia.txt";
        private string _delay;
        public string Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
                OnPropertyChanged(nameof(Delay));
            }
        }
        private string _newBackupSourcePath;
        public string NewBackupSourcePath
        {
            get
            {
                return _newBackupSourcePath;
            }
            set
            {
                _newBackupSourcePath = value;
                OnPropertyChanged(nameof(NewBackupSourcePath));
            }
        }
        private string _newBackupDestinationPath;
        public string NewBackupDestinationPath
        {
            get
            {
                return _newBackupDestinationPath;
            }
            set
            {
                _newBackupDestinationPath = value;
                OnPropertyChanged(nameof(NewBackupDestinationPath));
            }
        }
        private string _backupName;
        public string BackupName
        {
            get
            {
                return _backupName;
            }
            set
            {
                _backupName = value;
                OnPropertyChanged(nameof(BackupName));
            }
        }
        private string _fastButton0;
        public string FastButton0
        {
            get
            {
                return _fastButton0;
            }
            set
            {
                _fastButton0 = value;
                OnPropertyChanged(nameof(FastButton0));
            }
        }
        private string _fastButton1;
        public string FastButton1
        {
            get
            {
                return _fastButton1;
            }
            set
            {
                _fastButton1 = value;
                OnPropertyChanged(nameof(FastButton1));
            }
        }
        private string _fastButton2;
        public string FastButton2
        {
            get
            {
                return _fastButton2;
            }
            set
            {
                _fastButton2 = value;
                OnPropertyChanged(nameof(FastButton2));
            }
        }
        private string _fastButton3;
        public string FastButton3
        {
            get
            {
                return _fastButton3;
            }
            set
            {
                _fastButton3 = value;
                OnPropertyChanged(nameof(FastButton3));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            SetButtonsEmpty();
            FIleOperations fIleOperations = new FIleOperations(ProgramDataPath);
            listOfBackupNames = fIleOperations.ReadNames();
            listOfBackupSourcePaths = fIleOperations.ReadSourcePath();
            listOfBackupDestPaths = fIleOperations.ReadDestinationPath();
            listOfDates = fIleOperations.ReadDate();
            
            SetSlots();
            RefreshMyBackups();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /////////==============>>> BUTTON CLICKS <<<==============//////////
        private void NewBackup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(BackupName))
                BackupNameWarningAnimation();
            else
                newBackupHost.IsOpen = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            newBackupHost.IsOpen = false;
            BackupName = string.Empty;
            NewBackupSourcePath = string.Empty;
            NewBackupDestinationPath = string.Empty;
        }

        private void MakeNewBackup_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(() => CopyWorker(NewBackupSourcePath, NewBackupDestinationPath));
            thread.Start();
            CopyingProgressBar.Visibility = Visibility.Visible;
            ProgressAnimation();

            newBackupHost.IsOpen = false;

            FIleOperations fIleOperations = new FIleOperations(ProgramDataPath);
            if(listOfBackupNames == null)
                fIleOperations.SaveRecord(BackupName, NewBackupSourcePath, NewBackupDestinationPath);
            else if (!listOfBackupNames.Contains(BackupName))
                fIleOperations.SaveRecord(BackupName, NewBackupSourcePath, NewBackupDestinationPath);
            
            listOfBackupNames = fIleOperations.ReadNames();
            listOfBackupSourcePaths = fIleOperations.ReadSourcePath();
            listOfBackupDestPaths = fIleOperations.ReadDestinationPath();

            BackupName = string.Empty;
            NewBackupSourcePath = string.Empty;
            NewBackupDestinationPath = string.Empty;

            SetSlots();
        }

        private void FastButton0_Click(object sender, RoutedEventArgs e)
        {
            int index;
            if (listOfBackupNames != null)
            {
                index = listOfBackupNames.IndexOf(FastButton0);
                if (index != -1)
                {
                    Thread thread = new Thread(() => CopyWorker(listOfBackupSourcePaths.ElementAt(index), listOfBackupDestPaths.ElementAt(index)));
                    thread.Start();
                    CopyingProgressBar.Visibility = Visibility.Visible;
                    ProgressAnimation();
                }
            }
        }
        private void FastButton1_Click(object sender, RoutedEventArgs e)
        {
            int index;
            if (listOfBackupNames != null)
            {
                index = listOfBackupNames.IndexOf(FastButton1);
                if (index != -1)
                {
                    Thread thread = new Thread(() => CopyWorker(listOfBackupSourcePaths.ElementAt(index), listOfBackupDestPaths.ElementAt(index)));
                    thread.Start();
                    CopyingProgressBar.Visibility = Visibility.Visible;
                    ProgressAnimation();
                }
            }
        }
        private void FastButton2_Click(object sender, RoutedEventArgs e)
        {
            int index;
            if (listOfBackupNames != null)
            {
                index = listOfBackupNames.IndexOf(FastButton2);
                if (index != -1)
                {
                    Thread thread = new Thread(() => CopyWorker(listOfBackupSourcePaths.ElementAt(index), listOfBackupDestPaths.ElementAt(index)));
                    thread.Start();
                    CopyingProgressBar.Visibility = Visibility.Visible;
                    ProgressAnimation();
                }
            }
        }
        private void FastButton3_Click(object sender, RoutedEventArgs e)
        {
            int index;
            if (listOfBackupNames != null)
            {
                index = listOfBackupNames.IndexOf(FastButton3);
                if (index != -1)
                {
                    Thread thread = new Thread(() => CopyWorker(listOfBackupSourcePaths.ElementAt(index), listOfBackupDestPaths.ElementAt(index)));
                    thread.Start();
                    CopyingProgressBar.Visibility = Visibility.Visible;
                    ProgressAnimation();
                }
            }
        }

        private void AllBackupButton_Click(object sender, RoutedEventArgs e)
        {
            AllBackupsHost.IsOpen = true;
        }

        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string[] ReadFile(string path)
        {
            string[] text = null;

            try
            {
                text = File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return text;
        }

        private void BackupSourceTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            NewBackupSourcePath = OpenFolderBrowser("0:0:0.2");
        }

        private void BackupDestinationTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            NewBackupDestinationPath = OpenFolderBrowser("0:0:0.2");
        }

        /////////==============>>> ANIMATIONS <<<==============//////////
        private void NewBackupPopupAnimation(string delay)
        {
            Delay = delay;
            var resource = myWindow.Resources["NewBackupAnimation"] as Storyboard;
            resource?.Begin();
        }

        private void NewBackupPopupReverseAnimation(string delay)
        {
            Delay = delay;
            var resource = myWindow.Resources["NewBackupReverseAnimation"] as Storyboard;
            resource?.Begin();
        }

        private void BackupNameWarningAnimation()
        {
            var resource = myWindow.Resources["BackupNameWarningAnimation"] as Storyboard;
            resource?.Begin();
        }

        private void ProgressAnimation()
        {
            var resource = myWindow.Resources["ProgressAnimation"] as Storyboard;
            resource?.Begin();
        }

        private void ProgressAnimationReverse()
        {
            var resource = myWindow.Resources["ProgressAnimationReverse"] as Storyboard;
            resource?.Begin();
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myWindow_StateChanged(object sender, EventArgs e)
        {
            if (myWindow.WindowState == WindowState.Minimized)
            {
                //newBackupPopup.IsOpen = false;
            }
        }

        private void MyDelay(int delayValue)
        {
            Thread.Sleep(delayValue);

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate
            {
                CopyingProgressBar.Visibility = Visibility.Hidden;
                ProgressAnimationReverse();
            });
        }

        private void CopyWorker(string SourcePath, string DestPath)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate
            {
                RefreshMyBackups();
                CopyingStatus.Text = String.Format("Kopiowanie plików\nz: " + Path.GetFileName(SourcePath) + "\ndo: " + Path.GetFileName(DestPath));
                CopyingFileStaticText.Text = "Plik:";
            });
            GetReadyCalculating(SourcePath);
            DirectoryCopy(SourcePath, DestPath);
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate
            {
                CopyingFile.Text = "";
                CopyingFileStaticText.Text = "";
                CopyingStatus.Text = String.Format("Kopiowanie zakończone\nz: " + Path.GetFileName(SourcePath) + "\ndo: " + Path.GetFileName(DestPath));
                CopyingCapacity.Text = "";
            });
            Thread thread = new Thread(() => MyDelay(5000));
            thread.Start();
            //});
        }
        private void GetReadyCalculating(string ProgramDataPath)
        {
            maxLength = 0;
            currentLength = 0;
            DirectoryInfo directoryInfo = new DirectoryInfo(ProgramDataPath);
            maxLength = CalculateDirectorySize(directoryInfo, true);
        }

        private static long CalculateDirectorySize(DirectoryInfo directory, bool includeSubdirectories)
        {
            long totalSize = 0;

            FileInfo[] files = directory.GetFiles();
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }

            if (includeSubdirectories)
            {
                DirectoryInfo[] dirs = directory.GetDirectories();
                foreach (DirectoryInfo dir in dirs)
                {
                    totalSize += CalculateDirectorySize(dir, true);
                }
            }

            return totalSize;
        }

        private void DirectoryCopy(string sourcerDirPath, string destDirPath)
        {
            DirectoryInfo dir = new DirectoryInfo(sourcerDirPath);
            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: "+ sourcerDirPath);
        
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (!Directory.Exists(destDirPath))
                Directory.CreateDirectory(destDirPath);
            
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                try
                {
                    FileInfo destFile = new FileInfo(Path.Combine(destDirPath, file.Name));
                    if(destFile.Exists)
                    {
                        if (file.LastWriteTime > destFile.LastWriteTime)
                        {
                            file.CopyTo(destFile.FullName, true);
                            currentLength += file.Length;
                        }
                        else
                            currentLength += file.Length;
                            
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate
                        {
                            double progress = (currentLength * 100) / maxLength;
                            CopyingProgressBar.Value = progress;
                            CopyPercent.Text = progress.ToString() + "%";
                            CopyingFile.Text = file.Name;
                        });
                    }
                    else
                    {
                        file.CopyTo(destFile.FullName, true);
                        currentLength += file.Length;
                        //Console.WriteLine(currentLength*100/maxLength+"%");
                        Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, (Action)delegate
                        {
                            double progress = (currentLength * 100) / maxLength;
                            CopyingProgressBar.Value = progress;
                            CopyPercent.Text = progress.ToString()+"%";
                            CopyingFile.Text = file.Name;
                            long toEnd = maxLength - currentLength;
                            CopyingCapacity.Text = String.Format("Pozostało:\n{0:0} MB", toEnd / Math.Pow(2, 20));
                        });
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Yo there is a problem: ", e.ToString());
                }
            }
            
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirPath, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        private string OpenFolderBrowser(string delay)
        {
            string path = string.Empty;
            //NewBackupPopupReverseAnimation(delay);
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderBrowserDialog.Description = "Wybierz folder źródłowy";
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = folderBrowserDialog.SelectedPath;

            }
            //NewBackupPopupAnimation(delay);

            return path;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == Mouse.LeftButton && e.ClickCount == 2)
            {
                if(WindowState != WindowState.Maximized)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    WindowState = WindowState.Normal;
                }
            }

            this.DragMove();
        }

        private void SetSlots()
        {
            Console.WriteLine("ADASDA");
            if (listOfBackupNames != null)
            {
                if (listOfBackupNames.Count == 1)
                {
                    FastButton0 = listOfBackupNames.ElementAt(0);
                }
                else if (listOfBackupNames.Count == 2)
                {
                    FastButton0 = listOfBackupNames.ElementAt(0);
                    FastButton1 = listOfBackupNames.ElementAt(1);
                }
                else if (listOfBackupNames.Count == 3)
                {
                    FastButton0 = listOfBackupNames.ElementAt(0);
                    FastButton1 = listOfBackupNames.ElementAt(1);
                    FastButton2 = listOfBackupNames.ElementAt(2);
                }
                else if (listOfBackupNames.Count > 3)
                {
                    FastButton0 = listOfBackupNames.ElementAt(0);
                    FastButton1 = listOfBackupNames.ElementAt(1);
                    FastButton2 = listOfBackupNames.ElementAt(2);
                    FastButton3 = listOfBackupNames.ElementAt(3);
                }
            }
        }
         

        private void SetButtonsEmpty()
        {
            FastButton0 = "pusty slot";
            FastButton1 = "pusty slot";
            FastButton2 = "pusty slot";
            FastButton3 = "pusty slot";
        }

        //private void fun()
        //{
        //    for(int i = 0; i < listOfBackupNames.Count; i++)
        //    {
        //        TextBlock textBlock0 = new TextBlock();
        //        textBlock0.Text = listOfBackupNames.ElementAt(i);
        //        TextBlock textBlock1 = new TextBlock();
        //        textBlock1.Text = listOfBackupSourcePaths.ElementAt(i);
        //        TextBlock textBlock2 = new TextBlock();
        //        textBlock2.Text = listOfBackupDestPaths.ElementAt(i);

        //        StackPanel BackupElement = new StackPanel();
        //        BackupElement.Orientation = System.Windows.Controls.Orientation.Horizontal;
        //        BackupElement.Children.Add(textBlock0);
        //        BackupElement.Children.Add(textBlock1);
        //        BackupElement.Children.Add(textBlock2);

        //        AllBackupsStackPanel.Children.Add(BackupElement);
        //    }
        //}

        private void RefreshMyBackups()
        {
            if (listOfBackupNames != null)
            {
                for (int i = 0; i < listOfBackupNames.Count - 1; i++)
                {
                    string text = String.Format(listOfBackupNames.ElementAt(i) + ", " + listOfBackupSourcePaths.ElementAt(i) + ", " + listOfDates.ElementAt(i));
                    BackupBorder backupBorder = new BackupBorder();
                    backupBorder.SetText(text);
                    AllBackupsStackPanel.Children.Add(backupBorder.GetBorder());
                }
            }
        }

        private void CloseListButton_Click(object sender, RoutedEventArgs e)
        {
            AllBackupsHost.IsOpen = false;
        }

        private void DeleteDataButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteDataHost.IsOpen = true;
        }

        private void ContinueDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteDataHost.IsOpen = false;
            FIleOperations fIleOperations = new FIleOperations(ProgramDataPath);
            fIleOperations.DeleteData();
            System.Windows.Application.Current.Shutdown();
        }

        private void CancelDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteDataHost.IsOpen = false;
        }
    }
}
