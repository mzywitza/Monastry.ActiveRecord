using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.Tests.Model
{
	public class AssignedSoftware:IActiveRecordObject
	{
		public virtual string Key { get; set; }
		public virtual string Name { get; set; }
	}
}
