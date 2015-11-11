//-----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

// Modified from Josh Smith's FormattedSlider.

using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;

namespace AerialMapping
{
	/// <summary>
	/// A Slider which provides a way to modify the 
	/// auto tooltip text by using a format string.
	/// </summary>
	public class FormattedSlider : Slider
	{
        // Member Variables
		private ToolTip autoToolTip;
		private string autoToolTipFormat;

		/// <summary>
		/// Gets/sets a format string used to modify the auto tooltip's content.
		/// Note: This format string must contain exactly one placeholder value,
		/// which is used to hold the tooltip's original content.
		/// </summary>
		public string AutoToolTipFormat
		{
			get { return autoToolTipFormat; }
			set { autoToolTipFormat = value; }
		}

        // Dependency Variable for the content of the tooltip.
        public static readonly DependencyProperty AutoToolTipContentProperty = 
            DependencyProperty.Register("AutoToolTipContent", typeof(string), 
            typeof(FormattedSlider));

        /// <summary>
        /// The string that the tooltip will display.
        /// </summary>
        public string AutoToolTipContent
        {
            get
            {
                return GetValue(AutoToolTipContentProperty).ToString();
            }
            set
            {
                SetValue(AutoToolTipContentProperty, value);
            }
        }

        /// <summary>
        /// Event fired when the user begins to drag the slider thumb.
        /// </summary>
        /// <param name="e">Drag event args</param>
		protected override void OnThumbDragStarted(DragStartedEventArgs e)
		{
			base.OnThumbDragStarted(e);
			this.FormatAutoToolTipContent();
		}

        /// <summary>
        /// Event fired when the thumb changes values on the slider.
        /// </summary>
        /// <param name="e">Drg event args</param>
		protected override void OnThumbDragDelta(DragDeltaEventArgs e)
		{
			base.OnThumbDragDelta(e);
			this.FormatAutoToolTipContent();
		}

        /// <summary>
        /// Method to update the tooltip content.
        /// </summary>
		private void FormatAutoToolTipContent()
		{
			if (!string.IsNullOrEmpty(this.AutoToolTipContent))
			{
                //this.AutoToolTip.Content = string.Format(
                //    this.AutoToolTipFormat, 
                //    this.AutoToolTip.Content);
                this.AutoToolTip.Content = this.AutoToolTipContent;
			}
		}

        /// <summary>
        /// The actual tooltip for the slider.
        /// </summary>
		private ToolTip AutoToolTip
		{
			get
			{
				if (autoToolTip == null)
				{
					FieldInfo field = typeof(Slider).GetField(
						"_autoToolTip",
						BindingFlags.NonPublic | BindingFlags.Instance);

					autoToolTip = field.GetValue(this) as ToolTip;
				}

				return autoToolTip;
			}
		}	
	}
}