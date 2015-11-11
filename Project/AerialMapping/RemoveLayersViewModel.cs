//-----------------------------------------------------------------------
// <copyright file="FooViewModel.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------
namespace AerialMapping
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// View model for a treeview item on the RemoveLayers screen.
    /// </summary>
    public class RemoveLayersViewModel : INotifyPropertyChanged
    {
        // Member Variables
        private bool? isChecked = false;

        private RemoveLayersViewModel parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The string to put in the treeview.</param>
        /// <param name="filePath">The filepath associated with this object.</param>
        public RemoveLayersViewModel(string name, string filePath)
        {
            this.Name = name;
            this.FilePath = filePath;
            this.Children = new List<RemoveLayersViewModel>();
        }

        /// <summary>
        /// Event to tell the view to update a certain property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public List<RemoveLayersViewModel> Children { get; private set; }
        /// <summary>
        /// Determines if the element is selected by default
        /// when the window loads.
        /// </summary>
        public bool IsInitiallySelected { get; private set; }

        /// <summary>
        /// The Text shown on the window for the element.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// File path to the corresponding image.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets the state of the associated UI toggle (ex. CheckBox).
        /// The return value is calculated based on the check state of all
        /// child RemoveLayersViewModels.  Setting this property to true or false
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

        /// <summary>
        /// Takes the collection of locations and times from the main window and converts the data
        /// into a list of removelayerviewmodels for the removelayers window to use.
        /// </summary>
        /// <param name="locationsTimes">Locations and times from the treeview in the mainwindow.</param>
        /// <returns>The data from locationsTimes in a list of removelayerviewmodels format.</returns>
        public static List<RemoveLayersViewModel> CreateViewModels(ObservableCollection<MenuItem> locationsTimes)
        {
            // Create the root of the tree, which is for selecting all nodes.
            RemoveLayersViewModel root = new RemoveLayersViewModel("All", string.Empty)
            {
                IsInitiallySelected = false,
                
                Children = new List<RemoveLayersViewModel>()
            };

            // Create the rest of the treeview. Each location is a child of the 
            // root and has the times as its children.
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

            root.Initialize();
            return new List<RemoveLayersViewModel> { root };
        }

        /// <summary>
        /// Initializes a removelayerviewmodel by settings the children nodes
        /// and recursively calls each child to initialize.
        /// </summary>
        private void Initialize()
        {
            foreach (RemoveLayersViewModel child in this.Children)
            {
                child.parent = this;
                child.Initialize();
            }
        }

        /// <summary>
        /// This is the method called when a checkbox is checked. It has the ability to update
        /// the children and/or parent.
        /// </summary>
        /// <param name="value">New value for the checkbox.</param>
        /// <param name="updateChildren">Whether to update the children.</param>
        /// <param name="updateParent">Whether to update the parent.</param>
        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == this.isChecked)
            {
                return;
            }

            this.isChecked = value;

            // Potentially update the children recursively.
            if (updateChildren && this.isChecked.HasValue)
            {
                this.Children.ForEach(c => c.SetIsChecked(this.isChecked, true, false));
            }

            // Potentially update the parent.
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

        /// <summary>
        /// Method to notify the window to update the value of a property.
        /// </summary>
        /// <param name="prop">Denotes what needs to be updated on the window.</param>
        private void OnPropertyChanged(string prop)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}