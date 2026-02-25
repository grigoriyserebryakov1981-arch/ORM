using Atdi.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.CoreServices.EntityOrm.Metadata;

namespace Atdi.CoreServices.EntityOrm
{
    internal sealed class QueryModification
    {
        private readonly QueryRoot _root;
        private readonly Dictionary<string, FieldValueDescriptor> _fields;

        public QueryModification(QueryRoot root)
        {
            this._root = root;
            this._fields = new Dictionary<string, FieldValueDescriptor>();
        }

        public IReadOnlyDictionary<string, FieldValueDescriptor> Fields => _fields;

        public void Append(ColumnValue value)
        {
            var descriptor = this._root.EnsureField(value.Name);

			// тут допускается совместимость типов
			if (value.DataType == DataType.ClrEnum)
			{
				if (descriptor.Field.DataType.CodeVarType != DataType.ClrEnum
				&& descriptor.Field.DataType.CodeVarType != DataType.Byte
				&& descriptor.Field.DataType.CodeVarType != DataType.Short
				&& descriptor.Field.DataType.CodeVarType != DataType.UnsignedShort
				&& descriptor.Field.DataType.CodeVarType != DataType.Integer
				&& descriptor.Field.DataType.CodeVarType != DataType.UnsignedInteger
				&& descriptor.Field.DataType.CodeVarType != DataType.Long
				&& descriptor.Field.DataType.CodeVarType != DataType.UnsignedLong)
				{
					throw new InvalidOperationException($"Data type mismatch for the field with name '{descriptor.Field.Name}({descriptor.Field.Title}) in the entity '{descriptor.Field.Entity.Name}({descriptor.Field.Entity.Title})'. The expected type is '{descriptor.Field.DataType.CodeVarType.ToString()}' but the incoming type is '{value.DataType.ToString()}'");
				}
			}
            else if (descriptor.Field.DataType.CodeVarType != value.DataType)
            {
                throw new InvalidOperationException($"Data type mismatch for the field with name '{descriptor.Field.Name}({descriptor.Field.Title}) in the entity '{descriptor.Field.Entity.Name}({descriptor.Field.Entity.Title})'. The expected type is '{descriptor.Field.DataType.CodeVarType.ToString()}' but the incoming type is '{value.DataType.ToString()}'");
            }

            var fieldValue = new FieldValueDescriptor
            {
                Descriptor = descriptor,
                Store = value
            };

            if (this._fields.ContainsKey(value.Name))
            {
	            this._fields[value.Name] = fieldValue;

				//throw new InvalidOperationException($"A field with path '{value.Name}' has been specified more than once in the modification.");
            }
            else
            {
				this._fields.Add(value.Name, fieldValue);
			}

			// В случаи изменения значения енума ищим поле видимого значения
            if (value.DataType == DataType.ClrEnum && value.Name.EndsWith("Code"))
            {
	            var enumViewFieldName = value.Name.Substring(0, value.Name.Length - 4) + "Name";
	            if (_root.Entity.TryGetLocalField(enumViewFieldName, out IFieldMetadata fieldMetadata))
	            {
					this.Append(new StringColumnValue
					{
						Name = enumViewFieldName,
						Value = Convert.ToString(value.GetValue())
					});
	            }
            }

		}

        public override string ToString()
        {
            return $"Fields = {_fields.Count}";
        }
    }
}
