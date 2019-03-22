
namespace BiHuManBu.ExternalInterfaces.Models.IRepository
{
    public  interface ICarRenewalRepository
    {
        bx_car_renewal FindByLicenseno(string licenseno);

        bx_car_renewal FindCarRenewal(long buid);
    }
}
