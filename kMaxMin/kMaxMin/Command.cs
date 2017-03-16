using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace kMeans
{
    public class Command:ICommand
    {

        private Func<Task> command;

        public Command(Func<Task> action)
        {
            command = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync();
        }


        private Task ExecuteAsync()
        {
            return command();
        }

        public event EventHandler CanExecuteChanged;
    }
}