using Akka.Actor;
using Arches.Actors.Implementation;
using Arches.Contract.PumpService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Arches.Api.Controller
{
    /// <inheritdoc />
    /// <summary>
    /// Методы работы с исполняемым файлом ServiceStatic files\PumpService\PyHyCarSim.exe
    /// </summary>
    [Produces("application/json")]
    [Route("v1")]
    public class PumpController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly ActorSystem _actorSystem;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerFactory _factory;
        private readonly ILogger _logger;

        public PumpController(ActorSystem actorSystem, IHttpClientFactory httpClientFactory, ILoggerFactory factory)
        {
            _actorSystem = actorSystem;
            _httpClientFactory = httpClientFactory;
            _factory = factory;
            _logger = factory.CreateLogger(typeof(PumpController));
        }

        [HttpGet("pump/ping")]
        public IActionResult Get()
        {
            _logger.LogDebug(400, "Well done!");

            return Ok();
        }

        /// <summary>
        /// Начать калькуляцию в акторе асинхронно. С отчетом по указанному урлу в запросе.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /calc
        ///     {
        ///        "CalculationUuid": "34A81E47-7935-4951-A452-380036BBB4BE",
        ///        "VxDataUrl": "http://localhost:24001/",
        ///        "EcnDataUrl": "http://localhost:24001/",
        ///        "Temperature": 120,
        ///        "WallTemperature": 70,
        ///        "Pressure": 700
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        /// <response code="200">Task in progress</response>
        /// <response code="400">Actor instantiation error</response>
        [HttpPost("pump/calc")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public IActionResult Calc([FromBody]PumpCalcCommand request)
        {
            try
            {
                _actorSystem
                    .ActorOf(
                        Props.Create(() => new PumpCalcActor(_httpClientFactory, _factory.CreateLogger(typeof(PumpCalcActor)))),
                        request.CalculationUuid
                    )
                    .Tell(request);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Actor instantiation exception for CalculationUuid {request.CalculationUuid}.");
                return BadRequest();
            }

            return Accepted();
        }
    }
}