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

namespace Atdi.CoreServices.EntityOrm.ValueAdapters
{
    class EnumINT08 : ValueAdapter<Enum, sbyte?>
    {
        public EnumINT08(IDataTypeMetadata dataTypeMetadata, ILogger logger) : base(dataTypeMetadata, logger)
        {
        }

		public override Enum DecodeAs(object storeValue)
		{
			throw new InvalidOperationException("Unsupported decode without an enum type");
		}

		public override object DecodeAsType(object storeValue, Type type)
		{
			return Enum.ToObject(type, storeValue);
		}

		public override Enum DecodeAs(IEngineDataReader dataReader, int ordinal)
        {
            throw new InvalidOperationException("Unsupported decode without an enum type");
        }

		public override object DecodeAsType(IEngineDataReader dataReader, int ordinal, Type type)
		{
			return Enum.ToObject(type, ValueReader.ReadAsINT08(dataReader, ordinal));
		}

		public override sbyte? EncodeAs(ColumnValue columnValue)
        {
	        if (columnValue is ClrEnumColumnValue enumSource)
	        {
		        if (enumSource.Value == null)
		        {
			        return null;
		        }
		        return Convert.ToSByte(enumSource.Value);
	        }
			var source = (columnValue as ByteColumnValue).Value;
            if (!source.HasValue)
            {
                return null;
            }
            return (sbyte?)source.Value;
        }
    }
    
}
