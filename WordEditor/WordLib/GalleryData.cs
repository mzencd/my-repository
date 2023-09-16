//====================================================================================================//
//      Copyright (C)  2019 ZhaoYang Co., Ltd. All rights reserved.                                   //
//====================================================================================================//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ZYKJ.GreatWall
{
    public class GalleryData<T> : ControlData
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<GalleryCategoryData<T>> CategoryDataCollection
        {
            get
            {
                if (_controlDataCollection == null)
                {
                    _controlDataCollection = new ObservableCollection<GalleryCategoryData<T>>();
                }
                return _controlDataCollection;
            }
        }
        private ObservableCollection<GalleryCategoryData<T>> _controlDataCollection;

        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (!Object.Equals(value, _selectedItem))
                {
                    _selectedItem = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
                }
            }
        }
        T _selectedItem;

        public bool CanUserFilter
        {
            get
            {
                return _canUserFilter;
            }

            set
            {
                if (_canUserFilter != value)
                {
                    _canUserFilter = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CanUserFilter"));
                }
            }
        }

        private bool _canUserFilter;
    }

    public class GalleryCategoryData<T> : ControlData
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<T> GalleryItemDataCollection
        {
            get
            {
                if (_controlDataCollection == null)
                {
                    _controlDataCollection = new ObservableCollection<T>();
                }
                return _controlDataCollection;
            }
        }
        private ObservableCollection<T> _controlDataCollection;
    }
}
