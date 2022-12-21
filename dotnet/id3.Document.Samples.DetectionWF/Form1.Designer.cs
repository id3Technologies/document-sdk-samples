namespace id3.Document.Samples.DetectionWF
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
            this.pictureBoxAligned = new System.Windows.Forms.PictureBox();
            this.labelDetectionScore = new System.Windows.Forms.Label();
            this.labelDetectionTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAligned)).BeginInit();
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
            // pictureBoxAligned
            // 
            this.pictureBoxAligned.InitialImage = null;
            this.pictureBoxAligned.Location = new System.Drawing.Point(826, 90);
            this.pictureBoxAligned.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBoxAligned.Name = "pictureBoxAligned";
            this.pictureBoxAligned.Size = new System.Drawing.Size(320, 217);
            this.pictureBoxAligned.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxAligned.TabIndex = 6;
            this.pictureBoxAligned.TabStop = false;
            // 
            // labelDetectionScore
            // 
            this.labelDetectionScore.AutoSize = true;
            this.labelDetectionScore.Location = new System.Drawing.Point(699, 630);
            this.labelDetectionScore.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDetectionScore.Name = "labelDetectionScore";
            this.labelDetectionScore.Size = new System.Drawing.Size(100, 15);
            this.labelDetectionScore.TabIndex = 9;
            this.labelDetectionScore.Text = "Detection score: -";
            // 
            // labelDetectionTime
            // 
            this.labelDetectionTime.AutoSize = true;
            this.labelDetectionTime.Location = new System.Drawing.Point(699, 593);
            this.labelDetectionTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelDetectionTime.Name = "labelDetectionTime";
            this.labelDetectionTime.Size = new System.Drawing.Size(96, 15);
            this.labelDetectionTime.TabIndex = 10;
            this.labelDetectionTime.Text = "Detection time: -";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 717);
            this.Controls.Add(this.labelDetectionTime);
            this.Controls.Add(this.labelDetectionScore);
            this.Controls.Add(this.pictureBoxAligned);
            this.Controls.Add(this.buttonStartCapture);
            this.Controls.Add(this.pictureBoxPreview);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "id3Document Detection Sample";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAligned)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.Button buttonStartCapture;
        private System.Windows.Forms.PictureBox pictureBoxAligned;
        private System.Windows.Forms.Label labelDetectionScore;
        private System.Windows.Forms.Label labelDetectionTime;
    }

}