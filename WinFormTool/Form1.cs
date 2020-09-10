using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace WinFormTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        public string HttpGet(string Url, string contentType)
        {
            try
            {
                string retString = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = contentType;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(myResponseStream);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string HttpPost(string Url, string postDataStr, string contentType, out bool isOK)
        {
            string retString = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = contentType;
                request.Timeout = 600000;//设置超时时间
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                Stream requestStream = request.GetRequestStream();
                StreamWriter streamWriter = new StreamWriter(requestStream);
                streamWriter.Write(postDataStr);
                streamWriter.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                responseStream.Close();

                isOK = true;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(WebException))//捕获400错误
                {
                    var response = ((WebException)ex).Response;
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);
                    retString = streamReader.ReadToEnd();
                    streamReader.Close();
                    responseStream.Close();
                }
                else
                {
                    retString = ex.ToString();
                }
                isOK = false;
            }

            return retString;
        }
        private List<Engine> QueryList;
        private void Adddata(string engine)
        {
            string url = "http://www.hc39.com/desktops/checkEngine.php?key=";
            string gethtml = HttpGet(url + engine, "text/html;charset=utf-8");
            var tbody = gethtml.Split(new string[] { "</tbody>" }, StringSplitOptions.RemoveEmptyEntries);
            if (tbody.Length > 2)
            {
                string name = string.Empty, pingpai = string.Empty, xilie = string.Empty, changshang = string.Empty, qgshu = string.Empty,
                    rnzlei = string.Empty, qgpfxshi = string.Empty, pailiang = string.Empty, pfbzhun = string.Empty, zdscglv = string.Empty,
                    edzshu = string.Empty, zdmli = string.Empty, zdnju = string.Empty, zdnjzshu = string.Empty,
                spfwei = string.Empty, jqxshi = string.Empty, fdjxshi = string.Empty, qfuhe = string.Empty, fdjjzhong = string.Empty,
                    ysbi = string.Empty, yimiwai = string.Empty, gjXxc = string.Empty, mgqmshu = string.Empty, dhxsu = string.Empty;
                foreach (var kk in tbody[0].Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var splist = kk.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < splist.Length; i++)
                    {
                        if (splist[i].Contains(@"<td rowspan=""6"" style=""width:360px;""><img src="""))
                        {

                            Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
                            string imgsrc = string.Empty;
                            // 搜索匹配的字符串 
                            MatchCollection matches = regImg.Matches(splist[i]);
                            foreach (Match match in matches)
                            {
                                imgsrc = match.Groups["imgUrl"].Value;
                            }

                            DownloadPicture(imgsrc, 60000);
                            break;
                        }
                    }
                }
                foreach (var x in tbody[1].Split(new string[] { "</tr>" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var list = x.Split(new string[] { "</td>" }, StringSplitOptions.RemoveEmptyEntries);
                    if (list.Length == 1)
                    {
                        continue;
                    }
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i].Contains("发动机："))
                        {
                            name = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("系列："))
                        {
                            xilie = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("汽缸数："))
                        {
                            qgshu = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("排列形式："))
                        {
                            qgpfxshi = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("排放标准："))
                        {
                            pfbzhun = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("额定转速："))
                        {
                            edzshu = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("最大扭矩："))
                        {
                            zdnju = list[i + 1].Replace("<td>", "");
                        }

                        else if (list[i].Contains("品牌："))
                        {
                            pingpai = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("厂商："))
                        {
                            changshang = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("燃料种类："))
                        {
                            rnzlei = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("排量："))
                        {
                            pailiang = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("最大输出功率："))
                        {
                            zdscglv = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("最大马力："))
                        {
                            zdmli = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("最大扭矩转速："))
                        {
                            zdnjzshu = list[i + 1].Replace("<td>", "");
                        }
                        //-----------------------------------------
                        else if (list[i].Contains("适配范围："))
                        {
                            spfwei = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("进气形式："))
                        {
                            jqxshi = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("发动机形式："))
                        {
                            fdjxshi = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("全负荷："))
                        {
                            qfuhe = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("发动机净重："))
                        {
                            fdjjzhong = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("压缩比："))
                        {
                            ysbi = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("一米外噪音："))
                        {
                            yimiwai = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("缸径x行程："))
                        {
                            gjXxc = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("每缸气门数："))
                        {
                            mgqmshu = list[i + 1].Replace("<td>", "");
                        }
                        else if (list[i].Contains("点火次序："))
                        {
                            dhxsu = list[i + 1].Replace("<td>", "");
                        }
                    }
                }
                try
                {
                    using (SqlDbContext insertdb = new SqlDbContext())
                    {
                        string sql = "insert into [Web_ssp].[dbo].[Web_engine](EngineName,EngineSeries,CylindersNum," +
                            "CylindersArrange,EmissionStandard,Rpm,MaxTorque,EngineBrand,EngineVendor,FuelCategory,Displacement," +
                            "MaxPower,MaxHorsepower,MaxTorqueSpeed,AdaptiveRange,IntakeForm,EngineForm,FullLoad,EngineSuttle,CompressionRatio," +
                            "OneMeterOutside,CylinderXStroke,ValvesPerCylinder,FiringOrder,EnginePic) " +
                            "values('" + name + "','" + xilie + "','" + qgshu + "','" + qgpfxshi + "','" + pfbzhun + "','" + edzshu + "','" + "" + zdnju + "','"
                            + pingpai + "','" + changshang + "','" + rnzlei + "','" + pailiang + "','" + zdscglv + "','" + zdmli + "','" + zdnjzshu + "','" + spfwei + "','"
                            + jqxshi + "','" + fdjxshi + "','" + qfuhe + "','" + fdjjzhong + "','" + ysbi + "','" + yimiwai + "','" + gjXxc + "','" + mgqmshu + "','" + dhxsu + "','" + PicName + "')";
                        insertdb.Database.ExecuteSqlCommand(sql);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private string PicName = string.Empty;

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="picUrl">图片Http地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="timeOut">Request最大请求时间，如果为-1则无限制</param>
        /// <returns></returns>
        public bool DownloadPicture(string picUrl, int timeOut)
        {
            //picUrl = "http://static.hc39.com/engine/146139.jpg";
            PicName = DateTime.Now.ToString("HHmmssffff") + ".jpg";
            string savePath = "D:/img/" + PicName;
            bool value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                if (timeOut != -1) request.Timeout = timeOut;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    value = SaveBinaryFile(response, savePath);
            }
            finally
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
            return value;
        }

        private static bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                outStream = System.IO.File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                value = true;
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }

        private List<string> strList = new List<string>();
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    using (SqlDbContext db = new SqlDbContext())
                    {
                        QueryList = db.Database.SqlQuery<Engine>("SELECT id ,fadongjixinghao_d FROM [Web_ssp].[dbo].[Web_gonggao] where DATALENGTH(fadongjixinghao_d) > 1 and title not like '%摩托车%' and title not like '%电动%'").ToList();
                        //var fadongji = db.Database.SqlQuery<fadongji>("select EngineName,MaxHorsepower from ").ToList();
                        for (int i = 12165; i < QueryList.Count; i++)
                        {
                            string[] fdjxh = Regex.Split(QueryList[i].fadongjixinghao_d, "<br />", RegexOptions.IgnoreCase);
                            string updata = string.Empty;
                            if (fdjxh.Length > 1)
                            {
                                for (int q = 0; q < fdjxh.Length; q++)
                                {
                                    if (q == fdjxh.Length - 1)
                                    {
                                        updata += db.Database.SqlQuery<string>("select MaxHorsepower from Web_engine where EngineName='" + fdjxh[q] + "'").SingleOrDefault();
                                    }
                                    else
                                    {
                                        updata += db.Database.SqlQuery<string>("select MaxHorsepower from Web_engine where EngineName='" + fdjxh[q] + "'").SingleOrDefault() + "<br />";
                                    }
                                }
                            }
                            else
                            {
                                updata = db.Database.SqlQuery<string>("select MaxHorsepower from Web_engine where EngineName='" + fdjxh[0] + "'").SingleOrDefault();
                            }
                            if (string.IsNullOrEmpty(updata)) updata = "";
                            string ret = updata.Replace("马力", "");
                            db.Database.ExecuteSqlCommand("update Web_gonggao set mali_d='" + ret + "' where id='" + QueryList[i].Id + "'");
                        }
                        //for (int i = 0; i < QueryList.Count; i++)
                        //{
                        //    string[] fdjxh = Regex.Split(QueryList[i].fadongjixinghao_d, "<br />", RegexOptions.IgnoreCase);
                        //    strList.AddRange(fdjxh);
                        //}
                        //List<string> listString = new List<string>();
                        //foreach (string eachString in strList)
                        //{
                        //    if (!listString.Contains(eachString))
                        //        listString.Add(eachString);
                        //}
                        //var count = db.Database.SqlQuery<string>("SELECT EngineName FROM Web_engine").ToList();
                        //foreach (string thisString in count)
                        //{
                        //    if (listString.Contains(thisString) || listString.Contains(thisString.Trim()))
                        //        listString.Remove(thisString);
                        //}
                        //int jishu = 0;
                        //foreach (string thisString in listString)
                        //{
                        //    //Adddata(thisString);
                        //    jishu++;
                        //    this.textBox1.Text = jishu.ToString();
                        //    Thread.Sleep(3000);
                        //}
                    }
                    MessageBox.Show("添加完成");
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// POST请求接口调用
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string PostMoths(string url, string param)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            MD5 md5 = MD5.Create();
            //需要将字符串转成字节数组
            byte[] buffer = Encoding.Default.GetBytes("test1");
            //加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] md5buffer = md5.ComputeHash(buffer);
            string str = null;
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            foreach (byte b in md5buffer)
            {
                //得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                //但是在和对方测试过程中，发现我这边的MD5加密编码，经常出现少一位或几位的问题；
                //后来分析发现是 字符串格式符的问题， X 表示大写， x 表示小写， 
                //X2和x2表示不省略首位为0的十六进制数字；
                str += b.ToString("x2");
            }
            var aa = str;
            var tes = JsonConvert.SerializeObject("1");
            var result = HttpGet("https://localhost:44370/actionapi/GongGao/GetGongGaoHtml?id=4954", "text/html;charset=utf-8");
            bool bl = false;
            var userlist = "{\"num\":10001}";
            var userlists = "{\"title\":\"洒水\",\"chechang\":\"4000\",\"chechang2\":\"7000\",\"pageindex\":\"1\"}";
            var data = "{\"chanpinmingcheng_d\":\"载货\",\"pageindex\":\"1\"}";
            var thilist = "{\"UserName\":\"api\",\"UserPassword\":\"111111\"}";
            JavaScriptSerializer json = new JavaScriptSerializer();
            string jsonData = json.Serialize(thilist);//序列化
            //var result2 = HttpPost("https://localhost:44370/actionapi/Contact/PostContactByIDSex", jsonData, "application/x-www-form-urlencoded", out bl);
            //var result = PostMoths("https://localhost:44370/actionapi/GongGao/ConditionQuery", userlists);
            //var resultpage = PostMoths("https://localhost:44370/actionapi/GongGao/PageTjQuery", userlists);
            var resultpage = PostMoths("https://localhost:44370/actionapi/GongGao/UpQueryNum", "10001");
            try
            {
                var vvv = JsonConvert.DeserializeObject<string>(resultpage);
                JObject jo = JObject.Parse(vvv);
                JArray array = (JArray)jo["Table"];
                foreach (var jObject in array)
                {
                    //赋值属性        
                    textBox1.Text += jObject["UserName"].ToString();//获取字符串中id值      
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //string name = rt;
            //Stopwatch sw1 = new Stopwatch();
            //using (Model1 db = new Model1())
            //{
            //    var info = db.VipUsers.Where(x => x.Id > 0);
            //    var infos = db.VipUsers_Set.Where(x => x.Id == 1).SingleOrDefault();
            //    textBox1.Text = info.Count().ToString();
            //}
            //Model2 db2 = new Model2();
            //sw1.Start();
            //var ggao = db2.Web_gonggao.Where(x => x.title.Contains("洒水") && x.luntaishu=="6").ToList();
            //sw1.Stop();
            //textBox2.Text = sw1.ElapsedMilliseconds.ToString();
            //textBox3.Text = ggao.Count.ToString();
        }


        private string strFilePath = Application.StartupPath + "\\FileConfig.ini";//获取INI文件路径
        private string strSec = ""; //INI文件名

        /// <summary>
        /// 自定义读取INI文件中的内容方法
        /// </summary>
        /// <param name="Section">键</param>
        /// <param name="key">值</param>
        /// <returns></returns>
        private string ContentValue(string Section, string key)
        {
            StringBuilder temp = new StringBuilder(1024);
            SaveRecord.GetPrivateProfileString(Section, key, "", temp, 1024, strFilePath);
            return temp.ToString();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                //根据INI文件名设置要写入INI文件的节点名称
                //此处的节点名称完全可以根据实际需要进行配置
                strSec = Path.GetFileNameWithoutExtension(strFilePath);
                SaveRecord.WritePrivateProfileString(strSec, "Name", "123", strFilePath);
                SaveRecord.WritePrivateProfileString(strSec, "Sex", "五十字五十字", strFilePath);
                MessageBox.Show("写入成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (File.Exists(strFilePath))//读取时先要判读INI文件是否存在
            {
                strSec = Path.GetFileNameWithoutExtension(strFilePath);
                textBox2.Text = ContentValue(strSec, "Name");
                textBox3.Text = ContentValue(strSec, "Sex");
            }
            else
            {
                MessageBox.Show("INI文件不存在");
            }
        }
    }
}
