using System; 
using System.Data; 
using System.Xml; 
using System.Data.SqlClient; 
using System.Collections; 


namespace Goro.Check.Data
{ 
	///  
	/// The SqlHelper class is intended to encapsulate high performance, scalable best practices for  
	/// common uses of SqlClient. 
	///  
	public sealed class SqlHelper 
	{
 

		#region private utility methods & constructors 
 
		//Since this class provides only static methods, make the default constructor private to prevent  
		//instances from being created with "new SqlHelper()". 
		private SqlHelper() {} 
 
		///  
		/// This method is used to attach array of SqlParameters to a SqlCommand. 
		///  
		/// This method will assign a value of DbNull to any parameter with a direction of 
		/// InputOutput and a value of null.   
		///  
		/// This behavior will prevent default values from being used, but 
		/// this will be the less common case than an intended pure output parameter (derived as InputOutput) 
		/// where the user provided no input value. 
		///  
		/// The command to which the parameters will be added 
		/// an array of SqlParameters tho be added to command 
		private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters) 
		{ 
			foreach (SqlParameter p in commandParameters) 
			{ 
				//check for derived output value with no value assigned 
				if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null)) 
				{ 
					p.Value = DBNull.Value; 
				} 
				 
				command.Parameters.Add(p); 
			} 
		} 
 
		///  
		/// This method assigns an array of values to an array of SqlParameters. 
		///  
		/// array of SqlParameters to be assigned values 
		/// array of objects holding the values to be assigned 
		private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues) 
		{ 
			if ((commandParameters == null) || (parameterValues == null))  
			{ 
				//do nothing if we get no data 
				return; 
			} 
 
			// we must have the same number of values as we pave parameters to put them in 
			if (commandParameters.Length != parameterValues.Length) 
			{ 
				throw new ArgumentException("Parameter count does not match Parameter Value count."); 
			} 
 
			//iterate through the SqlParameters, assigning the values from the corresponding position in the  
			//value array 
			for (int i = 0, j = commandParameters.Length; i < j; i++) 
			{ 
				commandParameters[i].Value = parameterValues[i]; 
			} 
		} 
 
		///  
		/// This method opens (if necessary) and assigns a connection, transaction, command type and parameters  
		/// to the provided command. 
		///  
		/// the SqlCommand to be prepared 
		/// a valid SqlConnection, on which to execute this command 
		/// a valid SqlTransaction, or 'null' 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParameters to be associated with the command or 'null' if no parameters are required 
		private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters) 
		{ 
			//if the provided connection is not open, we will open it 
			if (connection.State != ConnectionState.Open) 
			{ 
				connection.Open(); 
			} 
 
			//associate the connection with the command 
			command.Connection = connection; 
 
			//set the command text (stored procedure name or SQL statement) 
			command.CommandText = commandText; 
 
			//if we were provided a transaction, assign it. 
			if (transaction != null) 
			{ 
				command.Transaction = transaction; 
			} 
 
			//set the command type 
			command.CommandType = commandType; 
 
			//attach the command parameters if they are provided 
			if (commandParameters != null) 
			{ 
				AttachParameters(command, commandParameters); 
			} 
 
			return; 
		} 
 
		#endregion private utility methods & constructors 
 
		#region ExecuteNonQuery 
 
		///  
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the database specified in  
		/// the connection string.  
		///  
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders"); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null); 
		}

        public static int ExecuteNonQuery(CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteNonQuery(ConnectionConfig.CurrentConnection, commandType, commandText, (SqlParameter[])null);
        }
 
 
		///  
		/// Execute a SqlCommand (that returns no resultset) against the database specified in the connection string  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create & open a SqlConnection, and dispose of it after we are done. 
			using (SqlConnection cn = new SqlConnection(connectionString)) 
			{ 
				cn.Open(); 
 
				//call the overload that takes a connection in place of the connection string 
				return ExecuteNonQuery(cn, commandType, commandText, commandParameters); 
			} 
		}

        public static int ExecuteNonQuery(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create & open a SqlConnection, and dispose of it after we are done. 
            using (SqlConnection cn = new SqlConnection(ConnectionConfig.CurrentConnection))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string 
                return ExecuteNonQuery(cn, commandType, commandText, commandParameters);
            }
        } 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the database specified in  
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(connString, "PublishOrders", 24, 36); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored prcedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName); 
			} 
		}


        public static int ExecuteNonQuery( string spName, params object[] parameterValues)
        {
            return ExecuteNonQuery(ConnectionConfig.CurrentConnection, spName, parameterValues);
        } 
 
		///  
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection.  
		///  
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders"); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlConnection  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{	 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters); 
			 
			//finally, execute the command. 
			int retval = cmd.ExecuteNonQuery(); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			return retval; 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified SqlConnection  
		/// using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(conn, "PublishOrders", 24, 36); 
		///  
		/// a valid SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		///  
		/// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlTransaction.  
		///  
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "PublishOrders"); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns no resultset) against the specified SqlTransaction 
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters); 
			 
			//finally, execute the command. 
			int retval = cmd.ExecuteNonQuery(); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			return retval; 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns no resultset) against the specified  
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  int result = ExecuteNonQuery(conn, trans, "PublishOrders", 24, 36); 
		///  
		/// a valid SqlTransaction 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an int representing the number of rows affected by the command 
		public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName); 
			} 
		} 
 
 
		#endregion ExecuteNonQuery 
 
		#region ExecuteDataSet 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in  
		/// the connection string.  
		///  
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null); 
		}

        public static DataSet ExecuteDataset( CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteDataset(ConnectionConfig.CurrentConnection, commandType, commandText, (SqlParameter[])null);
        } 
 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create & open a SqlConnection, and dispose of it after we are done. 
			using (SqlConnection cn = new SqlConnection(connectionString)) 
			{ 
				cn.Open(); 
 
				//call the overload that takes a connection in place of the connection string 
				return ExecuteDataset(cn, commandType, commandText, commandParameters); 
			} 
		}
        public static DataSet ExecuteDataset( CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteDataset(ConnectionConfig.CurrentConnection, commandType, commandText, commandParameters);
        } 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in  
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(connString, "GetOrders", 24, 36); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName); 
			} 
		}

        public static DataSet ExecuteDataset( string spName, params object[] parameterValues)
        {
             return  ExecuteDataset(ConnectionConfig.CurrentConnection,  spName,  parameterValues);
        } 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection.  
		///  
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null); 
		} 
		 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters); 
			 
			//create the DataAdapter & DataSet 
			SqlDataAdapter da = new SqlDataAdapter(cmd); 
			DataSet ds = new DataSet(); 
 
			//fill the DataSet using default values for DataTable names, etc. 
			da.Fill(ds); 
			 
			// detach the SqlParameters from the command object, so they can be used again.			 
			cmd.Parameters.Clear(); 
			 
			//return the dataset 
			return ds;						 
		} 
		 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection  
		/// using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(conn, "GetOrders", 24, 36); 
		///  
		/// a valid SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteDataset(connection, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.  
		///  
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null); 
		} 
		 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction 
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters); 
			 
			//create the DataAdapter & DataSet 
			SqlDataAdapter da = new SqlDataAdapter(cmd); 
			DataSet ds = new DataSet(); 
 
			//fill the DataSet using default values for DataTable names, etc. 
			da.Fill(ds); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			 
			//return the dataset 
			return ds; 
		} 
		 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified  
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  DataSet ds = ExecuteDataset(trans, "GetOrders", 24, 36); 
		///  
		/// a valid SqlTransaction 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a dataset containing the resultset generated by the command 
		public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteDataset(transaction, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		#endregion ExecuteDataSet 

        #region ExecuteDataTable

        ///  
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in  
        /// the connection string.  
        ///  
        ///  
        /// e.g.:   
        ///  DataTable dt = ExecuteDataTable(connString, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// a valid connection string for a SqlConnection 
        /// the CommandType (stored procedure, text, etc.) 
        /// the stored procedure name or T-SQL command 
        /// a DataTable containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteDataTable(connectionString, commandType, commandText, (SqlParameter[])null);
        }


        public static DataTable ExecuteDataTable(CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteDataTable(ConnectionConfig.CurrentConnection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string  
        /// using the provided parameters. 
        ///  
        ///  
        /// e.g.:   
        ///  DataTable dt = ExecuteDataTable(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// a valid connection string for a SqlConnection 
        /// the CommandType (stored procedure, text, etc.) 
        /// the stored procedure name or T-SQL command 
        /// an array of SqlParamters used to execute the command 
        /// a DataTable containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create & open a SqlConnection, and dispose of it after we are done. 
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string 
                return ExecuteDataTable(cn, commandType, commandText, commandParameters);
            }
        }

        public static DataTable ExecuteDataTable(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create & open a SqlConnection, and dispose of it after we are done. 
            using (SqlConnection cn = new SqlConnection(ConnectionConfig.CurrentConnection))
            {
                cn.Open();

                //call the overload that takes a connection in place of the connection string 
                return ExecuteDataTable(cn, commandType, commandText, commandParameters);
            }
        }

        ///  
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in  
        /// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the  
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
        ///  
        ///  
        /// This method provides no access to output parameters or the stored procedure's return value parameter. 
        ///  
        /// e.g.:   
        ///  DataTable dt = ExecuteDataTable(connString, "GetOrders", 24, 36); 
        ///  
        /// a valid connection string for a SqlConnection 
        /// the name of the stored procedure 
        /// an array of objects to be assigned as the input values of the stored procedure 
        /// a DataTable containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(string connectionString, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                //assign the provided values to these parameters based on parameter order 
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters 
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params 
            else
            {
                return ExecuteDataTable(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static DataTable ExecuteDataTable( string spName, params object[] parameterValues)
        {
             return  ExecuteDataTable(ConnectionConfig.CurrentConnection,  spName,   parameterValues);
        }

        ///  
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection.  
        ///  
        ///  
        /// e.g.:   
        ///  DataTable dt = ExecuteDataTable(conn, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// a valid SqlConnection 
        /// the CommandType (stored procedure, text, etc.) 
        /// the stored procedure name or T-SQL command 
        /// a DataTable containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteDataTable(connection, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection  
        /// using the provided parameters. 
        ///  
        ///  
        /// e.g.:   
        ///  DataTable dt = ExecuteDataTable(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// a valid SqlConnection 
        /// the CommandType (stored procedure, text, etc.) 
        /// the stored procedure name or T-SQL command 
        /// an array of SqlParamters used to execute the command 
        /// a DataTable containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution 
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet 
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //fill the DataSet using default values for DataTable names, etc. 
            da.Fill(dt);

            // detach the SqlParameters from the command object, so they can be used again.			 
            cmd.Parameters.Clear();

            //return the dataset 
            return dt;
        }

        ///  
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection  
        /// using the provided parameter values.  This method will query the database to discover the parameters for the  
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
        ///  
        ///  
        /// This method provides no access to output parameters or the stored procedure's return value parameter. 
        ///  
        /// e.g.:   
        ///  DataTable ds = ExecuteDataTable(conn, "GetOrders", 24, 36); 
        ///  
        /// a valid SqlConnection 
        /// the name of the stored procedure 
        /// an array of objects to be assigned as the input values of the stored procedure 
        /// a dataset containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(SqlConnection connection, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order 
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters 
                return ExecuteDataTable(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params 
            else
            {
                return ExecuteDataTable(connection, CommandType.StoredProcedure, spName);
            }
        }

        ///  
        /// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.  
        ///  
        ///  
        /// e.g.:   
        ///  DataTable ds = ExecuteDataTable(trans, CommandType.StoredProcedure, "GetOrders"); 
        ///  
        /// a valid SqlTransaction 
        /// the CommandType (stored procedure, text, etc.) 
        /// the stored procedure name or T-SQL command 
        /// a dataset containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteDataTable(transaction, commandType, commandText, (SqlParameter[])null);
        }

        ///  
        /// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction 
        /// using the provided parameters. 
        ///  
        ///  
        /// e.g.:   
        ///  DataTable ds = ExecuteDataTable(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
        ///  
        /// a valid SqlTransaction 
        /// the CommandType (stored procedure, text, etc.) 
        /// the stored procedure name or T-SQL command 
        /// an array of SqlParamters used to execute the command 
        /// a dataset containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            //create a command and prepare it for execution 
            SqlCommand cmd = new SqlCommand();
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters);

            //create the DataAdapter & DataSet 
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            //fill the DataSet using default values for DataTable names, etc. 
            da.Fill(dt);

            // detach the SqlParameters from the command object, so they can be used again. 
            cmd.Parameters.Clear();

            //return the dataset 
            return dt;
        }

        ///  
        /// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified  
        /// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the  
        /// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
        ///  
        ///  
        /// This method provides no access to output parameters or the stored procedure's return value parameter. 
        ///  
        /// e.g.:   
        ///  DataTable ds = ExecuteDataTable(trans, "GetOrders", 24, 36); 
        ///  
        /// a valid SqlTransaction 
        /// the name of the stored procedure 
        /// an array of objects to be assigned as the input values of the stored procedure 
        /// a dataset containing the resultset generated by the command 
        public static DataTable ExecuteDataTable(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            //if we receive parameter values, we need to figure out where they go 
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                //pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);

                //assign the provided values to these parameters based on parameter order 
                AssignParameterValues(commandParameters, parameterValues);

                //call the overload that takes an array of SqlParameters 
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            //otherwise we can just call the SP without params 
            else
            {
                return ExecuteDataTable(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion
		 
		#region ExecuteReader 
 
		///  
		/// this enum is used to indicate whether the connection was provided by the caller, or created by SqlHelper, so that 
		/// we can set the appropriate CommandBehavior when calling ExecuteReader() 
		///  
		private enum SqlConnectionOwnership	 
		{ 
			/// Connection is owned and managed by SqlHelper 
			Internal,  
			/// Connection is owned and managed by the caller 
			External 
		} 
 
		///  
		/// Create and prepare a SqlCommand, and call ExecuteReader with the appropriate CommandBehavior. 
		///  
		///  
		/// If we created and opened the connection, we want the connection to be closed when the DataReader is closed. 
		///  
		/// If the caller provided the connection, we want to leave it to them to manage. 
		///  
		/// a valid SqlConnection, on which to execute this command 
		/// a valid SqlTransaction, or 'null' 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParameters to be associated with the command or 'null' if no parameters are required 
		/// indicates whether the connection parameter was provided by the caller, or created by SqlHelper 
		/// SqlDataReader containing the results of the command 
		private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership) 
		{	 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters); 
			 
			//create a reader 
			SqlDataReader dr; 
 
			// call ExecuteReader with the appropriate CommandBehavior 
			if (connectionOwnership == SqlConnectionOwnership.External) 
			{ 
				dr = cmd.ExecuteReader(); 
			} 
			else 
			{ 
				dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); 
			} 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			 
			return dr; 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the database specified in  
		/// the connection string.  
		///  
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null); 
		}
        public static SqlDataReader ExecuteReader( CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteReader(ConnectionConfig.CurrentConnection, commandType, commandText, (SqlParameter[])null);
        } 
 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the database specified in the connection string  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(connString, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create & open a SqlConnection 
			SqlConnection cn = new SqlConnection(connectionString); 
			cn.Open(); 
 
			try 
			{ 
				//call the private overload that takes an internally owned connection in place of the connection string 
				return ExecuteReader(cn, null, commandType, commandText, commandParameters,SqlConnectionOwnership.Internal); 
			} 
			catch 
			{ 
				//if we fail to return the SqlDatReader, we need to close the connection ourselves 
				cn.Close(); 
				throw; 
			} 
		}

        public static SqlDataReader ExecuteReader( CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
             return ExecuteReader(ConnectionConfig.CurrentConnection,   commandType,   commandText,   commandParameters);
        } 
 
		public static SqlDataReader MyExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create & open a SqlConnection 
			SqlConnection cn = new SqlConnection(connectionString); 
			cn.Open(); 
 
			try 
			{ 
				//call the private overload that takes an internally owned connection in place of the connection string 
				SqlCommand cmd = new SqlCommand(); 
				PrepareCommand(cmd, cn, null, commandType, commandText, commandParameters); 
			 
				//create a reader 
				SqlDataReader dr; 
 
				// call ExecuteReader with the appropriate CommandBehavior 
					dr = cmd.ExecuteReader(CommandBehavior.CloseConnection); 
			 
				// detach the SqlParameters from the command object, so they can be used again. 
				//cmd.Parameters.Clear(); 
			 
				return dr; 
 
			} 
			catch 
			{ 
				//if we fail to return the SqlDatReader, we need to close the connection ourselves 
				cn.Close(); 
				throw; 
			} 
		}

        public static SqlDataReader MyExecuteReader(  CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return  MyExecuteReader(ConnectionConfig.CurrentConnection,   commandType,   commandText,  commandParameters);
        }
 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the database specified in  
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(connString, "GetOrders", 24, 36); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteReader(connectionString, CommandType.StoredProcedure, spName); 
			} 
		}
        public static SqlDataReader ExecuteReader( string spName, params object[] parameterValues)
        {
            return  ExecuteReader(ConnectionConfig.CurrentConnection,  spName, parameterValues);
        }
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection.  
		///  
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//pass through the call to the private overload using a null transaction value and an externally owned connection 
			return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External); 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection  
		/// using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(conn, "GetOrders", 24, 36); 
		///  
		/// a valid SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName); 
 
				AssignParameterValues(commandParameters, parameterValues); 
 
				return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters); 
			} 
			//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteReader(connection, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.  
		///  
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction 
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///   SqlDataReader dr = ExecuteReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//pass through to private overload, indicating that the connection is owned by the caller 
			return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External); 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  SqlDataReader dr = ExecuteReader(trans, "GetOrders", 24, 36); 
		///  
		/// a valid SqlTransaction 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a SqlDataReader containing the resultset generated by the command 
		public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName); 
 
				AssignParameterValues(commandParameters, parameterValues); 
 
				return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteReader(transaction, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		#endregion ExecuteReader 
 
		#region ExecuteScalar 
		 
		///  
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the database specified in  
		/// the connection string.  
		///  
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount"); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null); 
		}
        public static object ExecuteScalar( CommandType commandType, string commandText)
        {
            //pass through the call providing null for the set of SqlParameters 
            return ExecuteScalar(ConnectionConfig.CurrentConnection, commandType, commandText, (SqlParameter[])null);
        } 
 
		///  
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the database specified in the connection string  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(connString, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create & open a SqlConnection, and dispose of it after we are done. 
			using (SqlConnection cn = new SqlConnection(connectionString)) 
			{ 
				cn.Open(); 
 
				//call the overload that takes a connection in place of the connection string 
				return ExecuteScalar(cn, commandType, commandText, commandParameters); 
			} 
		}

        public static object ExecuteScalar( CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            return ExecuteScalar(ConnectionConfig.CurrentConnection, commandType, commandText, commandParameters);
        } 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the database specified in  
		/// the connection string using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(connString, "GetOrderCount", 24, 36); 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName); 
			} 
		}

        public static object ExecuteScalar(string spName, params object[] parameterValues)
        {
            return ExecuteScalar(ConnectionConfig.CurrentConnection, spName, parameterValues);
        } 
 
		///  
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlConnection.  
		///  
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount"); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(conn, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters); 
			 
			//execute the command & return the results 
			object retval = cmd.ExecuteScalar(); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			return retval; 
			 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified SqlConnection  
		/// using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(conn, "GetOrderCount", 24, 36); 
		///  
		/// a valid SqlConnection 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteScalar(connection, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a 1x1 resultset and takes no parameters) against the provided SqlTransaction.  
		///  
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount"); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a 1x1 resultset) against the specified SqlTransaction 
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(trans, CommandType.StoredProcedure, "GetOrderCount", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters used to execute the command 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters); 
			 
			//execute the command & return the results 
			object retval = cmd.ExecuteScalar(); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			return retval; 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a 1x1 resultset) against the specified 
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  int orderCount = (int)ExecuteScalar(trans, "GetOrderCount", 24, 36); 
		///  
		/// a valid SqlTransaction 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an object containing the value in the 1x1 resultset generated by the command 
		public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteScalar(transaction, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		#endregion ExecuteScalar	 
 
		#region ExecuteXmlReader 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlConnection.  
		///  
		///  
		/// e.g.:   
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command using "FOR XML AUTO" 
		/// an XmlReader containing the resultset generated by the command 
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlConnection  
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  XmlReader r = ExecuteXmlReader(conn, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlConnection 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command using "FOR XML AUTO" 
		/// an array of SqlParamters used to execute the command 
		/// an XmlReader containing the resultset generated by the command 
		public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters); 
			 
			//create the DataAdapter & DataSet 
			XmlReader retval = cmd.ExecuteXmlReader(); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			return retval; 
			 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified SqlConnection  
		/// using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  XmlReader r = ExecuteXmlReader(conn, "GetOrders", 24, 36); 
		///  
		/// a valid SqlConnection 
		/// the name of the stored procedure using "FOR XML AUTO" 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// an XmlReader containing the resultset generated by the command 
		public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName); 
			} 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset and takes no parameters) against the provided SqlTransaction.  
		///  
		///  
		/// e.g.:   
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders"); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command using "FOR XML AUTO" 
		/// an XmlReader containing the resultset generated by the command 
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText) 
		{ 
			//pass through the call providing null for the set of SqlParameters 
			return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null); 
		} 
 
		///  
		/// Execute a SqlCommand (that returns a resultset) against the specified SqlTransaction 
		/// using the provided parameters. 
		///  
		///  
		/// e.g.:   
		///  XmlReader r = ExecuteXmlReader(trans, CommandType.StoredProcedure, "GetOrders", new SqlParameter("@prodid", 24)); 
		///  
		/// a valid SqlTransaction 
		/// the CommandType (stored procedure, text, etc.) 
		/// the stored procedure name or T-SQL command using "FOR XML AUTO" 
		/// an array of SqlParamters used to execute the command 
		/// an XmlReader containing the resultset generated by the command 
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters) 
		{ 
			//create a command and prepare it for execution 
			SqlCommand cmd = new SqlCommand(); 
			PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters); 
			 
			//create the DataAdapter & DataSet 
			XmlReader retval = cmd.ExecuteXmlReader(); 
			 
			// detach the SqlParameters from the command object, so they can be used again. 
			cmd.Parameters.Clear(); 
			return retval;			 
		} 
 
		///  
		/// Execute a stored procedure via a SqlCommand (that returns a resultset) against the specified  
		/// SqlTransaction using the provided parameter values.  This method will query the database to discover the parameters for the  
		/// stored procedure (the first time each stored procedure is called), and assign the values based on parameter order. 
		///  
		///  
		/// This method provides no access to output parameters or the stored procedure's return value parameter. 
		///  
		/// e.g.:   
		///  XmlReader r = ExecuteXmlReader(trans, "GetOrders", 24, 36); 
		///  
		/// a valid SqlTransaction 
		/// the name of the stored procedure 
		/// an array of objects to be assigned as the input values of the stored procedure 
		/// a dataset containing the resultset generated by the command 
		public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues) 
		{ 
			//if we receive parameter values, we need to figure out where they go 
			if ((parameterValues != null) && (parameterValues.Length > 0))  
			{ 
				//pull the parameters for this stored procedure from the parameter cache (or discover them & populate the cache) 
				SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName); 
 
				//assign the provided values to these parameters based on parameter order 
				AssignParameterValues(commandParameters, parameterValues); 
 
				//call the overload that takes an array of SqlParameters 
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, commandParameters); 
			} 
				//otherwise we can just call the SP without params 
			else  
			{ 
				return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName); 
			} 
		} 
 
 
		#endregion ExecuteXmlReader 
	} 
 
	///  
	/// SqlHelperParameterCache provides functions to leverage a static cache of procedure parameters, and the 
	/// ability to discover parameters for stored procedures at run-time. 
	///  
	public sealed class SqlHelperParameterCache 
	{ 
		#region private methods, variables, and constructors 
 
		//Since this class provides only static methods, make the default constructor private to prevent  
		//instances from being created with "new SqlHelperParameterCache()". 
		private SqlHelperParameterCache() {} 
 
		private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable()); 
 
		///  
		/// resolve at run time the appropriate set of SqlParameters for a stored procedure 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored procedure 
		/// whether or not to include their return value parameter 
		///  
		private static SqlParameter[] DiscoverSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter) 
		{ 
			using (SqlConnection cn = new SqlConnection(connectionString))  
			using (SqlCommand cmd = new SqlCommand(spName,cn)) 
			{ 
				cn.Open(); 
				cmd.CommandType = CommandType.StoredProcedure; 
 
				SqlCommandBuilder.DeriveParameters(cmd); 
 
				if (!includeReturnValueParameter)  
				{ 
					cmd.Parameters.RemoveAt(0); 
				} 
 
				SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];; 
 
				cmd.Parameters.CopyTo(discoveredParameters, 0); 
 
				return discoveredParameters; 
			} 
		} 
 
		//deep copy of cached SqlParameter array 
		private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters) 
		{ 
			SqlParameter[] clonedParameters = new SqlParameter[originalParameters.Length]; 
 
			for (int i = 0, j = originalParameters.Length; i < j; i++) 
			{ 
				clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone(); 
			} 
 
			return clonedParameters; 
		} 
 
		#endregion private methods, variables, and constructors 
 
		#region caching functions 
 
		///  
		/// add parameter array to the cache 
		///  
		/// a valid connection string for a SqlConnection 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters to be cached 
		public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters) 
		{ 
			string hashKey = connectionString + ":" + commandText; 
 
			paramCache[hashKey] = commandParameters; 
		} 
 
		///  
		/// retrieve a parameter array from the cache 
		///  
		/// a valid connection string for a SqlConnection 
		/// the stored procedure name or T-SQL command 
		/// an array of SqlParamters 
		public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText) 
		{ 
			string hashKey = connectionString + ":" + commandText; 
 
			SqlParameter[] cachedParameters = (SqlParameter[])paramCache[hashKey]; 
			 
			if (cachedParameters == null) 
			{			 
				return null; 
			} 
			else 
			{ 
				return CloneParameters(cachedParameters); 
			} 
		} 
 
		#endregion caching functions 
 
		#region Parameter Discovery Functions 
 
		///  
		/// Retrieves the set of SqlParameters appropriate for the stored procedure 
		///  
		///  
		/// This method will query the database for this information, and then store it in a cache for future requests. 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored procedure 
		/// an array of SqlParameters 
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName) 
		{ 
			return GetSpParameterSet(connectionString, spName, false); 
		} 
 
		///  
		/// Retrieves the set of SqlParameters appropriate for the stored procedure 
		///  
		///  
		/// This method will query the database for this information, and then store it in a cache for future requests. 
		///  
		/// a valid connection string for a SqlConnection 
		/// the name of the stored procedure 
		/// a bool value indicating whether the return value parameter should be included in the results 
		/// an array of SqlParameters 
		public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter) 
		{ 
			string hashKey = connectionString + ":" + spName + (includeReturnValueParameter ? ":include ReturnValue Parameter":""); 
 
			SqlParameter[] cachedParameters; 
			 
			cachedParameters = (SqlParameter[])paramCache[hashKey]; 
 
			if (cachedParameters == null) 
			{			 
				cachedParameters = (SqlParameter[])(paramCache[hashKey] = DiscoverSpParameterSet(connectionString, spName, includeReturnValueParameter)); 
			} 
			 
			return CloneParameters(cachedParameters); 
		} 
 
		#endregion Parameter Discovery Functions 
 
	} 
} 