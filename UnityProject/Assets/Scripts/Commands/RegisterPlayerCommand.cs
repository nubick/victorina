using Injection;
using UnityEngine;
using Victorina.Commands;

namespace Victorina
{
    public class RegisterPlayerCommand : Command, IServerCommand
    {
        [Inject] private ConnectedPlayersData ConnectedPlayersData { get; set; }
        
        public string Guid { get; set; }
        public string Name { get; set; }
        
        public override CommandType Type => CommandType.RegisterPlayer;
        
        public bool CanExecuteOnServer()
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteOnServer()
        {
            
        }
    }
}