using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementSystem.Services
{
    public class BiometricListenerService
    {
        private readonly AttendanceService _attendanceService;
        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cts;

        public event EventHandler<PunchReceivedEventArgs> PunchReceived;

        public BiometricListenerService(AttendanceService attendanceService, int port = 4370)
        {
            _attendanceService = attendanceService;
            _port = port;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Task.Run(() => AcceptLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            _cts?.Cancel();
            _listener?.Stop();
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client, token);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                var buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (bytesRead == 0) return;

                string raw = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();
                ProcessPunch(raw);
            }
        }

        private void ProcessPunch(string raw)
        {
            var parts = raw.Split(',');
            if (parts.Length != 3) return;

            string biometricId = parts[0];
            string punchType = parts[2].Trim().ToUpperInvariant();

            if (!DateTime.TryParseExact(parts[1], "yyyyMMddHHmmss", CultureInfo.InvariantCulture,
                                         DateTimeStyles.None, out var punchTime))
                return;

            _attendanceService.RecordPunch(biometricId, punchTime, punchType, deviceId: "DEV-01");
            PunchReceived?.Invoke(this, new PunchReceivedEventArgs(biometricId, punchTime, punchType));
        }
    }

    public class PunchReceivedEventArgs : EventArgs
    {
        public string BiometricId { get; }
        public DateTime PunchTime { get; }
        public string PunchType { get; }

        public PunchReceivedEventArgs(string biometricId, DateTime punchTime, string punchType)
        {
            BiometricId = biometricId;
            PunchTime = punchTime;
            PunchType = punchType;
        }
    }
}