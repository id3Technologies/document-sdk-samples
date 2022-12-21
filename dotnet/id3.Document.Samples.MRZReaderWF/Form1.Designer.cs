namespace id3.Document.Samples.MRZReaderWF
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.buttonStartCapture = new System.Windows.Forms.Button();
            this.labelDetectionTime = new System.Windows.Forms.Label();
            this.labelMrzType = new System.Windows.Forms.Label();
            this.labelMRZ = new System.Windows.Forms.Label();
            this.labelMrzDecode = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(49, 14);
            this.pictureBoxPreview.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(746, 554);
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            // 
            // buttonStartCapture
            // 
            this.buttonStartCapture.Location = new System.Drawing.Point(49, 611);
            this.buttonStartCapture.Margin = new System.Windows.Forms.Padding(4);
            this.buttonStartCapture.Name = "buttonStartCapture";
            this.buttonStartCapture.Size = new System.Drawing.Size(186, 84);
            this.buttonStartCapture.TabIndex = 1;
            this.buttonStartCapture.Text = "Start capture";
            this.buttonStartCapture.UseVisualStyleBackColor = true;
            // 
            // labelDetectionTime
            // 
            this.labelDetectionTime.AutoSize = true;
            this.labelDetectionTime.Location = new System.Drawing.Point(699, 596);
            this.labelDetectionTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDetectionTime.Name = "labelDetectionTime";
            this.labelDetectionTime.Size = new System.Drawing.Size(96, 15);
            this.labelDetectionTime.TabIndex = 10;
            this.labelDetectionTime.Text = "Detection time: -";
            // 
            // labelMrzType
            // 
            this.labelMrzType.AutoSize = true;
            this.labelMrzType.Location = new System.Drawing.Point(819, 38);
            this.labelMrzType.Name = "labelMrzType";
            this.labelMrzType.Size = new System.Drawing.Size(59, 15);
            this.labelMrzType.TabIndex = 11;
            this.labelMrzType.Text = "MRZ Type";
            // 
            // labelMRZ
            // 
            this.labelMRZ.AutoSize = true;
            this.labelMRZ.Location = new System.Drawing.Point(819, 73);
            this.labelMRZ.Name = "labelMRZ";
            this.labelMRZ.Size = new System.Drawing.Size(32, 15);
            this.labelMRZ.TabIndex = 12;
            this.labelMRZ.Text = "MRZ";
            // 
            // labelMrzDecode
            // 
            this.labelMrzDecode.AutoSize = true;
            this.labelMrzDecode.Location = new System.Drawing.Point(819, 176);
            this.labelMrzDecode.Name = "labelMrzDecode";
            this.labelMrzDecode.Size = new System.Drawing.Size(89, 15);
            this.labelMrzDecode.TabIndex = 13;
            this.labelMrzDecode.Text = "Decoding fields";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1110, 748);
            this.Controls.Add(this.labelMrzDecode);
            this.Controls.Add(this.labelMRZ);
            this.Controls.Add(this.labelMrzType);
            this.Controls.Add(this.labelDetectionTime);
            this.Controls.Add(this.buttonStartCapture);
            this.Controls.Add(this.pictureBoxPreview);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "id3Document MRZ Reader Sample";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Button buttonStartCapture;
        private System.Windows.Forms.Label labelDetectionTime;
        private Label labelMrzType;
        private Label labelMRZ;
        private Label labelMrzDecode;
    }

}