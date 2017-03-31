// Copyright (c) Strisys Corporation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace DataAccessBlockFunc {
	/// <summary>
	/// Collection responsbile for managing <see cref="DatabaseParameter"/> instances.
	/// </summary>
	[DebuggerStepThrough]
	public class DatabaseParameterCollection : IEnumerable<DatabaseParameter> {
		#region Fields

		private readonly IDictionary<String, DatabaseParameter> _parameters = new Dictionary<String, DatabaseParameter>();
		private Int32 _timeout = 30;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseParameterCollection"/> class.
		/// </summary>
		public DatabaseParameterCollection() {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseParameterCollection"/> class.
		/// </summary>
		/// <param name="parameters">The parameter bags.</param>
		public DatabaseParameterCollection(IList<DatabaseParameter> parameters) {
			if (null == parameters) {
				return;
			}

			for (Int32 i = 0; (i < parameters.Count); i++) {
				parameters.Add(parameters[i]);
			}
		}
		
		#endregion

		#region Timeout

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		public virtual Int32 Timeout {
			get {  return _timeout; }
			set { _timeout = value; }
		}

		#endregion

		#region Indexers

		/// <summary>
		/// Gets the <see cref="DatabaseParameter"/> with the specified name.
		/// </summary>
		/// <value>
		/// The <see cref="DatabaseParameter"/>.
		/// </value>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		/// <exception cref="KeyNotFoundException">There is no <see cref="DatabaseParameter"/> for the specified name.</exception>
		public virtual DatabaseParameter this[String name] {
			get {
				String parameterName = (name ?? "null");
				DatabaseParameter parameter;

				if (parameterName.StartsWith("@")) {
					parameterName = parameterName.Substring(1);
				}

				if (_parameters.TryGetValue(parameterName, out parameter)) {
					return parameter;
				}

				if (parameterName.Equals("RETURN_VALUE", StringComparison.OrdinalIgnoreCase)) {
					return (_parameters[name] = new DatabaseParameter(parameterName, DbType.Int32, null));
				}
				
				throw new KeyNotFoundException(String.Format("Failed to get the '{0}' instances for the name [{1}]", typeof(DatabaseParameter), parameterName));
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the count.
		/// </summary>
		/// <value>
		/// The count.
		/// </value>
		public virtual Int32 Count {
			get { return _parameters.Count; }
		}

		#endregion

		#region Add

		/// <summary>
		/// Adds a <see cref="DatabaseParameter"/>
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="DatabaseParameter"/></returns>
		public virtual DatabaseParameter Add(String name, DbType type, Object value) {
			return Add(new DatabaseParameter(name, type, value));
		}

		/// <summary>
		/// Adds a <see cref="DatabaseParameter" />
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="direction">The direction.</param>
		/// <returns>
		///   <see cref="DatabaseParameter" />
		/// </returns>
		public virtual DatabaseParameter Add(String name, DbType type, ParameterDirection direction) {
			return Add(new DatabaseParameter(name, type, null, direction));
		}

		/// <summary>
		/// Adds a <see cref="DatabaseParameter" />
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="value">The value.</param>
		/// <param name="direction">The direction.</param>
		/// <returns>
		///   <see cref="DatabaseParameter" />
		/// </returns>
		public virtual DatabaseParameter Add(String name, DbType type, Object value, ParameterDirection direction) {
			return Add(new DatabaseParameter(name, type, value, direction));
		}

		/// <summary>
		/// Adds the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="direction">The direction.</param>
		/// <param name="size">The size.</param>
		/// <returns></returns>
		public virtual DatabaseParameter Add(String name, DbType type, ParameterDirection direction, Int32 size) {
			return Add(new DatabaseParameter(name, type, null, direction, size));
		}

		/// <summary>
		/// Adds the specified bag.
		/// </summary>
		/// <param name="parameter">The bag.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		public DatabaseParameter Add(DatabaseParameter parameter) {
			if (_parameters.ContainsKey(parameter.Name)) {
				throw new InvalidOperationException(String.Format("Failed to add '{0}' instance.  An object with the same key [{1}] is already in this collection.", typeof(DatabaseParameter), parameter.Name));
			}

			return (_parameters[parameter.Name] = parameter);
		}

		#endregion

		#region String Representation

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override String ToString() {
			if (0 == _parameters.Count) {
				return "Count:=0";
			}

			StringBuilder sb = new StringBuilder();
			Int32 count = 0;

			foreach(DatabaseParameter parameter in this) {
				sb.AppendFormat("{0}: {1}", (++count).ToString("0000"), parameter);

				if (count < Count) {
					sb.AppendLine();
				}
			}

			return sb.ToString();
		}

		#endregion

		#region Enumeration

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<DatabaseParameter> GetEnumerator() {
			return _parameters.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		#endregion
	}
}
