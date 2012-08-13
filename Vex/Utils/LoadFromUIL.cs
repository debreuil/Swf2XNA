using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Xml.Serialization;

namespace DDW.Vex
{
    public class LoadFromUIL
    {
        public static VexObject Load(string fileFullPathAndName)
        {
            VexObject vo;
            string rootFolder = Path.GetDirectoryName(fileFullPathAndName);
            string fileNameOnly = Path.GetFileNameWithoutExtension(fileFullPathAndName);
            Environment.CurrentDirectory = rootFolder;

            string definitionsFolder = "";
            string instancesFolder = "";
            uint rootId = 0;

            vo = new VexObject(fileNameOnly);

            FileStream fs = new FileStream(fileFullPathAndName, FileMode.Open);
            XmlTextReader r = new XmlTextReader(fs);

            r.ReadStartElement("UIL");

            while (r.Read())
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "DefinitionsFolder":
                            if (r.Read())
                            {
                                definitionsFolder = r.Value.Trim();
                            }
                            break;
                        case "InstancesFolder":
                            if (r.Read())
                            {
                                instancesFolder = r.Value.Trim();
                            }
                            break;
                        case "RootId":
                            if (r.Read())
                            {
                                rootId = uint.Parse(r.Value.Trim(), NumberStyles.Any);
                            }
                            break;
                    }
                }
            }

            fs.Close();

            string defsPath = rootFolder + Path.DirectorySeparatorChar + fileNameOnly + definitionsFolder;
            LoadDefinitions(defsPath, vo);

            string instsPath = rootFolder + Path.DirectorySeparatorChar + fileNameOnly + instancesFolder;
            LoadInstances(instsPath, vo);

            vo.Root = (Timeline)vo.Definitions[rootId];

            return vo;
        }

        private static void LoadDefinitions(string folderPath, VexObject vo)
        {
            uint definitionIdCounter = 1;
            List<string> dataPaths = new List<string>();
            List<string> headerPaths = new List<string>();
            List<string> types = new List<string>();
            List<string> names = new List<string>();

            string libFileName = folderPath + Path.DirectorySeparatorChar + "Library.xml";
            FileStream fs = new FileStream(libFileName, FileMode.Open);
            XmlTextReader r = new XmlTextReader(fs);
            r.WhitespaceHandling = WhitespaceHandling.None;
            r.ReadStartElement("Library");

            do
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "LibraryIdCounter":
                            if (r.Read())
                            {
                                definitionIdCounter = uint.Parse(r.Value.Trim(), NumberStyles.Any);
                                r.Read();
                            }
                            break;

                        case "LibraryItems":
                            while (r.Read())
                            {
                                if (r.IsStartElement() && r.Name == "Item")
                                {
                                    headerPaths.Add(r.GetAttribute("HeaderPath"));
                                    dataPaths.Add(r.GetAttribute("DataPath"));
                                    types.Add(r.GetAttribute("Type"));
                                    names.Add(r.GetAttribute("Name"));
                                }
                            }
                            break;

                        default:
                            r.Read();
                            break;
                    }
                }
            }
            while (r.Read());

            r.Close();
            fs.Close();

            // read actual library Items
            for (int i = 0; i < dataPaths.Count; i++)
            {
                string dp = folderPath + Path.DirectorySeparatorChar + dataPaths[i];
                string hp = folderPath + Path.DirectorySeparatorChar + headerPaths[i];
                string type = types[i];

                IDefinition def = LoadDefinition(type, hp, dp);
                vo.Definitions.Add(def.Id, def);
                //Name = names[i];
            }
        }
        private static IDefinition LoadDefinition(string type, string headerPath, string dataPath)
        {
            IDefinition result = null;

            FileStream fs = new FileStream(dataPath, FileMode.Open);
            XmlSerializer xs = null;
            switch (type)
            {
                case "DDW.Vex.Timeline":
                    xs = new XmlSerializer(typeof(Vex.Timeline));
                    break;
                case "DDW.Vex.Symbol":
                    xs = GetShapeSerializer();
                    break;
                case "DDW.Vex.Image":
                    xs = new XmlSerializer(typeof(Vex.Image));
                    break;
            }

            if (xs != null)
            {
                result = (Vex.IDefinition)xs.Deserialize(fs);
            }
            fs.Close();


            //load header
            if (result != null)
            {
                fs = new FileStream(headerPath, FileMode.OpenOrCreate);

                XmlTextReader r = new XmlTextReader(fs);
                r.WhitespaceHandling = WhitespaceHandling.None;
                r.ReadStartElement("Definition");

                do
                {
                    if (r.IsStartElement())
                    {
                        switch (r.Name)
                        {
                            default:
                                r.Read();
                                break;
                        }
                    }
                }
                while (r.Read());

                r.Close();
                fs.Close();
            }
            return result;
        }

        private static XmlSerializer GetShapeSerializer()
        {
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();
            XmlElementAttribute attr;

            XmlAttributes fillAttributes = new XmlAttributes();
            attr = new XmlElementAttribute(typeof(DDW.Vex.SolidFill));
            attr.ElementName = "SolidFill";
            fillAttributes.XmlElements.Add(attr);
            attr = new XmlElementAttribute(typeof(DDW.Vex.GradientFill));
            attr.ElementName = "GradientFill";
            fillAttributes.XmlElements.Add(attr);
            attr = new XmlElementAttribute(typeof(DDW.Vex.ImageFill));
            attr.ElementName = "ImageFill";
            fillAttributes.XmlElements.Add(attr);
            attrOverrides.Add(typeof(DDW.Vex.Shape), "Fill", fillAttributes);            

            XmlAttributes strokeAttributes = new XmlAttributes();
            attr = new XmlElementAttribute(typeof(DDW.Vex.SolidStroke));
            attr.ElementName = "SolidStroke";
            strokeAttributes.XmlElements.Add(attr);
            attrOverrides.Add(typeof(DDW.Vex.Shape), "Stroke", strokeAttributes);

            XmlSerializer xs = new XmlSerializer(typeof(Vex.Symbol), attrOverrides);

            return xs;
        }

        private static void LoadInstances(string folderPath, VexObject vo)
        {
            List<string> dataPaths = new List<string>();
            List<string> types = new List<string>();

            uint instanceIdCounter = 1;

            string libFileName = folderPath + Path.DirectorySeparatorChar + "Instances.xml";
            FileStream fs = new FileStream(libFileName, FileMode.Open);
            XmlTextReader r = new XmlTextReader(fs);
            r.WhitespaceHandling = WhitespaceHandling.None;
            r.ReadStartElement("Instances");

            do
            {
                if (r.IsStartElement())
                {
                    switch (r.Name)
                    {
                        case "InstanceIdCounter":
                            if (r.Read())
                            {
                                instanceIdCounter = uint.Parse(r.Value.Trim(), NumberStyles.Any);
                                r.Read();
                            }
                            break;

                        case "InstanceItems":
                            while (r.Read())
                            {
                                if (r.IsStartElement() && r.Name == "Item")
                                {
                                    dataPaths.Add(r.GetAttribute("DataPath"));
                                    types.Add(r.GetAttribute("Type"));
                                }
                            }
                            break;

                        default:
                            r.Read();
                            break;
                    }
                }
            }
            while (r.Read());

            r.Close();
            fs.Close();

            // read actual DesignInstance Items
            for (int i = 0; i < dataPaths.Count; i++)
            {
                string dp = folderPath + Path.DirectorySeparatorChar + dataPaths[i];
                string type = types[i];
                IInstance inst = LoadFromPath(type, dp);

                if(vo.Definitions.ContainsKey(inst.ParentDefinitionId))
                {
                    Timeline parent = (Timeline)vo.Definitions[inst.ParentDefinitionId];
                    parent.AddInstance(inst);
                }
            }

        }

        public static IInstance LoadFromPath(string type, string dataPath)
        {
            IInstance result;

            FileStream fs = new FileStream(dataPath, FileMode.Open);
            XmlSerializer xs = new XmlSerializer(typeof(Vex.Instance));
            result = (Vex.Instance)xs.Deserialize(fs);
            fs.Close();
            
            return result;
        }
    }
}
