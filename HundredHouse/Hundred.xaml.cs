using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;



namespace HundredHouse
{
	/// <summary>
	/// Window1.xaml에 대한 상호 작용 논리
	/// </summary>
	public partial class Window1 : Window
	{
		/// <summary>
		/// 주소 
		/// addr1	서울특별시
		/// addr2
		/// addr3
		/// 
		/// addr1	경기도
		/// 
		/// URL Encode사용하면 됨.
		/// 
		/// </summary>
		private string URL = "http://chart.r114.com/fusionchart/data/r114/syse/r114_Syse_NextTown.asp?shin=&addr1=%B0%E6%B1%E2%B5%B5&addr2=%BF%EB%C0%CE%BD%C3&addr3=&aptcode=&c1=eb6d70&c2=81c0cf&l1=da5454&l2=4090c3&bm=&mb=&relief_price_flag=&type_m=&orderby=&chartprint=";
		WebRequest req;
		List<Node> mList;

		public Window1()
		{
			InitializeComponent();
			
			this.DataContext = this;

		}

		public void RequestGet()
		{
			// WebProxy myProxy = new WebProxy("")
			this.req = WebRequest.Create(URL);
			WebProxy myProxy = new WebProxy("myproxy", 80);
			myProxy.BypassProxyOnLocal = true;			

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

			this.AddCategory(document);

			XmlNodeList xnList = document.GetElementsByTagName("dataset"); //접근할 노드			

			grid.Items.Clear();


			if( xnList.Count == 2 )
			{
				XmlNode xn = xnList[0];	// 매매가

				if ( xn.HasChildNodes )
				{
					if ( mList != null && mList.Count == xn.ChildNodes.Count )
					{
						for ( int a = 0; a < xn.ChildNodes.Count; a++ )
						{
							string amt = xn.ChildNodes[a].Attributes[0].Value;     // 금액
							mList[a].Amt = double.Parse(amt);
						}
					}
					else
					{
						throw new Exception("갯수가 다릅니다.");
					}
				}
			}
			

			mList.Sort(delegate (Node a, Node b) {
				if ( a.Amt > b.Amt ) return -1;
				else if ( a.Amt <= b.Amt ) return 1;
				return 0;
			});
			grid.ItemsSource = mList;
		}

		/// <summary>
		/// Category Data를 가져와서 추가합니다.
		/// </summary>
		/// <param name="doc"></param>
		private void AddCategory(XmlDocument doc)
		{
			XmlNodeList xnList = doc.GetElementsByTagName("category"); //접근할 노드

			mList = new List<Node>();

			foreach ( XmlNode xn in xnList )
			{
				string value = xn.Attributes[1].Value;   // 분류명칭
				mList.Add(new Node()
				{
					ReasonName = value
				});
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			RequestGet();
		}

		private void ExcelDownload(object sender, RoutedEventArgs e)
		{
			if( this.mList == null ||  this.mList.Count <= 0 )
			{
				var result = MessageBox.Show("조회된 데이터가 없습니다.");
				return;
			}

			Excel.Application excelApp = null;
			Excel.Workbook wb = null;
			Excel.Worksheet ws = null;

			try
			{
				// Excel 첫번째 워크시트 가져오기                
				excelApp = new Excel.Application();
				wb = excelApp.Workbooks.Add();
				excelApp.DisplayAlerts = true;
				ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;

				ws.Cells[1, 1] = "지역명";
				ws.Cells[1, 2] = "매매평단가";

				// 데이타 넣기
				int r = 2;
				foreach ( var d in mList )
				{
					ws.Cells[r, 1] = d.ReasonName;
					ws.Cells[r, 2] = d.TextAmtPerField;
					r++;
				}

				// 엑셀파일 저장
				wb.SaveAs(@"C:\100House.xls", 
					Excel.XlFileFormat.xlWorkbookNormal, 
					Type.Missing, 
					Type.Missing,
					Type.Missing,
					Type.Missing,
					Excel.XlSaveAsAccessMode.xlExclusive,
					Type.Missing,					
					Type.Missing,
					Type.Missing
					);
				wb.Close(true);
				excelApp.Quit();
			}
			finally
			{
				// Clean up
				ReleaseExcelObject(ws);
				ReleaseExcelObject(wb);
				ReleaseExcelObject(excelApp);
			}
		}

		private static void ReleaseExcelObject(object obj)
		{
			try
			{
				if ( obj != null )
				{
					Marshal.ReleaseComObject(obj);
					obj = null;
				}
			}
			catch ( Exception ex )
			{
				obj = null;
				throw ex;
			}
			finally
			{
				GC.Collect();
			}
		}
	}

	
}
