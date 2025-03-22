using Serilog;
using System.Net.Sockets;

namespace PlayerInput.Model.Managers.InputManager
{
    /// <summary>
    /// Singleton class responsible for handling player input via UDP.
    /// Listens for remote control and RFID card events and dispatches them as events.
    /// </summary>
    public class InputManager
    {
        // ====== ENUMS ======

        /// <summary>
        /// Represents the type of event received via UDP.
        /// </summary>
        public enum EventType
        {
            Card = 0x01,    // RFID card scanned
            Remote = 0x02   // Remote control button pressed
        }

        // ====== SINGLETON INSTANCE ======

        private static readonly InputManager _instance = new();
        public static InputManager Instance => _instance;

        // ====== EVENTS ======

        /// <summary>
        /// Event triggered when a remote control button is pressed.
        /// </summary>
        public event Action<int, int>? BtnPressed;

        /// <summary>
        /// Event triggered when an RFID card is scanned.
        /// </summary>
        public event Action<int>? CardScanned;

        // ====== PRIVATE FIELDS ======

        private readonly UdpClient _udpClient = new(8081); // UDP server for receiving input events
        private readonly CancellationTokenSource _cts = new(); // For graceful shutdown
        private Task? _inputTask; // Task handling UDP packet listening

        // ====== CONSTRUCTOR ======

        /// <summary>
        /// Private constructor to enforce the Singleton pattern.
        /// </summary>
        private InputManager() { }

        // ====== PUBLIC METHODS ======

        /// <summary>
        /// Starts listening for input events asynchronously.
        /// </summary>
        public void Start()
        {
            Log.Information($"{nameof(InputManager)}: Start");
            _inputTask = Task.Run(() => Run(_cts.Token));
        }

        /// <summary>
        /// Stops listening for input events and shuts down the UDP server.
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _inputTask?.Wait();
            Console.WriteLine("InputManager stopped.");
        }

        // ====== PRIVATE METHODS ======

        /// <summary>
        /// Runs the input listening loop, processing UDP packets.
        /// </summary>
        private async Task Run(CancellationToken cancellationToken)
        {
            try
            {
                await ProcessUdpPackets(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("InputManager: Shutdown requested.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InputManager Error: {ex.Message}");
            }
            finally
            {
                _udpClient.Close();
                Console.WriteLine("UDP Server stopped.");
            }
        }

        /// <summary>
        /// Continuously listens for UDP packets and processes them.
        /// </summary>
        private async Task ProcessUdpPackets(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Log.Information($"{nameof(ProcessUdpPackets)}");
                var receiveTask = _udpClient.ReceiveAsync();

                var completedTask = await Task.WhenAny(receiveTask, Task.Delay(-1, cancellationToken));

                if (completedTask == receiveTask)
                {
                    HandleReceivedPacket(receiveTask.Result.Buffer);
                }
            }
        }

        /// <summary>
        /// Processes received UDP packets and raises the appropriate event.
        /// </summary>
        private void HandleReceivedPacket(byte[] buffer)
        {
            Log.Information($"Received {buffer.Length}");

            if (buffer.Length != 8)
                return; // Ignore invalid packets

            // Extract two 4-byte integers from the buffer
            int messageType = BitConverter.ToInt32(buffer, 0);
            int payload = BitConverter.ToInt32(buffer, 4);

            Log.Information($"{messageType} {payload}");
            switch ((EventType)messageType)
            {
                case EventType.Card:
                    CardScanned?.Invoke(payload); // Card ID is in the second int
                    break;

                case EventType.Remote:
                    Log.Information($"{payload >> 4} {payload & 0x0F}");
                    BtnPressed?.Invoke(payload >> 4, payload & 0x0F); // Extract two values
                    break;
            }
        }

    }
}
