//-----------------------------------------------------------------------
// <copyright file="FooViewModel.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace AerialMapping
{
    public class FooViewModel : INotifyPropertyChanged
    {
        #region Data

        bool? _isChecked = false;
        FooViewModel _parent;

        #endregion // Data

        #region CreateFoos

        public static List<FooViewModel> CreateFoos(ObservableCollection<MenuItem> locationsTimes)
        {
            FooViewModel root = new FooViewModel("All", string.Empty)
            {
                IsInitiallySelected = false,
                
                Children = new List<FooViewModel>()
            };

            List<FooViewModel> locations = new List<FooViewModel>();

            foreach (MenuItem location in locationsTimes) 
            {
                FooViewModel loc = new FooViewModel(location.Title, location.FilePath);
                List<FooViewModel> times = new List<FooViewModel>();

                foreach (MenuItem time in location.Items)
                {
                    FooViewModel t = new FooViewModel(time.Title, time.FilePath);
                    times.Add(t);
                }

                loc.Children = times;
                locations.Add(loc);
            }

            root.Children = locations;

            //FooViewModel root = new FooViewModel("Weapons", "")
            //{
            //    IsInitiallySelected = true,
            //    Children =
            //    {
            //        new FooViewModel("Blades", "")
            //        {
            //            Children =
            //            {
            //                new FooViewModel("Dagger", ""),
            //                new FooViewModel("Machete", ""),
            //                new FooViewModel("Sword", ""),
            //            }
            //        },
            //        new FooViewModel("Vehicles", "")
            //        {
            //            Children =
            //            {
            //                new FooViewModel("Apache Helicopter", ""),
            //                new FooViewModel("Submarine", ""),
            //                new FooViewModel("Tank", ""),                            
            //            }
            //        },
            //        new FooViewModel("Guns", "")
            //        {
            //            Children =
            //            {
            //                new FooViewModel("AK 47", ""),
            //                new FooViewModel("Beretta", ""),
            //                new FooViewModel("Uzi", ""),
            //            }
            //        },
            //    }
            //};

            root.Initialize();
            return new List<FooViewModel> { root };
        }

        FooViewModel(string name, string filePath)
        {
            this.Name = name;
            this.FilePath = filePath;
            this.Children = new List<FooViewModel>();
        }

        void Initialize()
        {
            foreach (FooViewModel child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        #endregion // CreateFoos

        #region Properties

        public List<FooViewModel> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        public string FilePath { get; private set; }

        #region IsChecked

        /// <summary>
        /// Gets/sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get 
            { 
                return this._isChecked; 
            }

            set 
            { 
                this.SetIsChecked(value, true, true); 
            }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == this._isChecked)
            {
                return;
            }

            this._isChecked = value;

            if (updateChildren && this._isChecked.HasValue)
            {
                this.Children.ForEach(c => c.SetIsChecked(this._isChecked, true, false));
            }

            if (updateParent && this._parent != null)
            {
                this._parent.VerifyCheckState();
            }

            this.OnPropertyChanged("IsChecked");
        }

        void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        #endregion // IsChecked

        #endregion // Properties

        #region INotifyPropertyChanged Members

        void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}