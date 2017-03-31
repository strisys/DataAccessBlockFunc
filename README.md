# Data Access Block Func

----------

Simple wrapper for [Microsoft Patterns & Practices Enterprise Library Data Access Block](https://www.nuget.org/packages/EnterpriseLibrary.Data/) using a functional programming model.

To use, extend DatabaseCommandService, override abstract members, and then issue commands.  An instantiated parameter collection is passed to a provided function which, optionally, can be filled with values as follows.

    

    IDatabaseCommandService cs = new MyDatabaseCommandService();

	DataTable table = cs.ExecuteSpDataTable("SpGetCustomer", (parameters) => {
    	parameters.Add("CustomerID", DbType.Int32, 1);
   	});