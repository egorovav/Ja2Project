using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CommonWpfControls
{
	public class Command : ICommand
	{
		private Action FAction;
		private bool FCanExecute;
		public bool CanExecuteCommand
		{
			get { return this.FCanExecute; }
			set
			{
				this.FCanExecute = value;
				if (this.CanExecuteChanged != null)
					this.CanExecuteChanged(this, EventArgs.Empty);
			}
		}

		public Command(Action aAction, bool aCanExecute)
		{
			this.FAction = aAction;
			this.FCanExecute = aCanExecute;
		}

		public bool CanExecute(object aParameter)
		{
			return this.FCanExecute;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object aParameter)
		{
			this.FAction();
		}
	}
}
