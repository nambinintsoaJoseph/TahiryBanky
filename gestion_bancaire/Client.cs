using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace gestion_bancaire
{
    class Client : Base_de_donnee
    {
        protected int id_client;
        private string nom_client;
        private string adresse_client;
        private string telephone_client;
        private string date_naissance_client;
        private string photo_client;
        private string date_entree_client;
       

        // Constructeur de la classe client : 
        public Client(string nom, string adresse, string telephone, string date_naissance, string photo, string date_entree)
        {
            this.nom_client = nom;
            this.adresse_client = adresse;
            this.telephone_client = telephone;
            this.date_naissance_client = date_naissance;
            this.photo_client = photo;
            this.date_entree_client = date_entree; 
        }


        // Classe polymorphe (pour l'ajout, modification, supression, recherche) : 
        public Client(int id)
        {
            this.id_client = id; 
        }

        public void ajouter(string type_compte)
        {
            using (MySqlConnection connection = new MySqlConnection("datasource=127.0.0.1;port=3306;user=root;password=;database=gestion_bancaire"))
            {
                connection.Open(); 

                // Insertion de l'information du Client dans la base de donnée :  
                using (MySqlCommand command = new MySqlCommand("INSERT INTO client(nom, telephone, adresse, date_naissance, date_entree, chemin_image) VALUES(" + "\"" + nom_client + "\", \"" + adresse_client + "\", " + "\"" + telephone_client + "\", " + "\"" + date_naissance_client + "\", " + "\"" + date_entree_client + "\"" + ", \"" + photo_client + "\"" + ");", connection))
                {
                    command.ExecuteNonQuery(); 
                }

                // Récuperer l'id_client généré : 
                long dernierIDInseree;
                using (MySqlCommand command = new MySqlCommand("SELECT LAST_INSERT_ID();", connection))
                {
                    dernierIDInseree = Convert.ToInt64(command.ExecuteScalar()); 

                }

                // Création d'un compte bancaire pour le client dont la solde vaut 0 : 
                effectuer_operation("INSERT INTO compte(id_client, montant_compte, type_compte) VALUES(" + dernierIDInseree + ", 0," +  "'" +type_compte + "'" +  ");", "Compte crée. Votre numero bancaire est " + dernierIDInseree + ".", true);
            }
        }

        public void rechercher()
        { 
            // A implémenter...
        }

         
        public void modifier(string nom, string adresse, string telephone, string date_naissance, string photo, string date_entree)
        {
            string requetteModification = "UPDATE client SET nom=" + nom + ", telephone=" + telephone + ", adresse=" + adresse + ", date_naissance=" + date_naissance + ", chemin_image=" + photo + ");";
            effectuer_operation(requetteModification, "La modification de ce compte a été effectué.", true); 
        }

        public void supprimer()
        {
            string requetteSuppression = "DELETE FROM client WHERE id=" + id_client + ";";
            effectuer_operation(requetteSuppression, "Cette compte a été supprimé", true); 
        }
    }
}
