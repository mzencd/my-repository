using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ZYKJ.GreatWall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            //FontFamily = new FontFamily("Times New Roman");
            //FontSize = 18;

            ForegroundColor.Margin = HighlightColor.Margin = new Thickness(0,2,0,2);
            ForegroundColor.Width = HighlightColor.Width = SystemParameters.MenuButtonWidth * 2;

            FontFamilyItems.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            FontFamilyGallery.SelectedItem = FontFamily;

            FontSizeItems.Width = SystemParameters.MenuButtonWidth * 2;
            List<double> items = new List<double>(14){ 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 36, 48, 72 };
            FontSizeItems.ItemsSource = items;
            FontSizeGallery.SelectedValue = FontSize;

            List<double> spacing = new List<double>(4) { 1.0, 1.15, 1.5, 2, 2.5, 3.0, double.NaN };
            RowSpacingItems.ItemsSource = spacing;

            InitializeEditor();
            
            SourceInitialized += MainWindow_SourceInitialized;
        }

        private void InitializeEditor()
        {
            RegisterRichEditCommand();
            Editor.IsEditing = true;
            Editor.Focus();

            //Figure figure = new Figure();
            //Label label = new Label();
            //label.Content = "This is a test";
            //BlockUIContainer bu = new BlockUIContainer(label);

            //figure.Blocks.Add(bu);
            //figure.HorizontalOffset = 50;
            //figure.VerticalOffset = 100;

            //Editor.Document.Blocks.Add(figure);

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Editor.DataEdit.OnClose();
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            Win32.AddWindowProcedureHook(this, HwndSourceHook);
        }

        public IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WM_RICHTEXTBOX_SELECTION_CHANGED:
                    {
                        RichTextBoxEx rtex = null;
                        if (wParam != null)
                        {
                            GCHandle gch = GCHandle.FromIntPtr(wParam);
                            rtex = (RichTextBoxEx)gch.Target;
                            gch.Free();
                        }

                        OnRichTextBoxSelectionChanged(rtex);
                        break;
                    }
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        private void RegisterRichEditCommand()
        {
            RegisterSystemDefaultEditCommand();

            SolutionAPI.BindingUICommand(New, Editor, "New", Key.N, ModifierKeys.Control, New_Executed, New_CanExecute);
            SolutionAPI.BindingUICommand(Open, Editor, "Open", Key.O, ModifierKeys.Control, Open_Executed, Open_CanExecute);
            SolutionAPI.BindingUICommand(Save, Editor, "Save", Key.S, ModifierKeys.Control, Save_Executed, Save_CanExecute);
            SolutionAPI.BindingUICommand(SaveAs, Editor, "SaveAs", Key.S, ModifierKeys.Control | ModifierKeys.Alt, SaveAs_Executed, SaveAs_CanExecute);
            SolutionAPI.BindingUICommand(Print, Editor, "Print", Key.P, ModifierKeys.Control, Print_Executed, Print_CanExecute);
            SolutionAPI.BindingUICommand(PrintPreview, Editor, "PrintPreview", Key.V, ModifierKeys.Control, PrintPreview_Executed, PrintPreview_CanExecute);
            SolutionAPI.BindingUICommand(ToggleStrikethrough, Editor, "Strikethrough", Key.S, ModifierKeys.Control | ModifierKeys.Shift, Strikethrough_Executed, Strikethrough_CanExecute);
            SolutionAPI.BindingUICommand(InsertPicture, Editor, "Insert Picture", Key.P, ModifierKeys.Control, InsertPicture_Executed, InsertPicture_CanExecute);
            SolutionAPI.BindingUICommand(DateTimeList, Editor, "DateTime List", Key.T, ModifierKeys.Control, DateTimeList_Executed, DateTimeList_CanExecute);
            SolutionAPI.BindingUICommand(InsertTable, Editor, "Insert Table", Key.I, ModifierKeys.Control, CreateTable_Executed, CreateTable_CanExecute);
            SolutionAPI.BindingUICommand(Find, Editor, "Find", Key.F, ModifierKeys.Control, Find_Executed, Find_CanExecute);
            SolutionAPI.BindingUICommand(Replace, Editor, "Replace", Key.H, ModifierKeys.Control, Replace_Executed, Replace_CanExecute);

            ForegroundColor.SelectedColorChanged += Editor.ForegroudColor_SelectedColorChanged;
            HighlightColor.SelectedColorChanged += Editor.HighlightColor_SelectedColorChanged;
            FontFamilyGallery.SelectionChanged += Editor.FontFamilyList_SelectionChanged;
            FontSizeGallery.SelectionChanged += Editor.FontSizeList_SelectionChanged;
            RowSpacingGallery.SelectionChanged += Editor.RowSpacingGallery_SelectionChanged;
        }

        private void RegisterSystemDefaultEditCommand()
        {
            Copy.Command = ApplicationCommands.Copy;
            Cut.Command = ApplicationCommands.Cut;
            Paste.Command = ApplicationCommands.Paste;
            Undo.Command = ApplicationCommands.Undo;
            Redo.Command = ApplicationCommands.Redo;

            ToggleBold.Command = EditingCommands.ToggleBold;
            ToggleItalic.Command = EditingCommands.ToggleItalic;
            ToggleUnderline.Command = EditingCommands.ToggleUnderline;
            ToggleSubscript.Command = EditingCommands.ToggleSubscript;
            ToggleSuperscript.Command = EditingCommands.ToggleSuperscript;

            IncreaseFontSize.Command = EditingCommands.IncreaseFontSize;
            DecreaseFontSize.Command = EditingCommands.DecreaseFontSize;

            IncreaseIndentation.Command = EditingCommands.IncreaseIndentation;
            DecreaseIndentation.Command = EditingCommands.DecreaseIndentation;
            ToggleNumbering.Command = EditingCommands.ToggleNumbering;
            ToggleBullets.Command = EditingCommands.ToggleBullets;
            ToggleAlignLeft.Command = EditingCommands.AlignLeft;
            ToggleAlignCenter.Command = EditingCommands.AlignCenter;
            ToggleAlignRight.Command = EditingCommands.AlignRight;
            ToggleAlignJustify.Command = EditingCommands.AlignJustify;
            //ToggleSpellingCheck.Command = EditingCommands.CorrectSpellingError;

            SelectAll.Command = ApplicationCommands.SelectAll;
        }

        public void OnRichTextBoxSelectionChanged(object sender)
        {
            if (sender != null && sender is RichTextBoxEx)
                OnRichTextBoxSelectionChanged(sender);
        }

        private void OnRichTextBoxSelectionChanged(RichTextBoxEx richTextBox)
        {
            Object value = richTextBox.Selection.GetPropertyValue(Inline.FontWeightProperty);
            if (value != null)
                ToggleBold.IsChecked = (value != DependencyProperty.UnsetValue) && (value.Equals(FontWeights.Bold));

            value = richTextBox.Selection.GetPropertyValue(Inline.FontStyleProperty);
            if (value != null)
                ToggleItalic.IsChecked = (value != DependencyProperty.UnsetValue) && (value.Equals(FontStyles.Italic));

            value = richTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (value != null)
                ToggleUnderline.IsChecked = (value != DependencyProperty.UnsetValue) && (value.Equals(TextDecorations.Underline));

            value = richTextBox.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            if (value != null)
                FontFamilyGallery.SelectedItem = value;
            value = richTextBox.Selection.GetPropertyValue(Inline.FontSizeProperty);
            if (value != null)
                FontSizeGallery.SelectedItem = value;

            value = richTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            if (value != null)
                ToggleStrikethrough.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextDecorations.Strikethrough);

            value = richTextBox.Selection.GetPropertyValue(Typography.VariantsProperty);
            if (value != null)
            {
                ToggleSubscript.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(FontVariants.Subscript);
                ToggleSuperscript.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(FontVariants.Superscript);
            }

            value = richTextBox.Selection.GetPropertyValue(Block.TextAlignmentProperty);
            if (value != null)
            {
                ToggleAlignLeft.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextAlignment.Left);
                ToggleAlignCenter.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextAlignment.Center);
                ToggleAlignRight.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextAlignment.Right);
                ToggleAlignJustify.IsChecked = (value != DependencyProperty.UnsetValue) && value.Equals(TextAlignment.Justify);
            }

            RowSpacingGallery.SelectedItem = richTextBox.Document.LineHeight;
        }

        public void CreateTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_CreateTable_CanExecute();
        }
        public void CreateTable_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_CreateTable_Executed();
        }
        public void DateTimeList_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_DateTimeList_CanExecute();
        }
        public void DateTimeList_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_DateTimeList_Executed();
        }
        public void PrintPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_PrintPreview_CanExecute();
        }
        public void PrintPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_PrintPreview_Executed();
        }
        public void Print_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_Print_CanExecute();
        }
        public void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_Print_Executed();
        }
        public void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_SaveAs_CanExecute();
        }
        public void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_SaveAs_Executed();
        }
        public void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_Save_CanExecute();
        }
        public void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_Save_Executed();
        }
        public void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_Open_CanExecute();
        }
        public void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_Open_Executed();
        }
        public void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_New_CanExecute();
        }
        public void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_New_Executed();
        }
        public void Replace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_Replace_CanExecute();
        }
        public void Replace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_Replace_Executed();
        }
        public void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_Find_CanExecute();
        }
        public void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_Find_Executed();
        }
        public void InsertPicture_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_InsertPicture_CanExecute();
        }
        public void InsertPicture_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_InsertPicture_Executed();
        }
        public void Strikethrough_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Editor.Edit_Strikethrough_CanExecute();
        }
        public void Strikethrough_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Editor.Edit_Strikethrough_Executed();
        }

        private void HighlightColor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RibbonGalleryItem galleryItem = sender as RibbonGalleryItem;
            object content = galleryItem.Content;

            if (content is Rectangle rectangle)
            {
                HighlightColor.Background = rectangle.Fill;
            }
            else if (content is Grid)
            {
                object child = (content as Grid).Children[0];
                if (child is Rectangle rectangle1)
                {
                    HighlightColor.Background = rectangle1.Fill;
                }
            }

            HighlightColor_Click(null, null);
        }

        private void HighlightColor_Click(object sender, RoutedEventArgs e)
        {
            Editor.HighlightColor_SelectedColorChanged(HighlightColor.Background);
        }

        private void ForegroundColor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RibbonGalleryItem galleryItem = sender as RibbonGalleryItem;
            object content = galleryItem.Content;

            if (content is Rectangle rectangle)
            {
                ForegroundColor.Background = rectangle.Fill;
            }
            else if (content is Grid)
            {
                object child = (content as Grid).Children[0];
                if (child is Rectangle rectangle1)
                {
                    ForegroundColor.Background = rectangle1.Fill;
                }
            }

            ForegroundColor_Click(null, null);
        }

        private void ForegroundColor_Click(object sender, RoutedEventArgs e)
        {
            Editor.ForegroudColor_SelectedColorChanged(ForegroundColor.Background);
        }
    }
}