using System.Data;

namespace OzymandiasInvestments.Models.SolutionModels
{
    public class PositionModel
    {
        public AccountInfoModel info { get; set; }
        public DataTable positions { get; set; }
        public List<EquityModel> equityModelList { get; set; }
    }
}
