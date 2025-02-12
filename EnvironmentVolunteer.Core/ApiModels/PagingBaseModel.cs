using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EnvironmentVolunteer.Core.Enums;

namespace EnvironmentVolunteer.Core.ApiModels
{
    public class PagingBaseModel
    {
        [DefaultValue(1)]
        public int PageIndex { get; set; } = 1;
        [DefaultValue(50)]
        public int PageSize { get; set; } = 50;
    }
}
