
using System.Threading.Tasks;
using LiveGameApp.Models;

namespace LiveGameApp.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(Directmessage message);
    }
}
