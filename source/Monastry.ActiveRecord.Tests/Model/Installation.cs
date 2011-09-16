using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.Tests.Model
{
	public class Installation : IActiveRecordObject
	{
		public virtual Guid Id { get; set; }
		public virtual string Computer { get; set; }
		public virtual Software Software { get; set; }
	}

}
