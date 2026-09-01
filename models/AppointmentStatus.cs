using System.Text.Json.Serialization;

namespace backend.clinicalbackend.models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AppointmentStatus
{
    Pending,
    Confirmed,
    Completed,
    Cancelled
}
