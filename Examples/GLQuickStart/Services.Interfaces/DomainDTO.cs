// Genie Lamp Core (1.1.4798.27721)
// ServiceStack services interfaces genie (1.0.4798.27724)
// Starter application (1.1.4798.27722)
// This file was automatically generated at 2013-03-14 16:56:47
// Do not modify it manually.

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace GenieLamp.Examples.QuickStart.Services.Interfaces
{
	
	[DataContract]
	public enum DomainTypes
	{
		[EnumMember] TypeCustomer = 1,
		[EnumMember] TypeProduct = 2,
		[EnumMember] TypePurchaseOrder = 3,
		[EnumMember] TypePurchaseOrderLine = 4
	}
	
	[DataContract]
	[KnownType(typeof(PersistentObjectDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO))]
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
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.CustomerDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.ProductDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderDTO))]
	[KnownType(typeof(GenieLamp.Examples.QuickStart.Services.Interfaces.QuickStart.PurchaseOrderLineDTO))]
	public abstract class PersistentObjectDTO : DomainObjectDTO
	{
	}
	
	#region Enumerations
	#endregion
	
	#region Entities
	namespace QuickStart
	{
		[DataContract]
		public partial class CustomerDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 1; // DomainTypes.TypeCustomer
			public override int GetInternal_DomainTypeId() { return CustomerDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Code;
			[DataMember]
			public string Code
			{
				get { return m_Code; }
				set { m_Code = value != null && value.Length > 10 ? value.Substring(0, 10) : value; }
			}
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			private string m_Phone;
			[DataMember]
			public string Phone
			{
				get { return m_Phone; }
				set { m_Phone = value != null && value.Length > 40 ? value.Substring(0, 40) : value; }
			}
			private string m_Email;
			[DataMember]
			public string Email
			{
				get { return m_Email; }
				set { m_Email = value != null && value.Length > 255 ? value.Substring(0, 255) : value; }
			}
			
		}
		
		
		[DataContract]
		public partial class ProductDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 2; // DomainTypes.TypeProduct
			public override int GetInternal_DomainTypeId() { return ProductDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Reference;
			[DataMember]
			public string Reference
			{
				get { return m_Reference; }
				set { m_Reference = value != null && value.Length > 10 ? value.Substring(0, 10) : value; }
			}
			private string m_Name;
			[DataMember]
			public string Name
			{
				get { return m_Name; }
				set { m_Name = value != null && value.Length > 100 ? value.Substring(0, 100) : value; }
			}
			
		}
		
		
		[DataContract]
		public partial class PurchaseOrderDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 3; // DomainTypes.TypePurchaseOrder
			public override int GetInternal_DomainTypeId() { return PurchaseOrderDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			private string m_Number;
			[DataMember]
			public string Number
			{
				get { return m_Number; }
				set { m_Number = value != null && value.Length > 15 ? value.Substring(0, 15) : value; }
			}
			[DataMember]
			public DateTime IssueDate { get; set; }
			private bool m_Validated = false;
			[DataMember]
			public bool Validated
			{
				get { return m_Validated; }
				set { m_Validated = value; }
			}
			[DataMember]
			public DateTime? ShipmentDate { get; set; }
			[DataMember]
			public decimal TotalAmount { get; set; }
			
			[DataMember]
			public int CustomerId { get; set; }
		}
		
		
		[DataContract]
		public partial class PurchaseOrderLineDTO : PersistentObjectDTO
		{
			[DataMember]
			public const int Internal_DomainTypeId = 4; // DomainTypes.TypePurchaseOrderLine
			public override int GetInternal_DomainTypeId() { return PurchaseOrderLineDTO.Internal_DomainTypeId; }
			[DataMember]
			public int Id { get; set; }
			[DataMember]
			public short Position { get; set; }
			[DataMember]
			public decimal Price { get; set; }
			[DataMember]
			public int Quantity { get; set; }
			
			[DataMember]
			public int PurchaseOrderId { get; set; }
			[DataMember]
			public int ProductId { get; set; }
		}
		
		
	}
	#endregion
	
	
}

