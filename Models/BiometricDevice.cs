namespace EmployeeManagementSystem.Models
{
    public class BiometricDevice
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public bool IsOnline { get; set; }
    }
}