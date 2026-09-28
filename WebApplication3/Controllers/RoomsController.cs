using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class RoomsController : ApiController
    {
        [HttpGet, Route("api/rooms")]
        public IHttpActionResult GetAll()
        {
            return Ok(Data.Rooms);
        }
        [HttpGet, Route("api/rooms/available")]
        public IHttpActionResult Available(DateTime? start = null, DateTime? end = null,
            int? participants = null, bool? projector = null, bool? whiteboard = null)
        {
            if (start == null || end == null) return BadRequest("Нужно указать start и end.");
            if (start >= end) return BadRequest("start должен быть меньше end.");
            if (participants <= 0) return BadRequest("participants должен быть больше нуля.");

            var result = Data.Rooms.Where(r => r.IsActive
                && (participants == null || r.Capacity >= participants)
                && (projector != true || r.HasProjector)
                && (whiteboard != true || r.HasWhiteboard)
                && !Data.Overlaps(r.Id, start.Value, end.Value, 0));

            return Ok(result.ToList());
        }
        [HttpGet, Route("api/rooms/{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            Room room = Data.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return Content(HttpStatusCode.NotFound, new { message = "Комната " + id + " не найдена." });
            return Ok(room);
        }
        [HttpPost, Route("api/rooms")]
        public IHttpActionResult Create(Room room)
        {
            if (room == null || !ModelState.IsValid) return BadRequest("Некорректное тело запроса.");

            IHttpActionResult error = Check(room, 0);
            if (error != null) return error;

            room.Id = Data.NextRoomId++;
            room.Name = room.Name.Trim();
            Data.Rooms.Add(room);
            return Created(Request.RequestUri + "/" + room.Id, room);
        }
        [HttpPut, Route("api/rooms/{id:int}")]
        public IHttpActionResult Update(int id, Room newData)
        {
            Room room = Data.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return Content(HttpStatusCode.NotFound, new { message = "Комната " + id + " не найдена." });
            if (newData == null || !ModelState.IsValid) return BadRequest("Некорректное тело запроса.");

            IHttpActionResult error = Check(newData, id);
            if (error != null) return error;

            room.Name = newData.Name.Trim();
            room.Capacity = newData.Capacity;
            room.HasProjector = newData.HasProjector;
            room.HasWhiteboard = newData.HasWhiteboard;
            room.IsActive = newData.IsActive;
            return Ok(room);
        }
        [HttpPut, Route("api/rooms/{id:int}/deactivate")]
        public IHttpActionResult Deactivate(int id)
        {
            Room room = Data.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return Content(HttpStatusCode.NotFound, new { message = "Комната " + id + " не найдена." });
            if (!room.IsActive) return Content(HttpStatusCode.Conflict, new { message = "Комната уже неактивна." });

            room.IsActive = false;
            return Ok(room);
        }
        private IHttpActionResult Check(Room room, int selfId)
        {
            if (string.IsNullOrWhiteSpace(room.Name))
                return BadRequest("Название комнаты не может быть пустым.");
            if (room.Capacity <= 0)
                return BadRequest("Вместимость должна быть больше нуля.");

            bool exists = Data.Rooms.Any(r => r.Id != selfId
                && string.Equals(r.Name, room.Name.Trim(), StringComparison.OrdinalIgnoreCase));
            if (exists)
                return Content(HttpStatusCode.Conflict, new { message = "Комната с таким названием уже существует." });

            return null;
        }
    }
}