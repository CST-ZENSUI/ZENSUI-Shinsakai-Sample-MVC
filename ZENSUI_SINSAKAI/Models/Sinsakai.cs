namespace ZENSUI_SINSAKAI.Models
{
    public class Sinsakai
    {
        public int SinsakaiId { get; set; }
        public string? SinsakaiName { get; set; }
        public int SaitensyaId { get; set; }
        public string? SaitensyaName { get; set; }
        public List<Syuppin> SyuppinList { get; set; } = new List<Syuppin>();
        public List<int?> ScoreTotalList { get; set; } = new List<int?>();
    }
}
