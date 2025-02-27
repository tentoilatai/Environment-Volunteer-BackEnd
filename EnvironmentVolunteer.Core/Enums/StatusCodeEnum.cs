using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentVolunteer.Core.Enums
{
    public enum StatusCodeEnum
    {
        Success = 0,

        [Description("System Error.")]
        Error = 1,

        [Description("Concurrency Conflict")]
        ConcurrencyConflict = 2,

        [Description("Not Found")]
        PageIndexInvalid = 3,

        [Description("Page Size Invalid")]
        PageSizeInvalid = 4,

        [Description("{Required Field} is required.")]
        A01,

        [Description("Incorrect username or password. Please try again.")]
        A02,

        [Description("Register account failed. Please try again.")]
        B01,

        [Description("Password must be at least 8 characters long, contain at least one uppercase letter, one number, one digit, one lowercase, one unique chars , one non-alphanumeric. Please try again.")]
        B02,


        [Description("Invalid email. Please try to enter correctly format again.")]
        B03,

        [Description("Invalid email. Please try to enter correctly format again.")]
        B04,

        [Description("{{Object}} not found")]
        A03,

        [Description("{{Client_Name}} has been created successfully.")]
        A04,

        [Description("Download Interrupted. Please check your internet connection and try again.")]
        A05,

        [Description("Bad request.")]
        A06,

        [Description("Invalid filter option.")]
        InvalidOption,

        [Description("Unmatched columns found.")]
        UnmatchedColumns,

        [Description("Bad request.")]
        BadRequest,
    }
}
