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
    /// InsertTable.xaml 的交互逻辑
    /// </summary>
    public partial class InsertTable : Window
    {
        public InsertTable()
        {
            InitializeComponent();
            RowCount.Focus();
            btnDefault.Click += BtnDefault_Click;
        }

        private void BtnDefault_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


    }
}
