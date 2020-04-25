using System;
using System.Collections.Generic;

namespace GameVui.Server.CaroGameModels
{
    public class Client
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public Client Opponent { get; set; }
        public bool IsPlaying { get; set; }
        public bool LookingForOpponent { get; set; }
        public DateTime GameStarted { get; set; }
        public string Simbolo { get; set; }

        public List<string> ConnectionIds { get; set; }
        public string PlayingConnectionId { get; set; }
        public int Count { get; set; }
        public string UserId { get; set; }
        public Client()
        {
        }

        public Client(Client client)
        {
            Opponent = client.Opponent;
            IsPlaying = client.IsPlaying;
            LookingForOpponent = client.LookingForOpponent;
            GameStarted = client.GameStarted;
            Simbolo = client.Simbolo;
            Count = client.Count;
            UserId = client.UserId;
        }
    }
    public class ClientInfo
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public bool IsPlaying { get; set; }
        public string UserId { get; set; }
    }
}
