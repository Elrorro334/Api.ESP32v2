using Api.ESP32.Models;
using System;
using System.Collections.Concurrent;
using System.Web.Http;

namespace Api.ESP32.Controllers
{
    public class SensorDataController : ApiController
    {
        // Almacenamiento en memoria (últimos 10 registros)
        private static readonly ConcurrentQueue<SensorData> _latestData = new ConcurrentQueue<SensorData>();
        private static readonly object _lock = new object();
        private const int MaxRecords = 10;

        // POST: api/datos-sensores
        [HttpPost]
        [Route("api/datos-sensores")]
        public IHttpActionResult PostSensorData([FromBody] SensorData request)
        {
            if (request == null)
            {
                return BadRequest("Datos inválidos");
            }

            try
            {
                // Actualizar el timestamp
                request.Timestamp = DateTime.UtcNow.ToString("o");

                // Mantener solo los últimos 10 registros en memoria
                lock (_lock)
                {
                    _latestData.Enqueue(request);
                    while (_latestData.Count > MaxRecords)
                    {
                        _latestData.TryDequeue(out _);
                    }
                }

                // Mostrar los datos en la consola (para depuración)
                Console.WriteLine($"Datos recibidos - Temp: {request.Temperature}°C, pH: {request.PH}, TDS: {request.TDS}ppm");

                return Ok(new
                {
                    status = "success",
                    message = "Datos recibidos en tiempo real",
                    data = request
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/datos-sensores
        [HttpGet]
        [Route("api/datos-sensores")]
        public IHttpActionResult GetLatestData()
        {
            return Ok(new
            {
                status = "success",
                timestamp = DateTime.UtcNow.ToString("o"),
                data = _latestData.ToArray()
            });
        }

        // GET: api/datos-sensores/ultimo
        [HttpGet]
        [Route("api/datos-sensores/ultimo")]
        public IHttpActionResult GetLastDataPoint()
        {
            if (_latestData.TryPeek(out SensorData lastData))
            {
                return Ok(new
                {
                    status = "success",
                    data = lastData
                });
            }

            return NotFound();
        }
    }
}