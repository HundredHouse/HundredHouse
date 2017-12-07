using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace HundredHouse
{
	/// <summary>
	/// MainWindow.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class MainWindow : Window
	{
		private string URL = "http://chart.r114.com/fusionchart/data/r114/syse/r114_Syse_NextTown.asp?shin=&addr1=%B0%E6%B1%E2%B5%B5&addr2=%BF%EB%C0%CE%BD%C3&addr3=&aptcode=&c1=eb6d70&c2=81c0cf&l1=da5454&l2=4090c3&bm=&mb=&relief_price_flag=&type_m=&orderby=&chartprint=";
		WebRequest req;
		List<List<string>> adds = new List<List<string>>();

		public MainWindow()
		{
			InitializeComponent();

			// test();

			this.req = WebRequest.Create(URL);

		}

		public void test()
		{
			DateTime localDate = DateTime.Now;
			DateTime utcDate = DateTime.UtcNow;
			String[] cultureNames = { "en-US", "en-GB", "fr-FR",
								"de-DE", "ru-RU" };
			string toDate = localDate.ToString("yyyyMMdd");					
			Console.WriteLine(new String(toDate.ToCharArray().Reverse().ToArray()));

		}

		public void RequestGet()
		{
			// WebProxy myProxy = new WebProxy("")

			WebProxy myProxy = new WebProxy("myproxy", 80);
			myProxy.BypassProxyOnLocal = true;
			

			// req.Proxy = WebProxy.GetDefaultProxy();

			Stream objStream;
			objStream = req.GetResponse().GetResponseStream();

			StreamReader responseReader = new StreamReader(objStream, Encoding.GetEncoding("euc-kr"));
			string sLine = "";
			string strEmpty = "";
			int i = 0;
			while ( strEmpty != null )
			{
				i++;
				strEmpty = responseReader.ReadLine();
				sLine += strEmpty;
			}

			// Console.WriteLine(sLine);

			XmlDocument document = new XmlDocument();
			document.LoadXml(sLine);

			XmlNodeList xnList = document.GetElementsByTagName("dataset"); //접근할 노드

			List<ListBoxItem> list = new List<ListBoxItem>();

			grid.Items.Clear();

			foreach ( XmlNode xn in xnList )
			{
				if ( xn.HasChildNodes )
				{
					for ( int a = 0; a < xn.ChildNodes.Count; a++ )
					{
						//Console.WriteLine(xn.ChildNodes[a].Attributes[0].Value);
						//Console.WriteLine(xn.ChildNodes[a].Attributes[1].Value);

						string name = xn.ChildNodes[a].Attributes[0].Value;		// 금액
						string value = xn.ChildNodes[a].Attributes[1].Value;	// 명칭 + 금액						

						grid.Items.Add(value);
					}
				}			
			}

		}		

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			RequestGet();
		}
	}
}
