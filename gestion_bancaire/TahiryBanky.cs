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

// Pour manipuler un fichier : (copie de l'image du client)
using System.IO; 

namespace gestion_bancaire
{
    public partial class TahiryBanky : Form
    {
        public TahiryBanky()
        {
            InitializeComponent();
            fenetre_resultat_recherche = new resultat_recherche(); 
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
            List<string> info = compteClient.effectuer_une_transaction("retrait", Convert.ToDouble(montant_retirer.Text));

            Facture fc = new Facture();
            fc.GenererPDF(@"C:\Users\Nambinintsoa\Downloads\facture_retrait.pdf", info); 
           
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
            List<string> info = new List<string>(); 
            info = compteClient.effectuer_une_transaction("versement", Convert.ToDouble(montant_verser.Text));

            // Générer la facture en PDF : 
            Facture fc = new Facture();
            fc.GenererPDF(@"C:\Users\Nambinintsoa\Downloads\facture_versement.pdf", info); 
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
            photo_client_consulter.ImageLocation = info[6];

            photo_client_consulter.Visible = true; 
            conteneur_informations_consulter.Visible = true;
        }

        // Boutton consulter (menu)
        private void consulter_Click(object sender, EventArgs e)
        {
            ajouter_client.Visible = true;
            retrait.Visible = true;
            versement.Visible = true;
            
            // bug résolu : photo toujours affiche lorsqu'on accède a nouveau "Consulter le compte"
            if(photo_client_consulter.Visible)
                photo_client_consulter.Visible = false;

            if (conteneur_informations_consulter.Visible)
                conteneur_informations_consulter.Visible = false; 

            consultation_solde.Visible = true;
            liste.Visible = false; 

            // Puis on vide le champs (de la recherche précédente) 
            num_compte_info.Text = ""; 
        }


        // Boutton menu lister client : 
        private void lister_client_Click(object sender, EventArgs e)
        {
            
            string requette = "SELECT id, nom, telephone, adresse, date_naissance, date_entree FROM client";

            Base_de_donnee bdd = new Base_de_donnee();
            bdd.lire_BDD(requette, liste_client_lister); 
 
            // Puis on affiche le pannel correspondant : 
            ajouter_client.Visible = true;
            retrait.Visible = true;
            versement.Visible = true;
            consultation_solde.Visible = true;
            liste.Visible = true;
            modifier_client.Visible = false; 

            // On masque le conteneur d'information textuel du Liste des Client (pannel)
            conteneur_informations_consulter.Visible = false; 

            // Puis le photo de profil du Client : 
            photo_client_consulter.Visible = false; 

            // Puis le resultat de recherche DataGridView de la fonctionnalité rechercher : 
            // resultat_recherche.Visible = false; 
        }

        private void valider_modification_client_Click(object sender, EventArgs e)
        {
            if (num_bancaire_modif.Text != "")
            {
                Client client = new Client(Convert.ToInt32(num_bancaire_modif.Text));
                client.modifier(nom_modif.Text,adresse_modif.Text, tel_modif.Text); 
            }

            // Puis on vide les champs : 
            num_bancaire_modif.Text = "";
            nom_modif.Text = ""; 
            adresse_modif.Text = "";
            tel_modif.Text = ""; 
        }

        private void modifier_client_menu_Click(object sender, EventArgs e)
        {
            // Affichage panel : 
            ajouter_client.Visible = true;
            retrait.Visible = true;
            versement.Visible = true;
            consultation_solde.Visible = true;
            liste.Visible = true;
            modifier_client.Visible = true;
            rechercher.Visible = false; 
        }

        // Recherher (boutton menu) 
        private void rechercher_menu_Click(object sender, EventArgs e)
        {
            ajouter_client.Visible = true;
            retrait.Visible = true;
            versement.Visible = true;
            consultation_solde.Visible = true;
            liste.Visible = true;
            modifier_client.Visible = true;
            rechercher.Visible = true;

            // resultat_recherche.Visible = true;
        }

        private void valider_recherche_Click(object sender, EventArgs e)
        {
            // Recuperer valeur de chaque champs : 
            string date_debut = d_debut.Value.ToString("yyyy-MM-dd");
            string date_fin = d_fin.Value.ToString("yyyy-MM-dd");
            string type_recherche = "";
            string requette = ""; 

            if (recherche_client.Checked.ToString() == "True")
                type_recherche = "client";
            else if (recherche_versement.Checked.ToString() == "True")
                type_recherche = "versement";
            else
                type_recherche = "retrait";

            if (type_recherche == "client")
            {
                // On récupères les informations dans la table client : 
                requette = "SELECT id, nom, telephone, adresse, date_naissance, date_entree FROM client WHERE date_entree BETWEEN '" + date_debut + "' AND '" + date_fin + "';";  
            }
            else
            {
                requette = "SELECT * FROM transaction WHERE type_transaction='" + type_recherche + "' AND date_transaction BETWEEN '" + date_debut + "' AND '" + date_fin + "';";
            }

            fenetre_resultat_recherche.generer(requette);
            fenetre_resultat_recherche.Show(); 
            
        }

    }
}
