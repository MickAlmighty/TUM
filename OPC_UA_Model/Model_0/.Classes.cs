/* ========================================================================
 * Copyright (c) 2005-2016 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace 
{
    #region ObjectType Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypes
    {
        /// <summary>
        /// The identifier for the Client ObjectType.
        /// </summary>
        public const uint Client = 1;

        /// <summary>
        /// The identifier for the Product ObjectType.
        /// </summary>
        public const uint Product = 8;

        /// <summary>
        /// The identifier for the Order ObjectType.
        /// </summary>
        public const uint Order = 13;
    }
    #endregion

    #region Variable Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class Variables
    {
        /// <summary>
        /// The identifier for the Client_Username Variable.
        /// </summary>
        public const uint Client_Username = 2;

        /// <summary>
        /// The identifier for the Client_FirstName Variable.
        /// </summary>
        public const uint Client_FirstName = 3;

        /// <summary>
        /// The identifier for the Client_LastName Variable.
        /// </summary>
        public const uint Client_LastName = 4;

        /// <summary>
        /// The identifier for the Client_Street Variable.
        /// </summary>
        public const uint Client_Street = 5;

        /// <summary>
        /// The identifier for the Client_StreetNumber Variable.
        /// </summary>
        public const uint Client_StreetNumber = 6;

        /// <summary>
        /// The identifier for the Client_PhoneNumber Variable.
        /// </summary>
        public const uint Client_PhoneNumber = 7;

        /// <summary>
        /// The identifier for the Product_Id Variable.
        /// </summary>
        public const uint Product_Id = 9;

        /// <summary>
        /// The identifier for the Product_Name Variable.
        /// </summary>
        public const uint Product_Name = 10;

        /// <summary>
        /// The identifier for the Product_Price Variable.
        /// </summary>
        public const uint Product_Price = 11;

        /// <summary>
        /// The identifier for the Product_ProductType Variable.
        /// </summary>
        public const uint Product_ProductType = 12;

        /// <summary>
        /// The identifier for the Order_Id Variable.
        /// </summary>
        public const uint Order_Id = 14;

        /// <summary>
        /// The identifier for the Order_ClientUsername Variable.
        /// </summary>
        public const uint Order_ClientUsername = 15;

        /// <summary>
        /// The identifier for the Order_OrderDate Variable.
        /// </summary>
        public const uint Order_OrderDate = 16;

        /// <summary>
        /// The identifier for the Order_ProductIdQuantityMap Variable.
        /// </summary>
        public const uint Order_ProductIdQuantityMap = 17;

        /// <summary>
        /// The identifier for the Order_Price Variable.
        /// </summary>
        public const uint Order_Price = 18;

        /// <summary>
        /// The identifier for the Order_DeliveryDate Variable.
        /// </summary>
        public const uint Order_DeliveryDate = 19;
    }
    #endregion

    #region ObjectType Node Identifiers
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class ObjectTypeIds
    {
        /// <summary>
        /// The identifier for the Client ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId Client = new ExpandedNodeId(.ObjectTypes.Client, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Product ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId Product = new ExpandedNodeId(.ObjectTypes.Product, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId Order = new ExpandedNodeId(.ObjectTypes.Order, .Namespaces.cas);
    }
    #endregion

    #region Variable Node Identifiers
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public static partial class VariableIds
    {
        /// <summary>
        /// The identifier for the Client_Username Variable.
        /// </summary>
        public static readonly ExpandedNodeId Client_Username = new ExpandedNodeId(.Variables.Client_Username, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Client_FirstName Variable.
        /// </summary>
        public static readonly ExpandedNodeId Client_FirstName = new ExpandedNodeId(.Variables.Client_FirstName, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Client_LastName Variable.
        /// </summary>
        public static readonly ExpandedNodeId Client_LastName = new ExpandedNodeId(.Variables.Client_LastName, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Client_Street Variable.
        /// </summary>
        public static readonly ExpandedNodeId Client_Street = new ExpandedNodeId(.Variables.Client_Street, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Client_StreetNumber Variable.
        /// </summary>
        public static readonly ExpandedNodeId Client_StreetNumber = new ExpandedNodeId(.Variables.Client_StreetNumber, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Client_PhoneNumber Variable.
        /// </summary>
        public static readonly ExpandedNodeId Client_PhoneNumber = new ExpandedNodeId(.Variables.Client_PhoneNumber, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Product_Id Variable.
        /// </summary>
        public static readonly ExpandedNodeId Product_Id = new ExpandedNodeId(.Variables.Product_Id, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Product_Name Variable.
        /// </summary>
        public static readonly ExpandedNodeId Product_Name = new ExpandedNodeId(.Variables.Product_Name, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Product_Price Variable.
        /// </summary>
        public static readonly ExpandedNodeId Product_Price = new ExpandedNodeId(.Variables.Product_Price, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Product_ProductType Variable.
        /// </summary>
        public static readonly ExpandedNodeId Product_ProductType = new ExpandedNodeId(.Variables.Product_ProductType, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order_Id Variable.
        /// </summary>
        public static readonly ExpandedNodeId Order_Id = new ExpandedNodeId(.Variables.Order_Id, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order_ClientUsername Variable.
        /// </summary>
        public static readonly ExpandedNodeId Order_ClientUsername = new ExpandedNodeId(.Variables.Order_ClientUsername, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order_OrderDate Variable.
        /// </summary>
        public static readonly ExpandedNodeId Order_OrderDate = new ExpandedNodeId(.Variables.Order_OrderDate, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order_ProductIdQuantityMap Variable.
        /// </summary>
        public static readonly ExpandedNodeId Order_ProductIdQuantityMap = new ExpandedNodeId(.Variables.Order_ProductIdQuantityMap, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order_Price Variable.
        /// </summary>
        public static readonly ExpandedNodeId Order_Price = new ExpandedNodeId(.Variables.Order_Price, .Namespaces.cas);

        /// <summary>
        /// The identifier for the Order_DeliveryDate Variable.
        /// </summary>
        public static readonly ExpandedNodeId Order_DeliveryDate = new ExpandedNodeId(.Variables.Order_DeliveryDate, .Namespaces.cas);
    }
    #endregion

    #region BrowseName Declarations
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    public static partial class BrowseNames
    {
        /// <summary>
        /// The BrowseName for the Client component.
        /// </summary>
        public const string Client = "Client";

        /// <summary>
        /// The BrowseName for the ClientUsername component.
        /// </summary>
        public const string ClientUsername = "ClientUsername";

        /// <summary>
        /// The BrowseName for the DeliveryDate component.
        /// </summary>
        public const string DeliveryDate = "DeliveryDate";

        /// <summary>
        /// The BrowseName for the FirstName component.
        /// </summary>
        public const string FirstName = "FirstName";

        /// <summary>
        /// The BrowseName for the Id component.
        /// </summary>
        public const string Id = "Id";

        /// <summary>
        /// The BrowseName for the LastName component.
        /// </summary>
        public const string LastName = "LastName";

        /// <summary>
        /// The BrowseName for the Name component.
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// The BrowseName for the Order component.
        /// </summary>
        public const string Order = "Order";

        /// <summary>
        /// The BrowseName for the OrderDate component.
        /// </summary>
        public const string OrderDate = "OrderDate";

        /// <summary>
        /// The BrowseName for the PhoneNumber component.
        /// </summary>
        public const string PhoneNumber = "PhoneNumber";

        /// <summary>
        /// The BrowseName for the Price component.
        /// </summary>
        public const string Price = "Price";

        /// <summary>
        /// The BrowseName for the Product component.
        /// </summary>
        public const string Product = "Product";

        /// <summary>
        /// The BrowseName for the ProductIdQuantityMap component.
        /// </summary>
        public const string ProductIdQuantityMap = "ProductIdQuantityMap";

        /// <summary>
        /// The BrowseName for the ProductType component.
        /// </summary>
        public const string ProductType = "ProductType";

        /// <summary>
        /// The BrowseName for the Street component.
        /// </summary>
        public const string Street = "Street";

        /// <summary>
        /// The BrowseName for the StreetNumber component.
        /// </summary>
        public const string StreetNumber = "StreetNumber";

        /// <summary>
        /// The BrowseName for the Username component.
        /// </summary>
        public const string Username = "Username";
    }
    #endregion

    #region Namespace Declarations
    /// <summary>
    /// Defines constants for all namespaces referenced by the model design.
    /// </summary>
    public static partial class Namespaces
    {
        /// <summary>
        /// The URI for the cas namespace (.NET code namespace is '').
        /// </summary>
        public const string cas = "http://cas.eu/UA/CommServer/";

        /// <summary>
        /// The URI for the OpcUa namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUa = "http://opcfoundation.org/UA/";

        /// <summary>
        /// The URI for the OpcUaXsd namespace (.NET code namespace is 'Opc.Ua').
        /// </summary>
        public const string OpcUaXsd = "http://opcfoundation.org/UA/2008/02/Types.xsd";
    }
    #endregion

    #region ClientState Class
    #if (!OPCUA_EXCLUDE_ClientState)
    /// <summary>
    /// Stores an instance of the Client ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ClientState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public ClientState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(.ObjectTypes.Client, .Namespaces.cas, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAABwAAABodHRwOi8vY2FzLmV1L1VBL0NvbW1TZXJ2ZXIv/////wRggAABAAAAAQAOAAAAQ2xpZW50" +
           "SW5zdGFuY2UBAQEAAQEBAP////8GAAAAFWCJCgIAAAABAAgAAABVc2VybmFtZQEBAgAALwA/AgAAAAAP" +
           "/////wEB/////wAAAAAVYIkKAgAAAAEACQAAAEZpcnN0TmFtZQEBAwAALwA/AwAAAAAP/////wMD////" +
           "/wAAAAAVYIkKAgAAAAEACAAAAExhc3ROYW1lAQEEAAAvAD8EAAAAAA//////AwP/////AAAAABVgiQoC" +
           "AAAAAQAGAAAAU3RyZWV0AQEFAAAvAD8FAAAAAA//////AwP/////AAAAABVgiQoCAAAAAQAMAAAAU3Ry" +
           "ZWV0TnVtYmVyAQEGAAAvAD8GAAAAABz/////AwP/////AAAAABVgiQoCAAAAAQALAAAAUGhvbmVOdW1i" +
           "ZXIBAQcAAC8APwcAAAAAD/////8DA/////8AAAAA";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Username Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> Username
        {
            get
            {
                return m_username;
            }

            set
            {
                if (!Object.ReferenceEquals(m_username, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_username = value;
            }
        }

        /// <summary>
        /// A description for the FirstName Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> FirstName
        {
            get
            {
                return m_firstName;
            }

            set
            {
                if (!Object.ReferenceEquals(m_firstName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_firstName = value;
            }
        }

        /// <summary>
        /// A description for the LastName Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> LastName
        {
            get
            {
                return m_lastName;
            }

            set
            {
                if (!Object.ReferenceEquals(m_lastName, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_lastName = value;
            }
        }

        /// <summary>
        /// A description for the Street Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> Street
        {
            get
            {
                return m_street;
            }

            set
            {
                if (!Object.ReferenceEquals(m_street, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_street = value;
            }
        }

        /// <summary>
        /// A description for the StreetNumber Variable.
        /// </summary>
        public BaseDataVariableState StreetNumber
        {
            get
            {
                return m_streetNumber;
            }

            set
            {
                if (!Object.ReferenceEquals(m_streetNumber, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_streetNumber = value;
            }
        }

        /// <summary>
        /// A description for the PhoneNumber Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> PhoneNumber
        {
            get
            {
                return m_phoneNumber;
            }

            set
            {
                if (!Object.ReferenceEquals(m_phoneNumber, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_phoneNumber = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_username != null)
            {
                children.Add(m_username);
            }

            if (m_firstName != null)
            {
                children.Add(m_firstName);
            }

            if (m_lastName != null)
            {
                children.Add(m_lastName);
            }

            if (m_street != null)
            {
                children.Add(m_street);
            }

            if (m_streetNumber != null)
            {
                children.Add(m_streetNumber);
            }

            if (m_phoneNumber != null)
            {
                children.Add(m_phoneNumber);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case .BrowseNames.Username:
                {
                    if (createOrReplace)
                    {
                        if (Username == null)
                        {
                            if (replacement == null)
                            {
                                Username = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                Username = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = Username;
                    break;
                }

                case .BrowseNames.FirstName:
                {
                    if (createOrReplace)
                    {
                        if (FirstName == null)
                        {
                            if (replacement == null)
                            {
                                FirstName = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                FirstName = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = FirstName;
                    break;
                }

                case .BrowseNames.LastName:
                {
                    if (createOrReplace)
                    {
                        if (LastName == null)
                        {
                            if (replacement == null)
                            {
                                LastName = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                LastName = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = LastName;
                    break;
                }

                case .BrowseNames.Street:
                {
                    if (createOrReplace)
                    {
                        if (Street == null)
                        {
                            if (replacement == null)
                            {
                                Street = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                Street = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = Street;
                    break;
                }

                case .BrowseNames.StreetNumber:
                {
                    if (createOrReplace)
                    {
                        if (StreetNumber == null)
                        {
                            if (replacement == null)
                            {
                                StreetNumber = new BaseDataVariableState(this);
                            }
                            else
                            {
                                StreetNumber = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = StreetNumber;
                    break;
                }

                case .BrowseNames.PhoneNumber:
                {
                    if (createOrReplace)
                    {
                        if (PhoneNumber == null)
                        {
                            if (replacement == null)
                            {
                                PhoneNumber = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                PhoneNumber = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = PhoneNumber;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private BaseDataVariableState<byte[]> m_username;
        private BaseDataVariableState<byte[]> m_firstName;
        private BaseDataVariableState<byte[]> m_lastName;
        private BaseDataVariableState<byte[]> m_street;
        private BaseDataVariableState m_streetNumber;
        private BaseDataVariableState<byte[]> m_phoneNumber;
        #endregion
    }
    #endif
    #endregion

    #region ProductState Class
    #if (!OPCUA_EXCLUDE_ProductState)
    /// <summary>
    /// Stores an instance of the Product ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class ProductState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public ProductState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(.ObjectTypes.Product, .Namespaces.cas, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);
        }

        #region Initialization String
        private const string InitializationString =
           "AQAAABwAAABodHRwOi8vY2FzLmV1L1VBL0NvbW1TZXJ2ZXIv/////wRggAABAAAAAQAPAAAAUHJvZHVj" +
           "dEluc3RhbmNlAQEIAAEBCAD/////BAAAABVgiQoCAAAAAQACAAAASWQBAQkAAC8APwkAAAAAHP////8B" +
           "Af////8AAAAAFWCJCgIAAAABAAQAAABOYW1lAQEKAAAvAD8KAAAAAA//////AwP/////AAAAABVgiQoC" +
           "AAAAAQAFAAAAUHJpY2UBAQsAAC8APwsAAAAAC/////8DA/////8AAAAAFWCJCgIAAAABAAsAAABQcm9k" +
           "dWN0VHlwZQEBDAAALwA/DAAAAAAd/////wMD/////wAAAAA=";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Id Variable.
        /// </summary>
        public BaseDataVariableState Id
        {
            get
            {
                return m_id;
            }

            set
            {
                if (!Object.ReferenceEquals(m_id, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_id = value;
            }
        }

        /// <summary>
        /// A description for the Name Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> Name
        {
            get
            {
                return m_name;
            }

            set
            {
                if (!Object.ReferenceEquals(m_name, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_name = value;
            }
        }

        /// <summary>
        /// A description for the Price Variable.
        /// </summary>
        public BaseDataVariableState<double> Price
        {
            get
            {
                return m_price;
            }

            set
            {
                if (!Object.ReferenceEquals(m_price, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_price = value;
            }
        }

        /// <summary>
        /// A description for the ProductType Variable.
        /// </summary>
        public BaseDataVariableState<int> ProductType
        {
            get
            {
                return m_productType;
            }

            set
            {
                if (!Object.ReferenceEquals(m_productType, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_productType = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_id != null)
            {
                children.Add(m_id);
            }

            if (m_name != null)
            {
                children.Add(m_name);
            }

            if (m_price != null)
            {
                children.Add(m_price);
            }

            if (m_productType != null)
            {
                children.Add(m_productType);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case .BrowseNames.Id:
                {
                    if (createOrReplace)
                    {
                        if (Id == null)
                        {
                            if (replacement == null)
                            {
                                Id = new BaseDataVariableState(this);
                            }
                            else
                            {
                                Id = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = Id;
                    break;
                }

                case .BrowseNames.Name:
                {
                    if (createOrReplace)
                    {
                        if (Name == null)
                        {
                            if (replacement == null)
                            {
                                Name = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                Name = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = Name;
                    break;
                }

                case .BrowseNames.Price:
                {
                    if (createOrReplace)
                    {
                        if (Price == null)
                        {
                            if (replacement == null)
                            {
                                Price = new BaseDataVariableState<double>(this);
                            }
                            else
                            {
                                Price = (BaseDataVariableState<double>)replacement;
                            }
                        }
                    }

                    instance = Price;
                    break;
                }

                case .BrowseNames.ProductType:
                {
                    if (createOrReplace)
                    {
                        if (ProductType == null)
                        {
                            if (replacement == null)
                            {
                                ProductType = new BaseDataVariableState<int>(this);
                            }
                            else
                            {
                                ProductType = (BaseDataVariableState<int>)replacement;
                            }
                        }
                    }

                    instance = ProductType;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private BaseDataVariableState m_id;
        private BaseDataVariableState<byte[]> m_name;
        private BaseDataVariableState<double> m_price;
        private BaseDataVariableState<int> m_productType;
        #endregion
    }
    #endif
    #endregion

    #region OrderState Class
    #if (!OPCUA_EXCLUDE_OrderState)
    /// <summary>
    /// Stores an instance of the Order ObjectType.
    /// </summary>
    /// <exclude />
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Opc.Ua.ModelCompiler", "1.0.0.0")]
    public partial class OrderState : BaseObjectState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public OrderState(NodeState parent) : base(parent)
        {
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(.ObjectTypes.Order, .Namespaces.cas, namespaceUris);
        }

        #if (!OPCUA_EXCLUDE_InitializationStrings)
        /// <summary>
        /// Initializes the instance.
        /// </summary>
        protected override void Initialize(ISystemContext context)
        {
            Initialize(context, InitializationString);
            InitializeOptionalChildren(context);
        }

        protected override void Initialize(ISystemContext context, NodeState source)
        {
            InitializeOptionalChildren(context);
            base.Initialize(context, source);
        }

        /// <summary>
        /// Initializes the any option children defined for the instance.
        /// </summary>
        protected override void InitializeOptionalChildren(ISystemContext context)
        {
            base.InitializeOptionalChildren(context);

            if (DeliveryDate != null)
            {
                DeliveryDate.Initialize(context, DeliveryDate_InitializationString);
            }
        }

        #region Initialization String
        private const string DeliveryDate_InitializationString =
           "AQAAABwAAABodHRwOi8vY2FzLmV1L1VBL0NvbW1TZXJ2ZXIv/////xVgiQoCAAAAAQAMAAAARGVsaXZl" +
           "cnlEYXRlAQETAAAvAD8TAAAAAA3/////AwP/////AAAAAA==";

        private const string InitializationString =
           "AQAAABwAAABodHRwOi8vY2FzLmV1L1VBL0NvbW1TZXJ2ZXIv/////wRggAABAAAAAQANAAAAT3JkZXJJ" +
           "bnN0YW5jZQEBDQABAQ0A/////wYAAAAVYIkKAgAAAAEAAgAAAElkAQEOAAAvAD8OAAAAABz/////AQH/" +
           "////AAAAABVgiQoCAAAAAQAOAAAAQ2xpZW50VXNlcm5hbWUBAQ8AAC8APw8AAAAAD/////8DA/////8A" +
           "AAAAFWCJCgIAAAABAAkAAABPcmRlckRhdGUBARAAAC8APxAAAAAADf////8DA/////8AAAAAFWCJCgIA" +
           "AAABABQAAABQcm9kdWN0SWRRdWFudGl0eU1hcAEBEQAALwA/EQAAAAAY/////wMD/////wAAAAAVYIkK" +
           "AgAAAAEABQAAAFByaWNlAQESAAAvAD8SAAAAAAv/////AwP/////AAAAABVgiQoCAAAAAQAMAAAARGVs" +
           "aXZlcnlEYXRlAQETAAAvAD8TAAAAAA3/////AwP/////AAAAAA==";
        #endregion
        #endif
        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the Id Variable.
        /// </summary>
        public BaseDataVariableState Id
        {
            get
            {
                return m_id;
            }

            set
            {
                if (!Object.ReferenceEquals(m_id, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_id = value;
            }
        }

        /// <summary>
        /// A description for the ClientUsername Variable.
        /// </summary>
        public BaseDataVariableState<byte[]> ClientUsername
        {
            get
            {
                return m_clientUsername;
            }

            set
            {
                if (!Object.ReferenceEquals(m_clientUsername, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_clientUsername = value;
            }
        }

        /// <summary>
        /// A description for the OrderDate Variable.
        /// </summary>
        public BaseDataVariableState<DateTime> OrderDate
        {
            get
            {
                return m_orderDate;
            }

            set
            {
                if (!Object.ReferenceEquals(m_orderDate, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_orderDate = value;
            }
        }

        /// <summary>
        /// A description for the ProductIdQuantityMap Variable.
        /// </summary>
        public BaseDataVariableState ProductIdQuantityMap
        {
            get
            {
                return m_productIdQuantityMap;
            }

            set
            {
                if (!Object.ReferenceEquals(m_productIdQuantityMap, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_productIdQuantityMap = value;
            }
        }

        /// <summary>
        /// A description for the Price Variable.
        /// </summary>
        public BaseDataVariableState<double> Price
        {
            get
            {
                return m_price;
            }

            set
            {
                if (!Object.ReferenceEquals(m_price, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_price = value;
            }
        }

        /// <summary>
        /// A description for the DeliveryDate Variable.
        /// </summary>
        public BaseDataVariableState<DateTime> DeliveryDate
        {
            get
            {
                return m_deliveryDate;
            }

            set
            {
                if (!Object.ReferenceEquals(m_deliveryDate, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_deliveryDate = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_id != null)
            {
                children.Add(m_id);
            }

            if (m_clientUsername != null)
            {
                children.Add(m_clientUsername);
            }

            if (m_orderDate != null)
            {
                children.Add(m_orderDate);
            }

            if (m_productIdQuantityMap != null)
            {
                children.Add(m_productIdQuantityMap);
            }

            if (m_price != null)
            {
                children.Add(m_price);
            }

            if (m_deliveryDate != null)
            {
                children.Add(m_deliveryDate);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case .BrowseNames.Id:
                {
                    if (createOrReplace)
                    {
                        if (Id == null)
                        {
                            if (replacement == null)
                            {
                                Id = new BaseDataVariableState(this);
                            }
                            else
                            {
                                Id = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = Id;
                    break;
                }

                case .BrowseNames.ClientUsername:
                {
                    if (createOrReplace)
                    {
                        if (ClientUsername == null)
                        {
                            if (replacement == null)
                            {
                                ClientUsername = new BaseDataVariableState<byte[]>(this);
                            }
                            else
                            {
                                ClientUsername = (BaseDataVariableState<byte[]>)replacement;
                            }
                        }
                    }

                    instance = ClientUsername;
                    break;
                }

                case .BrowseNames.OrderDate:
                {
                    if (createOrReplace)
                    {
                        if (OrderDate == null)
                        {
                            if (replacement == null)
                            {
                                OrderDate = new BaseDataVariableState<DateTime>(this);
                            }
                            else
                            {
                                OrderDate = (BaseDataVariableState<DateTime>)replacement;
                            }
                        }
                    }

                    instance = OrderDate;
                    break;
                }

                case .BrowseNames.ProductIdQuantityMap:
                {
                    if (createOrReplace)
                    {
                        if (ProductIdQuantityMap == null)
                        {
                            if (replacement == null)
                            {
                                ProductIdQuantityMap = new BaseDataVariableState(this);
                            }
                            else
                            {
                                ProductIdQuantityMap = (BaseDataVariableState)replacement;
                            }
                        }
                    }

                    instance = ProductIdQuantityMap;
                    break;
                }

                case .BrowseNames.Price:
                {
                    if (createOrReplace)
                    {
                        if (Price == null)
                        {
                            if (replacement == null)
                            {
                                Price = new BaseDataVariableState<double>(this);
                            }
                            else
                            {
                                Price = (BaseDataVariableState<double>)replacement;
                            }
                        }
                    }

                    instance = Price;
                    break;
                }

                case .BrowseNames.DeliveryDate:
                {
                    if (createOrReplace)
                    {
                        if (DeliveryDate == null)
                        {
                            if (replacement == null)
                            {
                                DeliveryDate = new BaseDataVariableState<DateTime>(this);
                            }
                            else
                            {
                                DeliveryDate = (BaseDataVariableState<DateTime>)replacement;
                            }
                        }
                    }

                    instance = DeliveryDate;
                    break;
                }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private BaseDataVariableState m_id;
        private BaseDataVariableState<byte[]> m_clientUsername;
        private BaseDataVariableState<DateTime> m_orderDate;
        private BaseDataVariableState m_productIdQuantityMap;
        private BaseDataVariableState<double> m_price;
        private BaseDataVariableState<DateTime> m_deliveryDate;
        #endregion
    }
    #endif
    #endregion
}