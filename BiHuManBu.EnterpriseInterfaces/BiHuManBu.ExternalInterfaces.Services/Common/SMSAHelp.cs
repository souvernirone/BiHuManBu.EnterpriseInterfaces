using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.AppRequest;

namespace BiHuManBu.ExternalInterfaces.Services.Common
{
    class SMSAHelp
    {
       public  tx_clues RegZHBX(AnalysisSmsRequest request, int agentId)
        {

            int casetype = 0;
            Regex regSource = new Regex("【(.*?)】");
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;



       

            Regex regSMoldName = new Regex(@"主车，【.*?】【(.*?)】");
            Match smatch = regSMoldName.Match(request.SmsContent);
            string smoldName = smatch.Groups[1].Value;


            Regex regSCasetype = new Regex("（返修案件）");
            smatch = regSCasetype.Match(request.SmsContent);
            string scaseType = smatch.Groups[1].Value;

            Regex regReportCaseNum1 = new Regex(@"我司推荐:报案号(.*?)，");
            match = regReportCaseNum1.Match(request.SmsContent);
            string reportCaseNum1 = match.Groups[1].Value;


            if (scaseType == "送修案件")
            {
                casetype = 1;
            }
            else if (scaseType == "返修案件")
            {
                casetype = 2;
            }
            else if (scaseType == "三者车")
            {
                casetype = 3;
            }
            return new tx_clues()
            {
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                sourcename = source,//"太平洋车险",
                source = 4,
                agentid = agentId,
                casetype = casetype,
                mobile = "",
                ReportCasePeople = "",
                followupstate = -1,
                HasInsureInfo = 0,//初始没有保险
                Deleted = 0,//未删除
                
                MoldName = "",
                accidentremark = "",
                city_name = "",
                CarVIN = "",
                dangerarea = "",
                licenseno = "",
                ReportCaseNum = "",
                smsrecivedtime = ""
                ,
                last_follow_id = 0
            };
        }

        public tx_clues RegTPYBX(AnalysisSmsRequest request, int agentId)
        {
            int casetype = 0;
            Regex regSource = new Regex(@"\[(.*?)\]");
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;

            if (request.SmsContent.Contains("[太平洋保险]车商") || request.SmsContent.Contains("[太平洋保险]车商"))
            {
                //深圳
                Regex regSCarVIN = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})");
                Match smatch = regSCarVIN.Match(request.SmsContent);
                string sCarVIN = smatch.Groups[1].Value;

                Regex regSLicenseno = new Regex(@"车牌为：(.*?)，");
                smatch = regSLicenseno.Match(request.SmsContent);
                string slicenseno = smatch.Groups[1].Value;

                Regex regSMoldName = new Regex(@"品牌：(.*?)，");
                smatch = regSMoldName.Match(request.SmsContent);
                string smoldName = smatch.Groups[1].Value;


                Regex regSReportCasePeople = new Regex(@"客户：(.*?)，");
                smatch = regSReportCasePeople.Match(request.SmsContent);
                string sreportCasePeople = smatch.Groups[1].Value;


                Regex regSDesc = new Regex(@"出险经过：(.*?)；");
                smatch = regSDesc.Match(request.SmsContent);
                string sDesc = smatch.Groups[1].Value;

                Regex regSDangerarea = new Regex("事故车目前位置：(.*?)，");
                smatch = regSDangerarea.Match(request.SmsContent);
                string sdangerarea = smatch.Groups[1].Value;



                Regex regSCasetype = new Regex("（(..)案件）");
                smatch = regSCasetype.Match(request.SmsContent);
                string scaseType = smatch.Groups[1].Value;


                if (string.IsNullOrEmpty(scaseType))
                {
                    regSCasetype = new Regex("【深圳人保】人保(.*?)");
                    smatch = regSCasetype.Match(request.SmsContent);
                    scaseType = smatch.Groups[1].Value;
                }

