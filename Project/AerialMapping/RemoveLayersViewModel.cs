﻿//-----------------------------------------------------------------------
// <copyright file="FooViewModel.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------
namespace AerialMapping
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public class RemoveLayersViewModel : INotifyPropertyChanged
    {
        private bool? isChecked = false;

        private RemoveLayersViewModel parent;

        public RemoveLayersViewModel(string name, string filePath)
        {
            this.Name = name;
            this.FilePath = filePath;
            this.Children = new List<RemoveLayersViewModel>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<RemoveLayersViewModel> Children { get; private set; }

        public bool IsInitiallySelected { get; private set; }

        public string Name { get; private set; }

        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child FooViewModels.  Setting this property to true or false
        /// will set all children to the same check state, and setting it 
        /// to any value will cause the parent to verify its check state.
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                this.SetIsChecked(value, true, true);
            }
        }

        public static List<RemoveLayersViewModel> CreateFoos(ObservableCollection<MenuItem> locationsTimes)
        {
            RemoveLayersViewModel root = new RemoveLayersViewModel("All", string.Empty)
            {
                IsInitiallySelected = false,
                
                Children = new List<RemoveLayersViewModel>()
            };

            List<RemoveLayersViewModel> locations = new List<RemoveLayersViewModel>();

            foreach (MenuItem location in locationsTimes) 
            {
                RemoveLayersViewModel loc = new RemoveLayersViewModel(location.Title, location.FilePath);
                List<RemoveLayersViewModel> times = new List<RemoveLayersViewModel>();

                foreach (MenuItem time in location.Items)
                {
                    RemoveLayersViewModel t = new RemoveLayersViewModel(time.Title, time.FilePath);
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
            return new List<RemoveLayersViewModel> { root };
        }

        private void Initialize()
        {
            foreach (RemoveLayersViewModel child in this.Children)
            {
                child.parent = this;
                child.Initialize();
            }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == this.isChecked)
            {
                return;
            }

            this.isChecked = value;

            if (updateChildren && this.isChecked.HasValue)
            {
                this.Children.ForEach(c => c.SetIsChecked(this.isChecked, true, false));
            }

            if (updateParent && this.parent != null)
            {
                this.parent.VerifyCheckState();
            }

            this.OnPropertyChanged("IsChecked");
        }

        /// <summary>
        /// This function verifies the state of the checkboxes.
        /// </summary>
        private void VerifyCheckState()
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

        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}