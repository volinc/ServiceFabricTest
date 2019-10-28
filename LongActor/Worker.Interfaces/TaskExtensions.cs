namespace Worker.Interfaces
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public static class TaskExtensions
    {
        public static async void FireAndForget(this Task task, ILogger logger = null)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                if (logger == null)
                    Debug.WriteLine(exception);
                else
                    logger.LogError(exception, message: "Unable to forget running task.");
            }
        }
    }
}
