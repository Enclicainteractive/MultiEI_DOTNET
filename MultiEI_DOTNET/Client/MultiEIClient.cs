// Client/MultiEIClient.cs
using System;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using MultiEI.Interfaces;
using MultiEI.Models;
using MultiEI.Utilities;

namespace MultiEI.Client
{
    public class MultiEIClient : IMultiEIClient
    {
        private readonly string _serverUrl;
        private Socket _socket;

        // Events
        public event Action OnConnected;
        public event Action<string> OnDisconnected;

        public event Action<Player> OnPlayerInitialized;
        public event Action<Player> OnNetworkConnect;
        public event Action<Player> OnNetworkDisconnected;

        public event Action<Instance> OnInstanceStarted;
        public event Action<Instance> OnInstanceDeleted;
        public event Action<Instance> OnMasterChanged;

        public event Action<Player> OnPlayerJoined;
        public event Action<Player> OnPlayerLeft;
        public event Action<Player> OnPlayerKicked;

        public event Action<string, Vector3, string> OnObjectPositionUpdated;
        public event Action<string, string> OnObjectOwnershipReverted;
        public event Action<string, string> OnObjectOwnerSet;

        public event Action<string, bool, Vector3> OnPlayerTalking;

        public event Action<string, string> OnVoteKickInitiated;
        public event Action<string, string, bool> OnVoteInstance;
        public event Action<string> OnVoteReverted;

        public event Action<string, string> OnAvatarChanged;
        public event Action<string, string, object> OnAvatarElementChanged;

        public event Action<string, Vector3, Vector3, string> OnPortalDropped;

        public event Action<string, string> OnEmojiSent;

        public event Action<string, string> OnMessageReceived;
        public event Action<string, bool, Vector3> OnPlayerTyping;
        public event Action<string, bool, Vector3> OnPlayerStopTyping;

        public event Action<string> OnError;

        public MultiEIClient(string serverUrl)
        {
            _serverUrl = serverUrl;
        }

        public void Connect()
        {
            _socket = IO.Socket(_serverUrl);

            // Connection Events
            _socket.On(Socket.EVENT_CONNECT, () =>
            {
                OnConnected?.Invoke();
                AddLog("Connected to server.");
            });

            _socket.On(Socket.EVENT_DISCONNECT, (reason) =>
            {
                OnDisconnected?.Invoke(reason.ToString());
                AddLog($"Disconnected from server: {reason}");
            });

            // Player Events
            _socket.On("onNetworkConnect", (data) =>
            {
                var player = JsonConvert.DeserializeObject<Player>(data.ToString());
                OnNetworkConnect?.Invoke(player);
                AddLog($"Player connected: {player.PlayerId}");
            });

            _socket.On("onNetworkDisconnected", (data) =>
            {
                var player = JsonConvert.DeserializeObject<Player>(data.ToString());
                OnNetworkDisconnected?.Invoke(player);
                AddLog($"Player disconnected: {player.PlayerId}");
            });

            _socket.On("onPlayerInitialized", (data) =>
            {
                var player = JsonConvert.DeserializeObject<Player>(data.ToString());
                OnPlayerInitialized?.Invoke(player);
                AddLog($"Player initialized: {player.PlayerId}");
            });

            // Instance Events
            _socket.On("instanceStarted", (data) =>
            {
                var instance = JsonConvert.DeserializeObject<Instance>(data.ToString());
                OnInstanceStarted?.Invoke(instance);
                AddLog($"Instance started: {instance.Pin}");
            });

            _socket.On("instanceDeleted", (data) =>
            {
                var instance = JsonConvert.DeserializeObject<Instance>(data.ToString());
                OnInstanceDeleted?.Invoke(instance);
                AddLog($"Instance deleted: {instance.Pin}");
            });

            _socket.On("onMasterChanged", (data) =>
            {
                var instance = JsonConvert.DeserializeObject<Instance>(data.ToString());
                OnMasterChanged?.Invoke(instance);
                AddLog($"Master changed to: {instance.MasterId} for instance {instance.Pin}");
            });

            // Player Instance Events
            _socket.On("onPlayerJoined", (data) =>
            {
                var player = JsonConvert.DeserializeObject<Player>(data.ToString());
                OnPlayerJoined?.Invoke(player);
                AddLog($"Player joined: {player.PlayerId}");
            });

            _socket.On("onPlayerLeft", (data) =>
            {
                var player = JsonConvert.DeserializeObject<Player>(data.ToString());
                OnPlayerLeft?.Invoke(player);
                AddLog($"Player left: {player.PlayerId}");
            });

            _socket.On("playerKicked", (data) =>
            {
                var kickedPlayer = JsonConvert.DeserializeObject<Player>(data.ToString());
                OnPlayerKicked?.Invoke(kickedPlayer);
                AddLog($"Player kicked: {kickedPlayer.PlayerId}");
            });

            // Object Events
            _socket.On("onObjectPositionUpdated", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string objectId = obj.objectId;
                Vector3 position = JsonConvert.DeserializeObject<Vector3>(obj.position.ToString());
                string ownerId = obj.ownerId;
                OnObjectPositionUpdated?.Invoke(objectId, position, ownerId);
                AddLog($"Object {objectId} position updated by {ownerId}");
            });

