using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monastry.ActiveRecord
{
	public interface IActiveRecordObject {}
	public interface IEntity : IActiveRecordObject {}
	public interface IAggregateRoot : IActiveRecordObject {}
}
