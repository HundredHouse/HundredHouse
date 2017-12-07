using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HundredHouse
{
	public class Node
	{	
		// 위치 코드 필요함
		public string Location { get; set; }

		/// <summary>
		/// 지역 명
		/// </summary>
		public string ReasonName { get; set; }

		/// <summary>
		/// 1.1m3 단가
		/// </summary>
		public double Amt { get; set; }

		/// <summary>
		/// 한 평당 가격(평단가)
		/// </summary>
		public double AmtPerField
		{
			get
			{
				return Math.Round(this.Amt * 3.3, 0);
			}
			set { }
		}

		/// <summary>
		/// 보여주는 데이터
		/// </summary>
		public string TextAmtPerField
		{
			get
			{
				return String.Format("{0:#,##0}만원", Math.Round(this.Amt * 3.3, 0));
			}
			set { }
		}
	}
}
