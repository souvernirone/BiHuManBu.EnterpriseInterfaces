﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces
{
    public interface IGetTeamInfoByInComingService
    {
        TeamLeaderInfoViewModel GetTeamInfoByInComing(double incoming);
    }
}
