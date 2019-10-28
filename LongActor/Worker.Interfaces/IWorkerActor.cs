namespace Worker.Interfaces
{    
    using System.Threading.Tasks;
    using Microsoft.ServiceFabric.Actors;
    
    public interface IWorkerActor : IActor
    {
        Task<int> GetLongOpProgressAsync();

        Task StartLongOpAsync();

        Task StopLongOnAsync();
    }
}
