using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;
using Atdi.DataModels;
using Atdi.Platform.Logging;
using Atdi.Common.Extensions;

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
	class ClrTypeNTEXT : ValueAdapter<object, string>
	{
		public ClrTypeNTEXT(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
		{
		}

		public override object DecodeAs(object storeValue)
		{
			return storeValue;
			//if (storeValue == null)
			//{
			//	return (string)null;
			//}
			//var store = (string)storeValue;
			//var value = store.Deserialize<object>();
			//return value;
		}

		public override object DecodeAsType(object storeValue, Type type)
		{
			if (storeValue == null)
			{
				return (string)null;
			}
			var store = (string)storeValue;
			var value = Newtonsoft.Json.JsonConvert.DeserializeObject(store, type);
			return value;
		}

		public override object DecodeAsType(IEngineDataReader dataReader, int ordinal, Type type)
		{
			var store = ValueReader.ReadAsNTEXT(dataReader, ordinal);
			var value = Newtonsoft.Json.JsonConvert.DeserializeObject(store, type);
			return value;
		}

		public override object DecodeAs(IEngineDataReader dataReader, int ordinal)
		{
			return ValueReader.ReadAsNTEXT(dataReader, ordinal);
		}

		public override string EncodeAs(ColumnValue columnValue)
		{
			var source = (columnValue as ClrTypeColumnValue)?.Value;
			if (source == null)
			{
				return null;
			}

			return Newtonsoft.Json.JsonConvert.SerializeObject(source);
		}
	}
}
