//====================================================================================================//
//      Copyright (C)  2019 ZhaoYang Co., Ltd. All rights reserved.                                   //
//====================================================================================================//
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ZYKJ.GreatWall
{
    public static class WordModel
    {
        #region Data
        //private const string HelpFooterTitle = "Press F1 for more help.";
        //private static object _lockObject = new object();
        //private static Dictionary<string, ControlData> _dataCollection = new Dictionary<string, ControlData>();
        //// Store any data that doesnt inherit from ControlData
        //private static Dictionary<string, object> _miscData = new Dictionary<string, object>();
        #endregion Data

        //public static ControlData TextHighlightColor
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "Text Highlight Color";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                string ToolTipTitle = "Text Highlight Color";
        //                string ToolTipDescription = "Make the text look like it was marked with a highlighter pen.";

        //                SplitMenuItemData splitMenuItemData = new SplitMenuItemData()
        //                {
        //                    SmallImage = new Uri("/RibbonWindowSample;component/Images/Highlight_16x16.png", UriKind.Relative),
        //                    ToolTipTitle = ToolTipTitle,
        //                    ToolTipDescription = ToolTipDescription,
        //                    Command = new PreviewDelegateCommand<Brush>(ChangeTextHighlightColor, CanChangeTextHighlightColor, PreviewTextHighlightColor, CancelPreviewTextHighlightColor),
        //                    KeyTip = "I",
        //                };

        //                _dataCollection[Str] = splitMenuItemData;
        //            }

        //            return _dataCollection[Str];

        //        }
        //    }
        //}

        public static ControlData TextHighlightColorGallery
        {
            get
            {
                GalleryData<Brush> galleryData = new GalleryData<Brush>()
                {
                    SmallImage = new Uri("/RibbonWindowSample;component/Images/Highlight_16x16.png", UriKind.Relative),
                    //Command = new PreviewDelegateCommand<Brush>(ChangeTextHighlightColor, CanChangeTextHighlightColor, PreviewTextHighlightColor, CancelPreviewTextHighlightColor),
                    SelectedItem = SystemColors.ControlBrush,
                };

                GalleryCategoryData<Brush> galleryCategoryData = new GalleryCategoryData<Brush>();
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Yellow);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Green);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Turquoise);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Pink);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Blue);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Red);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.DarkBlue);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Teal);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Green);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Violet);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.DarkRed);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.DarkOrange);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.DarkSeaGreen);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Aqua);
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Gray);

                galleryData.CategoryDataCollection.Add(galleryCategoryData);

                galleryCategoryData = new GalleryCategoryData<Brush>()
                {
                    Label = "No Color"
                };
                galleryCategoryData.GalleryItemDataCollection.Add(Brushes.Transparent);
                galleryData.CategoryDataCollection.Add(galleryCategoryData);
                return galleryData;
            }
        }

        //public static ControlData StopHighlighting
        //{
        //    get
        //    {
        //        lock (_lockObject)
        //        {
        //            string Str = "_Stop Highlighting";

        //            if (!_dataCollection.ContainsKey(Str))
        //            {
        //                _dataCollection[Str] = new MenuItemData()
        //                {
        //                    Label = Str,
        //                    SmallImage = new Uri("/RibbonWindowSample;component/Images/Default_16x16.png", UriKind.Relative),
        //                    Command = new DelegateCommand(DefaultExecuted, DefaultCanExecute),
        //                };
        //            }

        //            return _dataCollection[Str];
        //        }
        //    }
        //}
    }
}
