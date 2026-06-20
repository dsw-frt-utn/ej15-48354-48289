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
}

public class DoctorRequest
{
    public string Name { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public Guid SpecialityId { get; set; }
} //-> sirve para mapear el json que recibimos --una vez que ande sacalo @lauti
 