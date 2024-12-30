using GeneratorBackend;
using LanguageManager;
using HereToSlay.Properties;
using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using Windows.Globalization;

#pragma warning disable CA1416 // It onlyworks on Windows

namespace HereToSlay
{
    public partial class Menu : Form
    {
        private static readonly Backend.AssetManager inst = Backend.AssetManager.Instance;
        bool initLang = false; // this is used to prevent the missing image error from showing twice when first loading the program

        private static List<LanguageManager.Manager.Language> tounges = LanguageManager.Manager.LoadJson(); // load languages
        public int SelectedLanguage; // this is used to pass selected language index to the backend (otherwise it only generation methods know selected language)

        public Menu()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            #region Fonts
            Font fontUI = FontLoader.GetFont("SourceSans3.ttf", 10);
            FontLoader.ChangeFontForAllControls(this, fontUI);
            selectImgButton.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            Font fontUIsmall = FontLoader.GetFont("SourceSansPro.ttf", 9);
            gradient.Font = fontUIsmall;
            nameWhite.Font = fontUIsmall;
            splitClass.Font = fontUIsmall;
            gitLabel1.Font = fontUIsmall;
            gitLabel2.Font = fontUIsmall;

            Font fontLeader = FontLoader.GetFont("PatuaOne_Polish.ttf", 13);
            nameText.Font = fontLeader;
            RENDER.Font = fontLeader;
            cardType.Font = fontLeader;

            Font fontLeaderSmall = FontLoader.GetFont("PatuaOne_Polish.ttf", 10);
            FontLoader.ChangeFontForAllLabels(this, fontLeaderSmall);
            labelAdditionalReq.Font = fontLeaderSmall; // This is a checkbox
            labelHeroBonus.Font = fontLeaderSmall; // This is a checkbox
            LeaderCard.Font = fontLeaderSmall;
            MonsterCard.Font = fontLeaderSmall;
            HeroCard.Font = fontLeaderSmall;
            ItemCard.Font = fontLeaderSmall;
            MagicCard.Font = fontLeaderSmall;
            ModifierCard.Font = fontLeaderSmall;
            ChallengeCard.Font = fontLeaderSmall;

            itemImgMore.Font = fontLeader;
            #endregion


            #region ComboBoxes
            #pragma warning disable CS8622 // the warnings were annoying me, so I disabled them
            language.ComboBox.DrawMode = DrawMode.OwnerDrawFixed;
            language.ComboBox.DrawItem += ImageCBox.ComboBox_DrawItem;
            language.DropDown += ImageCBox.ComboBox_WidthAutoAdjust;

            foreach (var i in tounges) // tounges. because i said so :)
            {
                language.Items.Add(new ImageCBox(i.lang_name, Properties.Resources.missingno));
            }

            language.SelectedIndex = Properties.Settings.Default.Language;
            initLang = true;
            SelectedLanguage = language.SelectedIndex;

            // Make all of the class boxes have a custom draw mode with images
            foreach (ComboBox c in new ComboBox[] { chosenClass, chosenSecondClass, itemChosenClass, classReq1, classReq2, classReq3, classReq4, classReq5 })
            {
                c.DrawMode = DrawMode.OwnerDrawFixed;
                c.DrawItem += ImageCBox.ComboBox_DrawItem;
                c.DropDown += ImageCBox.ComboBox_WidthAutoAdjust;
                c.ItemHeight = 19;
            }
            chosenClass.SelectedIndex = 0;
            itemChosenClass.SelectedIndex = 0;

            // Make the roll symbol boxes have custom draw mode with background colors
            badOutputSym.DrawMode = DrawMode.OwnerDrawFixed;
            badOutputSym.DrawItem += BackgroundColorCBox.ComboBox_DrawItem;
            badOutputSym.Items.Add(new BackgroundColorCBox("+", Color.FromArgb(109, 166, 88)));
            badOutputSym.Items.Add(new BackgroundColorCBox("-", Color.FromArgb(230, 44, 47)));
            badOutputSym.ItemHeight = 17;

            goodOutputSym.DrawMode = DrawMode.OwnerDrawFixed;
            goodOutputSym.DrawItem += BackgroundColorCBox.ComboBox_DrawItem;
            goodOutputSym.Items.Add(new BackgroundColorCBox("+", Color.FromArgb(109, 166, 88)));
            goodOutputSym.Items.Add(new BackgroundColorCBox("-", Color.FromArgb(230, 44, 47)));
            goodOutputSym.ItemHeight = 17;

            badOutputSym.SelectedIndex = 1;
            goodOutputSym.SelectedIndex = 0;

            #pragma warning restore CS8622
            #endregion

            switch (Properties.Settings.Default.CardType)
            {
                case 0:
                    LeaderCard_Click(null, null);
                    break;
                case 1:
                    MonsterCard_Click(null, null);
                    break;
                case 2:
                    HeroCard_Click(null, null);
                    break;
                case 3:
                    ItemCard_Click(null, null);
                    break;
                case 4:
                    MagicCard_Click(null, null);
                    break;
                case 5:
                    ModifierCard_Click(null, null);
                    break;
                case 6:
                    ChallengeCard_Click(null, null);
                    break;
            }

            LeaderCard.Image = Properties.Resources.empty.ToBitmap();
            MonsterCard.Image = Properties.Resources.monster.ToBitmap();
            ItemCard.Image = Properties.Resources.itemIcon.ToBitmap();
            MagicCard.Image = Properties.Resources.magic.ToBitmap();
            ModifierCard.Image = Properties.Resources.modifier.ToBitmap();
            ChallengeCard.Image = Properties.Resources.challenge.ToBitmap();
        }

