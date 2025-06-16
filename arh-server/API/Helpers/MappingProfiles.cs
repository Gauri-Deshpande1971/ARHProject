using System;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using System.Linq;
using System.Globalization;
using Core.Models;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<FormGridDetail, FormGridDetailDto>();

            CreateMap<FormGridDetailDto, FormGridDetail>();

            CreateMap<Attachment, AttachmentDto>()
            .ReverseMap();

            //CreateMap<AppRole, AppRoleDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            CreateMap<Attachment, AttachmentDto>()
            .ReverseMap();
            CreateMap<Medicine, MedicineDto>()
            .ReverseMap();

            CreateMap<patient,patientDto>() .ReverseMap();
            CreateMap<appointments, appointmentsDto>() .ReverseMap();   

      //      CreateMap<patientDto, patient>()
      //.ForMember(d => d.CreatedOn, o => o.MapFrom(s =>
      //    string.IsNullOrEmpty(s.CreatedOn)
      //        ? DateTime.MinValue
      //        : DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
           
      //      CreateMap<patient, patientDto>()
      //          .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.CreatedOn.ToString("dd-MM-yyyy")));

      //      CreateMap<appointmentsDto, appointments>()
      //   .ForMember(d => d.CreatedOn, o => o.MapFrom(s =>
      //       string.IsNullOrEmpty(s.CreatedOn)
      //           ? DateTime.MinValue
      //           : DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
      // //  .ForMember(d => d.UCode, o => o.MapFrom(s => ParseGuid(s.UCode)));
                

            // Helper method
            // Helper method
            //CreateMap<Department, DepartmentDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));


            //CreateMap<MailConfig, MailConfigDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            //CreateMap<MailLog, MailLogDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            // CreateMap<ActionLog, ActionLogDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            //CreateMap<Organization, OrganizationDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            //CreateMap<SysData, SysDataDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            //CreateMap<UserNavMenu, UserNavMenuDto>()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            //.ReverseMap()
            //    .ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.IsNullOrEmpty(s.CreatedOn) ? Convert.ToDateTime(null) :  
            //            DateTime.ParseExact(s.CreatedOn, "dd-MM-yyyy", CultureInfo.InvariantCulture)));

            CreateMap<FormGridDetail, FormGridDetailDto>()
                //.ForMember(d => d.CreatedOn, o => o.MapFrom(s => String.Format("{0:dd-MM-yyyy}", s.CreatedOn)))
            .ReverseMap();
          
        }
        private static Guid ParseGuid(string input)
        {
            return Guid.TryParse(input, out Guid guid) ? guid : Guid.NewGuid();
        }

    }
}
