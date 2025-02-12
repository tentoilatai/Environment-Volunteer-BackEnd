using EnvironmentVolunteer.Core.ApiModels;
using EnvironmentVolunteer.DataAccess.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using EnvironmentVolunteer.Core.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EnvironmentVolunteer.Core.Enums;
using EnvironmentVolunteer.Core.Exceptions;
using System.ComponentModel;

namespace EnvironmentVolunteer.Service.Implementation
{
    public class BaseService
    {
        protected AppSettings _appSettings { get; set; }
        protected IUnitOfWork _unitOfWork { get; set; }
        protected UserContext _userContext { get; set; }
        protected IMapper _mapper { get; set; }

        public BaseService(AppSettings appSettings, IUnitOfWork unitOfWork, UserContext userContext, IMapper mapper)
        {
            _appSettings = appSettings;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _mapper = mapper;
        }


    }
}
