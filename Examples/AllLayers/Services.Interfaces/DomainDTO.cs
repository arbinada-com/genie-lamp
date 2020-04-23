// Genie Lamp Core (1.1.4594.29523)
// ServiceStack services interfaces genie (1.0.4594.29525)
// Starter application (1.1.4594.29524)
// This file was automatically generated at 2012-07-30 16:36:49
// Do not modify it manually.

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Arbinada.GenieLamp.Warehouse.Services.Interfaces
{
	
	[DataContract]
	public enum DomainTypes
	{
		[EnumMember] TypeEntityType = 1,
		[EnumMember] TypeEntityRegistry = 2,
		[EnumMember] TypeExampleOneToOne = 3,
		[EnumMember] TypeExampleOneToOneEx = 4,
		[EnumMember] TypeProductType = 5,
		[EnumMember] TypeProduct = 6,
		[EnumMember] TypeStoreType = 7,
		[EnumMember] TypeStore = 8,
		[EnumMember] TypeContractor = 9,
		[EnumMember] TypePartner = 10,
		[EnumMember] TypeStoreDoc = 11,
		[EnumMember] TypeStoreDocLine = 12,
		[EnumMember] TypeStoreTransaction = 13
	}
	
	[DataContract]
	[KnownType(typeof(PersistentObjectDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO))]
	public abstract class DomainObjectDTO
	{
		[DataMember]
		public string Internal_ObjectId { get; set; }
		public abstract int GetInternal_DomainTypeId();
		[DataMember]
		public bool Changed { get; set; }
		public DomainObjectDTO()
		{
			this.Internal_ObjectId = Guid.NewGuid().ToString();
		}
	}
	
	[DataContract]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityTypeDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Core.EntityRegistryDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ExampleOneToOneExDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductTypeDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ProductDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTypeDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.PartnerDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreDocLineDTO))]
	[KnownType(typeof(Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.StoreTransactionDTO))]
	public abstract class PersistentObjectDTO : DomainObjectDTO
	{
	}
	
	#region Enumerations
	namespace Warehouse
	{
		[DataContract]
		public enum State
		{
			[EnumMember] InProgress = 0,
			[EnumMember] Validated = 1
		}
		
		[DataContract]
		public enum Direction
		{
			[EnumMember] Income = 0,
			[EnumMember] Outcome = 1
		}
		
	}
	#endregion
	
	#region Entities
	namespace Core
	{
		[DataContract]
		public partial class EntityTypeDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 1; // DomainTypes.TypeEntityType
			public override int GetInternal_DomainTypeId() { return EntityTypeDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_FullName;
			[DataMember]
			public string FullName
			{
				get { return m_FullName; }
				set { m_FullName = value != null && value.Length > 255 ? value.Substring(0, 255) : value; }
			}
			private string m_ShortName;
			[DataMember]
			public string ShortName
			{
				get { return m_ShortName; }
				set { m_ShortName = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			private string m_Description;
			[DataMember]
			public string Description
			{
				get { return m_Description; }
				set { m_Description = value != null && value.Length > 1000 ? value.Substring(0, 1000) : value; }
			}
			
		}
		
		
		[DataContract]
		public partial class EntityRegistryDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 2; // DomainTypes.TypeEntityRegistry
			public override int GetInternal_DomainTypeId() { return EntityRegistryDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			
			[DataMember]
			public int? EntityTypeIdId { get; set; }
		}
		
		
	}
	
	namespace Warehouse
	{
		[DataContract]
		public partial class ExampleOneToOneDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 3; // DomainTypes.TypeExampleOneToOne
			public override int GetInternal_DomainTypeId() { return ExampleOneToOneDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class ExampleOneToOneExDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 4; // DomainTypes.TypeExampleOneToOneEx
			public override int GetInternal_DomainTypeId() { return ExampleOneToOneExDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Caption;
			[DataMember]
			public string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int ExempleOneToOneId { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class ProductTypeDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 5; // DomainTypes.TypeProductType
			public override int GetInternal_DomainTypeId() { return ProductTypeDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Code;
			[DataMember]
			public string Code
			{
				get { return m_Code; }
				set { m_Code = value != null && value.Length > 3 ? value.Substring(0, 3) : value; }
			}
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class ProductDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 6; // DomainTypes.TypeProduct
			public override int GetInternal_DomainTypeId() { return ProductDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_RefCode;
			[DataMember]
			public string RefCode
			{
				get { return m_RefCode; }
				set { m_RefCode = value != null && value.Length > 10 ? value.Substring(0, 10) : value; }
			}
			private string m_Caption;
			[DataMember]
			public string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? TypeId { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class StoreTypeDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 7; // DomainTypes.TypeStoreType
			public override int GetInternal_DomainTypeId() { return StoreTypeDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 3 ? value.Substring(0, 3) : value; }
			}
			private string m_Caption;
			[DataMember]
			public string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class StoreDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 8; // DomainTypes.TypeStore
			public override int GetInternal_DomainTypeId() { return StoreDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Code;
			[DataMember]
			public string Code
			{
				get { return m_Code; }
				set { m_Code = value != null && value.Length > 15 ? value.Substring(0, 15) : value; }
			}
			private string m_Caption;
			[DataMember]
			public string Caption
			{
				get { return m_Caption; }
				set { m_Caption = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? StoreTypeId { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class ContractorDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 9; // DomainTypes.TypeContractor
			public override int GetInternal_DomainTypeId() { return ContractorDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_Address;
			[DataMember]
			public string Address
			{
				get { return m_Address; }
				set { m_Address = value != null && value.Length > 255 ? value.Substring(0, 255) : value; }
			}
			private string m_Phone;
			[DataMember]
			public string Phone
			{
				get { return m_Phone; }
				set { m_Phone = value != null && value.Length > 20 ? value.Substring(0, 20) : value; }
			}
			private string m_Email;
			[DataMember]
			public string Email
			{
				get { return m_Email; }
				set { m_Email = value != null && value.Length > 80 ? value.Substring(0, 80) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class PartnerDTO : Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.ContractorDTO
		{
			[DataMember]
			public new const int Internal_DomainTypeId = 10; // DomainTypes.TypePartner
			public override int GetInternal_DomainTypeId() { return PartnerDTO.Internal_DomainTypeId; }
			[DataMember]
			public DateTime Since { get; set; }
			
		}
		
		
		[DataContract]
		public partial class StoreDocDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 11; // DomainTypes.TypeStoreDoc
			public override int GetInternal_DomainTypeId() { return StoreDocDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_RefNum;
			[DataMember]
			public string RefNum
			{
				get { return m_RefNum; }
				set { m_RefNum = value != null && value.Length > 16 ? value.Substring(0, 16) : value; }
			}
			private DateTime m_Created = DateTime.Now;
			[DataMember]
			public DateTime Created
			{
				get { return m_Created; }
				set { m_Created = value; }
			}
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class StoreDocLineDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 12; // DomainTypes.TypeStoreDocLine
			public override int GetInternal_DomainTypeId() { return StoreDocLineDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Position { get; set; }
			[DataMember]
			public int Quantity { get; set; }
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int StoreDocId { get; set; }
			[DataMember]
			public int ProductId { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
		[DataContract]
		public partial class StoreTransactionDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 13; // DomainTypes.TypeStoreTransaction
			public override int GetInternal_DomainTypeId() { return StoreTransactionDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private DateTime m_TxDate = DateTime.Now;
			[DataMember]
			public DateTime TxDate
			{
				get { return m_TxDate; }
				set { m_TxDate = value; }
			}
			private Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.Direction m_Direction = (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.Direction)0;
			[DataMember]
			public Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.Direction Direction
			{
				get { return m_Direction; }
				set { m_Direction = value; }
			}
			private Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.State m_State = (Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.State)0;
			[DataMember]
			public Arbinada.GenieLamp.Warehouse.Services.Interfaces.Warehouse.State State
			{
				get { return m_State; }
				set { m_State = value; }
			}
			[DataMember]
			public int Quantity { get; set; }
			[DataMember]
			public int Version { get; set; }
			private string m_CreatedBy;
			[DataMember]
			public string CreatedBy
			{
				get { return m_CreatedBy; }
				set { m_CreatedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? CreatedDate { get; set; }
			private string m_LastModifiedBy;
			[DataMember]
			public string LastModifiedBy
			{
				get { return m_LastModifiedBy; }
				set { m_LastModifiedBy = value != null && value.Length > 64 ? value.Substring(0, 64) : value; }
			}
			[DataMember]
			public DateTime? LastModifiedDate { get; set; }
			
			[DataMember]
			public int SupplierId { get; set; }
			[DataMember]
			public int StoreId { get; set; }
			[DataMember]
			public int ProductId { get; set; }
			[DataMember]
			public int CustomerId { get; set; }
			[DataMember]
			public int? EntityRegistryId { get; set; }
		}
		
		
	}
	#endregion
	
	
}

