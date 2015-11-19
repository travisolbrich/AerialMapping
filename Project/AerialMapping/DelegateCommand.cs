//-----------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="CSCE 482: Aerial Mapping">
//     Copyright (c) CSCE 482 Aerial Mapping Design Team
// </copyright>
//-----------------------------------------------------------------------

namespace AerialMapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// Passes a command from the View to the Viewmodel.
    /// it allows for button callbacks within the Viewmodel. 
    /// </summary>
    public class DelegateCommand : System.Windows.Input.ICommand
    {
        /// <summary>
        /// a var to store the command's execute logic (button click, for example)
        /// </summary>
        private readonly Action<object> execute;

        /// <summary>
        /// a var to store the command's logic for enabling/disabling
        /// </summary>
        private readonly Func<object, bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the DelegateCommand class.
        /// constructor: store the logic for executing and enabling the command
        /// </summary>
        /// <param name="executeAction">method to execute</param>
        /// <param name="canExecuteFunc">test to see if method can execute</param>
        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc = null)
        {
            this.canExecute = canExecuteFunc;
            this.execute = executeAction;
        }

        /// <summary>
        /// an event for when the value of "CanExecute" changes (not implemented)
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// if it was passed in, execute the enabling logic for the command
        /// </summary>
        /// <param name="parameter">Paramater to be tested</param>
        /// <returns>Bool determining if the method can execute.</returns>
        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null) 
            { 
                return true; 
            }

            return this.canExecute(parameter);
        }

        /// <summary>
        /// execute the command logic 
        /// </summary>
        /// <param name="parameter">Paramater to be executed</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
