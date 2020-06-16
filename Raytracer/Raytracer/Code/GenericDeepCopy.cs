using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Raytracer.Tree
{
    class GenericDeepCopy<T>
    {
         
            public static T GetDeepCopy(object objectToCopy)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(memoryStream, objectToCopy);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return (T)binaryFormatter.Deserialize(memoryStream);
                }
            }
      
    }
}
