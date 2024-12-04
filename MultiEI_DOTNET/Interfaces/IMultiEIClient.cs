// Interfaces/IMultiEIClient.cs
using System;
using MultiEI.Models;
using MultiEI.Utilities;

namespace MultiEI.Interfaces
{
    public interface IMultiEIClient
    {
        // Connection Events
        event Action OnConnected;
        event Action<string> OnDisconnected;

        // Player Events
        event Action<Player> OnPlayerInitialized;
        event Action<Player> OnNetworkConnect;
        event Action<Player> OnNetworkDisconnected;

        // Instance Events
        event Action<Instance> OnInstanceStarted;
        event Action<Instance> OnInstanceDeleted;
        event Action<Instance> OnMasterChanged;

        // Player Instance Events
        event Action<Player> OnPlayerJoined;
        event Action<Player> OnPlayerLeft;
        event Action<Player> OnPlayerKicked;

        // Object Events
        event Action<string, Vector3, string> OnObjectPositionUpdated;
        event Action<string, string> OnObjectOwnershipReverted;
        event Action<string, string> OnObjectOwnerSet;

        // Voice Events
        event Action<string, bool, Vector3> OnPlayerTalking;

        // Voting Events
        event Action<string, string> OnVoteKickInitiated;
        event Action<string, string, bool> OnVoteInstance;
        event Action<string> OnVoteReverted;

        // Avatar Events
        event Action<string, string> OnAvatarChanged;
        event Action<string, string, object> OnAvatarElementChanged;

        // Portal Events
        event Action<string, Vector3, Vector3, string> OnPortalDropped;

        // Emoji Events
        event Action<string, string> OnEmojiSent;

        // Chatbox Events
        event Action<string, string> OnMessageReceived;
        event Action<string, bool, Vector3> OnPlayerTyping;
        event Action<string, bool, Vector3> OnPlayerStopTyping;

        // Error Event
        event Action<string> OnError;

        // Connection Management
        void Connect();
        void Disconnect();

        // Player Management
        void InitializePlayer(string cosmeticId, Vector3 location, Vector3 rotation);
        void UpdatePlayerState(Vector3 location, Vector3 rotation);
        void UpdateCosmeticId(string cosmeticId);
        void PlayerTalking(bool isTalking, Vector3 position);
        void PickupItem(string itemId);
        void ChangeAvatar(string newAvatarId);
        void AvatarElementChanged(string elementId, object newValue);

        // Instance Management
        void StartInstance(string world, string type);
        void JoinInstance(string pin);
        void PlayerKicked(string pin);
        void RevertVote(string targetPlayerId);

        // Object Management
        void UpdateObjectPosition(string objectId, Vector3 position, string ownerId);
        void SetObjectOwner(string objectId, string ownerId);

        // Voting Management
        void VoteKick(string targetPlayerId);
        void VoteInstance(string targetPlayerId, bool vote);

        // Portal Management
        void DropPortal(string portalId, Vector3 position, Vector3 destination);

        // Emoji Management
        void SendEmoji(string emoji);

        // Chatbox Management
        void SendMessage(string message);
        void Typing();
        void StopTyping();
    }
}
