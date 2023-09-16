//====================================================================================================//
//      Copyright (C)  2019 ZhaoYang Co., Ltd. All rights reserved.                                   //
//====================================================================================================//

using Microsoft.Win32;
using System;
using System.IO;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace ZYKJ.GreatWall
{
    public class RichTextBoxEx : RichTextBox
    {
        private PrintDialog printDialog = new PrintDialog();

        private String SaveFileName { get; set; } = string.Empty;
        private bool DocumentChanged { get; set; } = false;

        public RichTextBoxEx()
        {
            this.AcceptsTab = true;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.TextChanged += ZYRichTextBox_TextChanged;
            this.SelectionChanged += ZYRichTextBox_SelectionChanged;
            this.IsInactiveSelectionHighlightEnabled = true;
            this.Loaded += RichTextBox_Loaded;
        }

        private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            this.SelectionBrush = SystemColors.HotTrackBrush;
        }

        protected void ClearUndoRedo()
        {
            int limit = this.UndoLimit;
            this.UndoLimit = 0;
            this.UndoLimit = limit;
            DocumentChanged = false;
        }

        private void ZYRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            UpdateUIControlStatus(sender, e);
        }

        private void ZYRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DocumentChanged = true;
            UpdateUIControlStatus(sender, e);
        }

        private void CreateTable_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CreateTable_Executed();
        }

        public void CreateTable_Executed()
        {
            InsertTable insertTable = new InsertTable();
            insertTable.ShowInTaskbar = false;
            insertTable.Owner = Application.Current.MainWindow;
            insertTable.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            if (insertTable.ShowDialog() == true)
            {
                int rowCount, columnCount;
                if (int.TryParse(insertTable.RowCount.Text, out rowCount) && int.TryParse(insertTable.ColumnCount.Text, out columnCount))
                {
                    Table table = CreateTable(rowCount, columnCount);
                    Document.Blocks.Add(table);
                }
                else
                {
                    MessageBox.Show("请输入合法的行列数！", "错误：",MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                
            }
        }

        private Table CreateTable(int rows, int columns)
        {
            Table table = new Table()
            {
                CellSpacing = 0,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1, 1, 0, 0)
            };

            table.RowGroups.Add(new TableRowGroup());
            TableRow currentRow = null;
            for(int i = 0; i < rows; i++)
            {
                currentRow = new TableRow();
                table.RowGroups[0].Rows.Add(currentRow);
                for (int j = 0; j < columns; j++)
                {   
                    TableCell cell = new TableCell() { BorderThickness = new Thickness(0, 0, 1, 1), BorderBrush = Brushes.Black };
                    currentRow.Cells.Add(cell);
                }
            }
            return table;
        }

        private void CreateTable_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CreateTable_CanExecute();
        }

        public bool CreateTable_CanExecute()
        {
            return IsEnabled && !IsReadOnly;
        }

        public void DateTimeList_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DateTimeList_Executed();
        }

        public void DateTimeList_Executed()
        {
            DateTimeList dateTimeList = new DateTimeList();
            dateTimeList.ShowInTaskbar = false;
            dateTimeList.Owner = Application.Current.MainWindow;
            dateTimeList.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (dateTimeList.ShowDialog() == true && dateTimeList.DateTimeSelection != null)
            {
                this.Selection.Text = dateTimeList.DateTimeSelection;
            }
        }
        public void DateTimeList_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DateTimeList_CanExecute();
        }

        public bool DateTimeList_CanExecute()
        {
            return IsEnabled && !IsReadOnly; 
        }

        public void PrintPreview_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintPreview_Executed();
        }
        public void PrintPreview_Executed()
        {
            PrintPreview printPreview = new PrintPreview();
            printPreview.ShowInTaskbar = false;
            printPreview.Owner = Application.Current.MainWindow;
            printPreview.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            try
            {
                // Load the XPS content into memory.
                using (MemoryStream ms = new MemoryStream())
                {
                    using (Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite))
                    {
                        Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");

                        if (PackageStore.GetPackage(DocumentUri) != null)
                        {
                            PackageStore.RemovePackage(DocumentUri);
                        }
                        PackageStore.AddPackage(DocumentUri, package);

                        XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);
                        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);

                        FlowDocument viewDocument = DuplicateFlowDocument(Document);
                        writer.Write(((IDocumentPaginatorSource)viewDocument).DocumentPaginator);

                        printPreview.DocViewer.Document = xpsDocument.GetFixedDocumentSequence();
                        printPreview.ShowDialog();
                         
                        xpsDocument.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ExceptionManager.DefaultHandler(ex), "打印预览", MessageBoxButton.OK);
            }
        }

        public void PrintPreview_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PrintPreview_CanExecute();
        }
        public bool PrintPreview_CanExecute()
        {
            return Document != null;
        }

        public void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Print_Executed();
        }
        public void Print_Executed()
        {
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintDocument((((IDocumentPaginatorSource)Document).DocumentPaginator), "printing via zyeditor");
            }
        }

        public void Print_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Print_CanExecute();
        }

        public bool Print_CanExecute()
        {
            return Document != null;
        }

        public void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveAs_Executed();
        }
        public void SaveAs_Executed()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = SaveFileName;
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|Text File (*.txt)|*.txt|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                SaveFile(dlg.FileName);
            }
        }

        public void SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SaveAs_CanExecute();
        }

        public bool SaveAs_CanExecute()
        {
            TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
            return !tr.IsEmpty && tr.Text != $"\r\n" && DocumentChanged;
        }

        public void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save_Executed();
        }
        public void Save_Executed()
        {
            if (SaveFileName == string.Empty)
            {
                SaveAs_Executed(null, null);
            }
            else
            {
                SaveFile(SaveFileName);
            }
        }

        private FlowDocument DuplicateFlowDocument(FlowDocument source)
        {
            FlowDocument newDocument = new FlowDocument();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    TextRange range1 = new TextRange(source.ContentStart, source.ContentEnd);
                    range1.Save(ms, DataFormats.XamlPackage);

                    TextRange range2 = new TextRange(newDocument.ContentStart, newDocument.ContentEnd);
                    range2.Load(ms, DataFormats.XamlPackage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ExceptionManager.DefaultHandler(ex), "复制流文件", MessageBoxButton.OK);
            }

            return newDocument;
        }

        public void SaveFile(string fileName)
        {
            try
            {
                FileStream fileStream = new FileStream(fileName, FileMode.Create);
                TextRange range = new TextRange(Document.ContentStart, Document.ContentEnd);

                string format;
                string extension = Path.GetExtension(fileName);

                if (string.Compare(extension, ".rtf", true) == 0)
                    format = DataFormats.Rtf;
                else if (string.Compare(extension, ".xaml", true) == 0)
                    format = DataFormats.Xaml;
                else if (string.Compare(extension, ".txt", true) ==0)
                    format = DataFormats.Text;
                else
                    format = DataFormats.Rtf;

                range.Save(fileStream, format);
                ClearUndoRedo();
                SaveFileName = fileName;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ExceptionManager.DefaultHandler(ex), "保存文件", MessageBoxButton.OK);
            }
        }

        public void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Save_CanExecute();
        }

        public bool Save_CanExecute()
        {
            TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
            return !tr.IsEmpty && tr.Text != $"\r\n" && DocumentChanged;
        }

        public void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Open_Executed();
        }
        public void Open_Executed()
        {
            if (DocumentChanged)
            {
                MessageBoxResult result = MessageBox.Show($"文件内容已修改，要保存吗？", "保存文件", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    Save_Executed();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = SaveFileName;
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|XAML Files (*.xaml)|*.xaml|Text File (*.txt)|*.txt|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                    FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                    FlowDocument newDocument = new FlowDocument();
                    TextRange range = new TextRange(newDocument.ContentStart, newDocument.ContentEnd);

                    string format = string.Empty;
                    string extension = Path.GetExtension(dlg.FileName);

                    if (string.Compare(extension, ".rtf", true) == 0)
                        format = DataFormats.Rtf;
                    else if (string.Compare(extension, ".xaml", true) == 0)
                        format = DataFormats.Xaml;
                    else if (string.Compare(extension, ".txt", true) == 0)
                        format = DataFormats.Text;
                    else
                        format = DataFormats.Rtf;

                    range.Load(fileStream, format);
                    this.Document = newDocument;
                    ClearUndoRedo();
                    SaveFileName = dlg.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ExceptionManager.DefaultHandler(ex), "打开文件", MessageBoxButton.OK);
                }
            }
        }

        private void Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Open_CanExecute();
        }

        public bool Open_CanExecute()
        {
            return IsEnabled && !IsReadOnly;
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            New_Executed();
        }
        public void New_Executed()
        {
            if (DocumentChanged)
            {
                MessageBoxResult result = MessageBox.Show($"文件内容已修改，要保存吗？", "保存文件", MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                {
                    Save_Executed(null, null);
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    return;
                }
            }

            Document = new FlowDocument();
            ClearUndoRedo();
            SaveFileName = String.Empty;
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = New_CanExecute();
        }

        public bool New_CanExecute()
        {
            return IsEnabled && !IsReadOnly;
        }

        public void OnClose()
        {
            if (DocumentChanged)
            {
                MessageBoxResult result = MessageBox.Show($"文件内容已修改，要保存吗？", "保存文件", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Save_Executed(null, null);
                }
            }
        }

        private void Replace_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Replace_Executed();
        }
        public void Replace_Executed()
        {
            if (this.FindName("替换") == null)
            {
                FindReplaceDialog FindDialog = new FindReplaceDialog(this);
                FindDialog.ReplaceText();
            }
        }

        private void Replace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Replace_CanExecute();
        }

        public bool Replace_CanExecute()
        {
            TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
            return !tr.IsEmpty && tr.Text != $"\r\n";
        }

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Find_Executed();
        }
        public void Find_Executed()
        {
            if(this.FindName("查找") == null)
            {
                FindReplaceDialog FindDialog = new FindReplaceDialog(this);
                FindDialog.FindText();
            }
        }

        private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Find_CanExecute();
        }
        public bool Find_CanExecute()
        {
            TextRange tr = new TextRange(Document.ContentStart, Document.ContentEnd);
            return !tr.IsEmpty && tr.Text != $"\r\n";
        }

        private void InsertPicture_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            InsertPicture_Executed();
        }
        public void InsertPicture_Executed()
        {
            Image image = new Image();

            OpenFileDialog dialog = new OpenFileDialog();
            string strFileName = String.Empty;

            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            dialog.Multiselect = false;
            dialog.Title = $"选择图形文件";
            dialog.Filter = $"图形文件(*.bmp, *.jpg, *.gif) | *.bmp; *.jpg; *.gif";

            if (true == dialog.ShowDialog())
            {
                try
                {
                    strFileName = dialog.FileName;
                    image.Source = new BitmapImage(new Uri(strFileName, UriKind.Relative));
                    image.Stretch = Stretch.None;

                    if (!Selection.IsEmpty)
                        Selection.Text = String.Empty;
                    new InlineUIContainer(image, Selection.Start);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ExceptionManager.DefaultHandler(ex), "插入图形", MessageBoxButton.OK);
                }
            }
        }

        private void InsertPicture_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = InsertPicture_CanExecute();
        }

        public bool InsertPicture_CanExecute()
        {
            return IsEnabled && !IsReadOnly;
        }

        public void ForegroudColor_SelectedColorChanged(Brush selectColor)
        {
            Selection.ApplyPropertyValue(TextElement.ForegroundProperty, selectColor);
        }

        public void HighlightColor_SelectedColorChanged(Brush selectColor)
        {
            Selection.ApplyPropertyValue(TextElement.BackgroundProperty, selectColor);
        }

        public void FontSizeList_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Selection.ApplyPropertyValue(TextElement.FontSizeProperty, e.NewValue);
        }

        public void FontFamilyList_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, e.NewValue);
        }

        public void RowSpacingGallery_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.Document.LineHeight = (double)e.NewValue;
        }

        private void Strikethrough_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Strikethrough_CanExecute();
        }

        public bool Strikethrough_CanExecute()
        {
            return IsEnabled && !IsReadOnly;
        }

        private void Strikethrough_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Strikethrough_Executed();
        }
        public void Strikethrough_Executed()
        {
            // The following code has been copied from microsoft's WPF source code (File "TextEditorCharacters.cs" | class "TextEditorCharacters" | Method "OnToggleUnderline" | Line "233-247")
            // and modified accordingly to accommodate "Strikethrough" feature

            Object propertyValue = Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            TextDecorationCollection textDecorations = propertyValue != DependencyProperty.UnsetValue ? (TextDecorationCollection)propertyValue : null;

            TextDecorationCollection toggledTextDecorations;
            if (!((textDecorations is TextDecorationCollection) && ((TextDecorationCollection)textDecorations).Count > 0))
            {
                toggledTextDecorations = TextDecorations.Strikethrough;
            }
            else if (!textDecorations.TryRemove(TextDecorations.Strikethrough, out toggledTextDecorations))
            {
                // TextDecorations.Underline was not present, so add it 
                toggledTextDecorations.Add(TextDecorations.Strikethrough);
            }

            Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, toggledTextDecorations);
        }

        private void UpdateUIControlStatus(object sender, RoutedEventArgs e)
        {
            RichTextBoxEx rtex = (RichTextBoxEx)sender;
            GCHandle handle = GCHandle.Alloc(rtex);
            IntPtr pobj = GCHandle.ToIntPtr(handle);

            Win32.SendMessage(Win32.GetHandle(Application.Current.MainWindow), Win32.WM_RICHTEXTBOX_SELECTION_CHANGED, pobj, (IntPtr)0);
        }
    }
}
