using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 
using MySql.Data.MySqlClient;

using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf; 

namespace gestion_bancaire
{
    class Compte : Base_de_donnee
    {
        protected int id_compte;
        protected int id_client;
        private double montant_compte;
        private string type_compte;

        // A recuperer dans la table client a l'aide d'une jointure : 
        private string nom;
        private string date_naissance;
        private string telephone;
        private string adresse;
        private string image; 

        // Constructeur : 
        public Compte(int id_c)
        {
            this.id_client = id_c;

            string chaineConnexion = "datasource=127.0.0.1;port=3306;user=root;password=;database=gestion_bancaire"; 

            MySqlConnection connexionBDD = new MySqlConnection(chaineConnexion);

            MySqlCommand recuperationInfo = new MySqlCommand("SELECT * FROM compte WHERE id_client=" + id_c + ";", connexionBDD);
            
            try
            {
                connexionBDD.Open();

                MySqlDataReader resultat = recuperationInfo.ExecuteReader();

                if (resultat.HasRows)
                {
                    while (resultat.Read())
                    {
                        this.id_compte = Convert.ToInt32(resultat.GetString("id_compte"));
                        this.montant_compte = Convert.ToDouble(resultat.GetString("montant_compte"));
                        this.type_compte = resultat.GetString("type_compte");
                    }
                }
                else
                {
                    MessageBox.Show("Pas de reponse venant de la requette"); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur (dans catch) : " + ex.Message); 
            }
            connexionBDD.Close(); 
            

            // Recupération d'autre informations au table client (jointure) : 
            MySqlCommand autresInformations = new MySqlCommand("SELECT client.id _id, client.nom _nom, client.date_naissance _date_naissance, client.telephone _telephone, client.adresse _adresse, client.chemin_image _chemin_image FROM compte RIGHT JOIN client ON compte.id_compte=client.id WHERE client.id=" + id_c + ";", connexionBDD);
            try
            {
                connexionBDD.Open(); 
                
                MySqlDataReader resultatAutreInformations = autresInformations.ExecuteReader();

                if (resultatAutreInformations.HasRows)
                {
                    while (resultatAutreInformations.Read())
                    {
                        this.nom = resultatAutreInformations.GetString("_nom");
                        this.date_naissance = resultatAutreInformations.GetString("_date_naissance");
                        this.telephone = resultatAutreInformations.GetString("_telephone");
                        this.adresse = resultatAutreInformations.GetString("_adresse");
                        this.image = resultatAutreInformations.GetString("_chemin_image"); 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de récupération (jointure) : " + ex.Message); 
            }
        }

        public List<string> effectuer_une_transaction(string type_transaction, double montant)
        {
            double montantAvantTransaction = montant_compte;
            List<string> informationPourFacture = new List<string>();
 
            // Date de la transaction en formatage type Datetime MySQL : 
            string date_transaction = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;

            if (type_transaction == "retrait")
            {
                if (montant_compte >= montant)
                {
                    montant_compte -= montant;

                    informationPourFacture.Add("**************************************** FACTURE DE " + type_transaction.ToUpper() + " ****************************************");
                    informationPourFacture.Add("Retrait de la somme de : " + montant + " MGA");
                    informationPourFacture.Add("Numero du Compte Bancaire : " + this.id_client);
                    informationPourFacture.Add("Nom : " + this.nom);
                    informationPourFacture.Add("Date de l'Operation : " + DateTime.Now.ToString()); 
                    informationPourFacture.Add("**************************************** FACTURE DE " + type_transaction.ToUpper() + " ****************************************");


                    string requetteRetrait = "UPDATE compte SET montant_compte=" + this.montant_compte + " WHERE id_compte=" + this.id_compte + ";";
                    effectuer_operation(requetteRetrait, "Retrait effectué du montant " + montant + ", montant du compte actuel " + this.montant_compte, true);

                    // Requette pour inserer la transaction : 
                    string requetteTransaction = "INSERT INTO transaction(id_compte, id_client, type_transaction, date_transaction, montant_avant_transaction, montant_apres_transaction) VALUES(" + this.id_compte + ", " + this.id_client + ", '" + "retrait', '" + date_transaction + "', " + montantAvantTransaction + ", " + this.montant_compte + ");";
                    effectuer_operation(requetteTransaction, "Transaction inséré dans la base de donnée", true);
                }
                else
                {
                    MessageBox.Show("Solde insuffisant");
                }
            }
            else
            {
                montant_compte += montant;

                informationPourFacture.Add("************************************* FACTURE DE " + type_transaction.ToUpper() + " *************************************");
                informationPourFacture.Add("Versement de la somme de : " + montant + " MGA");
                informationPourFacture.Add("Numero du Compte Bancaire : " + this.id_client);
                informationPourFacture.Add("Nom : " + this.nom);
                informationPourFacture.Add("Date de l'Operation : " + DateTime.Now.ToString()); 
                informationPourFacture.Add("************************************* FACTURE DE " + type_transaction.ToUpper() + " *************************************");

                string requetteVersement = "UPDATE compte SET montant_compte=" + this.montant_compte + " WHERE id_compte=" + this.id_compte + ";";
                effectuer_operation(requetteVersement, "Versement effectué du montant " + montant + ", montant du compte actuel " + this.montant_compte, true);

           
                // Requette pour inserer la transaction : 
                string requetteTransaction = "INSERT INTO transaction(id_compte, id_client, type_transaction, date_transaction, montant_avant_transaction, montant_apres_transaction) VALUES(" + this.id_compte + ", " + this.id_client + ", '" + "versement', '" + date_transaction + "', " + montantAvantTransaction + ", " + this.montant_compte + ");";
                effectuer_operation(requetteTransaction, "Transaction inséré dans la base de donnée", true);
            }

            return informationPourFacture; 
            
        }

        // Getter qui retourne une liste des informations récupéré dans la table client : 
        public List<string> recuperationInformationSupplementaire()
        { 
            List<string> info = new List<string>();
            string tmp = ""; 

            info.Add(this.nom);

            // Pour éviter une chaine du type jj/mm/aaaa hh:mm:ss, mais avoir jj//mm/aaaa
            tmp = this.date_naissance.Substring(0, 10); 
            info.Add(tmp);

            info.Add(this.telephone); 
            info.Add(this.adresse);
            info.Add(this.montant_compte.ToString());
            info.Add(this.type_compte);
            info.Add(@"" + this.image); 

            return info;
        }

    }
}
