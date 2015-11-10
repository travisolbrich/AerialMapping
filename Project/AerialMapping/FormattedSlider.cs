// Written by Josh Smith - 9/2007
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
		private ToolTip _autoToolTip;
		private string _autoToolTipFormat;

		/// <summary>
		/// Gets/sets a format string used to modify the auto tooltip's content.
		/// Note: This format string must contain exactly one placeholder value,
		/// which is used to hold the tooltip's original content.
		/// </summary>
		public string AutoToolTipFormat
		{
			get { return _autoToolTipFormat; }
			set { _autoToolTipFormat = value; }
		}

        public static readonly DependencyProperty AutoToolTipContentProperty = 
            DependencyProperty.Register("AutoToolTipContent", typeof(string), 
            typeof(FormattedSlider));

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

		protected override void OnThumbDragStarted(DragStartedEventArgs e)
		{
			base.OnThumbDragStarted(e);
			this.FormatAutoToolTipContent();
		}

		protected override void OnThumbDragDelta(DragDeltaEventArgs e)
		{
			base.OnThumbDragDelta(e);
			this.FormatAutoToolTipContent();
		}

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

		private ToolTip AutoToolTip
		{
			get
			{
				if (_autoToolTip == null)
				{
					FieldInfo field = typeof(Slider).GetField(
						"_autoToolTip",
						BindingFlags.NonPublic | BindingFlags.Instance);

					_autoToolTip = field.GetValue(this) as ToolTip;
				}

				return _autoToolTip;
			}
		}	
	}
}