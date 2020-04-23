// --------------------------------------------------------------------------
// Description : CDiese Toolkit library
// Author	   : Serge Weinstock
//
//	You are free to use, distribute or modify this code
//	as long as this header is not removed or modified.
// --------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Collections;
using System.Diagnostics;

namespace CDiese.Actions
{
	/// <summary>
	/// <para>TypeConverter for an extender provider which provides an Action property which is one of the Action of an ActionCollection</para>
	/// <para>As I've found no way to get access to the associated collection, an Action provides information
	/// about the associated collection through the "Parent" property. As the Action item is initialised in the GetAction method of the extender provider,
	/// it's possible to get a reference to the ActionCollection (which is only needed for filling the list of 
	/// possible values) during the call to ConvertTo.</para>
	/// </summary>
	public class ActionConverter : TypeConverter
	{
		/// <summary>
		/// Returns whether this converter can convert an object of one type to the type of this converter.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="sourceType">A Type that represents the type you want to convert from.</param>
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) 
		{
			if (sourceType == typeof(string)) 
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		/// <summary>
		/// Converts the given value to the type of this converter.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">The CultureInfo to use as the current culture.</param>
		/// <param name="value">The Object to convert.</param>
		/// <returns>An Object that represents the converted value.</returns>
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) 
		{
			if (value is string) 
			{
				try
				{
					Debug.Assert(_actionList != null && _actionList.Actions != null);
					if ((string)value == "")
					{
						return _actionList.Actions.Null;
					}

					IReferenceService rs = (IReferenceService)context.GetService(typeof(IReferenceService));
					Debug.Assert(rs != null);
					return rs.GetReference((string)value);
				}
				catch
				{
					throw new ArgumentException("Can not convert '" + (string)value + "' to type Object");
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
		/// <summary>
		/// Converts the given value object to the specified type, using the specified context and culture information.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		/// <param name="culture">A CultureInfo object. If a null reference (Nothing in Visual Basic) is passed, the current culture is assumed.</param>
		/// <param name="value">The Object to convert.</param>
		/// <param name="destinationType">The Type to convert the value parameter to.</param>
		/// <returns>An Object that represents the converted value.</returns>
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) 
		{  
			if (destinationType == typeof(string) && context != null) 
			{
				IReferenceService rs = (IReferenceService)context.GetService(typeof(IReferenceService));
				Debug.Assert(rs != null);
				Action a = (Action)value;
				if (a != null)
				{
					// here's the hack for getting a reference to the associated collection
					_actionList = a.Parent;

					Debug.Assert(_actionList != null && _actionList.Actions != null);
					if (a == _actionList.Actions.Null)
					{
						return "";
					}
					return rs.GetName(a);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		/// <summary>
		/// Returns whether this object supports a standard set of values that can be picked from a list.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) 
		{
			return true;
		}
		/// <summary>
		/// Returns whether the collection of standard values returned from GetStandardValues is an exclusive list.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) 
		{
			return true;
		}
		/// <summary>
		/// Returns a collection of standard values for the data type this type converter is designed for.
		/// </summary>
		/// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) 
		{
			//System.Windows.Forms.MessageBox.Show("GetStandardValues");
			Debug.Assert(_actionList != null && _actionList.Actions != null);
			ArrayList res = new ArrayList();
			res.Add(_actionList.Actions.Null);
			foreach (Object o in _actionList.Actions)
			{
				res.Add(o);
			}
			return  new StandardValuesCollection(res);
		}

		private ActionList	_actionList;
	}
	/// <summary>
	/// Editor for an ActionCollection
	/// </summary>
	internal class ActionCollectionEditor : CollectionEditor
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ActionCollectionEditor()
			: base(typeof(ActionCollection))
		{
		}
		/// <summary>
		/// Gets an array of objects containing the specified collection.
		/// </summary>
		/// <param name="editValue">The collection to edit</param>
		/// <returns>An array containing the collection objects.</returns>
		protected override object[] GetItems(object editValue)
		{
			Debug.Assert(editValue != null && editValue is ActionCollection);
			ActionCollection	coll = (ActionCollection)editValue;
			Action [] res = new Action[coll.Count];
			if (coll.Count > 0)
			{
				coll.CopyTo(res, 0);
			}
			return res;
		}
		/// <summary>
		/// Sets the specified array as the items of the collection.
		/// </summary>
		/// <param name="editValue">The collection to edit.</param>
		/// <param name="value">An array of objects to set as the collection items.</param>
		/// <returns>The newly created collection object or, otherwise, the collection indicated by the editValue parameter.</returns>
		protected override object SetItems(
			object editValue,
			object[] value
			)
		{
			Debug.Assert(editValue != null && editValue is ActionCollection);
			ActionCollection	coll = (ActionCollection)editValue;
			
			coll.Clear();
			foreach(object o in value)
			{
				coll.Add((Action)o);
			}
			return coll;
		}
	}
}
