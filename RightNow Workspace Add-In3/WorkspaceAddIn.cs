using System.AddIn;
using System.Drawing;
using System.Windows.Forms;
using RightNow.AddIns.AddInViews;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
////////////////////////////////////////////////////////////////////////////////
//
// File: WorkspaceAddIn.cs
//
// Comments:
//
// Notes: 
//
// Pre-Conditions: 
//
////////////////////////////////////////////////////////////////////////////////
namespace Parts_Adjusted
{
    public class WorkspaceAddIn : Panel, IWorkspaceComponent2
    {
        /// <summary>
        /// The current workspace record context.
        /// </summary>
        private IRecordContext _recordContext;
        private Label label1;
    
        public static IGlobalContext _globalContext { get; private set; }
        private static IGenericObject _partsRecord;
        private static IGenericObject _laborRecord;
        private static IGenericObject _otherchRecord;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        public WorkspaceAddIn(bool inDesignMode, IRecordContext RecordContext, IGlobalContext GlobalContext)
        {
            if (!inDesignMode)
            {
                _recordContext = RecordContext;
                _globalContext = GlobalContext;

            }
            else
            {
                InitializeComponent();
            }

        }

        #region IAddInControl Members

        /// <summary>
        /// Method called by the Add-In framework to retrieve the control.
        /// </summary>
        /// <returns>The control, typically 'this'.</returns>
        public Control GetControl()
        {
            return this;
        }

        #endregion

        #region IWorkspaceComponent2 Members

        /// <summary>
        /// Sets the ReadOnly property of this control.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Method which is called when any Workspace Rule Action is invoked.
        /// </summary>
        /// <param name="ActionName">The name of the Workspace Rule Action that was invoked.</param>
        public void RuleActionInvoked(string ActionName)
        {
            string fieldVal = "";
            switch (ActionName)
            {

                case "PartsPriceAdj":
                    
                    _partsRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as IGenericObject;
                     fieldVal = GetFieldValue("part_price", _partsRecord);//Get value
                    
                    SetFieldValue("part_price_adj", fieldVal, _partsRecord);
                    break; 
                case "LaborRateAdj":
                    _laborRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as IGenericObject;
                     fieldVal = GetFieldValue("labor_rate_rqstd",_laborRecord );//Get value
                    
                    SetFieldValue("labor_rate_adj", fieldVal, _laborRecord);

                    break;
                case "OtherChargesAdj":
                    _otherchRecord = _recordContext.GetWorkspaceRecord(_recordContext.WorkspaceTypeName) as IGenericObject;
                     fieldVal = GetFieldValue("other_charges_amount", _otherchRecord);//Get value
                    
                    SetFieldValue("other_charges_amount_adj", fieldVal, _otherchRecord);

                    break; 
                default:
                    break;
            }
            _recordContext.RefreshWorkspace();//refresh the workspace

        }

        /// <summary>
        /// Method which is called when any Workspace Rule Condition is invoked.
        /// </summary>
        /// <param name="ConditionName">The name of the Workspace Rule Condition that was invoked.</param>
        /// <returns>The result of the condition.</returns>
        public string RuleConditionInvoked(string ConditionName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Method which is called to get value of a field.
        /// </summary>
        /// <param name="fieldName">The name of the custom field.</param>
        /// <returns>Value of the field</returns>
        public string GetFieldValue(string fieldName, IGenericObject _obj)
        {
            IList<IGenericField> fields = _obj.GenericFields;
            if (null != fields)
            {
                foreach (IGenericField field in fields)
                {
                    if (field.Name.Equals(fieldName))
                    {
                        if (field.DataValue.Value != null)
                            return field.DataValue.Value.ToString();
                    }
                }
            }
            return "";
        }
        /// <summary>
        /// Method which is use to set value to a field using record Context 
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="value">value of field</param>
        public void SetFieldValue(string fieldName, string value, IGenericObject _obj)
        {
            IList<IGenericField> fields = _obj.GenericFields;
            if (null != fields)
            {
                foreach (IGenericField field in fields)
                {
                    if (field.Name.Equals(fieldName))
                    {
                        switch (field.DataType)
                        {
                            case RightNow.AddIns.Common.DataTypeEnum.STRING:
                                field.DataValue.Value = value;
                                break;
                        }
                    }
                }
            }
            return;
        }

        #endregion

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            this.ResumeLayout(false);

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }

    [AddIn("Workspace Factory AddIn", Version = "1.0.0.0")]
    public class WorkspaceAddInFactory : IWorkspaceComponentFactory2
    {
        #region IWorkspaceComponentFactory2 Members
        static public IGlobalContext _globalContext;
        /// <summary>
        /// Method which is invoked by the AddIn framework when the control is created.
        /// </summary>
        /// <param name="inDesignMode">Flag which indicates if the control is being drawn on the Workspace Designer. (Use this flag to determine if code should perform any logic on the workspace record)</param>
        /// <param name="RecordContext">The current workspace record context.</param>
        /// <returns>The control which implements the IWorkspaceComponent2 interface.</returns>
        public IWorkspaceComponent2 CreateControl(bool inDesignMode, IRecordContext RecordContext)
        {
            return new WorkspaceAddIn(inDesignMode, RecordContext, _globalContext);
        }

        #endregion

        #region IFactoryBase Members

        /// <summary>
        /// The 16x16 pixel icon to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public Image Image16
        {
            get { return  Properties.Resources.AddIn16; }
        }

        /// <summary>
        /// The text to represent the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Text
        {
            get { return "Adjusted Prices"; }
        }

        /// <summary>
        /// The tooltip displayed when hovering over the Add-In in the Ribbon of the Workspace Designer.
        /// </summary>
        public string Tooltip
        {
            get { return "Adjusted Prices"; }
        }

        public bool Initialize(IGlobalContext GlobalContext)
        {
            _globalContext = GlobalContext;
            return true;
        }
        #endregion

        #region IAddInBase Members

        /// <summary>
        /// Method which is invoked from the Add-In framework and is used to programmatically control whether to load the Add-In.
        /// </summary>
        /// <param name="GlobalContext">The Global Context for the Add-In framework.</param>
        /// <returns>If true the Add-In to be loaded, if false the Add-In will not be loaded.</returns>
      
        #endregion
    }
}