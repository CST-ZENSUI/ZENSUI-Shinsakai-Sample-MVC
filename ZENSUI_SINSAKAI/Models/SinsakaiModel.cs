using MySql.Data.MySqlClient;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace ZENSUI_SINSAKAI.Models
{
    public class SinsakaiModel
    {
        IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

        public Sinsakai GetSinsakai(int sinsakaiId, int userId)
        {
            Sinsakai sinsakai = new Sinsakai();

            try
            {
                using (MySqlConnection con = new MySqlConnection(config["ConnectionStrings:DefaultConnection"]))
                {
                    con.Open();
                    var cmd = new MySqlCommand($"SELECT SINSAKAI_ID, SINSAKAI_NAME FROM SINSAKAI WHERE SINSAKAI_ID = {sinsakaiId} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        sinsakai.SinsakaiId = Convert.ToInt32(reader["SINSAKAI_ID"]);
                        sinsakai.SinsakaiName = reader["SINSAKAI_NAME"].ToString();
                    }
                    con.Close();

                    con.Open();
                    cmd = new MySqlCommand($"SELECT USER_ID, USER_NAME FROM USER WHERE USER_ID = {userId} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        sinsakai.SaitensyaId = Convert.ToInt32(reader["USER_ID"]);
                        sinsakai.SaitensyaName = reader["USER_NAME"].ToString();
                    }
                    con.Close();

                    con.Open();
                    cmd = new MySqlCommand($"SELECT ROW_NUMBER() OVER (ORDER BY A.SYUPPIN_NO) AS ROW_NO, A.SYUPPIN_NO, A.TITLE, B.SCORE_GENRYOU, B.SCORE_KANNOU_TOKUSEI, B.SCORE_TYAKUSOU, B.SCORE_GIZYUTU, B.SCORE_HOUSOU FROM SYUPPIN A INNER JOIN SAITEN B ON ( B.SINSAKAI_ID = A.SINSAKAI_ID AND B.SYUPPIN_NO = A.SYUPPIN_NO AND B.SAITENSYA_ID = {userId} ) WHERE A.SINSAKAI_ID = {sinsakaiId} AND A.SAKUZYO_FLAG = 0 ORDER BY A.SYUPPIN_NO", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Syuppin syuppin = new Syuppin();
                            syuppin.RowNo = Convert.ToInt32(reader["ROW_NO"]);
                            syuppin.SyuppinNo = Convert.ToInt32(reader["SYUPPIN_NO"]);
                            syuppin.Title = reader["TITLE"].ToString();
                            sinsakai.SyuppinList.Add(syuppin);

                            int? scoreGenryou = reader["SCORE_GENRYOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GENRYOU"]);
                            int? scoreKannouTokusei = reader["SCORE_KANNOU_TOKUSEI"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_KANNOU_TOKUSEI"]);
                            int? scoreTyakusou = reader["SCORE_TYAKUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_TYAKUSOU"]);
                            int? scoreGizyutu = reader["SCORE_GIZYUTU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GIZYUTU"]);
                            int? scoreHousou = reader["SCORE_HOUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_HOUSOU"]);
                            int? scoreTotal = GetScoreTotal(scoreGenryou, scoreKannouTokusei, scoreTyakusou, scoreGizyutu, scoreHousou);
                            sinsakai.ScoreTotalList.Add(scoreTotal);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return sinsakai;
        }

        public Syuppin GetSyuppin(int rowNo, int sinsakaiId, int syuppinNo)
        {
            Syuppin syuppin = new Syuppin();

            try
            {
                using (MySqlConnection con = new MySqlConnection(config["ConnectionStrings:DefaultConnection"]))
                {
                    con.Open();
                    var cmd = new MySqlCommand($"SELECT SYUPPIN_NO, TITLE, IMAGE1, IMAGE2, SEIHIN_KUBUN, SYUPPIN_KUBUN, IKKOATARI_ZYUURYOU, HYOUZYUN_KOURI_KAKAKU, SIYOU_GENZAIRYOU, GENSANKOKU_KUBUN, ALLERGY_TOKUTEI_HATIHINMOKU, SYOUMI_KIGEN, HOZON_ZYOUKEN, GENRYOU, OISISA, SEISAN_GIZYUTU, ANZENSEI, TABEKATA, TIIKI_KOUKEN, NYUUSUU, KOURIYOU_GYOUMUYOU, ZYUSYOUREKI, SEIZOU_HANBAI_KAISIZIKI FROM SYUPPIN WHERE SINSAKAI_ID = {sinsakaiId} AND SYUPPIN_NO = {syuppinNo} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        syuppin.RowNo = rowNo;
                        syuppin.SyuppinNo = Convert.ToInt32(reader["SYUPPIN_NO"]);
                        syuppin.Title = reader["TITLE"].ToString();
                        syuppin.Image1 = "data:image/jpeg;base64," + (reader["IMAGE1"] == DBNull.Value ? "" : Convert.ToBase64String((byte[])reader["IMAGE1"]));
                        syuppin.Image2 = "data:image/jpeg;base64," + (reader["IMAGE2"] == DBNull.Value ? "" : Convert.ToBase64String((byte[])reader["IMAGE2"]));
                        syuppin.SeihinKubun = reader["SEIHIN_KUBUN"].ToString();
                        syuppin.SyuppinKubun = reader["SYUPPIN_KUBUN"].ToString();
                        syuppin.IkkoatariZyuuryou = reader["IKKOATARI_ZYUURYOU"].ToString();
                        syuppin.HyouzyunKouriKakaku = reader["HYOUZYUN_KOURI_KAKAKU"].ToString();
                        syuppin.SiyouGenzairyou = reader["SIYOU_GENZAIRYOU"].ToString();
                        syuppin.GensankokuKubun = reader["GENSANKOKU_KUBUN"].ToString();
                        syuppin.AllergyTokuteiHatihinmoku = reader["ALLERGY_TOKUTEI_HATIHINMOKU"].ToString();
                        syuppin.SyoumiKigen = reader["SYOUMI_KIGEN"].ToString();
                        syuppin.HozonZyouken = reader["HOZON_ZYOUKEN"].ToString();
                        syuppin.Genryou = reader["GENRYOU"].ToString();
                        syuppin.Oisisa = reader["OISISA"].ToString();
                        syuppin.SeisanGizyutu = reader["SEISAN_GIZYUTU"].ToString();
                        syuppin.Anzensei = reader["ANZENSEI"].ToString();
                        syuppin.Tabekata = reader["TABEKATA"].ToString();
                        syuppin.TiikiKouken = reader["TIIKI_KOUKEN"].ToString();
                        syuppin.Nyuusuu = reader["NYUUSUU"].ToString();
                        syuppin.KouriyouGyoumuyou = reader["KOURIYOU_GYOUMUYOU"].ToString();
                        syuppin.Zyusyoureki = reader["ZYUSYOUREKI"].ToString();
                        syuppin.SeizouHanbaiKaisiziki = reader["SEIZOU_HANBAI_KAISIZIKI"].ToString();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return syuppin;
        }

        public string? ReadImage(int sinsakaiId, int syuppinNo, int saitensyaId)
        {
            string? image = "";

            try
            {
                using (MySqlConnection con = new MySqlConnection(config["ConnectionStrings:DefaultConnection"]))
                {
                    con.Open();
                    var cmd = new MySqlCommand($"SELECT MEMO FROM SAITEN WHERE SINSAKAI_ID = {sinsakaiId} AND SYUPPIN_NO = {syuppinNo} AND SAITENSYA_ID = {saitensyaId} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        image = "data:image/png;base64," + (reader["MEMO"] == DBNull.Value ? "" : Convert.ToBase64String((byte[])reader["MEMO"]));
                    }
                    con.Close();
                }

                return image;
            }
            catch (Exception ex)
            {
                return image;
            }
        }

        public bool UploadImage(int sinsakaiId, int syuppinNo, int saitensyaId, string data)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(data.Replace("data:image/png;base64,", ""));

                var sql = "UPDATE SAITEN SET MEMO = @bytes WHERE SINSAKAI_ID = @sinsakaiId AND SYUPPIN_NO = @syuppinNo AND SAITENSYA_ID = @saitensyaId";
                using (MySqlConnection con = new MySqlConnection(config["ConnectionStrings:DefaultConnection"]))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        MySqlTransaction tran = null!;

                        try
                        {
                            con.Open();
                            cmd.Connection = con;

                            tran = con.BeginTransaction();

                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@bytes", bytes);
                            cmd.Parameters.AddWithValue("@sinsakaiId", sinsakaiId);
                            cmd.Parameters.AddWithValue("@syuppinNo", syuppinNo);
                            cmd.Parameters.AddWithValue("@saitensyaId", saitensyaId);

                            cmd.ExecuteNonQuery();

                            tran.Commit();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            if (tran != null)
                            {
                                tran.Rollback();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Score GetScore(int sinsakaiId, int syuppinNo, int saitensyaId)
        {
            Score score = new Score();

            try
            {
                using (MySqlConnection con = new MySqlConnection(config["ConnectionStrings:DefaultConnection"]))
                {
                    con.Open();
                    var cmd = new MySqlCommand($"SELECT SCORE_GENRYOU, SCORE_KANNOU_TOKUSEI, SCORE_TYAKUSOU, SCORE_GIZYUTU, SCORE_HOUSOU FROM SAITEN WHERE SINSAKAI_ID = {sinsakaiId} AND SYUPPIN_NO = {syuppinNo} AND SAITENSYA_ID = {saitensyaId} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        score.ScoreGenryou = reader["SCORE_GENRYOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GENRYOU"]);
                        score.ScoreKannouTokusei = reader["SCORE_KANNOU_TOKUSEI"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_KANNOU_TOKUSEI"]);
                        score.ScoreTyakusou = reader["SCORE_TYAKUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_TYAKUSOU"]);
                        score.ScoreGizyutu = reader["SCORE_GIZYUTU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GIZYUTU"]);
                        score.ScoreHousou = reader["SCORE_HOUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_HOUSOU"]);
                        score.ScoreTotal = GetScoreTotal(score.ScoreGenryou, score.ScoreKannouTokusei, score.ScoreTyakusou, score.ScoreGizyutu, score.ScoreHousou);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return score;
        }

        public bool RegistScore(int sinsakaiId, int syuppinNo, int saitensyaId, int? scoreGenryou, int? scoreKannouTokusei, int? scoreTyakusou, int? scoreGizyutu, int? scoreHousou)
        {
            try
            {
                var sql = "UPDATE SAITEN SET SCORE_GENRYOU = @scoreGenryou, SCORE_KANNOU_TOKUSEI = @scoreKannouTokusei, SCORE_TYAKUSOU = @scoreTyakusou, SCORE_GIZYUTU = @scoreGizyutu, SCORE_HOUSOU = @scoreHousou WHERE SINSAKAI_ID = @sinsakaiId AND SYUPPIN_NO = @syuppinNo AND SAITENSYA_ID = @saitensyaId";
                using (MySqlConnection con = new MySqlConnection(config["ConnectionStrings:DefaultConnection"]))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        MySqlTransaction tran = null!;

                        try
                        {
                            con.Open();
                            cmd.Connection = con;

                            tran = con.BeginTransaction();

                            cmd.CommandText = sql;
                            cmd.Parameters.AddWithValue("@scoreGenryou", scoreGenryou);
                            cmd.Parameters.AddWithValue("@scoreKannouTokusei", scoreKannouTokusei);
                            cmd.Parameters.AddWithValue("@scoreTyakusou", scoreTyakusou);
                            cmd.Parameters.AddWithValue("@scoreGizyutu", scoreGizyutu);
                            cmd.Parameters.AddWithValue("@scoreHousou", scoreHousou);
                            cmd.Parameters.AddWithValue("@sinsakaiId", sinsakaiId);
                            cmd.Parameters.AddWithValue("@syuppinNo", syuppinNo);
                            cmd.Parameters.AddWithValue("@saitensyaId", saitensyaId);

                            cmd.ExecuteNonQuery();

                            tran.Commit();
                            con.Close();
                        }
                        catch (Exception ex)
                        {
                            if (tran != null)
                            {
                                tran.Rollback();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private int? GetScoreTotal(int? scoreGenryou, int? scoreKannouTokusei, int? scoreTyakusou, int? scoreGizyutu, int? scoreHousou)
        {
            if (scoreGenryou != null || scoreKannouTokusei != null || scoreTyakusou != null || scoreGizyutu != null || scoreHousou != null)
            {
                return (scoreGenryou ?? 0) + (scoreKannouTokusei ?? 0) + (scoreTyakusou ?? 0) + (scoreGizyutu ?? 0) + (scoreHousou ?? 0);
            }
            else
            {
                return null;
            }
        }
    }
}
