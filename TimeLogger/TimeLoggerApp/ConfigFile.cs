using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TimeLoggerApp
{
    public class ConfigFile
    {
        #region Propriétés
        private static String _configFileName = "Config\\Config.xml";
        public static String ConfigFileName
        {
            get
            {
                return _configFileName;
            }
        }


        private XDocument _xDoc;
        public XDocument XDoc
        {
            get
            {
                return _xDoc;
            }
        }
        #endregion

        #region Singleton
        private static ConfigFile _instance;
        public static ConfigFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigFile();
                }
                return _instance;
            }
            set { _instance = value; }
        }
        #endregion

        #region Constructeur
        private ConfigFile()
        {
            if (!System.IO.File.Exists(ConfigFileName))
            {
                Console.WriteLine(string.Format("Le fichier de configuration '{0}' n'a pas été trouvé. Les valeurs par défaut seront utilisées.", ConfigFileName));
                MessageBox.Show("Le fichier de configuation n'a pas été trouvé, fermeture de l'application");

            }
            else
            {
                _xDoc = XDocument.Load(_configFileName);
            }
        }
        #endregion

        #region Méthodes
        /// <summary>
        /// Get the value of a specific parameter - Default value if a problem occured
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="paramName">the name of the parameter</param>
        /// <param name="section">the section. Server by default. Look at the config.xml file for others</param>
        /// <param name="defaultValue">The value return if something cause exception</param>
        /// <returns>The value in the good type - could be exception. See exception section for details</returns>
        /// <exception cref="InvalidCastException">The cast is impossible</exception>
        /// <exception cref="Exception">Other exception</exception>
        public T GetValue<T>(String paramName, T defaultValue = default(T), String section = "Configuration")
        {
            try
            {
                String query = XDoc.Element(section)
                               .Element(paramName)
                               .Value;

                return ConvertValue<T>(query);
            }
            catch (Exception)
            {
                Console.WriteLine(string.Format("La clé {0} de la section {1} n'a pas été trouvé dans le fichier de configuration. La valeur par défaut {2} a été utilisée.", paramName, section, defaultValue.ToString()));
                return defaultValue;
            }
        }
        /// <summary>
        /// Convert a string in a specific type
        /// </summary>
        /// <typeparam name="T">The type to convert into</typeparam>
        /// <param name="value">The parameter name</param>
        /// <returns>The param converted in a specific type</returns>
        /// <exception cref="InvalidCastException"></exception>
        private static T ConvertValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        /// <summary>
        /// Set the value of a specific parameter - Returns true if all went well, false otherwise
        /// </summary>
        /// <typeparam name="T">The type of the parameter</typeparam>
        /// <param name="paramName">The name of the parameter</param>
        /// <param name="value">The value to set to the given parameter</param>
        /// <param name="section">The section, configuration by default</param>
        /// <returns>A result boolean to know if the operation went well</returns>
        public bool SetValue<T>(String paramName, T value, String section = "Configuration")
        {
            try
            {
                XElement query = XDoc.Element(section).
                                Element(paramName);
                query.Value = (String)Convert.ChangeType(value, typeof(string));
                XDoc.Save(ConfigFileName);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public List<CoupleParamValeurXml> GetFormatConfigDisplay()
        {
            List<CoupleParamValeurXml> ListeParametreView = new List<CoupleParamValeurXml>();

            //lecture des différents éléments XML
            IEnumerable<XElement> query = XDoc.Element("Configuration").Elements();

            //Pour chaque éléments XML
            foreach (XElement XmlItem in query)
            {
                CoupleParamValeurXml coupleParamValeur = new CoupleParamValeurXml();
                coupleParamValeur.Valeur = XmlItem.Value.ToString();
                coupleParamValeur.Nom = XmlItem.Name.ToString();

                ListeParametreView.Add(coupleParamValeur);
            }

            return ListeParametreView;
        }
        #endregion
    }

    /// <summary>
    /// Classe représentant un élément XML dans le fichier de configuration
    /// </summary>
    public class CoupleParamValeurXml
    {
        public string Nom { get; set; }

        public string Valeur { get; set; }
    }
}
