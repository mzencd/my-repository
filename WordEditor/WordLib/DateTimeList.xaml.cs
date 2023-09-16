//====================================================================================================//
//      Copyright (C)  2019 ZhaoYang Co., Ltd. All rights reserved.                                   //
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZYKJ.GreatWall
{
    /// <summary>
    /// DateTimeList.xaml 的交互逻辑
    /// </summary>
    public partial class DateTimeList : Window
    {
        public string DateTimeSelection { get; set; } = string.Empty;

        public DateTimeList()
        {
            InitializeComponent();

            DTList.SelectionMode = SelectionMode.Single;
            btnDefault.Click += BtnDefault_Click;
            InitializeDateTimeList();
        }

        private void InitializeDateTimeList()
        {
            DateTime dateTime = DateTime.Now;
            IFormatProvider culture = new System.Globalization.CultureInfo("zh-Hans", true);

            string[] shortDateFormats = dateTime.GetDateTimeFormats('d', culture);
            foreach (string format in shortDateFormats)
                DTList.Items.Add(format);

            string[] longDateFormats = dateTime.GetDateTimeFormats('D', culture);
            foreach (string format in longDateFormats)
                DTList.Items.Add(format);

            string[] longTimeFormats = dateTime.GetDateTimeFormats('T', culture);
            foreach (string format in longTimeFormats)
                DTList.Items.Add(format);

            if (DTList.Items.Count > 0)
                DTList.SelectedIndex = 0;
            DTList.Focus();
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            DateTimeSelection = DTList.SelectedItem?.ToString();
            this.DialogResult = true;
        }
    }
}
