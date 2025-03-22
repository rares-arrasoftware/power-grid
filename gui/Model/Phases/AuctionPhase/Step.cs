using PlayerInput.Model.Managers.CardManager;
using PlayerInput.Model.Managers.PlayerManager;
using PlayerInput.Model.Managers.RemoteManager;
using Serilog;

namespace PlayerInput.Model.Phases.AuctionPhase
{
    /// <summary>
    /// Represents an abstract step in a game phase. 
    /// Each step listens for input events and transitions when its conditions are met.
    /// </summary>
    public abstract class Step : IDisposable
    {

        /// <summary>
        /// Initializes a new step with a list of players.
        /// Subscribes to input events.
        /// </summary>
        /// <param name="players">The list of players involved in this step.</param>
        public Step(AuctionContext ctx)
        {
            _ctx = ctx;

            // Subscribe to input events
            RemoteManager.Instance.ButtonPressed += HandleButtonPressed;
            CardManager.Instance.NewCardScanned += HandleNewCardScanned;
        }

        // ====== PRIVATE FIELDS ======

        public readonly AuctionContext _ctx;

        /// <summary>
        /// Tracks whether the step has been disposed to prevent multiple disposals.
        /// </summary>
        private bool _isDisposed = false;

        // ====== PROTECTED FIELDS ======

        /// <summary>
        /// Manages asynchronous step transitions by waiting for the next step.
        /// </summary>
        protected readonly TaskCompletionSource<Step?> _stepCompletion = new();


        // ====== ABSTRACT METHODS ======

        /// <summary>
        /// Executes the step asynchronously. 
        /// This should contain the main logic for the step's execution.
        /// </summary>
        /// <returns>The next step to transition to, or null if this is the last step.</returns>
        public abstract Task<Step?> Execute();

        // ====== EVENT HANDLERS ======

        /// <summary>
        /// Handles button input events. 
        /// Override this method in derived classes to implement custom behavior.
        /// </summary>
        /// <param name="remoteId">The ID of the remote that triggered the input.</param>
        /// <param name="btn">The button that was pressed.</param>
        public virtual void HandleButtonPressed(Player player, Button btn) { }

        /// <summary>
        /// Handles card scan events. 
        /// Override this method in derived classes to implement custom behavior.
        /// </summary>
        /// <param name="cardId">The ID of the scanned card.</param>
        public virtual void HandleNewCardScanned(Card card) { }

        // ====== DISPOSAL METHODS ======

        /// <summary>
        /// Ensures event listeners are removed when the step is disposed.
        /// </summary>
        /// <param name="disposing">True if called explicitly, false if called from the destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            Log.Information("Disposed!");

            if (_isDisposed || !disposing) return;

            // Unsubscribe from input events
            RemoteManager.Instance.ButtonPressed -= HandleButtonPressed;
            CardManager.Instance.NewCardScanned -= HandleNewCardScanned;

            _isDisposed = true;
        }

        /// <summary>
        /// Public method to dispose of the step.
        /// Ensures proper cleanup of resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Prevents the destructor from running if Dispose() was explicitly called
        }

        /// <summary>
        /// Destructor (Fallback Cleanup)
        /// Called if Dispose() was not explicitly invoked.
        /// </summary>
        ~Step()
        {
            Dispose(false);
        }
    }
}
