using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
 

namespace gestion_bancaire
{
    class Base_de_donnee
    {
        
        // Pour les requettes INSERT, UPDATE
        public void effectuer_operation(string requetteOperation, string messageApresRequetteSiBon, bool afficherMessage)
        { 
            // Cette chaine de caractère represente la connexion a la base de donnée : 
            string MySQLConnectionString = "datasource=127.0.0.1;port=3306;user=root;password=;database=gestion_bancaire";

            // Objet representant la connexion a la base : 
            MySqlConnection databaseConnection = new MySqlConnection(MySQLConnectionString);

            // Opération que l'on souhaitte effectuer + temps excecution du commande en ms : 
            MySqlCommand commandDatabase = new MySqlCommand(requetteOperation, databaseConnection);
            // commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();

                MySqlDataReader myReader = commandDatabase.ExecuteReader();

                if (!myReader.HasRows)
                {
                    if (afficherMessage)
                    {
                        MessageBox.Show(messageApresRequetteSiBon);
                    }
                    
                }
               
            }
            catch (Exception e)
            {
                MessageBox.Show("Erreur de connexion a la base de donnée :  " + e.Message);
            }

        }

        // Pour les requettes SELECT, en stockant les résultat dans un datagridview
        public void lire_BDD(string requette, DataGridView datagridview)
        {
            string chaineConnexion = "datasource=127.0.0.1;port=3306;user=root;password=;database=gestion_bancaire";
            MySqlConnection connexion = new MySqlConnection(chaineConnexion);

            try
            {
                MySqlCommand commande = new MySqlCommand(requette, connexion);

                connexion.Open();

                MySqlDataAdapter resultat = new MySqlDataAdapter(commande);
                DataTable dataTable = new DataTable();

                resultat.Fill(dataTable);

                datagridview.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur de récupération du liste de tout le client : " + ex.Message);
            }
            finally
            {
                connexion.Close();
            }
        }
    }
}
