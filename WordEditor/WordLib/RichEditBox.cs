//====================================================================================================//
//      Copyright (C)  2021 ZhaoYang Co., Ltd. All rights reserved.                                   //
//====================================================================================================//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace ZYKJ.GreatWall
{
    public delegate void RichEditBoxDocumentChanged(object newValue);

    public partial class RichEditBox : RichTextBox
    {
        public RichEditBox()
        {
            Binding documentBinding = new Binding("Document") { Source = RelativeSource.Self, Mode = BindingMode.OneWay };
            SetBinding(RichEditBox.BindableDocumentProperty, documentBinding);
        }

        #region Dependence property
        public FlowDocument BindableDocument
        {
            get { return (FlowDocument)GetValue(BindableDocumentProperty); }
            set { SetValue(BindableDocumentProperty, value); }
        }
        public static readonly DependencyProperty BindableDocumentProperty =
            DependencyProperty.Register("BindableDocument", typeof(FlowDocument), typeof(RichEditBox), new PropertyMetadata(null, OnBindableDocumentChanged));

        private static void OnBindableDocumentChanged(DependencyObject dpObject, DependencyPropertyChangedEventArgs e)
        {
            RichEditBox editBox = dpObject as RichEditBox;
            editBox.Document = e.NewValue as FlowDocument;
            editBox.OnDocumentChanged?.Invoke(e.NewValue);
        }

        public RichEditBoxDocumentChanged OnDocumentChanged;
        #endregion
    }
}