                if (scaseType == "送修")
                {
                    casetype = 1;
                }
                else if (scaseType == "返修")
                {
                    casetype = 2;
                }
                else if (scaseType == "三者车")
                {
                    casetype = 3;
                }
                else if (scaseType == "派修")
                {
                    casetype = 2;
                }
                return new tx_clues()
                {
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    sourcename = source,//"太平洋车险",
                    source = 1,
                    agentid = agentId,
                    casetype = casetype,
                    mobile = "",
                    ReportCasePeople = sreportCasePeople,
                    followupstate = -1,
                    HasInsureInfo = 0,//初始没有保险
                    Deleted = 0,//未删除
                    
                    MoldName = smoldName,
                    accidentremark = "",
                    city_name = "",
                    CarVIN = sCarVIN,
                    dangerarea = "",
                    licenseno = slicenseno,
                    ReportCaseNum = "",
                    smsrecivedtime = ""
                    ,
                    last_follow_id = 0
                };
            }



            if (request.SmsContent.Contains("在贵厂投保的被保险人"))
            {
                Regex regReportCasePeople1 = new Regex(@"在贵厂投保的被保险人(.*?)现出险");
                match = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match.Groups[1].Value;

                Regex regDangerarea1 = new Regex("出险地点(.*?)回");
                match = regDangerarea1.Match(request.SmsContent);
                string dangerarea1 = match.Groups[1].Value;


           
                return new tx_clues()
                {
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    sourcename = source,//"太平洋车险",
                    source = 1,
                    agentid = agentId,
                    casetype = casetype,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    HasInsureInfo = 0,//初始没有保险
                    Deleted = 0,//未删除
                    
                    MoldName = "",
                    accidentremark = "",
                    city_name = "",
                    CarVIN = "",
                    dangerarea = dangerarea1,
                    licenseno = "",
                    ReportCaseNum = "",
                    smsrecivedtime = ""
                    ,
                    last_follow_id = 0
                };

            }





            Regex regMobile = new Regex(@"\[(.*?)\](\d{11})");
            match = regMobile.Match(request.SmsContent);
            string mobile = match.Groups[2].Value;
            if (!string.IsNullOrEmpty(mobile))
            {
                Regex regReportCasePeople1 = new Regex(@"\[(.*?)\](.*?)无(.*?)\d");
                match = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match.Groups[3].Value;




                return new tx_clues()
                {
                    CreateTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    sourcename = source,//"太平洋车险",
                    source = 1,
                    agentid = agentId,
                    casetype = casetype,
                    mobile = mobile,
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    HasInsureInfo = 0,//初始没有保险
                    Deleted = 0,//未删除
                    
                    MoldName = "",
                    accidentremark = "",
                    city_name = "",
                    CarVIN = "",
                    dangerarea = "",
                    licenseno = "",
                    ReportCaseNum = "",
                    smsrecivedtime = ""
                    ,
                    last_follow_id = 0
                };
            }


            Regex regCasetype = new Regex("“(.*?)”");
            match = regCasetype.Match(request.SmsContent);
            string caseType = match.Groups[1].Value;


            Regex regDangerarea = new Regex("在(.*?)出险");
            match = regDangerarea.Match(request.SmsContent);
            string dangerarea = match.Groups[1].Value;
            if (string.IsNullOrEmpty(dangerarea))
            {
                regDangerarea = new Regex("车(.*?)出险");
                match = regDangerarea.Match(request.SmsContent);
                dangerarea = match.Groups[1].Value;
            }

