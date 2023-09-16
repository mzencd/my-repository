//====================================================================================================//
//      Copyright (C)  2019 ZhaoYang Co., Ltd. All rights reserved.                                   //
//====================================================================================================//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZYKJ.GreatWall
{
    /// <summary>
    /// FindReplace.xaml 的交互逻辑
    /// </summary>
    public partial class FindReplaceDialog : Window
    {
        private readonly RichTextBox Editor;
        private TextPointer SearchPointer { get; set; }

        public FindReplaceDialog(RichTextBox richTextBox)
        {
            Editor = richTextBox;

            InitializeComponent();
            Owner = Application.Current.MainWindow;
            Loaded += FindReplaceDialog_Loaded;
            Unloaded += FindReplaceDialog_Unloaded;

            btnCancel.Click += BtnCancel_Click;
            btnFind.Click += BtnFind_Click;
            btnReplace.Click += BtnReplace_Click;
            btnReplaceAll.Click += BtnReplaceAll_Click;

            Binding TextLengthBinding = new Binding("Text");
            TextLengthBinding.Source = txtFind;
            TextLengthBinding.Converter = new TextLengthConverter();

            btnFind.SetBinding(Button.IsEnabledProperty, TextLengthBinding);
            btnReplace.SetBinding(Button.IsEnabledProperty, TextLengthBinding);
            btnReplaceAll.SetBinding(Button.IsEnabledProperty, TextLengthBinding);
            
            FindStringMethod = Find;
            ReplaceStringMethod = Replace;
        }

        private void Editor_KeyUp(object sender, KeyEventArgs e)
        {
            SearchPointer = Editor.Selection.Start;
        }

        private void Editor_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // NOTE: After testing, seems at this point the SearchPointer can not get value properly
            // SearchPointer = Editor.Selection.Start;
            // So, simply disable it, it will be recovered after find/replace dialog being colsed
            e.Handled = true;
        }

        private void Editor_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchPointer = Editor.Selection.Start;
        }

        private void FindReplaceDialog_Loaded(object sender, RoutedEventArgs e)
        {
            Editor.PreviewMouseLeftButtonUp += Editor_PreviewMouseLeftButtonUp;
            Editor.PreviewMouseRightButtonUp += Editor_PreviewMouseRightButtonUp;
            Editor.KeyUp += Editor_KeyUp;

            txtFind.Focus();
            SearchPointer = Editor.Selection.Start;
        }

        private void FindReplaceDialog_Unloaded(object sender, RoutedEventArgs e)
        {
            Editor.PreviewMouseLeftButtonUp -= Editor_PreviewMouseLeftButtonUp;
            Editor.PreviewMouseRightButtonUp -= Editor_PreviewMouseRightButtonUp;
            Editor.KeyUp -= Editor_KeyUp;
        }

        public delegate bool FindString(RichTextBox editor, string strFind, bool isWholeWord, bool isMatchCase);
        public delegate bool ReplaceString(RichTextBox editor, string strFind, string strReplace, bool isWholeWord, bool isMatchCase, bool isReplaceAll);
        public FindString FindStringMethod;
        public ReplaceString ReplaceStringMethod;

        private void BtnReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            if(Editor != null)
                ReplaceStringMethod(Editor, txtFind.Text, txtReplace.Text, (bool)btnWholeWord.IsChecked, (bool)btnMatchCase.IsChecked, true);
        }

        private void BtnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
                ReplaceStringMethod(Editor, txtFind.Text, txtReplace.Text, (bool)btnWholeWord.IsChecked, (bool)btnMatchCase.IsChecked, false);
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            if (Editor != null)
                FindStringMethod(Editor, txtFind.Text, (bool)btnWholeWord.IsChecked,(bool)btnMatchCase.IsChecked);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

         public void ClearContents()
        {
            txtFind.Text = string.Empty;
            txtReplace.Text = string.Empty;
            btnWholeWord.IsChecked = false;
            btnMatchCase.IsChecked = false;
        }

       public void FindText()
        {
            WindowCollection collection = Application.Current.Windows;
            foreach (Window wnd in collection)
            {
                if(wnd.Title == "替换")
                {
                    wnd.Close();
                    break;
                }
                else if (wnd.Title == "查找" && !wnd.IsFocused)
                {
                    wnd.Focus();
                    return;
                }
            }

            Title = "查找";
            labReplace.Visibility = Visibility.Collapsed;
            txtReplace.Visibility = Visibility.Collapsed;
            btnReplace.Visibility = Visibility.Collapsed;
            btnReplaceAll.Visibility = Visibility.Collapsed;

            btnWholeWord.SetValue(Grid.RowProperty, 1);
            btnMatchCase.SetValue(Grid.RowProperty, 2);
            btnCancel.SetValue(Grid.RowProperty, 1);

            MainGrid.RowDefinitions.ElementAt(3).Height = GridLength.Auto;
            MainGrid.RowDefinitions.ElementAt(4).Height = GridLength.Auto;
            Height = 160;

            ClearContents();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            txtFind.Text = Editor.Selection.Text;
            txtFind.SelectAll();

            Show();
        }

        public void ReplaceText()
        {
            WindowCollection collection = Application.Current.Windows;
            foreach (Window wnd in collection)
            {
                if(wnd.Title == "查找")
                {
                    wnd.Close();
                    break;
                }
                else if (wnd.Title == "替换" && !wnd.IsFocused)
                {
                    wnd.Focus();
                    return;
                }
            }

            Title = "替换";
            ClearContents();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            txtFind.Text = Editor.Selection.Text;
            txtFind.SelectAll();

            Show();
        }

        protected bool Find(RichTextBox editor, string strFind, bool isWholeWord, bool isMatchCase)
        {
            if (strFind == string.Empty)
                return false;

            StringComparison sc;
            if (isWholeWord)
            {
                sc = isMatchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
            }
            else
            {
                sc = isMatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            }
            TextRange range = new TextRange(SearchPointer, editor.Document.ContentEnd);

            bool success = false;
            int index = range.Text.IndexOf(strFind, sc);
            if(index != -1)
            {
                TextPointer start = range.Start.GetPositionAtOffset(index);
                SearchPointer = range.Start.GetPositionAtOffset(index + strFind.Length);
                Editor.Selection.Select(start, SearchPointer);
                success = true;
            }

            Editor.Focus();
            this.Focus();

            return success;
        }

        protected bool Replace(RichTextBox editor, string strFind, string strReplace, bool isWholeWord, bool isMatchCase, bool isReplaceAll)
        {
            if (strFind == string.Empty)
                return false;

            bool isReplacedOne = false;
            bool isFindNext = false;

            if(isReplaceAll)
            {
                Editor.Selection.Select(editor.Document.ContentStart, editor.Document.ContentStart);
            }

            do
            {
                CompareOptions co = isMatchCase ? CompareOptions.None : CompareOptions.IgnoreCase;

                if (string.Compare(strFind, Editor.Selection.Text, CultureInfo.CurrentCulture, co) == 0)
                {
                    Editor.Selection.Text = (strReplace == string.Empty) ? string.Empty : Editor.Selection.Text.Replace(strFind, strReplace);
                    Editor.Selection.Select(Editor.Selection.End, Editor.Selection.End);
                    isReplacedOne = true;
                }

                SearchPointer = Editor.Selection.End;
                Editor.Selection.Select(SearchPointer, SearchPointer);
                isFindNext = Find(editor, strFind, isWholeWord, isMatchCase);
            } while (isFindNext && isReplaceAll);

            return isReplacedOne;
        }
    }
}
