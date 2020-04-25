using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameVui.Server.Models;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Threading;
using GameVui.Server.TimsoFameModels;

namespace GameVui.Server.TimSoGameModels
{
    [Authorize]
    public class TimSoHub : Hub
    {
        private static object _syncRoot = new object();
        private static int _gamesPlayed = 0;
        /// <summary>
        /// The list of clients is used to keep track of registered clients and clients that are looking for games
        /// The client will be removed from this list as soon as the client is in a game or has left the game
        /// </summary>
        private static readonly List<Client> clients = new List<Client>();
        private static readonly List<Invitation> invitations = new List<Invitation>();

        private Timer timer;
        public TimSoHub()
        {
            timer = new Timer(timerTick, null, 0, 1000);
        }

        private void timerTick(object state)
        {
            CheckInvitations();
        }
        
        /// <summary>
        /// This list is used to keep track of games and their states
        /// </summary>
        private static readonly List<TimSoGame> games = new List<TimSoGame>();

        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                var userId = Context.User.Identity.GetUserId();

                var game = games.FirstOrDefault(x => x.Player1.UserId == userId || x.Player2.UserId == userId);
                var quitClient = clients.FirstOrDefault(x => x.UserId == userId);

                //safe code
                if (quitClient == null) 
                {
                    throw new Exception("client is null");
                }

                quitClient.ConnectionIds.Remove(Context.ConnectionId);
                //Tất cả các kết nối đều tắt
                if (quitClient.ConnectionIds.Count == 0)
                { 
                    NotifyOffline();
                    clients.Remove(quitClient);
                   
                }
                //Kết thúc game nếu kết nối game bị tắt
                if (game!=null && quitClient.PlayingConnectionId == Context.ConnectionId)
                {
                    games.Remove(game);
                   
                    if (quitClient.Opponent != null)
                    {
                        quitClient.Opponent.LookingForOpponent = false;
                        quitClient.Opponent.IsPlaying = false;
                        quitClient.Opponent.PlayingConnectionId = "";
                        using (var context = new ApplicationDbContext())
                        {
                            var match = context.Matches.Find(game.Player1.UserId, game.Player2.UserId, game.CreatedTime);
                            match.Player1Achievement = game.Player1.Count;
                            match.Player2Achievement = game.Player2.Count;
                            match.WinnerId = quitClient.Opponent.UserId;
                            match.GameEnd = DateTime.Now;
                            if (match.PlayerId1 == match.WinnerId)
                            {
                                match.Player1.Point += (match.Player1Achievement / 10 + 2);
                                match.Player2.Point -= (match.Player1Achievement / 10 + 1);
                            }
                            else
                            {
                                match.Player2.Point += (match.Player2Achievement / 10 + 2);
                                match.Player1.Point -= (match.Player2Achievement / 10 + 1);
                            }
                            context.SaveChanges();
                        }
                        var opponentId = quitClient.Opponent.UserId;
                        quitClient.Opponent = null;
                        return Clients.User(opponentId).opponentDisconnected(quitClient.DisplayName);
                    }
                }
                
