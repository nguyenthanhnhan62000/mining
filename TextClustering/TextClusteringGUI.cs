using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using TextClustering.Lib;


namespace TextClustering
{
    public partial class TextClusteringGUI : Form
    {
        private DocumentCollection docCollection;
        HttpClient client = new HttpClient();
           

        public TextClusteringGUI()
        {
            InitializeComponent();
            docCollection = new DocumentCollection() { DocumentList = new List<string>() };

        }

      

        private async void btnAdd_Click(object sender, EventArgs e)
        {
                
  /*        httpMessageRequest.Headers.Add("Accept", "application/json");*/
            var httpMessageRequest = new HttpRequestMessage();
            httpMessageRequest.Method = HttpMethod.Post;    
            httpMessageRequest.RequestUri = new Uri("http://127.0.0.1:8000/data_mining/post_test");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("url", "https://thuvienphapluat.vn/van-ban/Bat-dong-san/Luat-dat-dai-2013-215836.aspx"));
            var content = new FormUrlEncodedContent(parameters);
            httpMessageRequest.Content = content;

            var httpMessageMessage = await client.SendAsync(httpMessageRequest);

            var html = await httpMessageMessage.Content.ReadAsStringAsync();


        }

        private async void btnStartClustering_Click(object sender, EventArgs e)
        {

            //begin client
            var httpMessageRequest = new HttpRequestMessage();
            httpMessageRequest.Method = HttpMethod.Post;
            httpMessageRequest.RequestUri = new Uri("http://127.0.0.1:8000/data_mining/post_test");

            var parameters = new List<KeyValuePair<string, string>>();
            parameters.Add(new KeyValuePair<string, string>("url", "https://thuvienphapluat.vn/van-ban/Bat-dong-san/Luat-Dat-dai-2003-13-2003-QH11-51685.aspx"));
            var content = new FormUrlEncodedContent(parameters);
            httpMessageRequest.Content = content;

            var httpMessageMessage = await client.SendAsync(httpMessageRequest);

            var res = await httpMessageMessage.Content.ReadAsStringAsync();
            //end client

          /*  string res = await client.GetStringAsync("http://127.0.0.1:8000/data_mining/data");*/
            
            var list = JsonConvert.DeserializeObject<List<Data>>(res);

            docCollection.DocumentList.Clear();
           
            for(int i = 0; i < 20; i++)
            {
                docCollection.DocumentList.Add(list[i].nd);
            }
            /*  docCollection.DocumentList.Clear();*/


            List<DocumentVector> vSpace = VectorSpaceModel.ProcessDocumentCollection(docCollection);
            int totalIteration = 0;
            List<Centroid> resultSet = DocumnetClustering.PrepareDocumentCluster(int.Parse(txtClusterNo.Text), vSpace, ref totalIteration);
            string msg = string.Empty;
            int count = 1;
            foreach (Centroid c in resultSet)
            {
                msg += String.Format("------------------------------[ CLUSTER {0} ]-----------------------------{1}", count, System.Environment.NewLine);
                foreach (DocumentVector document in c.GroupedDocument)
                {
                    msg += document.Content + System.Environment.NewLine;
                    if (c.GroupedDocument.Count > 1)
                    {
                        msg += String.Format("{0}-------------------------------------------------------------------------------{0}", System.Environment.NewLine);
                    }
                }
                msg += "-------------------------------------------------------------------------------" + System.Environment.NewLine;
                count++;
            }

            richTextBox1.Text = msg;
            lblTotalIteration.Text = totalIteration.ToString();
        }

        private void btnStopProcess_Click(object sender, EventArgs e)
        {
            docCollection = new DocumentCollection();
            this.ClearField();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            this.ClearField();
            docCollection = new DocumentCollection();
        }


        private void ClearField()
        {
            txtClusterNo.Clear();
            txtDoc1.Clear();
            txtDoc2.Clear();
            txtDoc3.Clear();
            txtDoc4.Clear();
            lblTotalDoc.Text="";
            lblError.Text = "";
            lblTotalCluster.Text = "";
            lblTotalIteration.Text = "";
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.ClearField();
        }

        private void txtClusterNo_TextChanged(object sender, EventArgs e)
        {
            int totalDoc = 0;
            if(int.TryParse(txtClusterNo.Text, out totalDoc))
            {
                lblError.Text = "";
                lblTotalCluster.Text = txtClusterNo.Text;
            }
            else
            {
                lblError.Text = "Enter a valid integer";
                lblTotalCluster.Text = " ";
            }

            
        }

    }
}