        #region Card Type Selection 
        // TODO: possibly make it one method
        // To anyone trying to understand the code below: I'm sorry, I'm not proud of it either. The lazynies is too great.
        private void LeaderCard_Click(object? sender, EventArgs? e)
        {
            if (LeaderCard.Checked == false)
            {
                Properties.Settings.Default.CardType = 0;
                Properties.Settings.Default.Save();

                if (chosenClass.SelectedIndex == -1)
                {
                    this.Icon = Properties.Resources.LEADER;
                }

                Uncheck();

                LeaderCard.Checked = true;
                LeaderCard.BackColor = SystemColors.ControlLight;

                chosenClass.Visible = true;
                labelClass.Visible = true;
                advancedClass.Visible = true;
                advancedClass.Image = Properties.Resources.closed;
                splitClass.Checked = false;
                itemChosenClass.Visible = false;

                advancedName.Visible = true;

                labelImg.Location = new Point(labelImg.Location.X, 329);
                selectImgText.Location = new Point(selectImgText.Location.X, 349);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 349);

                RENDER.Location = new Point(RENDER.Location.X, 525);

                labelReq.Visible = false;
                classReq1.Visible = false;
                classReq2.Visible = false;
                classReq3.Visible = false;
                classReq4.Visible = false;
                classReq5.Visible = false;

                labelBad.Visible = false;
                badOutputText.Visible = false;
                badOutputNum.Visible = false;
                badOutputSym.Visible = false;

                labelGood.Visible = false;
                goodOutputText.Visible = false;
                goodOutputNum.Visible = false;
                goodOutputSym.Visible = false;

                maxItems.Visible = false;
                labelMaxItem.Visible = false;
                itemImg.Visible = false;
                itemImg2.Visible = false;
                itemImgMore.Visible = false;

                descriptionText.Size = new Size(300, descriptionText.Size.Height);
                labelDescription.Location = new Point(53, 426);
                descriptionText.Location = new Point(57, 446);

                advancedGeneral.Visible = false;
                advancedGeneral.Image = Properties.Resources.closed;
                advancedGeneralBox.Visible = false;

                foreach (Control c in this.Controls)
                {
                    if (c.Name.Contains("clear")) { c.Visible = false; };
                }

                nameWhite.Checked = false;

                labelAdditionalReq.Visible = false;
                additionalReq.Visible = false;
                labelHeroBonus.Visible = false;
                heroBonus.Visible = false;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void MonsterCard_Click(object? sender, EventArgs? e)
        {
            if (MonsterCard.Checked == false)
            {
                Properties.Settings.Default.CardType = 1;
                Properties.Settings.Default.Save();

                this.Icon = Properties.Resources.monster;

                Uncheck();

                MonsterCard.Checked = true;
                MonsterCard.BackColor = SystemColors.ControlLight;

                chosenClass.Visible = false;
                labelClass.Visible = false;
                chosenSecondClass.Visible = false;
                clearSecondClass.Visible = false;
                labelSecondClass.Visible = false;
                advancedClass.Visible = false;
                advancedClassBox.Visible = false;
                itemChosenClass.Visible = false;

                advancedName.Visible = true;

                labelImg.Location = new Point(labelImg.Location.X, 229);
                selectImgText.Location = new Point(selectImgText.Location.X, 249);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 249);

                RENDER.Location = new Point(RENDER.Location.X, 591);

                labelReq.Visible = true;
                classReq1.Visible = true;
                classReq2.Visible = true;
                classReq3.Visible = true;
                classReq4.Visible = true;
                classReq5.Visible = true;

                labelBad.Visible = true;
                badOutputText.Visible = true;
                badOutputNum.Visible = true;
                badOutputSym.Visible = true;
                badOutputNum.Location = new Point(84, 425);
                badOutputSym.Location = new Point(118, 425);

                labelGood.Visible = true;
                goodOutputText.Visible = true;

                goodOutputNum.Visible = true;
                goodOutputSym.Visible = true;
                goodOutputNum.Location = new Point(84, 467);
                goodOutputSym.Location = new Point(118, 467);

                maxItems.Visible = false;
                labelMaxItem.Visible = false;
                itemImg.Visible = false;
                itemImg2.Visible = false;
                itemImgMore.Visible = false;

                descriptionText.Size = new Size(300, descriptionText.Size.Height);
                labelDescription.Location = new Point(53, 492);
                descriptionText.Location = new Point(57, 512);

                advancedGeneral.Visible = true;

                foreach (Control c in this.Controls)
                {
                    if (c.Name.Contains("clear") && c.Name != "clearSecondClass") { c.Visible = true; };
                }

                nameWhite.Checked = true;

                labelAdditionalReq.Visible = true;
                additionalReq.Visible = true;
                labelHeroBonus.Visible = true;
                heroBonus.Visible = true;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void HeroCard_Click(object? sender, EventArgs? e)
        {
            if (HeroCard.Checked == false)
            {
                Properties.Settings.Default.CardType = 2;
                Properties.Settings.Default.Save();

                if (chosenClass.SelectedIndex == -1)
                {
                    this.Icon = Properties.Resources.hero;
                }

                Uncheck();

                HeroCard.Checked = true;
                HeroCard.BackColor = SystemColors.ControlLight;

                chosenClass.Visible = true;
                labelClass.Visible = true;
                advancedClass.Visible = false;
                advancedClass.Image = Properties.Resources.closed;
                splitClass.Checked = false;
                advancedClassBox.Visible = false;
                itemChosenClass.Visible = false;

                advancedName.Visible = false;
                advancedName.Image = Properties.Resources.closed;
                advancedNameBox.Visible = false;

                labelImg.Location = new Point(labelImg.Location.X, 304);
                selectImgText.Location = new Point(selectImgText.Location.X, 324);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 324);

                RENDER.Location = new Point(RENDER.Location.X, 525);

                labelReq.Visible = false;
                classReq1.Visible = false;
                classReq2.Visible = false;
                classReq3.Visible = false;
                classReq4.Visible = false;
                classReq5.Visible = false;

                labelBad.Visible = false;
                badOutputText.Visible = false;
                badOutputNum.Visible = false;
                badOutputSym.Visible = false;

                labelGood.Visible = false;
                goodOutputText.Visible = false;

                goodOutputNum.Visible = true;
                goodOutputSym.Visible = true;
                goodOutputNum.Location = new Point(57, 466);
                goodOutputSym.Location = new Point(92, 466);

                maxItems.Visible = true;
                labelMaxItem.Visible = true;
                itemImg.Visible = true;

                descriptionText.Size = new Size(225, descriptionText.Size.Height);
                labelDescription.Location = new Point(128, 426);
                descriptionText.Location = new Point(132, 446);

                advancedGeneral.Visible = false;
                advancedGeneral.Image = Properties.Resources.closed;
                advancedGeneralBox.Visible = false;

                foreach (Control c in this.Controls)
                {
                    if (c.Name.Contains("clear")) { c.Visible = false; };
                }

                labelAdditionalReq.Visible = false;
                additionalReq.Visible = false;
                labelHeroBonus.Visible = false;
                heroBonus.Visible = false;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void ItemCard_Click(object? sender, EventArgs? e)
        {
            if (ItemCard.Checked == false)
            {
                Properties.Settings.Default.CardType = 3;
                Properties.Settings.Default.Save();

                if (chosenClass.SelectedIndex == -1)
                {
                    this.Icon = Properties.Resources.itemIcon;
                }

                Uncheck();

                LeaderCard.Checked = false;
                LeaderCard.BackColor = SystemColors.Control;
                MonsterCard.Checked = false;
                MonsterCard.BackColor = SystemColors.Control;
                HeroCard.Checked = false;
                HeroCard.BackColor = SystemColors.Control;
                ItemCard.Checked = true;
                ItemCard.BackColor = SystemColors.ControlLight;
                MagicCard.Checked = false;
                MagicCard.BackColor = SystemColors.Control;
                ModifierCard.Checked = false;
                ModifierCard.BackColor = SystemColors.Control;

                chosenClass.Visible = false;
                labelClass.Visible = true;
                advancedClass.Visible = false;
                advancedClass.Image = Properties.Resources.closed;
                advancedClassBox.Visible = false;
                splitClass.Checked = false;
                itemChosenClass.Visible = true;

                advancedName.Image = Properties.Resources.closed;
                advancedNameBox.Visible = false;
                advancedName.Visible = false;

                labelImg.Location = new Point(labelImg.Location.X, 329);
                selectImgText.Location = new Point(selectImgText.Location.X, 349);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 349);

                RENDER.Location = new Point(RENDER.Location.X, 525);

                labelReq.Visible = false;
                classReq1.Visible = false;
                classReq2.Visible = false;
                classReq3.Visible = false;
                classReq4.Visible = false;
                classReq5.Visible = false;

                labelBad.Visible = false;
                badOutputText.Visible = false;
                badOutputNum.Visible = false;
                badOutputSym.Visible = false;

                labelGood.Visible = false;
                goodOutputText.Visible = false;
                goodOutputNum.Visible = false;
                goodOutputSym.Visible = false;

                maxItems.Visible = false;
                labelMaxItem.Visible = false;
                itemImg.Visible = false;
                itemImg2.Visible = false;
                itemImgMore.Visible = false;

                descriptionText.Size = new Size(300, descriptionText.Size.Height);
                labelDescription.Location = new Point(53, 426);
                descriptionText.Location = new Point(57, 446);

                advancedGeneral.Visible = false;
                advancedGeneral.Image = Properties.Resources.closed;
                advancedGeneralBox.Visible = false;

                foreach (Control c in this.Controls)
                {
                    if (c.Name.Contains("clear")) { c.Visible = false; };
                }

                nameWhite.Checked = false;

                labelAdditionalReq.Visible = false;
                additionalReq.Visible = false;
                labelHeroBonus.Visible = false;
                heroBonus.Visible = false;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void MagicCard_Click(object? sender, EventArgs? e)
        {
            if (MagicCard.Checked == false)
            {
                Properties.Settings.Default.CardType = 4;
                Properties.Settings.Default.Save();

                this.Icon = Properties.Resources.magic;

                Uncheck();

                MagicCard.Checked = true;
                MagicCard.BackColor = SystemColors.ControlLight;

                chosenClass.Visible = false;
                labelClass.Visible = false;
                advancedClass.Visible = false;
                advancedClass.Image = Properties.Resources.closed;
                advancedClassBox.Visible = false;
                splitClass.Checked = false;
                itemChosenClass.Visible = false;

                advancedName.Image = Properties.Resources.closed;
                advancedNameBox.Visible = false;
                advancedName.Visible = false;

                labelImg.Location = new Point(labelImg.Location.X, 304);
                selectImgText.Location = new Point(selectImgText.Location.X, 324);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 324);

                RENDER.Location = new Point(RENDER.Location.X, 525);

                labelReq.Visible = false;
                classReq1.Visible = false;
                classReq2.Visible = false;
                classReq3.Visible = false;
                classReq4.Visible = false;
                classReq5.Visible = false;

                labelBad.Visible = false;
                badOutputText.Visible = false;
                badOutputNum.Visible = false;
                badOutputSym.Visible = false;

                labelGood.Visible = false;
                goodOutputText.Visible = false;
                goodOutputNum.Visible = false;
                goodOutputSym.Visible = false;

                maxItems.Visible = false;
                labelMaxItem.Visible = false;
                itemImg.Visible = false;
                itemImg2.Visible = false;
                itemImgMore.Visible = false;

                descriptionText.Size = new Size(300, descriptionText.Size.Height);
                labelDescription.Location = new Point(53, 426);
                descriptionText.Location = new Point(57, 446);

                advancedGeneral.Visible = false;
                advancedGeneral.Image = Properties.Resources.closed;
                advancedGeneralBox.Visible = false;

                foreach (Control c in this.Controls)
                {
                    if (c.Name.Contains("clear")) { c.Visible = false; };
                }

                nameWhite.Checked = false;

                labelAdditionalReq.Visible = false;
                additionalReq.Visible = false;
                labelHeroBonus.Visible = false;
                heroBonus.Visible = false;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void ModifierCard_Click(object? sender, EventArgs? e) 
        {
            if (ModifierCard.Checked == false) {
                Properties.Settings.Default.CardType = 5;
                Properties.Settings.Default.Save();

                this.Icon = Properties.Resources.modifier;

                Uncheck();

                ModifierCard.Checked = true;
                ModifierCard.BackColor = SystemColors.ControlLight;

                chosenClass.Visible = false;
                labelClass.Visible = false;
                advancedClass.Visible = false;
                advancedClass.Image = Properties.Resources.closed;
                advancedClassBox.Visible = false;
                splitClass.Checked = false;
                itemChosenClass.Visible = false;

                advancedName.Image = Properties.Resources.closed;
                advancedNameBox.Visible = false;
                advancedName.Visible = false;

                labelImg.Location = new Point(labelImg.Location.X, 304);
                selectImgText.Location = new Point(selectImgText.Location.X, 324);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 324);

                RENDER.Location = new Point(RENDER.Location.X, 525);

                labelReq.Visible = false;
                classReq1.Visible = false;
                classReq2.Visible = false;
                classReq3.Visible = false;
                classReq4.Visible = false;
                classReq5.Visible = false;

                labelBad.Visible = false;
                badOutputText.Visible = false;

                badOutputNum.Visible = true;
                badOutputSym.Visible = true;
                badOutputNum.Location = new Point(200, 398);
                badOutputSym.Location = new Point(234, 398);

                labelGood.Visible = false;
                goodOutputText.Visible = false;

                goodOutputNum.Visible = true;
                goodOutputSym.Visible = true;
                goodOutputNum.Location = new Point(120, 398);
                goodOutputSym.Location = new Point(154, 398);

                maxItems.Visible = false;
                labelMaxItem.Visible = false;
                itemImg.Visible = false;
                itemImg2.Visible = false;
                itemImgMore.Visible = false;

                descriptionText.Size = new Size(300, descriptionText.Size.Height);
                labelDescription.Location = new Point(53, 426);
                descriptionText.Location = new Point(57, 446);

                advancedGeneral.Visible = false;
                advancedGeneral.Image = Properties.Resources.closed;
                advancedGeneralBox.Visible = false;

                foreach (Control c in this.Controls) {
                    if (c.Name.Contains("clear")) { c.Visible = false; };
                }

                nameWhite.Checked = false;

                labelAdditionalReq.Visible = false;
                additionalReq.Visible = false;
                labelHeroBonus.Visible = false;
                heroBonus.Visible = false;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void ChallengeCard_Click(object? sender, EventArgs? e) {
            if (ChallengeCard.Checked == false) {
                Properties.Settings.Default.CardType = 6;
                Properties.Settings.Default.Save();

                this.Icon = Properties.Resources.challenge;

                Uncheck();

                ChallengeCard.Checked = true;
                ChallengeCard.BackColor = SystemColors.ControlLight;

                chosenClass.Visible = false;
                labelClass.Visible = false;
                advancedClass.Visible = false;
                advancedClass.Image = Properties.Resources.closed;
                advancedClassBox.Visible = false;
                splitClass.Checked = false;
                itemChosenClass.Visible = false;

                advancedName.Image = Properties.Resources.closed;
                advancedNameBox.Visible = false;
                advancedName.Visible = false;

                labelImg.Location = new Point(labelImg.Location.X, 304);
                selectImgText.Location = new Point(selectImgText.Location.X, 324);
                selectImgButton.Location = new Point(selectImgButton.Location.X, 324);

                RENDER.Location = new Point(RENDER.Location.X, 525);

                labelReq.Visible = false;
                classReq1.Visible = false;
                classReq2.Visible = false;
                classReq3.Visible = false;
                classReq4.Visible = false;
                classReq5.Visible = false;

                labelBad.Visible = false;
                badOutputText.Visible = false;
                badOutputNum.Visible = false;
                badOutputSym.Visible = false;
                labelGood.Visible = false;
                goodOutputText.Visible = false;
                goodOutputNum.Visible = false;
                goodOutputSym.Visible = false;

                maxItems.Visible = false;
                labelMaxItem.Visible = false;
                itemImg.Visible = false;
                itemImg2.Visible = false;
                itemImgMore.Visible = false;

                descriptionText.Size = new Size(300, descriptionText.Size.Height);
                labelDescription.Location = new Point(53, 426);
                descriptionText.Location = new Point(57, 446);

                advancedGeneral.Visible = false;
                advancedGeneral.Image = Properties.Resources.closed;
                advancedGeneralBox.Visible = false;

                foreach (Control c in this.Controls) {
                    if (c.Name.Contains("clear")) { c.Visible = false; };
                }

                nameWhite.Checked = false;

                labelAdditionalReq.Visible = false;
                additionalReq.Visible = false;
                labelHeroBonus.Visible = false;
                heroBonus.Visible = false;

                this.Update();
                updateLanguage(sender, e);
                this.Update();

                renderNow(sender, e, null);
                previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png");
                previewImg.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
        //This is here so that I dont have to set everything im not using to false
        private void Uncheck() 
        {
            LeaderCard.Checked = false;
            LeaderCard.BackColor = SystemColors.Control;
            MonsterCard.Checked = false;
            MonsterCard.BackColor = SystemColors.Control;
            HeroCard.Checked = false;
            HeroCard.BackColor = SystemColors.Control;
            ItemCard.Checked = false;
            ItemCard.BackColor = SystemColors.Control;
            MagicCard.Checked = false;
            MagicCard.BackColor = SystemColors.Control;
            ModifierCard.Checked = false;
            ModifierCard.BackColor = SystemColors.Control;
            ChallengeCard.Checked = false;
            ChallengeCard.BackColor = SystemColors.Control;
        }
        #endregion

        #region Direct Rendering (Preview and Save)
        private void RenderButton_Press(object sender, EventArgs e)
        {
            using SaveFileDialog SaveRenderDialog = new();
            SaveRenderDialog.Filter = "Supported Image Files (*.png;)|*.png;|All Files (*.*)|*.*";
            SaveRenderDialog.FilterIndex = 1;

            if (SaveRenderDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = SaveRenderDialog.FileName;
                renderNow(sender, e, filePath);

                previewImg.ImageLocation = filePath;
            }
        }

        private CancellationTokenSource? cancellationTokenSource;
        private async void renderPreview(object? sender, EventArgs? e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                int timer = 1;
                while (timer > 0)
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    await Task.Delay(200, cancellationTokenSource.Token);
                    timer--;
                }
                if (timer >= 0)
                {
                    renderNow(sender, e, null);
                    previewImg.ImageLocation = Path.Combine(Directory.GetCurrentDirectory(), "preview.png"); ;
                }
            }
            catch (OperationCanceledException) { }
        }

        private void renderNow(object? sender, EventArgs? e, string? saveLocation)
        {
            Backend.RollOutput good;
            Backend.RollOutput bad;
            switch (Properties.Settings.Default.CardType)
            {
                // Leader
                case 0:
                    GeneratorBackend.Backend.Program.GenerateLeader(saveLocation, language.SelectedIndex, nameText.Text, new int[] { chosenClass.SelectedIndex, chosenSecondClass.SelectedIndex }, selectImgText.Text, descriptionText.Text, gradient.Checked, nameWhite.Checked);
                    break;
                // Monster
                case 1:
                    good = new((int)goodOutputNum.Value, goodOutputSym.SelectedIndex, goodOutputText.Text);
                    bad = new((int)badOutputNum.Value, badOutputSym.SelectedIndex, badOutputText.Text);
                    int[] desiredRequirements = new int[] { classReq1.SelectedIndex, classReq2.SelectedIndex, classReq3.SelectedIndex, classReq4.SelectedIndex, classReq5.SelectedIndex };
                    string adReq = labelAdditionalReq.Checked ? additionalReq.Text : "";
                    string hBon = labelHeroBonus.Checked ? heroBonus.Text : "";

                    GeneratorBackend.Backend.Program.GenerateMonster(saveLocation, language.SelectedIndex, nameText.Text, desiredRequirements, selectImgText.Text, good, bad, descriptionText.Text, adReq, hBon, gradient.Checked, nameWhite.Checked, alternativeColor.Checked);
                    break;
                // Hero
                //Todo 
                case 2:
                    Backend.RollOutput description = new((int)goodOutputNum.Value, goodOutputSym.SelectedIndex, descriptionText.Text);
                    GeneratorBackend.Backend.Program.GenerateHero(saveLocation, language.SelectedIndex, nameText.Text, chosenClass.SelectedIndex, selectImgText.Text, description, (int)maxItems.Value);
                    break;
                // Item
                case 3:
                    GeneratorBackend.Backend.Program.GenerateItem(saveLocation, language.SelectedIndex, nameText.Text, itemChosenClass.SelectedIndex, selectImgText.Text, descriptionText.Text);
                    break;
                // Magic
                case 4:
                    GeneratorBackend.Backend.Program.GenerateMagic(saveLocation, language.SelectedIndex, nameText.Text, selectImgText.Text, descriptionText.Text);
                    break;
                // Modifier
                case 5:
                    good = new((int)goodOutputNum.Value, goodOutputSym.SelectedIndex, goodOutputText.Text);
                    bad = new((int)badOutputNum.Value, badOutputSym.SelectedIndex, badOutputText.Text);

                    GeneratorBackend.Backend.Program.GenerateModifier(saveLocation, language.SelectedIndex, nameText.Text, selectImgText.Text, good, bad, descriptionText.Text);
                    break;
                // Challenge
                case 6:
                    GeneratorBackend.Backend.Program.GenerateChallenge(saveLocation, language.SelectedIndex, nameText.Text, selectImgText.Text, descriptionText.Text);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Language
        private void language_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateLanguage(sender, e);
            renderPreview(sender, e);
        }

        private void updateLanguage(object? sender, EventArgs? e)
        {
            Properties.Settings.Default.Language = language.SelectedIndex;
            Properties.Settings.Default.Save();

            #region old language code
            string toolTipImageDimentions;

            if (tounges[language.SelectedIndex].lang_code == "pl")
            {
                // Polish
                logo.Image = Properties.Resources.Logo1;
                labelLeader.Text = Properties.Settings.Default.CardType switch
                {
                    1 => "Nazwa potwora",
                    2 => "Nazwa bohatera",
                    3 => "Nazwa przedmiotu",
                    4 => "Nazwa magii",
                    _ => "Nazwa przywódcy"
                };
                labelClass.Text = Properties.Settings.Default.CardType switch
                {
                    2 => "Klasa bohatera",
                    3 => "Klasa przedmiotu",
                    _ => "Klasa przywódcy"
                };
                labelSecondClass.Text = "Druga klasa";
                labelImg.Text = Properties.Settings.Default.CardType switch
                {
                    1 => "Obrazek potwora",
                    2 => "Obrazek bohatera",
                    3 => "Obrazek przedmiotu",
                    4 => "Obrazek magii",
                    _ => "Obrazek przywódcy"
                };
                labelDescription.Text = "Opis";
                leaderImgToolTip.ToolTipTitle = "Wymiary obazka";
                toolTipImageDimentions = Properties.Settings.Default.CardType switch
                {
                    1 => "Obrazek potwora (nie ca³a karta) ma wymiary 745x817.",
                    2 => "Obrazek bohatera (nie ca³a karta) ma wymiary 545x545.",
                    3 => "Obrazek przedmiotu (nie ca³a karta) ma wymiary 545x545.",
                    4 => "Obrazek magii (nie ca³a karta) ma wymiary 545x545.",
                    _ => "Obrazek przywódcy (nie ca³a karta) ma wymiary 745x1176.",
                };
                leaderImgToolTip.SetToolTip(selectImgButton, toolTipImageDimentions + "\nProgram automatycznie przytnie i przybli¿y obraz, je¿eli bêdzie to potrzebne.\n\nWspierane rozszerzenia plików:\n.png, .jpeg, .jpg, .gif(pierwsza klatka), .bmp, .webp, .pbm, .tiff, .tga");

                gradient.Text = "Tylni gradient";
                nameWhite.Text = "Bia³a nazwa";
                splitClass.Text = "Podwójna Klasa";
                this.Text = "To ja go tnê - Generator kart";
                labelBad.Text = "Wymagania rzutu - Pora¿ka";
                labelGood.Text = "Wymagania rzutu - UBIJ potwora";
                goodOutputText.Text = "UBIJ tego potwora";
                labelReq.Text = "Wymagania klas";
                RENDER.Text = "ZAPISZ OBRAZ";
                copyImageToClipboardToolStripMenuItem.Text = "Kopiuj obraz";
                openImageLocationToolStripMenuItem.Text = "Otwórz lokalizacjê obrazu";
                labelMaxItem.Text = "Max. iloæ przedmiotów";
                alternativeColor.Text = "Alternatywny kolor (?)";
                altColorToolTip.ToolTipTitle = "Alternatywny kolor";
                altColorToolTip.SetToolTip(alternativeColor, "Na niektórych drukarkach, standardowy kolor mo¿e znacznie odstawiaæ od po¿¹danego.\nStandardowy kolor by³ wziêty prosto z instruckji, wiêc powinnien byæ dobry,\nale niektóre durkarki nie maj¹ poprawnej g³ebi kolorów.\n\nAlternatywny kolor (czarny) mo¿e wygl¹daæ lepiej na niektórych drukarkach.");

                if (chosenClass.SelectedIndex == -1 && Properties.Settings.Default.CardType == 2)
                {
                    this.Icon = Properties.Resources.bohater;
                }
                HeroCard.Image = Properties.Resources.bohater.ToBitmap();

                cardType.Text = "Typ karty";
                LeaderCard.Text = "Przywódca";
                MonsterCard.Text = "Potwór";
                HeroCard.Text = "Bohater";
                ItemCard.Text = "Przedmiot";
                MagicCard.Text = "Magia";

                labelAdditionalReq.Text = "Dodatkowe wymagania";
                additionalReq.Text = "ODRZUÆ X kart";
                labelHeroBonus.Text = "Bonus dodatkowych boh.";
                heroBonus.Text = "Za ka¿dego dodatkowego bohatera w twojej dru¿ynie, +X do twojego rzutu";
            }
            else
            {
                // English
                logo.Image = Properties.Resources.Logo0;
                labelLeader.Text = Properties.Settings.Default.CardType switch
                {
                    1 => "Monster name",
                    2 => "Hero name",
                    3 => "Item name",
                    4 => "Magic name",
                    5 => "Modifier name",
                    6 => "Challenge name",
                    _ => "Leader name"
                };
                labelClass.Text = Properties.Settings.Default.CardType switch
                {
                    2 => "Hero class",
                    3 => "Item class",
                    _ => "Leader class"
                };
                labelSecondClass.Text = "Second class";
                labelImg.Text = Properties.Settings.Default.CardType switch
                {
                    1 => "Monster image",
                    2 => "Hero image",
                    3 => "Item image",
                    4 => "Magic image",
                    5 => "Modifier image",
                    6 => "Challenge image",
                    _ => "Leader image"
                };
                labelDescription.Text = "Description";
                leaderImgToolTip.ToolTipTitle = "Image dimentions";
                toolTipImageDimentions = Properties.Settings.Default.CardType switch
                {
                    1 => "The monster image (not the whole card) dimentions are 745x817.",
                    2 => "The hero image (not the whole card) dimentions are 545x545.",
                    3 => "The item image (not the whole card) dimentions are 545x545.",
                    4 => "The magic image (not the whole card) dimentions are 545x545.",
                    5 => "The modifier image (not the whole card) dimentions are 545x545.",
                    6 => "The challenge image (not the whole card) dimentions are 645x550.",
                    _ => "The leader image (not the whole card) dimentions are 745x1176.",
                };
                leaderImgToolTip.SetToolTip(selectImgButton, toolTipImageDimentions + "\nThe program will automatically crop and zoom the image, if needed.\n\nSupported file extensions:\n.png, .jpeg, .jpg, .gif(first frame), .bmp, .webp, .pbm, .tiff, .tga");

                gradient.Text = "Back gradient";
                nameWhite.Text = "White name";
                splitClass.Text = "Split Class";
                this.Text = "Here to Slay - Card generator";
                labelBad.Text = "Roll Requirements - Fail";
                labelGood.Text = "Roll Requirements - SLAY monster";
                goodOutputText.Text = "SLAY this Monster card";
                labelReq.Text = "Class Requirements";
                RENDER.Text = "SAVE IMAGE";
                copyImageToClipboardToolStripMenuItem.Text = "Copy image";
                openImageLocationToolStripMenuItem.Text = "Open image location";
                labelMaxItem.Text = "Max Item Ammount";
                alternativeColor.Text = "Alternative Color (?)";
                altColorToolTip.ToolTipTitle = "Alternative Color";
                altColorToolTip.SetToolTip(alternativeColor, "On some printers, the standard color might largely differ from the disered one.\nThe standard color is taken straight from the manual, so it should be good,\nbut some printers don't have a sufficient color depth.\n\nThe alternative color (black), might look better on some printers.");

                if (chosenClass.SelectedIndex == -1 && Properties.Settings.Default.CardType == 2)
                {
                    this.Icon = Properties.Resources.hero;
                }
                HeroCard.Image = Properties.Resources.hero.ToBitmap();

                cardType.Text = "Card Type";
                LeaderCard.Text = "Leader";
                MonsterCard.Text = "Monster";
                HeroCard.Text = "Hero";
                ItemCard.Text = "Item";
                MagicCard.Text = "Magic";
                ModifierCard.Text = "Modifier";
                ChallengeCard.Text = "Challenge";

                additionalReq.Text = "Addidional Requirements";
                additionalReq.Text = "DISCARD X cards";
                labelHeroBonus.Text = "Additional Hero Bonus";
                heroBonus.Text = "For each additional Hero card in your Party, +X to your roll.";
            }
            #endregion
            LocaliseClassOptions(language.SelectedIndex); // change class options based on selected language
        }

        // These are made so that after changing the language, the selected classes stay the same.
        // TODO: reduce this all to a single operation              NO. useless. 
        int currentClassIndex;
        int currentSecondClassIndex;
        int currentItemClassIndex;
        int currentHeroReq1Index;
        int currentHeroReq2Index;
        int currentHeroReq3Index;
        int currentHeroReq4Index;
        int currentHeroReq5Index;
        private void LocaliseClassOptions(int lang)
        {
            currentClassIndex = chosenClass.SelectedIndex;
            chosenClass.Items.Clear();

            currentSecondClassIndex = chosenSecondClass.SelectedIndex;
            chosenSecondClass.Items.Clear();

            currentItemClassIndex = itemChosenClass.SelectedIndex;
            itemChosenClass.Items.Clear();

            currentHeroReq1Index = classReq1.SelectedIndex;
            classReq1.Items.Clear();
            currentHeroReq2Index = classReq2.SelectedIndex;
            classReq2.Items.Clear();
            currentHeroReq3Index = classReq3.SelectedIndex;
            classReq3.Items.Clear();
            currentHeroReq4Index = classReq4.SelectedIndex;
            classReq4.Items.Clear();
            currentHeroReq5Index = classReq5.SelectedIndex;
            classReq5.Items.Clear();

            // These have to be hardcoded mainly because they each use an icon from the resources so it looks better in the combobox.
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[0], Properties.Resources.empty.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[1], Properties.Resources.lowca.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[2], Properties.Resources.mag.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[3], Properties.Resources.najebus.ToBitmap())); //przewybornie
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[4], Properties.Resources.straznik.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[5], Properties.Resources.wojownik.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[6], Properties.Resources.zlodziej.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[7], Properties.Resources.druid.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[8], Properties.Resources.awanturnik.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[9], Properties.Resources.berserk.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[10], Properties.Resources.nekromanta.ToBitmap()));
            chosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].class_name[11], Properties.Resources.czarownik.ToBitmap()));

            //Add custom classes
            inst.ClassList.Skip(12).ToList().ForEach(c =>
            {
                string capitalisedName = char.ToUpper(c.Name[0]) + c.Name.Substring(1);

                // Load class icon
                Bitmap classIcon = new(1, 1);
                if (File.Exists(c.ImagePath))                                   //temporarily disabled, probably doesn't work now
                {
                    classIcon = new Bitmap(c.ImagePath);
                }
                else
                {
                    if (initLang == true) // don't show this error when first loading the program (it would show up twice, due to lazy programming)
                    {
                        MessageBox.Show($"Error loading icon for the {c.Name} class.\nCould not find image in {c.ImagePath}.\n\nClass icon will be left empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                chosenClass.Items.Add(new ImageCBox(capitalisedName, new Bitmap(1, 1)));
            });

            // Add monster related options
            classReq1.Items.Add(new ImageCBox("HERO", Properties.Resources.hero.ToBitmap()));
            classReq2.Items.Add(new ImageCBox("HERO", Properties.Resources.hero.ToBitmap()));
            classReq3.Items.Add(new ImageCBox("HERO", Properties.Resources.hero.ToBitmap()));
            classReq4.Items.Add(new ImageCBox("HERO", Properties.Resources.hero.ToBitmap()));
            classReq5.Items.Add(new ImageCBox("HERO", Properties.Resources.hero.ToBitmap()));

            // Add item related options
            itemChosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].card_item_label, Properties.Resources.itemIcon.ToBitmap()));
            itemChosenClass.Items.Add(new ImageCBox(tounges[language.SelectedIndex].card_item_cursed, Properties.Resources.cursed.ToBitmap()));

            foreach (var item in chosenClass.Items)
            {
                chosenSecondClass.Items.Add(item);
                classReq1.Items.Add(item);
                classReq2.Items.Add(item);
                classReq3.Items.Add(item);
                classReq4.Items.Add(item);
                classReq5.Items.Add(item);
                itemChosenClass.Items.Add(item);
            }

            chosenClass.SelectedIndex = currentClassIndex;
            chosenSecondClass.SelectedIndex = currentSecondClassIndex;

            itemChosenClass.SelectedIndex = currentItemClassIndex;

            classReq1.SelectedIndex = currentHeroReq1Index;
            classReq2.SelectedIndex = currentHeroReq2Index;
            classReq3.SelectedIndex = currentHeroReq3Index;
            classReq4.SelectedIndex = currentHeroReq4Index;
            classReq5.SelectedIndex = currentHeroReq5Index;
        }
        #endregion

