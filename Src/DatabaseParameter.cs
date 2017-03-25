// Copyright (c) Strisys Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Diagnostics;

namespace DataAccessFoundation {	
	/// <summary>
	/// Encapsulates a set of values used to set parameters.
	/// </summary>
	[DebuggerStepThrough]
	public class DatabaseParameter {
		#region Fields

		private const Int32 DEFAULT_MAX_STRING_SIZE = 8000;
		private readonly String _name;
		private readonly DbType _type;
		private readonly ParameterDirection _direction;
		private readonly Int32 _size;
		private Object _value;

		#endregion
		
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseParameter"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		public DatabaseParameter(String name, DbType type, Object value)
			: this(name, type, value, ParameterDirection.Input) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseParameter" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="direction">The direction.</param>
		public DatabaseParameter(String name, DbType type, Object value, ParameterDirection direction)
			: this(name, type, value, direction, DEFAULT_MAX_STRING_SIZE) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseParameter" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="size">The size.</param>
		public DatabaseParameter(String name, DbType type, Object value, Int32 size) 
			: this(name, type, value, ParameterDirection.Output, size) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseParameter" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="direction">The direction.</param>
		/// <param name="size">The size.</param>
		public DatabaseParameter(String name, DbType type, Object value, ParameterDirection direction, Int32 size) {
			_name = name;
			_type = type;
			_value = value;
			_direction = direction;
			_size = size;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name.
		/// </summary>
		public String Name {
			get {  return _name; }
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		public DbType Type {
			get { return _type; }
		}

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		public Object Value {
			get { return _value; }
			set { _value = value;}
		}

		/// <summary>
		/// Gets the direction.
		/// </summary>
		/// <value>
		/// The direction.
		/// </value>
		public ParameterDirection Direction {
			get { return _direction; }
		}

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <value>
		/// The size.
		/// </value>
		public Int32 Size {
			get { return _size; }
		}

		#endregion

		#region String Representation

		/// <summary>
		/// Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> containing a fully qualified type name.
		/// </returns>
		public override String ToString() {
			return String.Format("Name:={0}, Type:={1}, Value:={2}", Name, Type, (((null == Value) || (DBNull.Value == Value)) ? "null" : Value.ToString()));
		}

		#endregion 
	}
}
