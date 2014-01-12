﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PJanssen.Outliner.UpdateClient.Service {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutlinerInstallation", Namespace="http://outliner.pjanssen.nl/")]
    [System.SerializableAttribute()]
    public partial class OutlinerInstallation : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private PJanssen.Outliner.UpdateClient.Service.OutlinerVersion OutlinerVersionField;
        
        private int MaxVersionField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public PJanssen.Outliner.UpdateClient.Service.OutlinerVersion OutlinerVersion {
            get {
                return this.OutlinerVersionField;
            }
            set {
                if ((object.ReferenceEquals(this.OutlinerVersionField, value) != true)) {
                    this.OutlinerVersionField = value;
                    this.RaisePropertyChanged("OutlinerVersion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=1)]
        public int MaxVersion {
            get {
                return this.MaxVersionField;
            }
            set {
                if ((this.MaxVersionField.Equals(value) != true)) {
                    this.MaxVersionField = value;
                    this.RaisePropertyChanged("MaxVersion");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OutlinerVersion", Namespace="http://outliner.pjanssen.nl/")]
    [System.SerializableAttribute()]
    public partial class OutlinerVersion : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private int MajorField;
        
        private int MinorField;
        
        private int BuildField;
        
        private int RevisionField;
        
        private bool IsBetaField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Major {
            get {
                return this.MajorField;
            }
            set {
                if ((this.MajorField.Equals(value) != true)) {
                    this.MajorField = value;
                    this.RaisePropertyChanged("Major");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public int Minor {
            get {
                return this.MinorField;
            }
            set {
                if ((this.MinorField.Equals(value) != true)) {
                    this.MinorField = value;
                    this.RaisePropertyChanged("Minor");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=2)]
        public int Build {
            get {
                return this.BuildField;
            }
            set {
                if ((this.BuildField.Equals(value) != true)) {
                    this.BuildField = value;
                    this.RaisePropertyChanged("Build");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=3)]
        public int Revision {
            get {
                return this.RevisionField;
            }
            set {
                if ((this.RevisionField.Equals(value) != true)) {
                    this.RevisionField = value;
                    this.RaisePropertyChanged("Revision");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true, Order=4)]
        public bool IsBeta {
            get {
                return this.IsBetaField;
            }
            set {
                if ((this.IsBetaField.Equals(value) != true)) {
                    this.IsBetaField = value;
                    this.RaisePropertyChanged("IsBeta");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UpdateData", Namespace="http://outliner.pjanssen.nl/")]
    [System.SerializableAttribute()]
    public partial class UpdateData : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private bool IsUpdateAvailableField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private PJanssen.Outliner.UpdateClient.Service.OutlinerVersion NewVersionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DownloadUrlField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SignatureField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReleaseNotesUrlField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public bool IsUpdateAvailable {
            get {
                return this.IsUpdateAvailableField;
            }
            set {
                if ((this.IsUpdateAvailableField.Equals(value) != true)) {
                    this.IsUpdateAvailableField = value;
                    this.RaisePropertyChanged("IsUpdateAvailable");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false)]
        public PJanssen.Outliner.UpdateClient.Service.OutlinerVersion NewVersion {
            get {
                return this.NewVersionField;
            }
            set {
                if ((object.ReferenceEquals(this.NewVersionField, value) != true)) {
                    this.NewVersionField = value;
                    this.RaisePropertyChanged("NewVersion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string DownloadUrl {
            get {
                return this.DownloadUrlField;
            }
            set {
                if ((object.ReferenceEquals(this.DownloadUrlField, value) != true)) {
                    this.DownloadUrlField = value;
                    this.RaisePropertyChanged("DownloadUrl");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string Signature {
            get {
                return this.SignatureField;
            }
            set {
                if ((object.ReferenceEquals(this.SignatureField, value) != true)) {
                    this.SignatureField = value;
                    this.RaisePropertyChanged("Signature");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string ReleaseNotesUrl {
            get {
                return this.ReleaseNotesUrlField;
            }
            set {
                if ((object.ReferenceEquals(this.ReleaseNotesUrlField, value) != true)) {
                    this.ReleaseNotesUrlField = value;
                    this.RaisePropertyChanged("ReleaseNotesUrl");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://outliner.pjanssen.nl/", ConfigurationName="Service.UpdateSoap")]
    public interface UpdateSoap {
        
        // CODEGEN: Generating message contract since element name installation from namespace http://outliner.pjanssen.nl/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://outliner.pjanssen.nl/GetUpdateData", ReplyAction="*")]
        PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponse GetUpdateData(PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest request);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://outliner.pjanssen.nl/GetUpdateData", ReplyAction="*")]
        System.IAsyncResult BeginGetUpdateData(PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest request, System.AsyncCallback callback, object asyncState);
        
        PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponse EndGetUpdateData(System.IAsyncResult result);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetUpdateDataRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetUpdateData", Namespace="http://outliner.pjanssen.nl/", Order=0)]
        public PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequestBody Body;
        
        public GetUpdateDataRequest() {
        }
        
        public GetUpdateDataRequest(PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://outliner.pjanssen.nl/")]
    public partial class GetUpdateDataRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation;
        
        public GetUpdateDataRequestBody() {
        }
        
        public GetUpdateDataRequestBody(PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation) {
            this.installation = installation;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetUpdateDataResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetUpdateDataResponse", Namespace="http://outliner.pjanssen.nl/", Order=0)]
        public PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponseBody Body;
        
        public GetUpdateDataResponse() {
        }
        
        public GetUpdateDataResponse(PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://outliner.pjanssen.nl/")]
    public partial class GetUpdateDataResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public PJanssen.Outliner.UpdateClient.Service.UpdateData GetUpdateDataResult;
        
        public GetUpdateDataResponseBody() {
        }
        
        public GetUpdateDataResponseBody(PJanssen.Outliner.UpdateClient.Service.UpdateData GetUpdateDataResult) {
            this.GetUpdateDataResult = GetUpdateDataResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface UpdateSoapChannel : PJanssen.Outliner.UpdateClient.Service.UpdateSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetUpdateDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetUpdateDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public PJanssen.Outliner.UpdateClient.Service.UpdateData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((PJanssen.Outliner.UpdateClient.Service.UpdateData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class UpdateSoapClient : System.ServiceModel.ClientBase<PJanssen.Outliner.UpdateClient.Service.UpdateSoap>, PJanssen.Outliner.UpdateClient.Service.UpdateSoap {
        
        private BeginOperationDelegate onBeginGetUpdateDataDelegate;
        
        private EndOperationDelegate onEndGetUpdateDataDelegate;
        
        private System.Threading.SendOrPostCallback onGetUpdateDataCompletedDelegate;
        
        public UpdateSoapClient() {
        }
        
        public UpdateSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public UpdateSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UpdateSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public UpdateSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<GetUpdateDataCompletedEventArgs> GetUpdateDataCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponse PJanssen.Outliner.UpdateClient.Service.UpdateSoap.GetUpdateData(PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest request) {
            return base.Channel.GetUpdateData(request);
        }
        
        public PJanssen.Outliner.UpdateClient.Service.UpdateData GetUpdateData(PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation) {
            PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest inValue = new PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest();
            inValue.Body = new PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequestBody();
            inValue.Body.installation = installation;
            PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponse retVal = ((PJanssen.Outliner.UpdateClient.Service.UpdateSoap)(this)).GetUpdateData(inValue);
            return retVal.Body.GetUpdateDataResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult PJanssen.Outliner.UpdateClient.Service.UpdateSoap.BeginGetUpdateData(PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest request, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetUpdateData(request, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetUpdateData(PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation, System.AsyncCallback callback, object asyncState) {
            PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest inValue = new PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequest();
            inValue.Body = new PJanssen.Outliner.UpdateClient.Service.GetUpdateDataRequestBody();
            inValue.Body.installation = installation;
            return ((PJanssen.Outliner.UpdateClient.Service.UpdateSoap)(this)).BeginGetUpdateData(inValue, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponse PJanssen.Outliner.UpdateClient.Service.UpdateSoap.EndGetUpdateData(System.IAsyncResult result) {
            return base.Channel.EndGetUpdateData(result);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public PJanssen.Outliner.UpdateClient.Service.UpdateData EndGetUpdateData(System.IAsyncResult result) {
            PJanssen.Outliner.UpdateClient.Service.GetUpdateDataResponse retVal = ((PJanssen.Outliner.UpdateClient.Service.UpdateSoap)(this)).EndGetUpdateData(result);
            return retVal.Body.GetUpdateDataResult;
        }
        
        private System.IAsyncResult OnBeginGetUpdateData(object[] inValues, System.AsyncCallback callback, object asyncState) {
            PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation = ((PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation)(inValues[0]));
            return this.BeginGetUpdateData(installation, callback, asyncState);
        }
        
        private object[] OnEndGetUpdateData(System.IAsyncResult result) {
            PJanssen.Outliner.UpdateClient.Service.UpdateData retVal = this.EndGetUpdateData(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetUpdateDataCompleted(object state) {
            if ((this.GetUpdateDataCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetUpdateDataCompleted(this, new GetUpdateDataCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetUpdateDataAsync(PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation) {
            this.GetUpdateDataAsync(installation, null);
        }
        
        public void GetUpdateDataAsync(PJanssen.Outliner.UpdateClient.Service.OutlinerInstallation installation, object userState) {
            if ((this.onBeginGetUpdateDataDelegate == null)) {
                this.onBeginGetUpdateDataDelegate = new BeginOperationDelegate(this.OnBeginGetUpdateData);
            }
            if ((this.onEndGetUpdateDataDelegate == null)) {
                this.onEndGetUpdateDataDelegate = new EndOperationDelegate(this.OnEndGetUpdateData);
            }
            if ((this.onGetUpdateDataCompletedDelegate == null)) {
                this.onGetUpdateDataCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetUpdateDataCompleted);
            }
            base.InvokeAsync(this.onBeginGetUpdateDataDelegate, new object[] {
                        installation}, this.onEndGetUpdateDataDelegate, this.onGetUpdateDataCompletedDelegate, userState);
        }
    }
}
