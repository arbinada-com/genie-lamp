// --------------------------------------------------------------------------
// Description : CDiese Toolkit library
// Author	   : Serge Weinstock
//
//	You are free to use, distribute or modify this code
//	as long as this header is not removed or modified.
// --------------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using CDiese.Design;

namespace CDiese.Actions
{
	/// <summary>
	/// Action.
	/// </summary>
	[
		DesignTimeVisible(false),
		ToolboxItem(false),
		DefaultEvent("Execute"),
		DefaultProperty("Text")
	]
	public class Action : System.ComponentModel.Component
	{
		#region member variables
		private object _tag;
		private string _text;
		private int	_imageIndex = -1;
		private Hashtable _components = new Hashtable();
		internal ActionList _owner = null;
		private bool _enabled = true;
		private bool _checked = false;
		private bool _visible = true;
		private Shortcut _shortcut = Shortcut.None;
		private string	_hint;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		#endregion
		#region public interface
		public Action(System.ComponentModel.IContainer container)
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			container.Add(this);
			InitializeComponent();
		}

		public Action()
		{
			/// <summary>
			/// Required for Windows.Forms Class Composition Designer support
			/// </summary>
			InitializeComponent();
		}
		/// <summary>
		/// The text used in controls associated to this Action.
		/// </summary>
		[
		Category("Misc"), 
		Localizable(true),
		Description("The text used in controls associated to this Action.")
		]
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).Text = _text;
				}
			}
		}
		/// <summary>
		/// Indicates whether the associated components are enabled.
		/// </summary>
		[
		Category("Behavior"), 
		Description("Indicates whether the associated components are enabled.")
		]
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).Enabled = _enabled;
				}
			}
		}
		/// <summary>
		/// Indicates whether the associated components are checked.
		/// </summary>
		[
		Category("Behavior"), 
		Description("Indicates whether the associated components are checked.")
		]
		public bool Checked
		{
			get
			{
				return _checked;
			}
			set
			{
				_checked = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).Checked = _checked;
				}
			}
		}
		/// <summary>
		/// Indicates the shorcut for this Action.
		/// </summary>
		[
		Category("Misc"), 
		Description("Indicates the shorcut for this Action.")
		]
		public Shortcut Shortcut
		{
			get
			{
				return _shortcut;
			}
			set
			{
				_shortcut = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).Shortcut = _shortcut;
				}
			}
		}
		/// <summary>
		/// Indicates whether the associated components are visibled or hidden.
		/// </summary>
		[
		Category("Behavior"), 
		Description("Indicates the shorcut for this Action.")
		]
		public bool Visible
		{
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).Visible = _visible;
				}
			}
		}
		/// <summary>
		/// User defined data associated with this Action.
		/// </summary>
		[
		Category("Data"), 
		Description("User defined data associated with this Action.")
		]
		public object Tag
		{
			get
			{
				return _tag;
			}
			set 
			{
				_tag = value;
			}
		}
		/// <summary>
		/// Indicates the index of the image in the parent ActionList's ImageList this Action will use to obtains its image.
		/// </summary>
		[
		Category("Misc"),
		Localizable(true),
		Description("Indicates the index of the image in the parent ActionList's ImageList this Action will use to obtains its image."),
		TypeConverter(typeof(System.Windows.Forms.ImageIndexConverter)),
		Editor(typeof(ImageIndexEditor), typeof(UITypeEditor)),
		DefaultValue(-1)
		]
		public int ImageIndex
		{
			get
			{
				return _imageIndex;
			}
			set
			{
				_imageIndex = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).ImageIndex = _imageIndex;
				}
			}
		}
		/// <summary>
		/// Indicates the text that appears as a ToolTip for a control.
		/// </summary>
		[
		Category("Misc"),
		Localizable(true),
		Description("Indicates the text that appears as a ToolTip for a control."),
		]
		public string Hint
		{
			get
			{
				return _hint;
			}
			set
			{
				_hint = value;
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).Hint = _hint;
				}
			}
		}
		/// <summary>
		/// The ActionList to which this action belongs
		/// </summary>
		[Browsable(false)]
		public ActionList Parent
		{
			get
			{
				return _owner;
			}
		}
		/// <summary>
		/// This event is triggered when the path changes
		/// </summary>
		[Description("Triggered when the action is executed")]
		public event EventHandler Execute;
		/// <summary>
		/// This event is triggered when the path changes
		/// </summary>
		[Description("Triggered when the application is idle or when the action list updates.")]
		public event EventHandler Update;
		#endregion
		#region implementation
		[Browsable(false)]
		internal ImageList ImageList
		{
			set
			{
				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).ImageList = value;
				}
			}
		}
		internal void OnExecute(Object sender, EventArgs e)
		{
			if (Execute != null)
			{
				Execute(this, e);
			}
		}
		internal void OnUpdate(Object sender, EventArgs e)
		{
			if (Update != null)
			{
				Update(this, e);
			}
		}
		internal void SetComponent(Component comp, bool add)
		{
			ActionData ad = (ActionData)_components[comp];

			if (add)
			{
				if (ad == null)
				{
					ad = new ActionData();
					ad.Attach(this, comp, DesignMode);
					_components[comp] = ad;
				}
			}
			else if (ad != null)
			{
				ad.Detach();
				_components.Remove(comp);
			}
		}
		internal bool HandleComponent(Component comp)
		{
			return (_components[comp] != null);
		}
		/// <summary>
		/// The ActionList to which this action belongs
		/// </summary>
		[Browsable(false)]
		internal bool ShowTextOnToolBar
		{
			set
			{
				string dtext = (value ? Text : null);

				IDictionaryEnumerator i = _components.GetEnumerator();
				while(i.MoveNext())
				{
					((ActionData)i.Value).ShowTextOnToolBar = dtext;
				}
			}
		}
		#endregion
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		internal void FinishInit()
		{
			IDictionaryEnumerator i = _components.GetEnumerator();
			while(i.MoveNext())
			{
				((ActionData)i.Value).FinishInit();
			}
		}
		#endregion
	}

	/// <summary>
	/// Internal data about a control used by an Action
	/// </summary>
	internal class ActionData : IDisposable
	{
		#region member variables
		private		PropertyInfo	_text;
		private		PropertyInfo	_enabled;
		private		PropertyInfo	_checked;
		private		PropertyInfo	_visible;
		private		PropertyInfo	_shortcut;
		private		PropertyInfo	_imageIndex;
		private		PropertyInfo	_imageList;
		private		bool			_click = false;
		private		Component		_component;
		private		Action			_owner;
		#endregion
		#region "public" interface
		internal void Attach(Action a, Component o, bool designMode)
		{
			Debug.Assert(o != null && a != null);
			_component = o;
			_owner = a;
			Debug.Assert(_owner.Parent != null);
			// Text
			_text = o.GetType().GetProperty("Text");
			if (_text != null && (!_text.CanRead || !_text.CanWrite) && (_text.PropertyType == typeof(string)))
			{
				// we must be able to read and write a boolean property
				_text = null;
			}
			Text = _owner.Text;
			// Enabled
			_enabled = o.GetType().GetProperty("Enabled");
			if (_enabled != null && (!_enabled.CanRead || !_enabled.CanWrite) && (_enabled.PropertyType == typeof(bool)))
			{
				// we must be able to read and write a boolean property
				_enabled = null;
			}
			Enabled = _owner.Enabled;
			// Checked
			// special case of a toolbarButton
			if (_component is ToolBarButton)
			{
				_checked = o.GetType().GetProperty("Pushed");
				Debug.Assert(_checked != null && _checked.CanRead && _checked.CanWrite && (_checked.PropertyType == typeof(bool)));
			}
			else
			{
				_checked = o.GetType().GetProperty("Checked");
				if (_checked != null && (!_checked.CanRead || !_checked.CanWrite) && (_checked.PropertyType == typeof(bool)))
				{
					// we must be able to read and write a boolean property
					_checked = null;
				}
			}
			Checked = _owner.Checked;
			// Visible
			_visible = o.GetType().GetProperty("Visible");
			if (_visible != null && (!_visible.CanRead || !_visible.CanWrite) && (_visible.PropertyType == typeof(bool)))
			{
				// we must be able to read and write a boolean property
				_visible = null;
			}
			Visible = _owner.Visible;
			// Shortcut
			_shortcut = o.GetType().GetProperty("Shortcut");
			if (_shortcut != null && (!_shortcut.CanRead || !_shortcut.CanWrite) && (_enabled.PropertyType == typeof(Shortcut)))
			{
				// we must be able to read and write a shortcut property
				_shortcut = null;
			}
			Shortcut = _owner.Shortcut;
			// ImageList
			// don't handle toolbarButtons here
			if (!(_component is ToolBarButton))
			{
				_imageList = o.GetType().GetProperty("ImageList");
				if (_imageList != null && (!_imageList.CanRead || !_imageList.CanWrite) && (_imageList.PropertyType == typeof(ImageList)))
				{
					// we must be able to read and write an ImageList property
					_imageList = null;
				}
			}
			ImageList = _owner.Parent.ImageList;
			// ImageIndex
			_imageIndex = o.GetType().GetProperty("ImageIndex");
			if (_imageIndex != null && (!_imageIndex.CanRead || !_imageIndex.CanWrite) && (_imageIndex.PropertyType == typeof(int)))
			{
				// we must be able to read and write an integer property
				_imageIndex = null;
			}
			ImageIndex = _owner.ImageIndex;
			// Hint
			Hint = _owner.Hint;
			// click
			if (!designMode)
			{
				// special case of a toolbarButton
				if (_component is ToolBarButton)
				{
					ToolBar tb = ((ToolBarButton)_component).Parent;
					if (tb != null)
					{
						tb.ButtonClick += new ToolBarButtonClickEventHandler(OnToolbarClick);
						_click = true;
					}
				}
				else
				{
					EventInfo e = o.GetType().GetEvent("Click");
					if (e != null && e.EventHandlerType == typeof(EventHandler))
					{
						e.AddEventHandler(_component, new EventHandler(_owner.OnExecute));
						_click = true;
					}
				}
			}
			// Dispose
			Debug.Assert(_owner.Parent != null);
			_component.Disposed += new EventHandler(_owner.Parent.OnComponentDisposed);
		}
		internal void Detach()
		{
			_text = null;
			_enabled = null;
			_checked = null;
			_shortcut = null;
			if (_component != null && _click)
			{
				if (_component is ToolBarButton)
				{
					ToolBar tb = ((ToolBarButton)_component).Parent;
					if (tb != null)
					{
						tb.ButtonClick -= new ToolBarButtonClickEventHandler(OnToolbarClick);
					}
				}
				else
				{
					EventInfo e = _component.GetType().GetEvent("Click");
					e.RemoveEventHandler(_component, new EventHandler(_owner.OnExecute));
				}
			}
			Debug.Assert(_owner.Parent != null);
			_component.Disposed -= new EventHandler(_owner.Parent.OnComponentDisposed);
		}
		internal string Text
		{
			set
			{
				if (_text != null)
				{
					if (_component is ToolBarButton && !_owner.Parent.ShowTextOnToolBar)
					{
						_text.SetValue(_component, null, null);
					}
					else if ((string)_text.GetValue(_component, null) != value)
					{
						_text.SetValue(_component, value, null);
					}
				}
			}
		}
		internal bool Enabled
		{
			set
			{
				if (_enabled != null && ((bool)_enabled.GetValue(_component, null) != value))
				{
					_enabled.SetValue(_component, value, null);
				}
			}
		}
		internal bool Checked
		{
			set
			{
				if (_checked != null && ((bool)_checked.GetValue(_component, null) != value))
				{
					_checked.SetValue(_component, value, null);
				}
			}
		}
		internal bool Visible
		{
			set
			{
				if (_visible != null && ((bool)_visible.GetValue(_component, null) != value))
				{
					_visible.SetValue(_component, value, null);
				}
			}
		}
		internal Shortcut Shortcut
		{
			set
			{
				if (_shortcut != null && ((Shortcut)_shortcut.GetValue(_component, null) != value))
				{
					_shortcut.SetValue(_component, value, null);
				}
			}
		}
		internal ImageList ImageList
		{
			set
			{
				if (_component is ToolBarButton)
				{
					ToolBarButton tb = (ToolBarButton)_component;

					if (tb.Parent != null && tb.Parent.ImageList != value)
					{
						tb.Parent.ImageList = value;
					}
					return;
				}
				if (_imageList != null && ((ImageList)_imageList.GetValue(_component, null) != value))
				{
					_imageList.SetValue(_component, value, null);
				}
			}
		}
		internal int ImageIndex
		{
			set
			{
				if (_imageIndex != null && ((int)_imageIndex.GetValue(_component, null) != value))
				{
					_imageIndex.SetValue(_component, value, null);
				}
			}
		}
		private void OnToolbarClick(Object sender, ToolBarButtonClickEventArgs e)
		{
			if	(e.Button == _component)
			{
				_owner.OnExecute(sender, e);
			}
		}
		public void Dispose()
		{
			Detach();
		}
		internal void FinishInit()
		{
			if (_component is ToolBarButton && !_click)
			{
				ToolBar tb = ((ToolBarButton)_component).Parent;
				if (tb != null)
				{
					tb.ButtonClick += new ToolBarButtonClickEventHandler(OnToolbarClick);
					_click = true;
				}
			}
		}
		internal string ShowTextOnToolBar
		{
			set
			{
				if (_component is ToolBarButton)
				{
					Text = value;
				}
			}
		}
		internal string Hint
		{
			set
			{
				if (_component is ToolBarButton)
				{
					if (((ToolBarButton)_component).ToolTipText != value)
					{
						((ToolBarButton)_component).ToolTipText = value;
					}
				}
				else if (_component is Control)
				{
					Debug.Assert(_owner != null &&  _owner.Parent != null && _owner._owner._toolTip != null);
					Control	c = (Control)_component;
					ToolTip t = _owner._owner._toolTip;
					if (t.GetToolTip(c) != value)
					{
						t.SetToolTip(c, value);
					}
				}
			}
		}
		#endregion
	}
}