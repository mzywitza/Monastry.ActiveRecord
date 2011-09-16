using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord.Tests.Model
{
	public class Software
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
	}
}
