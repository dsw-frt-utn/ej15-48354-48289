using Microsoft.AspNetCore.Mvc;
using Dsw2026Ej15.Domain;
using Dsw2026Ej15.Data;

namespace Dsw2026Ej15.Api.Controllers;

[ApiController]

[Route("api/[controller]")]

public class DoctorsController : ControllerBase
{
    private readonly IPersistence _persistence;
    public DoctorsController(IPersistence persistence)
    {
        _persistence = persistence;
    }

    [HttpPost]
    public IActionResult Post([FromBody] DoctorRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Name es requerido");

        if (string.IsNullOrWhiteSpace(request.LicenseNumber))
            throw new ValidationException("LicenseNumber es requerido");

        var speciality = _persistence.GetSpecialityById(request.SpecialityId);

        if (speciality == null)
            throw new ValidationException("La especialidad indicada no existe");

        var doctor = new Doctor
        {
            Name = request.Name,
            LicenseNumber = request.LicenseNumber,
            IsActive = true,
            Speciality = speciality
        };
        _persistence.AddDoctor(doctor);

        return Created(string.Empty, doctor);
    }

    [HttpGet]
    public IActionResult Get()
    {
        var doctors = _persistence.GetActiveDoctors();
        return Ok(doctors);
    }
    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var doctor = _persistence.GetActiveDoctorById(id);
        if (doctor == null) 
            return NotFound(new { Message = "Médico no encontrado o inactivo" });

        // Retornamos solo los datos solicitados por la consigna
        var result = new
        {
            doctor.Name,
            doctor.LicenseNumber,
            SpecialityName = doctor.Speciality.Name
        };

        return Ok(result);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var success = _persistence.DeactivateDoctor(id);
        if (!success) 
            return NotFound(new { Message = "Médico no encontrado o inactivo" });

        return NoContent();
    }
}

public class DoctorRequest
{
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public Guid SpecialityId { get; set; }
} //-> sirve para mapear el json que recibimos --una vez que ande sacalo @lauti
 