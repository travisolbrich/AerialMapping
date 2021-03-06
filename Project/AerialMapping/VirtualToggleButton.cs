﻿//-----------------------------------------------------------------------
// <copyright file="VirtualToggleButton.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Class for three-state button.
    /// </summary>
    public static class VirtualToggleButton
    {
        /// <summary>
        /// IsThreeState Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsThreeStateProperty =
            DependencyProperty.RegisterAttached(
                "IsThreeState",
                typeof(bool),
                typeof(VirtualToggleButton),
                new FrameworkPropertyMetadata((bool)false));

        /// <summary>
        /// IsVirtualToggleButton Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsVirtualToggleButtonProperty =
            DependencyProperty.RegisterAttached(
                "IsVirtualToggleButton",
                typeof(bool),
                typeof(VirtualToggleButton),
                new FrameworkPropertyMetadata(
                    (bool)false,
                    new PropertyChangedCallback(OnIsVirtualToggleButtonChanged)));

        /// <summary>
        /// IsChecked Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.RegisterAttached(
                                    "IsChecked", 
                                    typeof(bool?), 
                                    typeof(VirtualToggleButton),
                                    new FrameworkPropertyMetadata(
                                        (bool?)false,
                                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
                                        new PropertyChangedCallback(OnIsCheckedChanged)));

        /// <summary>
        /// Gets the IsChecked property.  This dependency property 
        /// indicates whether the toggle button is checked.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <returns>A bool showing if it it checked.</returns>
        public static bool? GetIsChecked(DependencyObject d)
        {
            return (bool?)d.GetValue(IsCheckedProperty);
        }

        /// <summary>
        /// Sets the IsChecked property.  This dependency property 
        /// indicates whether the toggle button is checked.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <param name="value">A bool determining if it it checked.</param>
        public static void SetIsChecked(DependencyObject d, bool? value)
        {
            d.SetValue(IsCheckedProperty, value);
        }

        /// <summary>
        /// Gets the IsThreeState property.  This dependency property 
        /// indicates whether the control supports two or three states.  
        /// IsChecked can be set to null as a third state when IsThreeState is true.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <returns>A bool showing if it is three state or not.</returns>
        public static bool GetIsThreeState(DependencyObject d)
        {
            return (bool)d.GetValue(IsThreeStateProperty);
        }

        /// <summary>
        /// Sets the IsThreeState property.  This dependency property 
        /// indicates whether the control supports two or three states. 
        /// IsChecked can be set to null as a third state when IsThreeState is true.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <param name="value">A bool determining if it is three state or not.</param>
        public static void SetIsThreeState(DependencyObject d, bool value)
        {
            d.SetValue(IsThreeStateProperty, value);
        }

        /// <summary>
        /// Gets the IsVirtualToggleButton property.  This dependency property 
        /// indicates whether the object to which the property is attached is treated as a VirtualToggleButton.  
        /// If true, the object will respond to keyboard and mouse input the same way a ToggleButton would.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <returns>A bool telling if it is a virtual toggle button.</returns>
        public static bool GetIsVirtualToggleButton(DependencyObject d)
        {
            return (bool)d.GetValue(IsVirtualToggleButtonProperty);
        }

        /// <summary>
        /// Sets the IsVirtualToggleButton property.  This dependency property 
        /// indicates whether the object to which the property is attached is treated as a VirtualToggleButton.  
        /// If true, the object will respond to keyboard and mouse input the same way a ToggleButton would.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <param name="value">A bool that determines if it is a virtualtogglebutton.</param>
        public static void SetIsVirtualToggleButton(DependencyObject d, bool value)
        {
            d.SetValue(IsVirtualToggleButtonProperty, value);
        }

        /// <summary>
        /// A static helper method to raise the Checked event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <returns>The Routed Event Arguments</returns>
        internal static RoutedEventArgs RaiseCheckedEvent(UIElement target)
        {
            if (target == null)
            {
                return null;
            }

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ToggleButton.CheckedEvent;
            RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// A static helper method to raise the Unchecked event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <returns>The Routed Event Arguments</returns>
        internal static RoutedEventArgs RaiseUncheckedEvent(UIElement target)
        {
            if (target == null)
            {
                return null;
            }

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ToggleButton.UncheckedEvent;
            RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// A static helper method to raise the Indeterminate event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        /// <returns>The Routed Event Arguments</returns>
        internal static RoutedEventArgs RaiseIndeterminateEvent(UIElement target)
        {
            if (target == null)
            {
                return null;
            }

            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ToggleButton.IndeterminateEvent;
            RaiseEvent(target, args);
            return args;
        }

        /// <summary>
        /// Handles changes to the IsVirtualToggleButton property.
        /// </summary>
        /// <param name="d">The corresponding Dependency Object</param>
        /// <param name="e">The Dependency Property Changed Event Arguments</param>
        private static void OnIsVirtualToggleButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IInputElement element = d as IInputElement;
            if (element != null)
            {
                if ((bool)e.NewValue)
                {
                    element.MouseLeftButtonDown += OnMouseLeftButtonDown;
                    element.KeyDown += OnKeyDown;
                }
                else
                {
                    element.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                    element.KeyDown -= OnKeyDown;
                }
            }
        }

        /// <summary>
        /// Specifies the action when the left mouse button is pressed down.
        /// </summary>
        /// <param name="sender">Mouse event</param>
        /// <param name="e">Mouse event args</param>
        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            UpdateIsChecked(sender as DependencyObject);
        }

        /// <summary>
        /// Specifies the action when a key is pressed down. 
        /// </summary>
        /// <param name="sender">Key press event</param>
        /// <param name="e">Key press event args</param>
        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource == sender)
            {
                if (e.Key == Key.Space)
                {
                    // ignore alt+space which invokes the system menu
                    if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                    {
                        return;
                    }

                    UpdateIsChecked(sender as DependencyObject);
                    e.Handled = true;
                }
                else if (e.Key == Key.Enter && (bool)(sender as DependencyObject).GetValue(KeyboardNavigation.AcceptsReturnProperty))
                {
                    UpdateIsChecked(sender as DependencyObject);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Update the check status
        /// </summary>
        /// <param name="d">The dependency object that is getting checked.</param>
        private static void UpdateIsChecked(DependencyObject d)
        {
            bool? isChecked = GetIsChecked(d);
            if (isChecked == true)
            {
                SetIsChecked(d, GetIsThreeState(d) ? (bool?)null : (bool?)false);
            }
            else
            {
                SetIsChecked(d, isChecked.HasValue);
            }
        }

        /// <summary>
        /// Method to raise an event.
        /// </summary>
        /// <param name="target">The object to raise an event on</param>
        /// <param name="args">Event args</param>
        private static void RaiseEvent(DependencyObject target, RoutedEventArgs args)
        {
            if (target is UIElement)
            {
                (target as UIElement).RaiseEvent(args);
            }
            else if (target is ContentElement)
            {
                (target as ContentElement).RaiseEvent(args);
            }
        }

        /// <summary>
        /// Handles changes to the IsChecked property.
        /// </summary>
        /// <param name="d">The object to raise an event on</param>
        /// <param name="e">Event args</param>
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement pseudobutton = d as UIElement;
            if (pseudobutton != null)
            {
                bool? newValue = (bool?)e.NewValue;
                if (newValue == true)
                {
                    RaiseCheckedEvent(pseudobutton);
                }
                else if (newValue == false)
                {
                    RaiseUncheckedEvent(pseudobutton);
                }
                else
                {
                    RaiseIndeterminateEvent(pseudobutton);
                }
            }
        }
    }
}