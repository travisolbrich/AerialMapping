//-----------------------------------------------------------------------
// <copyright file="FormattedSlider.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

// Modified from Josh Smith's FormattedSlider.

namespace AerialMapping
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// A Slider which provides a way to modify the 
    /// auto tooltip text by using a format string.
    /// </summary>
    public class FormattedSlider : Slider
    {
        // Dependency Variable for the content of the tooltip.
        public static readonly DependencyProperty AutoToolTipContentProperty =
            DependencyProperty.Register(
                "AutoToolTipContent", 
                typeof(string),
                typeof(FormattedSlider));

        // Member Variables
        private ToolTip autoToolTip;
        private string autoToolTipFormat;

        /// <summary>
        /// Gets or sets a format string used to modify the auto tooltip's content.
        /// Note: This format string must contain exactly one placeholder value,
        /// which is used to hold the tooltip's original content.
        /// </summary>
        public string AutoToolTipFormat
        {
            get
            { 
                return this.autoToolTipFormat; 
            }

            set 
            { 
                this.autoToolTipFormat = value; 
            }
        }

        /// <summary>
        /// Gets or sets the string that the tooltip will display.
        /// </summary>
        public string AutoToolTipContent
        {
            get
            {
                return GetValue(AutoToolTipContentProperty).ToString();
            }

            set
            {
                this.SetValue(AutoToolTipContentProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual tooltip for the slider.
        /// </summary>
        private ToolTip AutoToolTip
        {
            get
            {
                if (this.autoToolTip == null)
                {
                    FieldInfo field = typeof(Slider).GetField(
                        "_autoToolTip",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    this.autoToolTip = field.GetValue(this) as ToolTip;
                }

                return this.autoToolTip;
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
                this.AutoToolTip.Content = this.AutoToolTipContent;
            }
        } 
    }
}