        #region Class Related Methods
        private void chosenClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            updateIcon_to_chosenClass(sender, e);
            renderPreview(sender, e);
        }

        private void updateIcon_to_chosenClass(object? sender, EventArgs? e)
        {
            if (Properties.Settings.Default.CardType != 1 && Properties.Settings.Default.CardType != 4)
            {
                int iconIndex;
                if (Properties.Settings.Default.CardType == 3)
                {
                    // This is to handle Item and Cursed Item icons, while still using the normal class icon list.
                    iconIndex = itemChosenClass.SelectedIndex == 0 || itemChosenClass.SelectedIndex == 1 ? itemChosenClass.SelectedIndex + 100 : itemChosenClass.SelectedIndex - 2;
                }
                else
                {
                    iconIndex = chosenClass.SelectedIndex;
                }

                this.Icon = iconIndex switch
                {
                    0 => Properties.Resources.empty,
                    1 => Properties.Resources.lowca,
                    2 => Properties.Resources.mag,
                    3 => Properties.Resources.najebus, // wspaniale
                    4 => Properties.Resources.straznik,
                    5 => Properties.Resources.wojownik,
                    6 => Properties.Resources.zlodziej,
                    7 => Properties.Resources.druid,
                    8 => Properties.Resources.awanturnik,
                    9 => Properties.Resources.berserk,
                    10 => Properties.Resources.nekromanta,
                    11 => Properties.Resources.czarownik,
                    100 => Properties.Resources.itemIcon,
                    101 => Properties.Resources.cursed,
                    _ => Properties.Resources.LEADER
                };
            }
        }

        private void splitClass_CheckStateChanged(object sender, EventArgs e)
        {
            switch (splitClass.Checked)
            {
                case false:
                    chosenSecondClass.Visible = false;
                    labelSecondClass.Visible = false;
                    clearSecondClass.Visible = false;
                    chosenClass.Location = new Point(chosenClass.Location.X + 63, chosenClass.Location.Y);
                    labelClass.Location = new Point(labelClass.Location.X + 63, labelClass.Location.Y);
                    chosenSecondClass.SelectedIndex = -1;
                    break;
                case true:
                    chosenClass.Location = new Point(chosenClass.Location.X - 63, chosenClass.Location.Y);
                    labelClass.Location = new Point(labelClass.Location.X - 63, labelClass.Location.Y);
                    chosenSecondClass.Visible = true;
                    labelSecondClass.Visible = true;
                    clearSecondClass.Visible = true;
                    break;
            }
        }

        private void clearSelectedClass(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "clearSecondClass":
                        chosenSecondClass.SelectedIndex = -1;
                        break;
                    case "clearClassReq1":
                        classReq1.SelectedIndex = -1;
                        break;
                    case "clearClassReq2":
                        classReq2.SelectedIndex = -1;
                        break;
                    case "clearClassReq3":
                        classReq3.SelectedIndex = -1;
                        break;
                    case "clearClassReq4":
                        classReq4.SelectedIndex = -1;
                        break;
                    case "clearClassReq5":
                        classReq5.SelectedIndex = -1;
                        break;
                }
            }
        }
        #endregion

        #region UI Elements
        private void advanced_Click(object sender, EventArgs e)
        {
            PictureBox? pictureBox = sender as PictureBox ?? throw new NotImplementedException();
            FlowLayoutPanel list = pictureBox.Name switch
            {
                "advancedName" => advancedNameBox,
                "advancedClass" => advancedClassBox,
                "advancedGeneral" => advancedGeneralBox,
                _ => throw new NotImplementedException(),
            };
            list.Visible = !list.Visible;
            pictureBox.Image = list.Visible switch
            {
                true => Properties.Resources.open,
                false => Properties.Resources.closed
            };
        }

        private void selectImg_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Select a File";
            openFileDialog.Filter = "Supported Image Files (*.png;*.jpeg;*.jpg;*.gif;*.bmp;*.webp;*.pbm;*.tiff;*.tga;)|*.png;*.jpeg;*.jpg;*.gif;*.bmp;*.webp;*.pbm;*.tiff;*.tga;|All Files (*.*)|*.*"; ;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;
                selectImgText.Text = selectedFilePath;
            }
        }

        private void previewImg_Click(object sender, EventArgs e)
        {
            if (previewImg != null)
            {
                string previewImgPath = previewImg.ImageLocation;
                string? folderPath = Path.GetDirectoryName(previewImgPath);
                if (folderPath != null)
                {
                    Process.Start("explorer.exe", folderPath);
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(new ProcessStartInfo { FileName = "https://github.com/Danrejk", UseShellExecute = true }); }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { System.Diagnostics.Process.Start(new ProcessStartInfo { FileName = "https://github.com/Beukot", UseShellExecute = true }); }

        private void copyImageToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(previewImg.Image);
        }

        private void OutputSym_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.BackColor = comboBox.SelectedIndex switch
                {
                    0 => Color.FromArgb(109, 166, 88),
                    1 => Color.FromArgb(230, 44, 47),
                    _ => throw new NotImplementedException(),
                };

                switch (comboBox.Name)
                {
                    case "goodOutputSym":
                        badOutputSym.SelectedIndex = goodOutputSym.SelectedIndex switch
                        {
                            0 => 1,
                            _ => 0,
                        };
                        break;
                    case "badOutputSym":
                        goodOutputSym.SelectedIndex = badOutputSym.SelectedIndex switch
                        {
                            0 => 1,
                            _ => 0,
                        };
                        break;
                }
            }
            renderPreview(sender, e);
        }

        private void maxItems_ValueChanged(object sender, EventArgs e)
        {
            switch (maxItems.Value)
            {
                case 0:
                    itemImg.Image = Properties.Resources.noItem;
                    itemImg2.Visible = false;
                    itemImgMore.Visible = false;
                    break;
                case 1:
                    itemImg.Image = Properties.Resources.item;
                    itemImg2.Visible = false;
                    itemImgMore.Visible = false;
                    break;
                case 2:
                    itemImg.Image = Properties.Resources.item;
                    itemImg2.Visible = true;
                    itemImgMore.Visible = false;
                    break;
                default:
                    itemImg.Image = Properties.Resources.item;
                    itemImg2.Visible = true;
                    itemImgMore.Visible = true;
                    break;
            }
            itemImg.Image = maxItems.Value switch
            {
                0 => Properties.Resources.noItem,
                _ => Properties.Resources.item,
            };

            renderPreview(sender, e);
        }
        private void labelAdditionalReq_CheckedChanged(object sender, EventArgs e)
        {
            additionalReq.Enabled = labelAdditionalReq.Checked;

            classReq4.Enabled = !labelAdditionalReq.Checked;
            classReq5.Enabled = !labelAdditionalReq.Checked;

            clearClassReq4.Enabled = !labelAdditionalReq.Checked;
            clearClassReq5.Enabled = !labelAdditionalReq.Checked;

            classReq4.SelectedIndex = -1;
            classReq5.SelectedIndex = -1;

            renderPreview(sender, e);
        }

        private void labelHeroBonus_CheckedChanged(object sender, EventArgs e)
        {
            heroBonus.Enabled = labelHeroBonus.Checked;
            renderPreview(sender, e);
        }
    }
    #endregion
}
