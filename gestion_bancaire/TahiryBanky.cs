using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// Pour utilser une base de donnée MySQL : 
using MySql.Data.MySqlClient;

// Pour manipuler le fichier : (copie de l'image du client)
using System.IO; 

namespace gestion_bancaire
{
    public partial class TahiryBanky : Form
    {
        public TahiryBanky()
        {
            InitializeComponent();
        }

        private string cheminImage; 
       

        private void ajouter_Click(object sender, EventArgs e)
        {
            // Récupération du valeur du boutton radio (type de compte)
            string type_compte = "*";
            if (compte_courant.Checked.ToString() == "True")
            {
                type_compte = "courant";
            }
            else
            {
                type_compte = "epargne"; 
            }
            
            Client nouveauClient = new Client(nom.Text, adresse.Text, numero_telephone.Text, date_naissance.Value.ToString("yyyy-MM-dd"), cheminImage, date_inscription.Value.ToString("yyyy-MM-dd"));
            nouveauClient.ajouter(type_compte);

            vider_champs_inscription(); 
        }

        private void vider_champs_inscription()
        {
            nom.Text = "";
            adresse.Text = "";
            numero_telephone.Text = "";
            date_naissance.ResetText();
            date_inscription.ResetText(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            vider_champs_inscription(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFIle = new OpenFileDialog();

            openFIle.Filter = "Fichier image|*.jpg;*.jpeg;*.png|Tous les fichiers|*.*";

            if (openFIle.ShowDialog() == DialogResult.OK)
            {
                string chemin = @openFIle.FileName;
                Random aleatoire = new Random(); 

                // + Gestion nom du fichier unique (milliseconde + random) : 
                string destinationImage = @"C:\Users\" + Environment.UserName + @"\Documents\gestion_bancaire\photos_clients\IMG" + DateTime.Now.Millisecond.ToString() + aleatoire.Next(0, 1000).ToString() + ".jpg"; 

                // MessageBox.Show(destinationImage); 

                try
                {
                    File.Copy(chemin, destinationImage);
                    MessageBox.Show("Le fichier a été copier dans le dossier du projet");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Une erreur s'est produit" + ex.Message); 
                }

                // Modifier \ en \\ sur le chemin absolue (pour être accepté en syntaxe SQL) :  
                cheminImage = destinationImage.Replace("\\", "\\\\"); 
            }
        }
        
        // Boutton menu Inscription 
        private void creation_compte_Click(object sender, EventArgs e)
        {
            ajouter_client.Visible = true;
            retrait.Visible = false;
            versement.Visible = false; 
        }

        // Boutton menu (Faire retrait)
        private void faire_retrait_Click(object sender, EventArgs e)
        {
            ajouter_client.Visible = true; 
            retrait.Visible = true;
            versement.Visible = false;
            consultation_solde.Visible = false;
        }

        // Boutton de validation du retrait : 
        private void valider_retrait_Click(object sender, EventArgs e)
        {
            Compte compteClient = new Compte(Convert.ToInt32(numero_compte.Text));
             
            compteClient.effectuer_une_transaction("retrait", Convert.ToDouble(montant_retirer.Text)); 
           
        }

        // Boutton menu (Faire versement) 
        private void faire_versement_Click(object sender, EventArgs e)
        {
            ajouter_client.Visible = true;
            retrait.Visible = true;
            versement.Visible = true;
            consultation_solde.Visible = false; 
        }

        private void valider_versement_Click(object sender, EventArgs e)
        {
            Compte compteClient = new Compte(Convert.ToInt32(numero_compte_v.Text));

            compteClient.effectuer_une_transaction("versement", Convert.ToDouble(montant_verser.Text)); 
        }

        // Boutton de validation dans Information Compte : 
        private void valider_information_compte_Click(object sender, EventArgs e)
        {
            Compte compteClient = new Compte(Convert.ToInt32(num_compte_info.Text)); 
            List<string> info = compteClient.recuperationInformationSupplementaire(); 

            // On affiche les informations du client dans l'interface : 
            nom_info.Text = info[0];
            date_n_info.Text = info[1];
            tel_info.Text = info[2];
            adresse_info.Text = info[3];
            montant_info.Text = info[4];
            type_info.Text = info[5]; 
        }

        // Boutton consulter (menu)
        private void consulter_Click(object sender, EventArgs e)
        {
            ajouter_client.Visible = true;
            retrait.Visible = true;
            versement.Visible = true;
            consultation_solde.Visible = true; 
        }

        

        

        

        

    }
}
