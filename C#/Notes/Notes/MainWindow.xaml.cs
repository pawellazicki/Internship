﻿using Notes.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace Notes
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Remind remind = null;
        private int gridColumnIterator = 0;
        //private List<string> FilesPaths = new List<string>();
        private  Dictionary<string, Note> Notes = new Dictionary<string, Note>();
        public ObservableCollection<int> MinuteCollection { get; set; }
        public ObservableCollection<int> HoursCollection { get; set; }
        private string directoryPath = @"C:/notes";
        private string _noteStatus;
        public string NoteStatus
        {
            get
            {
                return _noteStatus;
            }
            set
            {
                _noteStatus = value;
                OnPropertyChanged(nameof(NoteStatus));
            }
        }
        private string _newNoteTitle;
        public string NewNoteTitle
        {
            get
            {
                return _newNoteTitle;
            }
            set
            {
                _newNoteTitle = value;
                OnPropertyChanged(nameof(NewNoteTitle));
            }
        }
        private string _newNoteTxt;
        public string NewNoteTxt
        {
            get
            {
                return _newNoteTxt;
            }
            set
            {
                _newNoteTxt = value;
                OnPropertyChanged(nameof(NewNoteTxt));
            }
        }
        


        // Main
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            NewNoteTitle = "Add note title...";
            NewNoteTxt = "Add note text...";
            
            FileOperations fileOperations = new FileOperations(directoryPath);

            if (!fileOperations.CreateDirectory())
            {
                Application.Current.Shutdown();
            }
            AddNewNotes();
        }

        private void myWindow_LocationChanged(object sender, EventArgs e)
        {
            Note note = Notes.Values.FirstOrDefault(x => x.popup.IsOpen == true);
            if (note != null)
                note.popup.IsOpen = false;

            if (remind != null)
            {
                var offset = remind.remindPopup.HorizontalOffset;
                remind.remindPopup.HorizontalOffset = offset + 1;
                remind.remindPopup.HorizontalOffset = offset;
            }
        }

        //////////////---> BUTTONS <---///////////////////////////
        // Buttons click
        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            DateTime time;
            string strTime = null;
            if (myTimePicker.SelectedTime != null)
            {
                time = (DateTime)myTimePicker.SelectedTime;
                strTime = time.ToString("HH:mm tt");
            }

            DateTime date;
            string strDate = null;
            if (myDatePicker.SelectedDate != null)
            {
                date = (DateTime)myDatePicker.SelectedDate;
                strDate = date.ToString("MM/dd/yyyy");
            }

            if (WriteNote(strDate, strTime))
            {
                NoteStatus = "New note added";
                StatusAnimation();
                remind = new Remind();
                //remind.remindPopup.IsOpen = true;
                remind.SetReminder(strDate, strTime, mainGrid, NewNoteTitle, NewNoteTxt);
                centerGrid.Children.Add(remind);
            }
            else
            {
                NoteStatus = "Canceled empty text";
                StatusAnimation();
            }
            TopPanelAnimationReverse();
            SetNoteDefaultTxt();
            AddNewNotes();
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            TopPanelAnimationReverse();
            SetNoteDefaultTxt();
            NoteStatus = "Canceled";
            StatusAnimation();
        }

        //private void DateButtonClick(object sender, RoutedEventArgs e)
        //{
        //    Console.WriteLine(myDatePicker.SelectedDate);
        //}

        // Buttons operations
        private void EnableButtons()
        {
            addButton.IsEnabled = true;
            clearButton.IsEnabled = true;
            myDatePicker.IsEnabled = true;
            myTimePicker.IsEnabled = true;
        }
        private void DisableButtons()
        {
            addButton.IsEnabled = false;
            clearButton.IsEnabled = false;
            myDatePicker.IsEnabled = false;
            myTimePicker.IsEnabled = false;
        }

        ////////////////---> MOUSE EVENTS <---///////////////////////
        private void TitleName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            EnableButtons();
            if(NewNoteTitle == "Add note title...")
                NewNoteTitle = string.Empty;
            TopPanelAnimation();
        }

        private void TxtName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(NewNoteTxt == "Add note text...")
                NewNoteTxt = string.Empty;
        }

        ////////////////---> HELPERS <---///////////////////////
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetNoteDefaultTxt()
        {
            NewNoteTxt = "Add note text...";
            NewNoteTitle = "Add note title...";
        }

        private bool WriteNote(string date, string time)
        {
            if (!String.IsNullOrEmpty(NewNoteTitle))
            {
                FileOperations fileOperations = new FileOperations(directoryPath);
                fileOperations.WriteNoteText(NewNoteTitle, $@"{NewNoteTxt}", date, time);
                return true;
            }
            return false;
        }

        private void AddNewNotes()
        {
            //NoteOperations noteOperations = new NoteOperations();
            FileOperations fileOperations = new FileOperations(directoryPath);
            List<string> files = fileOperations.ReadFilesPaths();

            foreach (string filePath in files)
            {
                if (!Notes.ContainsKey(filePath))
                {
                    // layout
                    var note = new Note();
                    note.ReadedText = fileOperations.ReadTextFromFile(filePath);
                    note.NoteTitle = Path.GetFileNameWithoutExtension(filePath);

                    note.removeButton.Click += RemoveNoteHandler;
                    note.modifyButton.Click += ModifyNoteHandler;

                    Notes.Add(filePath, note);

                    switch (GridColumnControl())
                    {
                        case 0:
                            scroll0.Children.Add(note);
                            break;
                        case 1:
                            scroll1.Children.Add(note);
                            break;
                        case 2:
                            scroll2.Children.Add(note);
                            break;
                    }
                    //Grid newGrid = noteOperations.CreateNewNoteGrid(Title, textReaded);

                    //ChooseColumn();
                }
                else
                {
                    var note = Notes[filePath];
                    note.ReadedText = fileOperations.ReadTextFromFile(filePath);
                    note.NoteTitle = Path.GetFileNameWithoutExtension(filePath);
                }
            }
        }

        // animations
        private void TopPanelAnimation()
        {
            var resource = myWindow.Resources["TopPanelAnimation"] as Storyboard;
            resource?.Begin();

            addButton.Content = "Add";
        }

        private void TopPanelAnimationModify()
        {
            NoteStatus = "Note saved";
            var resource = myWindow.Resources["TopPanelAnimation"] as Storyboard;
            resource?.Begin();

            addButton.Content = "Save";
        }

        private void TopPanelAnimationReverse()
        {
            var resource = myWindow.Resources["TopPanelAnimationReverse"] as Storyboard;
            resource?.Begin();
        }

        private void StatusAnimation()
        {
            var resource = myWindow.Resources["StatusAnimation"] as Storyboard;
            resource?.Begin();
        }

        // grid column controler
        private int GridColumnControl()
        {
            if (++gridColumnIterator == 3)
                gridColumnIterator = 0;
            return gridColumnIterator;
        }

        ////////////// ---> HANDLERS <--- /////////////////////
        private void ModifyNoteHandler(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            FileOperations fileOperations = new FileOperations(directoryPath);
            
            if (button != null)
            {
                var note = Notes.Values.FirstOrDefault(x => x.modifyButton == button);

                if (note != null)
                {
                    //var parent = note.Parent as Panel;
                    string notePath = Notes.First(x => x.Value == note).Key;

                    EnableButtons();
                    TopPanelAnimationModify();
                    fileOperations.RemoveTimeFile(notePath);
                    NewNoteTitle = Path.GetFileNameWithoutExtension(notePath);
                    NewNoteTxt = fileOperations.ReadTextFromFileWithoutDate(notePath);

                }
            }
        }

        private void RemoveNoteHandler(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                var note = Notes.Values.FirstOrDefault(x => x.removeButton == button);

                if (note != null)
                {
                    var parent = note.Parent as Panel;
                    parent.Children.Remove(note);
                    string noteKey = Notes.First(x => x.Value == note).Key;
                    File.Delete(noteKey);
                    Notes.Remove(noteKey);
                    NoteStatus = "Note removed";
                    StatusAnimation();
                }
            }
        }

        private void myWindow_StateChanged(object sender, EventArgs e)
        {
            if(myWindow.WindowState == WindowState.Minimized)
            {
                Note note = Notes.Values.FirstOrDefault(x => x.popup.IsOpen == true);
                note.popup.IsOpen = false;

                remind.remindPopup.IsOpen = false; 
            }
        }

        private void titleField_TextChanged()
        {

        }
    }
}