            _socket.On("onObjectOwnershipReverted", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string objectId = obj.objectId;
                string ownerId = obj.ownerId;
                OnObjectOwnershipReverted?.Invoke(objectId, ownerId);
                AddLog($"Object {objectId} ownership reverted to {ownerId}");
            });

            _socket.On("onObjectOwnerSet", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string objectId = obj.objectId;
                string ownerId = obj.ownerId;
                OnObjectOwnerSet?.Invoke(objectId, ownerId);
                AddLog($"Object {objectId} ownership set to {ownerId}");
            });

            // Voice Events
            _socket.On("onPlayerTalking", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string playerId = obj.playerId;
                bool isTalking = obj.isTalking;
                Vector3 position = JsonConvert.DeserializeObject<Vector3>(obj.position.ToString());
                OnPlayerTalking?.Invoke(playerId, isTalking, position);
                AddLog($"Player {playerId} is {(isTalking ? "talking" : "silent")}");
            });

            // Voting Events
            _socket.On("onVoteKick", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string targetPlayerId = obj.targetPlayerId;
                string voterId = obj.voterId;
                OnVoteKickInitiated?.Invoke(targetPlayerId, voterId);
                AddLog($"Vote kick initiated by {voterId} against {targetPlayerId}");
            });

            _socket.On("onVoteInstance", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string targetPlayerId = obj.targetPlayerId;
                string voterId = obj.voterId;
                bool vote = obj.vote;
                OnVoteInstance?.Invoke(targetPlayerId, voterId, vote);
                AddLog($"Player {voterId} voted {(vote ? "YES" : "NO")} to kick {targetPlayerId}");
            });

            _socket.On("onVoteReverted", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string targetPlayerId = obj.targetPlayerId;
                OnVoteReverted?.Invoke(targetPlayerId);
                AddLog($"Vote reverted for player {targetPlayerId}");
            });

            // Avatar Events
            _socket.On("onAvatarChanged", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string playerId = obj.playerId;
                string newAvatarId = obj.newAvatarId;
                OnAvatarChanged?.Invoke(playerId, newAvatarId);
                AddLog($"Player {playerId} changed avatar to {newAvatarId}");
            });

            _socket.On("onAvatarElementChanged", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string playerId = obj.playerId;
                string elementId = obj.elementId;
                object newValue = obj.newValue;
                OnAvatarElementChanged?.Invoke(playerId, elementId, newValue);
                AddLog($"Player {playerId} changed avatar element {elementId} to {newValue}");
            });

            // Portal Events
            _socket.On("onPortalDropped", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string portalId = obj.portalId;
                Vector3 position = JsonConvert.DeserializeObject<Vector3>(obj.position.ToString());
                Vector3 destination = JsonConvert.DeserializeObject<Vector3>(obj.destination.ToString());
                string droppedBy = obj.droppedBy;
                OnPortalDropped?.Invoke(portalId, position, destination, droppedBy);
                AddLog($"Portal {portalId} dropped by Player {droppedBy} at position ({position.X}, {position.Y}, {position.Z}) to destination ({destination.X}, {destination.Y}, {destination.Z})");
            });

            // Emoji Events
            _socket.On("onEmojiSent", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string playerId = obj.playerId;
                string emoji = obj.emoji;
                OnEmojiSent?.Invoke(playerId, emoji);
                AddLog($"Player {playerId} sent emoji: {emoji}");
            });

            // Chatbox Events
            _socket.On("onMessageReceived", (data) =>
            {
                var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(data.ToString());
                OnMessageReceived?.Invoke(chatMessage.PlayerId, chatMessage.Message);
                AddLog($"Chat message from Player {chatMessage.PlayerId}: {chatMessage.Message}");
            });

            _socket.On("onPlayerTyping", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string playerId = obj.playerId;
                bool isTyping = obj.isTyping;
                Vector3 position = JsonConvert.DeserializeObject<Vector3>(obj.position.ToString());
                OnPlayerTyping?.Invoke(playerId, isTyping, position);
                AddLog($"Player {playerId} is {(isTyping ? "typing" : "not typing")}");
            });

            _socket.On("onPlayerStopTyping", (data) =>
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(data.ToString());
                string playerId = obj.playerId;
                bool isTyping = obj.isTyping;
                Vector3 position = JsonConvert.DeserializeObject<Vector3>(obj.position.ToString());
                OnPlayerStopTyping?.Invoke(playerId, isTyping, position);
                AddLog($"Player {playerId} stopped typing.");
            });

            // Error Event
            _socket.On("error", (data) =>
            {
                string errorMessage = data.ToString();
                OnError?.Invoke(errorMessage);
                AddLog($"Error received: {errorMessage}");
            });
        }

        public void Disconnect()
        {
            if (_socket != null)
            {
                _socket.Disconnect();
                AddLog("Disconnected from server.");
            }
        }

        // Player Management Methods
        public void InitializePlayer(string cosmeticId, Vector3 location, Vector3 rotation)
        {
            var data = new
            {
                cosmeticId = cosmeticId,
                location = location,
                rotation = rotation
            };
            _socket.Emit("init", JsonConvert.SerializeObject(data));
            AddLog("Initialized player.");
        }

        public void UpdatePlayerState(Vector3 location, Vector3 rotation)
        {
            var data = new
            {
                location = location,
                rotation = rotation
            };
            _socket.Emit("updatePlayerState", JsonConvert.SerializeObject(data));
            AddLog("Updated player state.");
        }

        public void UpdateCosmeticId(string cosmeticId)
        {
            var data = new
            {
                cosmeticId = cosmeticId
            };
            _socket.Emit("updateCosmeticId", JsonConvert.SerializeObject(data));
            AddLog($"Updated cosmetic ID to {cosmeticId}.");
        }

        public void PlayerTalking(bool isTalking, Vector3 position)
        {
            var data = new
            {
                isTalking = isTalking,
                position = position
            };
            _socket.Emit("playerTalking", JsonConvert.SerializeObject(data));
            AddLog($"Player talking status: {isTalking}");
        }

        public void PickupItem(string itemId)
        {
            var data = new
            {
                itemId = itemId
            };
            _socket.Emit("pickupItem", JsonConvert.SerializeObject(data));
            AddLog($"Picked up item: {itemId}");
        }

        public void ChangeAvatar(string newAvatarId)
        {
            var data = new
            {
                newAvatarId = newAvatarId
            };
            _socket.Emit("changeAvatar", JsonConvert.SerializeObject(data));
            AddLog($"Change avatar to {newAvatarId}.");
        }

        public void AvatarElementChanged(string elementId, object newValue)
        {
            var data = new
            {
                elementId = elementId,
                newValue = newValue
            };
            _socket.Emit("avatarElementChanged", JsonConvert.SerializeObject(data));
            AddLog($"Avatar element {elementId} changed to {newValue}.");
        }

        // Instance Management Methods
        public void StartInstance(string world, string type)
        {
            var data = new
            {
                world = world,
                type = type
            };
            _socket.Emit("startInstance", JsonConvert.SerializeObject(data));
            AddLog($"Start instance emitted with world: {world}, type: {type}");
        }

        public void JoinInstance(string pin)
        {
            var data = new
            {
                pin = pin
            };
            _socket.Emit("joinInstance", JsonConvert.SerializeObject(data));
            AddLog($"Join instance emitted for PIN: {pin}");
        }

        public void PlayerKicked(string pin)
        {
            var data = new
            {
                pin = pin
            };
            _socket.Emit("playerKicked", JsonConvert.SerializeObject(data));
            AddLog($"Player kicked from instance: {pin}");
        }

        public void RevertVote(string targetPlayerId)
        {
            var data = new
            {
                targetPlayerId = targetPlayerId
            };
            _socket.Emit("onVoteReverted", JsonConvert.SerializeObject(data));
            AddLog($"Reverted vote for player: {targetPlayerId}");
        }

        // Object Management Methods
        public void UpdateObjectPosition(string objectId, Vector3 position, string ownerId)
        {
            var data = new
            {
                objectId = objectId,
                position = position,
                ownerId = ownerId
            };
            _socket.Emit("updateObjectPosition", JsonConvert.SerializeObject(data));
            AddLog($"Updated object position: {objectId} by {ownerId}");
        }

        public void SetObjectOwner(string objectId, string ownerId)
        {
            var data = new
            {
                objectId = objectId,
                ownerId = ownerId
            };
            _socket.Emit("setObjectOwner", JsonConvert.SerializeObject(data));
            AddLog($"Set object owner: {objectId} to {ownerId}");
        }

        // Voting Management Methods
        public void VoteKick(string targetPlayerId)
        {
            var data = new
            {
                targetPlayerId = targetPlayerId
            };
            _socket.Emit("voteKick", JsonConvert.SerializeObject(data));
            AddLog($"Voted to kick player: {targetPlayerId}");
        }

        public void VoteInstance(string targetPlayerId, bool vote)
        {
            var data = new
            {
                targetPlayerId = targetPlayerId,
                vote = vote
            };
            _socket.Emit("voteInstance", JsonConvert.SerializeObject(data));
            AddLog($"Voted {(vote ? "YES" : "NO")} to kick player: {targetPlayerId}");
        }

        // Portal Management Methods
        public void DropPortal(string portalId, Vector3 position, Vector3 destination)
        {
            var data = new
            {
                portalId = portalId,
                position = position,
                destination = destination
            };
            _socket.Emit("dropPortal", JsonConvert.SerializeObject(data));
            AddLog($"Dropped portal {portalId} at position ({position.X}, {position.Y}, {position.Z}) to destination ({destination.X}, {destination.Y}, {destination.Z})");
        }

        // Emoji Management Methods
        public void SendEmoji(string emoji)
        {
            var data = new
            {
                emoji = emoji
            };
            _socket.Emit("sendEmoji", JsonConvert.SerializeObject(data));
            AddLog($"Sent emoji: {emoji}");
        }

        // Chatbox Management Methods
        public void SendMessage(string message)
        {
            var data = new
            {
                message = message
            };
            _socket.Emit("sendMessage", JsonConvert.SerializeObject(data));
            AddLog($"Sent message: {message}");
        }

        public void Typing()
        {
            _socket.Emit("typing", "{}");
            AddLog("Player is typing.");
        }

        public void StopTyping()
        {
            _socket.Emit("stopTyping", "{}");
            AddLog("Player stopped typing.");
        }

        // Private Helper Method for Logging
        private void AddLog(string message)
        {
            string timestamp = DateTime.UtcNow.ToString("o");
            Console.WriteLine($"[{timestamp}] {message}");
        }
    }
}
