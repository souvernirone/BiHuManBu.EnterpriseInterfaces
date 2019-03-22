using System;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Messages.RemoteMessage;
using ServiceStack.Text;

namespace BiHuManBu.ExternalInterfaces.Services.Mapper.AppMapper
{
    public static  class VehicleInfoMapper
    {
        public static string VehicleInfoMethod(bx_quoteresult_carinfo result)
        {
           // string info = "{\"ExhaustScale\":\"1.984\",\"PurchasePrice\":221300.0,\"AutoModelCode\":\"DZAAWD0058\",\"SeatCount\":\"5\",\"VehicleAlias\":null,\"VehicleYear\":null}";
            if (result == null)
            {
                return string.Empty;
            }
            else
            {
                var model =string.IsNullOrWhiteSpace(result.Cardepict)?new VehicleInfo(): result.Cardepict.FromJson<VehicleInfo>();
                return ConvertVehicleInfo(model);
            }
        }

        private static string ConvertVehicleInfo(VehicleInfo info)
        {
            if (string.IsNullOrWhiteSpace(info.VehicieName) && string.IsNullOrWhiteSpace(info.VehicleAlias) &&
                string.IsNullOrWhiteSpace(info.ExhaustScale) && string.IsNullOrWhiteSpace(info.SeatCount) &&
                string.IsNullOrWhiteSpace(info.VehicleYear) && string.IsNullOrWhiteSpace(info.PurchasePrice))
                return string.Empty;
            return string.Format("{0}/{1}/{2}/{3}/{4}/{5}", info.VehicieName, info.VehicleAlias, info.ExhaustScale, info.SeatCount, info.PurchasePrice, string.IsNullOrWhiteSpace(info.VehicleYear) ? string.Empty : (info.VehicleYear.Length < 4 ? string.Empty : info.VehicleYear.Substring(0, 4)));
        }
    }
}
