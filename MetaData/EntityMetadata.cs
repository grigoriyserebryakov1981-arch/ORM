using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Atdi.Contracts.CoreServices.EntityOrm.Metadata
{
    [Serializable]
    public class EntityMetadata : IEntityMetadata
    {
        private readonly Dictionary<string, IFieldMetadata> _localFields;
        private Type _codeType;

        public EntityMetadata(IEntityOrm entityOrm)
        {
	        this.EntityOrm = entityOrm;
            this._localFields = new Dictionary<string, IFieldMetadata>();
        }

        public IEntityOrm EntityOrm { get; }

		public string Name { get; set; }

        public string Namespace { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public EntityType Type { get; set; }

        public IEntityMetadata BaseEntity { get; set; }

        public IDataSourceMetadata DataSource { get; set; }

        public IPrimaryKeyMetadata PrimaryKey { get; set; }

        public IReadOnlyDictionary<string, IFieldMetadata> Fields => _localFields;

        public string QualifiedName { get; set; }

        public Type CodeType
        {
	        get
	        {
		        if (_codeType == null)
		        {
			        var typeName = $"{this.Namespace}.I{this.Name}, {EntityOrm.Config.Assembly}";
			        _codeType = System.Type.GetType(typeName);
			        if (_codeType == null)
			        {
				        throw new InvalidOperationException($"Cannot found Entity Interface by name '{typeName}'");
			        }
				}

		        return _codeType;
	        }
        }

		public override string ToString()
        {
            return $"{this.Name}: Type = '{Type}', Base = '{BaseEntity?.QualifiedName}', Count fields = {Fields.Count}";
        }

        public void AppendField(IFieldMetadata field)
        {
            if (_localFields.ContainsKey(field.Name))
            {
                throw new InvalidOperationException($"A local field named {field.Name} already exists in the local field set");
            }
            this._localFields.Add(field.Name, field);
        }

        public void CopyField(IFieldMetadata field)
        {
            if (_localFields.ContainsKey(field.Name))
            {
                throw new InvalidOperationException($"A local field named {field.Name} already exists in the local field set");
            }
            var cloneField = ((FieldMetadata)field).CopyTo(this);
            this._localFields.Add(cloneField.Name, cloneField);
        }
    }
}
