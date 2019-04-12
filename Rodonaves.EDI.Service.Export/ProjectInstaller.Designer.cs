namespace Rodonaves.EDI.Service.Export
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.RodonavesEDIExportServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.RodonavesEDIExportServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // RodonavesEDIExportServiceProcessInstaller
            // 
            this.RodonavesEDIExportServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.RodonavesEDIExportServiceProcessInstaller.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RodonavesEDIExportServiceInstaller});
            this.RodonavesEDIExportServiceProcessInstaller.Password = null;
            this.RodonavesEDIExportServiceProcessInstaller.Username = null;
            // 
            // RodonavesEDIExportServiceInstaller
            // 
            this.RodonavesEDIExportServiceInstaller.Description = "Serviço de criação e envio de arquivo";
            this.RodonavesEDIExportServiceInstaller.DisplayName = "Rodonaves EDI Export Service";
            this.RodonavesEDIExportServiceInstaller.ServiceName = "Rodonaves EDI Export Service";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.RodonavesEDIExportServiceProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller RodonavesEDIExportServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller RodonavesEDIExportServiceInstaller;
    }
}