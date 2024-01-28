using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Windows.Forms; 

namespace gestion_bancaire
{
    class Facture
    {
        public void GenererPDF(string cheminFichierPdf, List<string> info)
        {
            Document document = new Document(PageSize.A4);

            using (FileStream fs = new FileStream(cheminFichierPdf, FileMode.Create))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, fs);

                document.Open();

                // Ajout d'un logo dans la facture : 
                string cheminLogo = @"" + Environment.CurrentDirectory + @"\logo_facture.jpg";
                MessageBox.Show("Repertoire courant : " + cheminLogo); 

                // On teste l'existance du fichier : 
                if (File.Exists(cheminLogo))
                {
                    Image image = Image.GetInstance(cheminLogo);
                    document.Add(image); 
                }

                foreach (string i in info)
                {
                    document.Add(new Paragraph(i)); 
                }

                document.Close();
            }
        }
    }
}
