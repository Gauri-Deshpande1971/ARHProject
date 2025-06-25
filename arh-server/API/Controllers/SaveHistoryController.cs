using API.Dtos;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Services;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Authorization;
using API.Errors;

namespace API.Controllers
{
    public class PatientHistoryController : BaseWithUserApiController
    {
        ILogger<PatientController> _logger;

        public PatientHistoryController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
                IMapper mapper,
                IMastersService ms,
                ILogger<PatientController> logger,
                IFormGridService<patientDto> fgs
                ) : base(userManager, signInManager, mapper, ms)
        {
            _logger = logger;
        }

        [HttpPost("savepatientComplaints")]
        public async Task<ActionResult<complaintsDto>> SavePatientcomplaints(complaintsDto complaints)
        {
            if (complaints == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!complaints.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if(complaints.UCode==Guid.Empty) 
                complaints.UCode = Guid.NewGuid();
            complaints o = null;
            try
            {
                o = _mapper.Map<complaintsDto, complaints>(complaints);
                o = await _ms.SaveComplaintsAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<complaints, complaintsDto>(o));
        }
        [HttpPost("savepatientPhysicalExam")]
        public async Task<ActionResult<physicalexamDto>> SavePatientcomplaints(physicalexamDto physicalexam)
        {
            if (physicalexam == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!physicalexam.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (physicalexam.UCode == Guid.Empty)
                physicalexam.UCode = Guid.NewGuid();

            physicalexam o = null;
            try
            {
                o = _mapper.Map<physicalexamDto, physicalexam>(physicalexam);
                o = await _ms.SavePhysicalexamAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<physicalexam, physicalexamDto>(o));
        }
        [HttpPost("savepatientPhysicalGeneral")]
        public async Task<ActionResult<physicalexamDto>> SavePatientcomplaints(physicalgenDto physicalgen)
        {
            if (physicalgen == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!physicalgen.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (physicalgen.UCode == Guid.Empty)
                physicalgen.UCode = Guid.NewGuid();

            physicalgen o = null;
            try
            {
                o = _mapper.Map<physicalgenDto, physicalgen>(physicalgen);
                o = await _ms.SavePhysicalgeneralAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<physicalgen, physicalgenDto>(o));
        }
        [HttpPost("savepatientPastHistory")]
        public async Task<ActionResult<pasthistory>> SavePatientpasthistory(pasthistoryDto pasthistory)
        {
            if (pasthistory == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!pasthistory.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (pasthistory.UCode == Guid.Empty)
                pasthistory.UCode = Guid.NewGuid();

            pasthistory o = null;
            try
            {
                o = _mapper.Map<pasthistoryDto, pasthistory>(pasthistory);
                o = await _ms.SavePastHistoryAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<pasthistory, pasthistoryDto>(o));
        }
        [HttpPost("savepatientMedications")]
        public async Task<ActionResult<pasthistory>> SavePatientMedications(medicationsDto medications)
        {
            if (medications == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!medications.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (medications.UCode == Guid.Empty)
                medications.UCode = Guid.NewGuid();

            medications o = null;
            try
            {
                o = _mapper.Map<medicationsDto, medications>(medications);
                o = await _ms.SaveMedicationsAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<medications, medicationsDto>(o));
        }
        [HttpPost("savepatientAdditionalReports")]
        public async Task<ActionResult<additionalreports>> SavePatientAdditionalReports(additionalreportsDto additionalreports)
        {
            if (additionalreports == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!additionalreports.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (additionalreports.UCode == Guid.Empty)
                additionalreports.UCode = Guid.NewGuid();

            additionalreports o = null;
            try
            {
                o = _mapper.Map<additionalreportsDto, additionalreports>(additionalreports);
                o = await _ms.SaveAdditionalReportsAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<additionalreports, additionalreportsDto>(o));
        }
        [HttpPost("savepatientfamilyHistory")]
        public async Task<ActionResult<family>> SavePatientFamilyHistory(familyDto family)
        {
            if (family == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!family.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (family.UCode == Guid.Empty)
                family.UCode = Guid.NewGuid();

            family o = null;
            try
            {
                o = _mapper.Map<familyDto, family>(family);
                o = await _ms.SaveFamilyHistoryAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<family, familyDto>(o));
        }
        [HttpPost("savepatientsystemeticExam")]
        public async Task<ActionResult<family>> SavePatientsystemeticExam(systemeticDto systemetic)
        {
            if (systemetic == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!systemetic.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (systemetic.UCode == Guid.Empty)
                systemetic.UCode = Guid.NewGuid();

            systemeticexam o = null;
            try
            {
                o = _mapper.Map<systemeticDto, systemeticexam>(systemetic);
                o = await _ms.SavesystemicExamAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<systemeticexam, systemeticDto>(o));
        }

        [HttpPost("save-investigations")]
        public async Task<ActionResult<List<investigationsDto>>> SaveInvestigations(List<investigationsDto> investigationsDtoList)
        {
            if (investigationsDtoList == null || investigationsDtoList.Count == 0)
            {
                return BadRequest(new ApiResponse(401, "No investigation data received."));
            }

            if (!investigationsDtoList.All(i => i.IsValidForSave()))
            {
                return BadRequest(new ApiResponse(401, "Some investigations have missing data."));
            }

            var cu = await GetCurrentUser();
            foreach (var item in investigationsDtoList)
            {
                if (item.UCode == Guid.Empty)
                    item.UCode = Guid.NewGuid();
            }
            try
            {
                var mappedList = _mapper.Map<List<investigationsDto>, List<investigations>>(investigationsDtoList);
                var savedList = await _ms.SaveInvestigationsAsync(mappedList);

                if (savedList.Any(x => x.HasErrors()))
                {
                    return BadRequest(new ApiResponse(401, savedList.First().Errors.errormessage));
                }

                var dtoList = _mapper.Map<List<investigations>, List<investigationsDto>>(savedList);
                return Ok(dtoList);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Server error: " + ex.Message));
            }
        }
        [HttpPost("updatepatientHistory")]
        public async Task<ActionResult<family>> UpdatePatientHistory(patientDto history)
        {
            if (history == null)
            {
                return BadRequest(new ApiResponse(401, "No User info received !!"));
            }

            if (!history.IsValidForSave())
            {
                return BadRequest(new ApiResponse(401, "Incomplete info received !!"));
            }

            var cu = await GetCurrentUser();
            if (history.UCode == Guid.Empty)
                history.UCode = Guid.NewGuid();

            patient o = null;
            try
            {
                o = _mapper.Map<patientDto, patient>(history);
                o = await _ms.UpdatePatientHistoryAsync(o);
                if (o == null)
                {
                    return BadRequest(new ApiResponse(401, "Unable to Save"));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(401, "Data received issue:\r\n" + ex.Message));
            }
            return Ok(_mapper.Map<patient, patientDto>(o));
        }


    }
}