                SendStatsUpdate();
            }
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnConnected()
        {
            return RegisterClient();
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        public Task SendStatsUpdate()
        {
            return Clients.All.refreshAmountOfPlayers(new { totalGamesPlayed = _gamesPlayed, amountOfGames = games.Count, amountOfClients = clients.Count });
        }
        public Task NotifyOnline()
        {
            var userId = Context.User.Identity.GetUserId();
            var client = clients.FirstOrDefault(c=>c.UserId == userId);
            ClientInfo me = new ClientInfo{ DisplayName = client.DisplayName, Name = client.Name, IsPlaying = client.IsPlaying, UserId = client.UserId};
            return Clients.AllExcept(client.ConnectionIds.ToArray()).notifyOnline(me);
        }

        public Task NotifyOffline()
        {
            var userId = Context.User.Identity.GetUserId();
            var client = clients.FirstOrDefault(c => c.UserId == userId);
            ClientInfo me = new ClientInfo { DisplayName = client.DisplayName, Name = client.Name, IsPlaying = client.IsPlaying, UserId = client.UserId };
            return Clients.All.notifyOffline(me);
        }
        public void GetListUser()
        {
            List<ClientInfo> users = clients.Where(c => c.Name != Context.User.Identity.Name).Select(c => new ClientInfo { Name = c.Name, IsPlaying = c.IsPlaying, UserId = c.UserId, DisplayName = c.DisplayName }).ToList();
            Clients.Caller.receiveListUser(users);
        }
        /// <summary>
        /// registering a new client will add the client to the current list of clients and save the connection id which will be used to communicate with the client
        /// </summary>
        /// <param name="data">The player name</param>
        public Task RegisterClient()
        {
            lock (_syncRoot)
            {
                var client = clients.FirstOrDefault(x => x.Name == Context.User.Identity.Name);
                if (client == null)
                {
                    var userId = Context.User.Identity.GetUserId();
                    var context = new ApplicationDbContext();
                    var user = context.Users.Find(userId);
                    client = new Client { Name = user.UserName, DisplayName = user.DisplayName, UserId = userId, Count = 0, ConnectionIds = new List<string> { Context.ConnectionId} };
                    clients.Add(client);
                    NotifyOnline();
                }
                else
                {
                    client.ConnectionIds.Add(Context.ConnectionId);
                    Clients.Caller.playingAnother();
                }
            }

            return SendStatsUpdate();
            //Clients.Caller.registerComplete();
        }

        /// <summary>
        /// Play a marker at a given positon
        /// </summary>
        /// <param name="position">The position where to place the marker</param>
        public void Play(int position)
        {
            var userId = Context.User.Identity.GetUserId();
            // Find the game where there is a player1 and player2 and either of them have the current connection id
            var game = games.FirstOrDefault(x => x.Player1.UserId == userId || x.Player2.UserId == userId);

            if (game == null || game.IsGameOver) return;

            var player = game.Player2.UserId == userId ? game.Player2 : game.Player1;

            bool endGame;
            int currentNumber;
            if (game.Play(position, out endGame, out currentNumber))
            {
                player.Count += 1;
                var player1Complete = game.Player1.Count*200/game.TotalNumber;
                var player2Complete = game.Player2.Count*200/game.TotalNumber;
                Clients.User(game.Player1.UserId).addMarkerPlacement(new GameInformation { MyPoint = player1Complete, OpponentPoint = player2Complete, MarkerPosition = position, Simbolo = player.Simbolo, CurrentNumber = currentNumber });
                Clients.User(game.Player2.UserId).addMarkerPlacement(new GameInformation { MyPoint = player2Complete, OpponentPoint = player1Complete, MarkerPosition = position, Simbolo = player.Simbolo, CurrentNumber = currentNumber });
                if (player.Count > game.TotalNumber / 2) endGame = true;
                if (endGame)
                {
                    var checkWin = game.CheckWinner();
                    games.Remove(game);

                    game.Player1.IsPlaying = false;
                    game.Player1.LookingForOpponent = false;
                    game.Player1.PlayingConnectionId = "";
                    game.Player2.IsPlaying = false;
                    game.Player2.LookingForOpponent = false;
                    game.Player2.PlayingConnectionId = "";

                    _gamesPlayed += 1;
                    
                    //var winnerInfo = winner == null?null : new ClientInfo
                    //{
                    //    ConnectionId = winner.ConnectionId,
                    //    Name = winner.Name
                    //};
                    
                    Clients.User(game.Player1.UserId).gameOver(checkWin);
                    Clients.User(game.Player2.UserId).gameOver(3-checkWin);

                    //Save to database
                    using (var context = new ApplicationDbContext())
                    {
                       var match = context.Matches.Find(game.Player1.UserId, game.Player2.UserId, game.CreatedTime);
                        match.Player1Achievement = game.Player1.Count;
                        match.Player2Achievement = game.Player2.Count;
                        match.Finished = true;
                        if (checkWin == 1) match.WinnerId = game.Player1.UserId;
                        else if(checkWin == 2) match.WinnerId = game.Player2.UserId;
                        match.GameEnd = DateTime.Now;
                        if (match.PlayerId1 == match.WinnerId)
                        {
                            match.Player1.Point += (match.Player1Achievement / 10 + 2);
                            match.Player2.Point -= (match.Player1Achievement / 10 + 1);
                        }
                        else
                        {
                            match.Player2.Point += (match.Player2Achievement / 10 + 2);
                            match.Player1.Point -= (match.Player2Achievement / 10 + 1);
                        }
                        context.SaveChanges();
                    }
                    
                }
            }

            SendStatsUpdate();
        }

        public void Invite(string opponentId, int total)
        {
            var client = clients.FirstOrDefault(x => x.Name == Context.User.Identity.Name);
            if (client == null)
                return;
            if (client.IsPlaying)
            {
                Clients.Caller.clientPlaying();
                return;
            }
            var opponent = clients.FirstOrDefault(c => c.UserId == opponentId);
            if (opponent == null)
            {
                Clients.Caller.opponentOffline();
                return;
            }
            if (opponent.IsPlaying)
            {
                Clients.Caller.opponentPlaying();
                return;
            }
            invitations.Add(new Invitation 
            { 
                Player1 = client,
                Player2 = opponent,
                InvitedTime = DateTime.Now
            });
            Clients.Caller.inviteSuccessful(opponent.UserId);
            ClientInfo me = new ClientInfo {ConnectionId = Context.ConnectionId, DisplayName = client.DisplayName, IsPlaying = client.IsPlaying, UserId = client.UserId };
            Clients.User(opponentId).receiveInvitation(me, total);
            
        }

        public void AcceptInvitation(string opponentId, string connectionId, int totalNumber)
        {
            var player = clients.FirstOrDefault(x => x.Name == Context.User.Identity.Name);
            if (player == null) 
                return;
            var opponent = clients.FirstOrDefault(x => x.UserId == opponentId && x.ConnectionIds.Contains(connectionId));
            
            if (opponent == null)
            {
                Clients.Caller.opponentOffline();
                return;
            }
            if (!invitations.Any(i => i.Player1 == opponent && i.Player2 == player))
            {
                Clients.Caller.invitationNotExist();
                return;
            }
            if(opponent.IsPlaying)
            {
                Clients.Caller.opponentPlaying();
                return;
            }
            //Hủy các lời mời khác
            invitations.RemoveAll(i => i.Player1 == player || i.Player2 == player || i.Player1 == opponent || i.Player2 == opponent);

            player.IsPlaying = true;
            player.LookingForOpponent = false;
            player.Simbolo = "X";
            player.Count = 0;
            player.PlayingConnectionId = Context.ConnectionId;

            opponent.IsPlaying = true;
            opponent.LookingForOpponent = false;
            opponent.Simbolo = "O";
            opponent.Count = 0;
            opponent.PlayingConnectionId = connectionId;

            player.Opponent = opponent;
            opponent.Opponent = player;

            var game = new TimSoGame(totalNumber) { Player1 = player, Player2 = opponent };
            //Save to database
            var match = Match.FromGame(game);
            match.TotalNumber = totalNumber;
            using (var context = new ApplicationDbContext())
            {
                context.Matches.Add(match);
                context.SaveChanges();
            }
            lock (_syncRoot)
            {
                games.Add(game);
            }

            Clients.User(opponent.UserId).opponentAcceptInvitation(Context.ConnectionId);
            // Notify both players that a game was found
            Clients.Caller.foundOpponent(game.Field, game.CurrentNumber, opponent.Name, "/Content/Images/TicTacToeX.png");
            Clients.Client(connectionId).foundOpponent(game.Field, game.CurrentNumber, player.Name, "/Content/Images/TicTacToeO.png");

            SendStatsUpdate();
        }
        public void CancelInvitation(string opponentId)
        {
            var userId = Context.User.Identity.GetUserId();
            invitations.RemoveAll(i => i.Player1.UserId == opponentId && i.Player2.UserId == userId);
            Clients.User(opponentId).invitationCanceled(userId);
        }
        private void CheckInvitations()
        {
            var now = DateTime.Now;
            var timeoutInvitations = invitations.Where(iv=> now.Subtract(iv.InvitedTime).TotalMinutes >= 1).ToList();

            foreach(var invitation in timeoutInvitations)
            {
                InvitationTimeout(invitation);
                invitations.Remove(invitation);
            }
        }
        private void InvitationTimeout(Invitation invitation)
        {
            Clients.User(invitation.Player1.UserId).invitationTimeout(invitation.Player2.UserId, true);
            Clients.User(invitation.Player2.UserId).invitationTimeout(invitation.Player1.UserId, false);
        }
        public void SendMessage(string userId, string message)
        {
            Clients.Caller.receiveMessage(message, true);
            Clients.User(userId).receiveMessage(message, false);
        }
               
    }
}