            Regex regReportCasePeople = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})(.*?)\d");
            match = regReportCasePeople.Match(request.SmsContent);
            string reportCasePeople = match.Groups[2].Value;

            Regex regMoldName = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})(.*?)(\d){11}(.*?)，(.*?车)");
            match = regMoldName.Match(request.SmsContent);
            string moldName = match.Groups[5].Value;

            if (string.IsNullOrEmpty(moldName))
            {
                regMoldName = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})(.*?)(\d){11}(.*?)，(.*?在)");
                match = regMoldName.Match(request.SmsContent);
                moldName = match.Groups[5].Value;
            }


            Regex regCarVIN = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})");
            match = regCarVIN.Match(request.SmsContent);
            string carVIN = match.Groups[1].Value;

            if (caseType == "送")
            {
                casetype = 1;
            }
            else if (caseType == "返")
            {
                casetype = 2;
            }
            else if (caseType == "三者车")
            {
                casetype = 3;
            }
            return new tx_clues()
            {
                CarVIN = carVIN,
                CreateTime = DateTime.Now,
                sourcename = source,//"太平洋车险",
                source = 1,
                agentid = agentId,
                casetype = casetype,
                licenseno = "",
                dangerarea = dangerarea,
                MoldName = moldName,
                mobile = mobile,
                ReportCasePeople = reportCasePeople,
                followupstate = -1,
                UpdateTime = DateTime.Now,
                
                accidentremark = "",
                HasInsureInfo = 0,
                Deleted = 0,
                city_name = "",
                ReportCaseNum = "",
                smsrecivedtime = "",
                last_follow_id = 0

            };

        }

       public  tx_clues RegTPYCX(AnalysisSmsRequest request, int agentId)
        {
            int casetype = 0;


            Regex regSource = new Regex(@"\【(.*?)\】");
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;

            Regex regLicenseno = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})(.*?)(\d){11}的(.*?)(.*?([0-9a-zA-Z]{4,23}))");
            match = regLicenseno.Match(request.SmsContent);
            string licenseno = match.Groups[5].Value;

            if (!string.IsNullOrEmpty(licenseno))
            {
                Regex regReportCasePeople = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})(.*?)\d");
                match = regReportCasePeople.Match(request.SmsContent);
                string reportCasePeople = match.Groups[2].Value;

                Regex regMoldName = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})(.*?)(\d){11}的(.*?)(.*?([0-9a-zA-Z]{4,23}))(.*?)送至");
                match = regMoldName.Match(request.SmsContent);
                string moldName = match.Groups[7].Value;


                Regex regCarVIN = new Regex(@"我司已推荐.*?([0-9a-zA-Z]{4,23})");
                match = regCarVIN.Match(request.SmsContent);
                string carVIN = match.Groups[1].Value;
                return new tx_clues()
                {
                    CarVIN = carVIN,
                    CreateTime = DateTime.Now,
                    sourcename = source,//"太平洋车险",
                    source = 1,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName,
                    mobile = "",
                    ReportCasePeople = reportCasePeople,
                    followupstate = -1,
                    Deleted = 0,
                    UpdateTime = DateTime.Now,
                    city_name = "",
                    accidentremark = "",
                    
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = "",
                    smsrecivedtime = "",
                    last_follow_id = 0


                };

            }


            Regex regmobile1 = new Regex(@"我司已推荐三者车(\d){11}");
            match = regmobile1.Match(request.SmsContent);
            string mobile1 = match.Groups[1].Value;


            Regex regCaseType1 = new Regex(@"我司已推荐(三者车)");
            match = regCaseType1.Match(request.SmsContent);
            string caseType1 = match.Groups[1].Value;

           

            Regex regReportCaseNum1 = new Regex(@"报案号(.*?)，");
            match = regReportCaseNum1.Match(request.SmsContent);
            string reportCaseNum1 = match.Groups[1].Value;


            Regex regReportCasePeople1 = new Regex(@"报案人(.*?)。");
            match = regReportCasePeople1.Match(request.SmsContent);
            string reportCasePeople1 = match.Groups[1].Value;


            if (caseType1 == "送修")
            {
                casetype = 1;
            }
            else if (caseType1 == "返修")
            {
                casetype = 2;
            }
            else if (caseType1 == "三者车")
            {
                casetype = 3;
            }
            return new tx_clues()
            {
                CarVIN = "",
                CreateTime = DateTime.Now,
                sourcename = source,//"太平洋车险",
                source = 1,
                agentid = agentId,
                casetype = casetype,
                licenseno = "",
                MoldName = "",
                mobile = mobile1,
                ReportCasePeople = reportCasePeople1,
                followupstate = -1,
                Deleted = 0,
                UpdateTime = DateTime.Now,
                city_name = "",
                accidentremark = "",
                
                dangerarea = "",
                HasInsureInfo = 0,
                ReportCaseNum = reportCaseNum1,
                smsrecivedtime = "",
                last_follow_id = 0
            };





        }


        public tx_clues RegRBCX(AnalysisSmsRequest request, int agentId)
        {
            int casetype = 0;
            if (request.SmsContent.Contains("“广州人保财险”官方微信"))
            {
                Regex regReportCaseNum = new Regex(@"报案号是：(.*?)[\u4e00-\u9fa5]");
                Match match1 = regReportCaseNum.Match(request.SmsContent);
                string reportCaseNum1 = match1.Groups[1].Value;
                Regex regReportCasePeople = new Regex(@"\d([\u4e00 - \u9fa5].*?)(?< !\d)(?: (?: 1[358]\d{ 9})| (?: 861[358]\d{ 9}))(? !\d)");
                match1 = regReportCasePeople.Match(request.SmsContent);
                string reportCasePeople = match1.Groups[1].Value;

                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = "广州人保财险",//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = "",
                    mobile = "",
                    ReportCasePeople = reportCasePeople,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = reportCaseNum1,
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };

            }

            if (request.SmsContent.Contains("【人保财险】新车型推送"))
            {
                Regex regSource1 = new Regex("【(.*?)】");
                Match match1 = regSource1.Match(request.SmsContent);
                string source1 = match1.Groups[1].Value;


                Regex regMoldName = new Regex(@"新车型推送.*?([0-9a-zA-Z]{4,23})，(.*?)，(.*?)，");
                match1 = regMoldName.Match(request.SmsContent);
                string moldName = match1.Groups[3].Value;

                Regex regReportCasePeople = new Regex(@"新车型推送.*?([0-9a-zA-Z]{4,23})，(.*?)，(.*?)，(.*?)\d");
                match1 = regReportCasePeople.Match(request.SmsContent);
                string reportCasePeople = match1.Groups[4].Value;


                Regex regReportCaseNum = new Regex(@"新车型推送，(.*?)，");
                match1 = regReportCaseNum.Match(request.SmsContent);
                string reportCaseNum1 = match1.Groups[1].Value;

             

                Regex regDangerarea = new Regex(@"新车型推送.*?([0-9a-zA-Z]{4,23})，(.*?)，(.*?)，(.*?)(\d{11})，(.*?)，");
                match1 = regDangerarea.Match(request.SmsContent);
                string dangerarea = match1.Groups[6].Value;

                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = source1,//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName,
                    mobile = "",
                    ReportCasePeople = reportCasePeople,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = dangerarea,
                    HasInsureInfo = 0,
                    ReportCaseNum = reportCaseNum1,
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };

            }
            if (request.SmsContent.Contains("【人保财险】承保"))
            {
                Regex regSource1 = new Regex("【(.*?)】");
                Match match1 = regSource1.Match(request.SmsContent);
                string source1 = match1.Groups[1].Value;

                Regex regReportCaseNum1 = new Regex(@"【人保财险】承保，(.*?)，");
                match1 = regReportCaseNum1.Match(request.SmsContent);
                string reportCaseNum1 = match1.Groups[1].Value;

                Regex regReportCasePeople1 = new Regex(@"【人保财险】承保，.*?，.*?，.*?，(.*?)\d");
                match1 = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match1.Groups[1].Value;


                Regex regMoldName1 = new Regex(@"【人保财险】承保，.*?，.*?，(.*?)，");
                match1 = regMoldName1.Match(request.SmsContent);
                string moldName1 = match1.Groups[1].Value;

                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = source1,//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName1,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = reportCaseNum1,
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };

            }


            if (request.SmsContent.Contains("深圳人保"))
            {
                Regex regSource1 = new Regex("【(.*?)】");
                Match match1 = regSource1.Match(request.SmsContent);
                string source1 = match1.Groups[1].Value;

                Regex regReportCaseNum1 = new Regex(@"报案号:(.*?),");
                match1 = regReportCaseNum1.Match(request.SmsContent);
                string reportCaseNum1 = match1.Groups[1].Value;



                Regex regCaseType1 = new Regex(@"【人保财险】标的车(..)");
                match1 = regCaseType1.Match(request.SmsContent);
                string caseType1 = match1.Groups[1].Value;

                if (string.IsNullOrEmpty(caseType1))
                {
                    regCaseType1 = new Regex(@"【深圳人保】人保(..)");
                    match1 = regCaseType1.Match(request.SmsContent);
                    caseType1 = match1.Groups[1].Value;
                }

                Regex regReportCasePeople1 = new Regex(@"客户:(.*?),");
                match1 = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match1.Groups[1].Value;

               

                Regex regMoldName1 = new Regex(@"车型:(.*?),");
                match1 = regMoldName1.Match(request.SmsContent);
                string moldName1 = match1.Groups[1].Value;

              

                Regex regDangerarea1 = new Regex("出险地址:(.*?),");
                match1 = regDangerarea1.Match(request.SmsContent);
                string dangerarea1 = match1.Groups[1].Value;


                Regex regReportTime1 = new Regex("报案时间:(.*?),");
                match1 = regReportTime1.Match(request.SmsContent);
                string reportTime1 = match1.Groups[1].Value;
                if (string.IsNullOrEmpty(reportTime1))
                {
                    regReportTime1 = new Regex("出险时间：(.*?)。");
                    match1 = regReportTime1.Match(request.SmsContent);
                    reportTime1 = match1.Groups[1].Value;
                }

                if (caseType1 == "送修")
                {
                    casetype = 1;
                }
                else if (caseType1 == "返修")
                {
                    casetype = 2;
                }
                else if (caseType1 == "三者车")
                {
                    casetype = 3;
                }

                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = source1,//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName1,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = dangerarea1,
                    HasInsureInfo = 0,
                    ReportCaseNum = reportCaseNum1,
                    smsrecivedtime = reportTime1,
                    CarVIN = "",
                    last_follow_id = 0
                };

            }



            if (request.SmsContent.Contains("定损告知通知"))
            {

                Regex regSource1 = new Regex("【(.*?)】");
                Match match1 = regSource1.Match(request.SmsContent);
                string source1 = match1.Groups[1].Value;


                Regex regReportCaseNum1 = new Regex(@"报案号:(.*?),");
                match1 = regReportCaseNum1.Match(request.SmsContent);
                string reportCaseNum1 = match1.Groups[1].Value;

     

                Regex regMoldName1 = new Regex(@"品牌:(.*?),");
                match1 = regMoldName1.Match(request.SmsContent);
                string moldName1 = match1.Groups[1].Value;


                Regex regReportCasePeople1 = new Regex(@"客户名称:(.*?),");
                match1 = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match1.Groups[1].Value;


                Regex regDangerarea1 = new Regex("出险地:(.*?)已在该店定损");
                match1 = regDangerarea1.Match(request.SmsContent);
                string dangerarea1 = match1.Groups[1].Value;

                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = source1,//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName1,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = dangerarea1,
                    HasInsureInfo = 0,
                    ReportCaseNum = reportCaseNum1,
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };
            }


            Regex regSource = new Regex("【(.*?)】");
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;



            if (request.SmsContent.Contains("新车型推送"))
            {
                Regex regMoldName = new Regex(@"车型：(.*?)，");
                match = regMoldName.Match(request.SmsContent);
                string moldName = match.Groups[1].Value;
                Regex regReportCasePeople = new Regex(@"客户：(.*?)，");
                match = regReportCasePeople.Match(request.SmsContent);
                string reportCasePeople = match.Groups[1].Value;
                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = source,//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName,
                    mobile = "",
                    ReportCasePeople = reportCasePeople,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = "",
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };



            }

            if (request.SmsContent.Contains("新承保推送"))
            {
                Regex regMoldName = new Regex(@"车型：(.*?)，");
                match = regMoldName.Match(request.SmsContent);
                string moldName = match.Groups[1].Value;
                if (string.IsNullOrEmpty(moldName))
                {
                    regMoldName = new Regex(@"新承保推送.*?([0-9a-zA-Z]{4,23})，(.*?)，(.*?)，");
                    match = regMoldName.Match(request.SmsContent);
                    moldName = match.Groups[3].Value;
                }
                Regex regReportCasePeople = new Regex(@"联系人：(.*?)，");
                match = regReportCasePeople.Match(request.SmsContent);
                string reportCasePeople = match.Groups[1].Value;
                if (string.IsNullOrEmpty(reportCasePeople))
                {
                    regReportCasePeople = new Regex(@"新承保推送.*?([0-9a-zA-Z]{4,23})，(.*?)，(.*?)，(.*?)\d");
                    match = regReportCasePeople.Match(request.SmsContent);
                    reportCasePeople = match.Groups[4].Value;
                }

                return new tx_clues()
                {

                    CreateTime = DateTime.Now,
                    sourcename = source,//"人保车险",
                    source = 2,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName,
                    mobile = "",
                    ReportCasePeople = reportCasePeople,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = "",
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };
            }

          

            Regex regMoldName2 = new Regex(@"品牌:(.*?),");
            match = regMoldName2.Match(request.SmsContent);
            string moldName2 = match.Groups[1].Value;


            Regex regReportCasePeople2 = new Regex(@"客户名称:(.*?),");
            match = regReportCasePeople2.Match(request.SmsContent);
            string reportCasePeople2 = match.Groups[1].Value;


            Regex regDangerarea2 = new Regex(@"出险地:(.*?).");
            match = regDangerarea2.Match(request.SmsContent);
            string dangerarea2 = match.Groups[1].Value;

            Regex regReportCaseNum2 = new Regex(@"报案号:(.*?),");
            match = regReportCaseNum2.Match(request.SmsContent);
            string reportCaseNum2 = match.Groups[1].Value;


            Regex regCaseType2 = new Regex(@"【人保财险】(..)，");
            match = regCaseType2.Match(request.SmsContent);
            string caseType2 = match.Groups[1].Value;


            if (caseType2 == "送修")
            {
                casetype = 1;
            }
            else if (caseType2 == "返修")
            {
                casetype = 2;
            }
            else if (caseType2 == "三者车")
            {
                casetype = 3;
            }
            return new tx_clues()
            {
                CreateTime = DateTime.Now,
                sourcename = source,//"人保车险",
                source = 2,
                agentid = agentId,
                casetype = casetype,
                licenseno = "",
                MoldName = moldName2,
                mobile = "",
                ReportCasePeople = reportCasePeople2,
                followupstate = -1,
                UpdateTime = DateTime.Now,
                
                Deleted = 0,
                city_name = "",
                accidentremark = "",
                dangerarea = dangerarea2,
                HasInsureInfo = 0,
                ReportCaseNum = reportCaseNum2,
                smsrecivedtime = "",
                CarVIN = "",
                last_follow_id = 0
            };

        }


        public tx_clues RegPACX(AnalysisSmsRequest request, int agentId)
        {

            Regex regSource = new Regex("【(.*?)】");
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;


            Regex regCasetype = new Regex("【平安产险】(..)");
            match = regCasetype.Match(request.SmsContent);
            string caseType = match.Groups[1].Value;



            Regex regDangerarea = new Regex("出险地：(.*?)，");
            match = regDangerarea.Match(request.SmsContent);
            string dangerarea = match.Groups[1].Value;


          

            Regex regReportCaseNum = new Regex(@"报案号(.*?)，");
            match = regReportCaseNum.Match(request.SmsContent);
            string reportCaseNum = match.Groups[1].Value;

            Regex regReportCasePeople = new Regex(@"客户：(.*?)，");
            match = regReportCasePeople.Match(request.SmsContent);
            string reportCasePeople = match.Groups[1].Value;
            if (string.IsNullOrEmpty(reportCasePeople))
            {
                regReportCasePeople = new Regex(@"客户(.*?)，");
                match = regReportCasePeople.Match(request.SmsContent);
                reportCasePeople = match.Groups[1].Value;
            }



            Regex regMoldName = new Regex(@"品牌：(.*?)，");
            match = regMoldName.Match(request.SmsContent);
            string moldName = match.Groups[1].Value;


            if (string.IsNullOrEmpty(moldName))
            {
                regMoldName = new Regex(@"品牌(.*?)。");
                match = regMoldName.Match(request.SmsContent);
                moldName = match.Groups[1].Value;
            }

            int casetype = 0;
            if (caseType == "送修")
            {
                casetype = 1;
            }
            else if (caseType == "返修")
            {
                casetype = 2;
            }
            else if (caseType == "三者车")
            {
                casetype = 3;
            }
            return new tx_clues()
            {
                CreateTime = DateTime.Now,
                sourcename = source,//"平安车险",
                source = 0,
                agentid = agentId,
                casetype = casetype,
                licenseno = "",
                MoldName = moldName,
                mobile = "",
                ReportCasePeople = reportCasePeople,
                followupstate = -1,
                UpdateTime = DateTime.Now,
                
                Deleted = 0,
                city_name = "",
                accidentremark = "",
                dangerarea = dangerarea,
                HasInsureInfo = 0,
                ReportCaseNum = reportCaseNum,
                smsrecivedtime = "",
                CarVIN = "",
                last_follow_id = 0

            };

        }



        public tx_clues RegZGPA(AnalysisSmsRequest request, int agentId)
        {
            Regex regSource = new Regex("【(.*?)】");
            int casetype = 0;
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;
            if (request.SmsContent.Contains("【中国平安】非代码单"))
            {
                //深圳
                Regex regReportCaseNum1 = new Regex(@"报案号(.*?)，");
                match = regReportCaseNum1.Match(request.SmsContent);
                string reportCaseNum1 = match.Groups[1].Value;

                Regex regMoldName1 = new Regex(@"品牌(.*?)，");
                match = regMoldName1.Match(request.SmsContent);
                string moldName1 = match.Groups[1].Value;

                Regex regReportCasePeople1 = new Regex(@"报案人(.*?)，");
                match = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match.Groups[1].Value;

                return new tx_clues()
                {
                    CreateTime = DateTime.Now,
                    sourcename = source,//"平安车险",
                    source = 0,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName1,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = reportCaseNum1,
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0

                };

            }
            

            Regex regCasetype = new Regex("【中国平安】(三者车)");
            match = regCasetype.Match(request.SmsContent);
            string caseType = match.Groups[1].Value;


            if (string.IsNullOrEmpty(caseType))
            {
                regCasetype = new Regex("(..)");
                match = regCasetype.Match(request.SmsContent);
                caseType = match.Groups[1].Value;
                if (caseType != "送修" && caseType != "返修")
                {
                    regCasetype = new Regex("【中国平安】(..)");
                    match = regCasetype.Match(request.SmsContent);
                    caseType = match.Groups[1].Value;
                }




            }

            Regex regDangerarea = new Regex("出险地：(.*?)，");
            match = regDangerarea.Match(request.SmsContent);
            string dangerarea = match.Groups[1].Value;



            Regex regDangerDesc = new Regex("出险原因：(.*?)。");
            match = regDangerDesc.Match(request.SmsContent);
            string dangerDesc = match.Groups[1].Value;


        



            Regex regReportCaseNum = new Regex(@"报案号(.*?)，");
            match = regReportCaseNum.Match(request.SmsContent);
            string reportCaseNum = match.Groups[1].Value;



            Regex regReportCasePeople = new Regex(@"客户：(.*?)，");
            match = regReportCasePeople.Match(request.SmsContent);
            string reportCasePeople = match.Groups[1].Value;
            if (string.IsNullOrEmpty(reportCasePeople))
            {
                regReportCasePeople = new Regex(@"客户(.*?)，");
                match = regReportCasePeople.Match(request.SmsContent);
                reportCasePeople = match.Groups[1].Value;
            }



            Regex regMoldName = new Regex(@"品牌：(.*?)，");
            match = regMoldName.Match(request.SmsContent);
            string moldName = match.Groups[1].Value;


            if (string.IsNullOrEmpty(moldName))
            {
                regMoldName = new Regex(@"品牌(.*?)，");
                match = regMoldName.Match(request.SmsContent);
                moldName = match.Groups[1].Value;


                if (string.IsNullOrEmpty(moldName))
                {

                    regMoldName = new Regex(@"品牌：.*?，.*?，(\d{11})，");
                    match = regMoldName.Match(request.SmsContent);
                    moldName = match.Groups[1].Value;

                }
            }


            if (caseType == "送修")
            {
                casetype = 1;
            }
            else if (caseType == "返修")
            {
                casetype = 2;
            }
            else if (caseType == "三者车")
            {
                casetype = 3;
            }
            return new tx_clues()
            {
                CreateTime = DateTime.Now,
                sourcename = source,//"平安车险",
                source = 0,
                agentid = agentId,
                casetype = casetype,
                licenseno = "",
                MoldName = moldName,
                mobile = "",
                ReportCasePeople = reportCasePeople,
                followupstate = -1,
                UpdateTime = DateTime.Now,
                
                Deleted = 0,
                city_name = "",
                accidentremark = "",
                dangerarea = dangerarea,
                HasInsureInfo = 0,
                ReportCaseNum = reportCaseNum,
                smsrecivedtime = "",
                CarVIN = "",
                last_follow_id = 0

            };

        }

        public tx_clues RegZGRSCX(AnalysisSmsRequest request, int agentId)
        {
            int casetype = 0;
            Regex regSource1 = new Regex("【(.*?)】");
            Match match1 = regSource1.Match(request.SmsContent);
            string source1 = match1.Groups[1].Value;

            if (request.SmsContent.Contains("【中国人寿财险】事故号"))
            {
                Regex regReportCaseNum1 = new Regex(@"事故号：(.*?)，");
                match1 = regReportCaseNum1.Match(request.SmsContent);
                string reportCaseNum1 = match1.Groups[1].Value;

                Regex regmoldName1 = new Regex("车型：(.*?)，");
                match1 = regmoldName1.Match(request.SmsContent);
                string moldName1 = match1.Groups[1].Value;

                Regex regReportCasePeople1 = new Regex(@"联系人：(.*?)，");
                match1 = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match1.Groups[1].Value;

                return new tx_clues()
                {
                    CreateTime = DateTime.Now,
                    sourcename = source1,//"中国人寿财险",
                    source = 3,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName1,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = "",
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };

            }


            if (request.SmsContent.Contains("请贵方速与客户取得联系。") && request.SmsContent.Contains("重庆"))
            {
                //重庆



                Regex regmoldName1 = new Regex("【中国人寿财险】.*?的(.*?)，");
                match1 = regmoldName1.Match(request.SmsContent);
                string moldName1 = match1.Groups[1].Value;



                Regex regDangerarea = new Regex("【中国人寿财险】.*?的.*?，在(.*?)出险");
                match1 = regDangerarea.Match(request.SmsContent);
                string dangerarea = match1.Groups[1].Value;


                Regex regReportCasePeople1 = new Regex(@"报案人：(.*?)\d");
                match1 = regReportCasePeople1.Match(request.SmsContent);
                string reportCasePeople1 = match1.Groups[1].Value;

                




                return new tx_clues()
                {
                    CreateTime = DateTime.Now,
                    sourcename = source1,//"中国人寿财险",
                    source = 3,
                    agentid = agentId,
                    casetype = casetype,
                    licenseno = "",
                    MoldName = moldName1,
                    mobile = "",
                    ReportCasePeople = reportCasePeople1,
                    followupstate = -1,
                    UpdateTime = DateTime.Now,
                    
                    Deleted = 0,
                    city_name = "",
                    accidentremark = "",
                    dangerarea = "",
                    HasInsureInfo = 0,
                    ReportCaseNum = "",
                    smsrecivedtime = "",
                    CarVIN = "",
                    last_follow_id = 0
                };
            }




            Regex regSource = new Regex("【(.*?)】");
            Match match = regSource.Match(request.SmsContent);
            string source = match.Groups[1].Value;


            Regex regRecivedtime = new Regex("于(.*?)成功报案");
            match = regRecivedtime.Match(request.SmsContent);
            string smsRecivedtime = match.Groups[1].Value;

           

            Regex regmoldName = new Regex("车型为(.*?)的");
            match = regmoldName.Match(request.SmsContent);
            string moldName = match.Groups[1].Value;


            Regex regCasetype = new Regex("已成功推荐(..)");
            match = regCasetype.Match(request.SmsContent);
            string caseType = match.Groups[1].Value;

            Regex regReportCasePeople = new Regex(@"报案人(.*?)，");
            match = regReportCasePeople.Match(request.SmsContent);
            string reportCasePeople = match.Groups[1].Value;




            if (caseType == "送修")
            {
                casetype = 1;
            }
            else if (caseType == "返修")
            {
                casetype = 2;
            }
            else if (caseType == "三者车")
            {
                casetype = 3;
            }
            else if (caseType == "回修")
            {
                casetype = 2;
            }
            return new tx_clues()
            {
                CreateTime = DateTime.Now,
                sourcename = source,//"中国人寿财险",
                source = 3,
                agentid = agentId,
                casetype = casetype,
                licenseno = "",
                MoldName = moldName,
                mobile = "",
                ReportCasePeople = reportCasePeople,
                followupstate = -1,
                UpdateTime = DateTime.Now,
                
                Deleted = 0,
                city_name = "",
                accidentremark = "",
                dangerarea = "",
                HasInsureInfo = 0,
                ReportCaseNum = "",
                smsrecivedtime = smsRecivedtime,
                CarVIN = "",
                last_follow_id = 0
            };



        }
    }
}
