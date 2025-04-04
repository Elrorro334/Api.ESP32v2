using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.ESP32.Models
{
    public class SensorData
    {
        public float Temperature { get; set; }
        public float PH { get; set; }
        public float TDS { get; set; }
        public string DeviceId { get; set; }
        public string Timestamp { get; set; } = System.DateTime.UtcNow.ToString("o");
    }
}