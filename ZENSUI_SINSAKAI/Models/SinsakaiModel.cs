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
                    cmd = new MySqlCommand($"SELECT ROW_NUMBER() OVER (ORDER BY A.SYUPPIN_NO) AS ROW_NO, A.SYUPPIN_NO, A.TITLE1, B.SCORE_KANNOU_TOKUSEI, B.SCORE_TYAKUSOU, B.SCORE_GENRYOU, B.SCORE_GIZYUTU, B.SCORE_HOUSOU FROM SYUPPIN A INNER JOIN SAITEN B ON ( B.SINSAKAI_ID = A.SINSAKAI_ID AND B.SYUPPIN_NO = A.SYUPPIN_NO AND B.SAITENSYA_ID = {userId} ) WHERE A.SINSAKAI_ID = {sinsakaiId} AND A.SAKUZYO_FLAG = 0 ORDER BY A.SYUPPIN_NO", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Syuppin syuppin = new Syuppin();
                            syuppin.RowNo = Convert.ToInt32(reader["ROW_NO"]);
                            syuppin.SyuppinNo = Convert.ToInt32(reader["SYUPPIN_NO"]);
                            syuppin.Title1 = reader["TITLE1"].ToString();
                            sinsakai.SyuppinList.Add(syuppin);

                            int? scoreKannouTokusei = reader["SCORE_KANNOU_TOKUSEI"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_KANNOU_TOKUSEI"]);
                            int? scoreTyakusou = reader["SCORE_TYAKUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_TYAKUSOU"]);
                            int? scoreGenryou = reader["SCORE_GENRYOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GENRYOU"]);
                            int? scoreGizyutu = reader["SCORE_GIZYUTU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GIZYUTU"]);
                            int? scoreHousou = reader["SCORE_HOUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_HOUSOU"]);
                            int? scoreTotal = GetScoreTotal(scoreKannouTokusei, scoreTyakusou, scoreGenryou, scoreGizyutu, scoreHousou);
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
                    var cmd = new MySqlCommand($"SELECT SYUPPIN_NO, TITLE1, TITLE2, IMAGE1, IMAGE2, IMAGE3, SEIHIN_KUBUN, SYUPPIN_KUBUN, ZYUSYOUREKI, HANBAI_KAISIZIKI, HANBAI_KUBUN, HANBAI_TAISYOU, SIYOU_GENZAIRYOU, ALLERGY_HYOUZI, SYUGENRYOU, GENSANTI, HYOUZYUN_KOURI_KAKAKU, IKKOATARI_ZYUURYOU, HOZON_ZYOUKEN, SYOUMI_KIGEN, TABEKATA, SYOUHIN_TOKUTYOU, KANNOU_TOKUSEI, TYAKUSOU, GENRYOU, SEISAN_GIZYUTU, SONOTA FROM SYUPPIN WHERE SINSAKAI_ID = {sinsakaiId} AND SYUPPIN_NO = {syuppinNo} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        syuppin.RowNo = rowNo;
                        syuppin.SyuppinNo = Convert.ToInt32(reader["SYUPPIN_NO"]);
                        syuppin.Title1 = reader["TITLE1"].ToString();
                        syuppin.Title2 = reader["TITLE2"].ToString();
                        syuppin.Image1 = "data:image/jpeg;base64," + (reader["IMAGE1"] == DBNull.Value ? "" : Convert.ToBase64String((byte[])reader["IMAGE1"]));
                        syuppin.Image2 = "data:image/jpeg;base64," + (reader["IMAGE2"] == DBNull.Value ? "" : Convert.ToBase64String((byte[])reader["IMAGE2"]));
                        syuppin.Image3 = "data:image/jpeg;base64," + (reader["IMAGE3"] == DBNull.Value ? "" : Convert.ToBase64String((byte[])reader["IMAGE3"]));
                        syuppin.SeihinKubun = reader["SEIHIN_KUBUN"].ToString();
                        syuppin.SyuppinKubun = reader["SYUPPIN_KUBUN"].ToString();
                        syuppin.Zyusyoureki = reader["ZYUSYOUREKI"].ToString();
                        syuppin.HanbaiKaisiziki = reader["HANBAI_KAISIZIKI"].ToString();
                        syuppin.HanbaiKubun = reader["HANBAI_KUBUN"].ToString();
                        syuppin.HanbaiTaisyou = reader["HANBAI_TAISYOU"].ToString();
                        syuppin.SiyouGenzairyou = reader["SIYOU_GENZAIRYOU"].ToString();
                        syuppin.AllergyHyouzi = reader["ALLERGY_HYOUZI"].ToString();
                        syuppin.Syugenryou = reader["SYUGENRYOU"].ToString();
                        syuppin.Gensanti = reader["GENSANTI"].ToString();
                        syuppin.HyouzyunKouriKakaku = reader["HYOUZYUN_KOURI_KAKAKU"].ToString();
                        syuppin.IkkoatariZyuuryou = reader["IKKOATARI_ZYUURYOU"].ToString();
                        syuppin.HozonZyouken = reader["HOZON_ZYOUKEN"].ToString();
                        syuppin.SyoumiKigen = reader["SYOUMI_KIGEN"].ToString();
                        syuppin.Tabekata = reader["TABEKATA"].ToString();
                        syuppin.SyouhinTokutyou = reader["SYOUHIN_TOKUTYOU"].ToString();
                        syuppin.KannouTokusei = reader["KANNOU_TOKUSEI"].ToString();
                        syuppin.Tyakusou = reader["TYAKUSOU"].ToString();
                        syuppin.Genryou = reader["GENRYOU"].ToString();
                        syuppin.SeisanGizyutu = reader["SEISAN_GIZYUTU"].ToString();
                        syuppin.Sonota = reader["SONOTA"].ToString();
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
                    var cmd = new MySqlCommand($"SELECT SCORE_KANNOU_TOKUSEI, SCORE_TYAKUSOU, SCORE_GENRYOU, SCORE_GIZYUTU, SCORE_HOUSOU FROM SAITEN WHERE SINSAKAI_ID = {sinsakaiId} AND SYUPPIN_NO = {syuppinNo} AND SAITENSYA_ID = {saitensyaId} AND SAKUZYO_FLAG = 0", con);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        score.ScoreKannouTokusei = reader["SCORE_KANNOU_TOKUSEI"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_KANNOU_TOKUSEI"]);
                        score.ScoreTyakusou = reader["SCORE_TYAKUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_TYAKUSOU"]);
                        score.ScoreGenryou = reader["SCORE_GENRYOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GENRYOU"]);
                        score.ScoreGizyutu = reader["SCORE_GIZYUTU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_GIZYUTU"]);
                        score.ScoreHousou = reader["SCORE_HOUSOU"] == DBNull.Value ? null : Convert.ToInt32(reader["SCORE_HOUSOU"]);
                        score.ScoreTotal = GetScoreTotal(score.ScoreKannouTokusei, score.ScoreTyakusou, score.ScoreGenryou, score.ScoreGizyutu, score.ScoreHousou);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
            }

            return score;
        }

        public bool RegistScore(int sinsakaiId, int syuppinNo, int saitensyaId, int? scoreKannouTokusei, int? scoreTyakusou, int? scoreGenryou, int? scoreGizyutu, int? scoreHousou)
        {
            try
            {
                var sql = "UPDATE SAITEN SET SCORE_KANNOU_TOKUSEI = @scoreKannouTokusei, SCORE_TYAKUSOU = @scoreTyakusou, SCORE_GENRYOU = @scoreGenryou, SCORE_GIZYUTU = @scoreGizyutu, SCORE_HOUSOU = @scoreHousou WHERE SINSAKAI_ID = @sinsakaiId AND SYUPPIN_NO = @syuppinNo AND SAITENSYA_ID = @saitensyaId";
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
                            cmd.Parameters.AddWithValue("@scoreKannouTokusei", scoreKannouTokusei);
                            cmd.Parameters.AddWithValue("@scoreTyakusou", scoreTyakusou);
                            cmd.Parameters.AddWithValue("@scoreGenryou", scoreGenryou);
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

        private int? GetScoreTotal(int? scoreKannouTokusei, int? scoreTyakusou, int? scoreGenryou, int? scoreGizyutu, int? scoreHousou)
        {
            if (scoreKannouTokusei != null || scoreTyakusou != null || scoreGenryou != null || scoreGizyutu != null || scoreHousou != null)
            {
                return (scoreKannouTokusei ?? 0) + (scoreTyakusou ?? 0) + (scoreGenryou ?? 0) + (scoreGizyutu ?? 0) + (scoreHousou ?? 0);
            }
            else
            {
                return null;
            }
        }
    }
}
