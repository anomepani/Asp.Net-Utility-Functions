using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace DBAccess
{
    public class DBHelper
    {
       /* Reference link: http://www.codeproject.com/Articles/23853/DbHelper-for-DotNet-using-DbProviderFactory
        * Author Name: Shyam SS
        */
        #region DECLARATIONS

        private DbProviderFactory oFactory;
        private DbConnection oConnection;
        private ConnectionState oConnectionState;
        public DbCommand oCommand;
        private DbParameter oParameter;
        private DbTransaction oTransaction;
        private bool mblTransaction;

        private static readonly string S_CONNECTION = string.Empty;
        private static readonly string S_PROVIDER = string.Empty;

        #endregion

        #region ENUMERATORS

        public enum TransactionType : uint
        {
            Open = 1,
            Commit = 2,
            Rollback = 3
        }

        #endregion

        #region STRUCTURES

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS 
        ///Date				:	28 June 2007
        ///Input			:	
        ///OutPut			:	
        ///Comments			:	
        /// </summary>
        public struct Parameters
        {
            public string ParamName;
            public object ParamValue;
            public ParameterDirection ParamDirection;

            public Parameters(string Name, object Value, ParameterDirection Direction)
            {
                ParamName = Name;
                ParamValue = Value;
                ParamDirection = Direction;
            }

            public Parameters(string Name, object Value)
            {
                ParamName = Name;
                ParamValue = Value;
                ParamDirection = ParameterDirection.Input;
            }
        }

        #endregion

        #region CONSTRUCTOR

        public DBHelper()
        {
            oFactory = DbProviderFactories.GetFactory(S_PROVIDER);
        }

        #endregion

        #region DESTRUCTOR

        ~DBHelper()
        {
            oFactory = null;
        }

        #endregion

        #region CONNECTIONS

        /// <summary>
        ///Description	    :	This function is used to Open Database Connection
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	NA
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        public void EstablishFactoryConnection()
        {
            /*
            // This check is not required as it will throw "Invalid Provider Exception" on the contructor itself.
            if (0 == DbProviderFactories.GetFactoryClasses().Select("InvariantName='" + S_PROVIDER + "'").Length)
                throw new Exception("Invalid Provider");
            */
            oConnection = oFactory.CreateConnection();

            if (oConnection.State == ConnectionState.Closed)
            {
                oConnection.ConnectionString = S_CONNECTION;
                oConnection.Open();
                oConnectionState = ConnectionState.Open;
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Close Database Connection
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	NA
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        public void CloseFactoryConnection()
        {
            //check for an open connection            
            try
            {
                if (oConnection.State == ConnectionState.Open)
                {
                    oConnection.Close();
                    oConnectionState = ConnectionState.Closed;
                }
            }
            catch (DbException oDbErr)
            {
                //catch any SQL server data provider generated error messag
                throw new Exception(oDbErr.Message);
            }
            catch (System.NullReferenceException oNullErr)
            {
                throw new Exception(oNullErr.Message);
            }
            finally
            {
                if (null != oConnection)
                    oConnection.Dispose();
            }
        }

        #endregion

        #region TRANSACTION

        /// <summary>
        ///Description	    :	This function is used to Handle Transaction Events
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction Event Type
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        public void TransactionHandler(TransactionType veTransactionType)
        {
            switch (veTransactionType)
            {
                case TransactionType.Open:  //open a transaction
                    try
                    {
                        oTransaction = oConnection.BeginTransaction();
                        mblTransaction = true;
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw new Exception("@TransactionHandler - " + oErr.Message);
                    }
                    break;

                case TransactionType.Commit:  //commit the transaction
                    if (null != oTransaction.Connection)
                    {
                        try
                        {
                            oTransaction.Commit();
                            mblTransaction = false;
                        }
                        catch (InvalidOperationException oErr)
                        {
                            throw new Exception("@TransactionHandler - " + oErr.Message);
                        }
                    }
                    break;

                case TransactionType.Rollback:  //rollback the transaction
                    try
                    {
                        if (mblTransaction)
                        {
                            oTransaction.Rollback();
                        }
                        mblTransaction = false;
                    }
                    catch (InvalidOperationException oErr)
                    {
                        throw new Exception("@TransactionHandler - " + oErr.Message);
                    }
                    break;
            }

        }

        #endregion

        #region COMMANDS

        #region PARAMETERLESS METHODS

        /// <summary>
        ///Description	    :	This function is used to Prepare Command For Execution
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	NA
        ///Comments			:	Has to be changed/removed if object based array concept is removed.
        /// </summary>
        private void PrepareCommand(bool blTransaction, CommandType cmdType, string cmdText)
        {

            if (oConnection.State != ConnectionState.Open)
            {
                oConnection.ConnectionString = S_CONNECTION;
                oConnection.Open();
                oConnectionState = ConnectionState.Open;
            }

            if (null == oCommand)
                oCommand = oFactory.CreateCommand();

            oCommand.Connection = oConnection;
            oCommand.CommandText = cmdText;
            oCommand.CommandType = cmdType;

            if (blTransaction)
                oCommand.Transaction = oTransaction;
        }

        #endregion

        #region OBJECT BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to Prepare Command For Execution
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        private void PrepareCommand(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms)
        {

            if (oConnection.State != ConnectionState.Open)
            {
                oConnection.ConnectionString = S_CONNECTION;
                oConnection.Open();
                oConnectionState = ConnectionState.Open;
            }

            if (null == oCommand)
                oCommand = oFactory.CreateCommand();

            oCommand.Connection = oConnection;
            oCommand.CommandText = cmdText;
            oCommand.CommandType = cmdType;

            if (blTransaction)
                oCommand.Transaction = oTransaction;

            if (null != cmdParms)
                CreateDBParameters(cmdParms);
        }

        #endregion

        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to Prepare Command For Execution
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        private void PrepareCommand(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {

            if (oConnection.State != ConnectionState.Open)
            {
                oConnection.ConnectionString = S_CONNECTION;
                oConnection.Open();
                oConnectionState = ConnectionState.Open;
            }

            oCommand = oFactory.CreateCommand();
            oCommand.Connection = oConnection;
            oCommand.CommandText = cmdText;
            oCommand.CommandType = cmdType;

            if (blTransaction)
                oCommand.Transaction = oTransaction;

            if (null != cmdParms)
                CreateDBParameters(cmdParms);
        }

        #endregion

        #endregion

        #region PARAMETER METHODS

        #region OBJECT BASED

        /// <summary>
        ///Description	    :	This function is used to Create Parameters for the Command For Execution
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	2-Dimensional Parameter Array
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        private void CreateDBParameters(object[,] colParameters)
        {
            for (int i = 0; i < colParameters.Length / 2; i++)
            {
                oParameter = oCommand.CreateParameter();
                oParameter.ParameterName = colParameters[i, 0].ToString();
                oParameter.Value = colParameters[i, 1];
                oCommand.Parameters.Add(oParameter);
            }
        }

        #endregion

        #region STRUCTURE BASED

        /// <summary>
        ///Description	    :	This function is used to Create Parameters for the Command For Execution
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	2-Dimensional Parameter Array
        ///OutPut			:	NA
        ///Comments			:	
        /// </summary>
        private void CreateDBParameters(Parameters[] colParameters)
        {
            for (int i = 0; i < colParameters.Length; i++)
            {
                Parameters oParam = (Parameters)colParameters[i];

                oParameter = oCommand.CreateParameter();
                oParameter.ParameterName = oParam.ParamName;
                oParameter.Value = oParam.ParamValue;
                oParameter.Direction = oParam.ParamDirection;
                oCommand.Parameters.Add(oParameter);

            }
        }

        #endregion

        #endregion

        #region EXCEUTE METHODS

        #region PARAMETERLESS METHODS

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        ///                     Has to be changed/removed if object based array concept is removed.
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText)
        {
            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText);
                return oCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array, Clear Paramaeters
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        ///                     Has to be changed/removed if object based array concept is removed.
        /// </summary>
        public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText)
        {
            try
            {
                PrepareCommand(blTransaction, cmdType, cmdText);
                int val = oCommand.ExecuteNonQuery();

                return val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
            }
        }

        #endregion

        #region OBJECT BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array, Clear Parameters
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
        {
            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Count of Records Affected
        ///Comments			:	Overloaded method. 
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, object[,] cmdParms)
        {
            return ExecuteNonQuery(cmdType, cmdText, cmdParms, true);
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array, Clear Paramaeters
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        /// </summary>
        public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
        {
            try
            {

                PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Count of Records Affected
        ///Comments			:	Overloaded function. 
        /// </summary>
        public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms)
        {
            return ExecuteNonQuery(blTransaction, cmdType, cmdText, cmdParms, true);
        }

        #endregion

        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, Parameter Structure Array, Clear Parameters
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, Parameter Structure Array
        ///OutPut			:	Count of Records Affected
        ///Comments			:	Overloaded method. 
        /// </summary>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {
            return ExecuteNonQuery(cmdType, cmdText, cmdParms, true);
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, Parameter Structure Array, Clear Parameters
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        /// </summary>
        public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {

                PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to Execute the Command
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Transaction, Command Type, Command Text, Parameter Structure Array
        ///OutPut			:	Count of Records Affected
        ///Comments			:	
        /// </summary>
        public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {
            return ExecuteNonQuery(blTransaction, cmdType, cmdText, cmdParms, true);
        }

        #endregion

        #endregion

        #region READER METHODS

        #region PARAMETERLESS METHODS

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Reader	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Data Reader
        ///Comments			:	
        ///                     Has to be changed/removed if object based array concept is removed.
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText);
                DbDataReader dr = oCommand.ExecuteReader(CommandBehavior.CloseConnection);
                oCommand.Parameters.Clear();
                return dr;

            }
            catch (Exception ex)
            {
                CloseFactoryConnection();
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
            }
        }

        #endregion

        #region OBJECT BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Reader	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Data Reader
        ///Comments			:	
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, object[,] cmdParms)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work

            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText, cmdParms);
                DbDataReader dr = oCommand.ExecuteReader(CommandBehavior.CloseConnection);
                oCommand.Parameters.Clear();
                return dr;

            }
            catch (Exception ex)
            {
                CloseFactoryConnection();
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
            }
        }

        #endregion

        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Reader	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, Parameter AStructure Array
        ///OutPut			:	Data Reader
        ///Comments			:	
        /// </summary>
        public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteReader(CommandBehavior.CloseConnection);

            }
            catch (Exception ex)
            {
                CloseFactoryConnection();
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
            }
        }

        #endregion

        #endregion

        #region ADAPTER METHODS

        #region PARAMETERLESS METHODS

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Adapter	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Data Set
        ///Comments			:	
        ///                     Has to be changed/removed if object based array concept is removed.
        /// </summary>
        public DataSet DataAdapter(CommandType cmdType, string cmdText)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            DbDataAdapter dda = null;
            try
            {
                EstablishFactoryConnection();
                dda = oFactory.CreateDataAdapter();
                PrepareCommand(false, cmdType, cmdText);

                dda.SelectCommand = oCommand;
                DataSet ds = new DataSet();
                dda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        #endregion

        #region OBJECT BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Adapter	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Data Set
        ///Comments			:	
        /// </summary>
        public DataSet DataAdapter(CommandType cmdType, string cmdText, object[,] cmdParms)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            DbDataAdapter dda = null;
            try
            {
                EstablishFactoryConnection();
                dda = oFactory.CreateDataAdapter();
                PrepareCommand(false, cmdType, cmdText, cmdParms);

                dda.SelectCommand = oCommand;
                DataSet ds = new DataSet();
                dda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        #endregion

        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to fetch data using Data Adapter	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Data Set
        ///Comments			:	
        /// </summary>
        public DataSet DataAdapter(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            DbDataAdapter dda = null;
            try
            {
                EstablishFactoryConnection();
                dda = oFactory.CreateDataAdapter();
                PrepareCommand(false, cmdType, cmdText, cmdParms);

                dda.SelectCommand = oCommand;
                DataSet ds = new DataSet();
                dda.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        #endregion

        #endregion

        #region SCALAR METHODS

        #region PARAMETERLESS METHODS

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText)
        {
            try
            {
                EstablishFactoryConnection();

                PrepareCommand(false, cmdType, cmdText);

                object val = oCommand.ExecuteScalar();

                return val;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        #endregion

        #region OBJECT BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
        {
            try
            {

                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	Overloaded Method. 
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, object[,] cmdParms)
        {
            return ExecuteScalar(cmdType, cmdText, cmdParms, true);
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
        {
            try
            {

                PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms)
        {
            return ExecuteScalar(blTransaction, cmdType, cmdText, cmdParms, true);
        }

        #endregion

        #region STRUCTURE BASED PARAMETER ARRAY

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {
                EstablishFactoryConnection();
                PrepareCommand(false, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
                CloseFactoryConnection();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	Overloaded Method. 
        /// </summary>
        public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {
            return ExecuteScalar(cmdType, cmdText, cmdParms, true);
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
        {
            try
            {

                PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
                return oCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (blDisposeCommand && null != oCommand)
                    oCommand.Dispose();
            }
        }

        /// <summary>
        ///Description	    :	This function is used to invoke Execute Scalar Method	
        ///Author			:	Shyam SS
        ///Date				:	28 June 2007
        ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
        ///OutPut			:	Object
        ///Comments			:	
        /// </summary>
        public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
        {
            return ExecuteScalar(blTransaction, cmdType, cmdText, cmdParms, true);
        }

        #endregion

        #endregion

    }

}
