using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace travelManagement.Models
{
    [System.Serializable]
    public class DataTable : System.ComponentModel.MarshalByValueComponent, System.ComponentModel.IListSource, System.ComponentModel.ISupportInitializeNotification, System.Runtime.Serialization.ISerializable, System.Xml.Serialization.IXmlSerializable
    {
        public bool ContainsListCollection => throw new NotImplementedException();

        public bool IsInitialized => throw new NotImplementedException();

        public event EventHandler Initialized;

        public void BeginInit()
        {
            throw new NotImplementedException();
        }

        public void EndInit()
        {
            throw new NotImplementedException();
        }

        public IList GetList()
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
