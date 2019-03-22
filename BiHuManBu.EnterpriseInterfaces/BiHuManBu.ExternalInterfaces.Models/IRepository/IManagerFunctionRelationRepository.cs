﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BiHuManBu.ExternalInterfaces.Models.Model;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.Dtos;
using BiHuManBu.ExternalInterfaces.Models.ReportModel;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;

namespace BiHuManBu.ExternalInterfaces.Models
{
    public interface IManagerFunctionRepository
    {

        List<manager_function_db> GetByParentCode(string parentCode);

    }
}

