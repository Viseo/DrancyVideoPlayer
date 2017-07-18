using System;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using MediaPlayer.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Tests
{
    [TestClass]
    public class SettingsManagerTests
    {
        private ISettingsManager _manager;

        public SettingsManagerTests()
        {
            _manager = new SettingsManager();
        }

        private Stream MockStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [TestMethod]
        public void GetSettingsFileTest()
        {
            //var validJson = "{\"ScreenId\":\"55-444-888\",\"CalledURL\":www.fakeUrl.com,\"SecurityKey\":securityKey,\"DefaultClipURL\":www.fakeUrl2.com,\"CronUpdateTime\":5,\"ImagesDisplayTime\":50}";
            //var invalidJson = "{\"ScreenId\":\"55-444-888\",\"CalledURL\":www.fakeUrl.com,\"SecurityKey\":securityKey,\"DefaultClipURL\":www.fakeUrl2.com,\"CronUpdateTime\":5,\"ImagesDisplayTime\":50}";
            //var storageFileMock = new Mock<IStorageFile>();
            //var mockedStream = new Mock<IRandomAccessStreamWithContentType>();
            
            //storageFileMock.Object.


            //mockedStream.SetupGet(x => x.AsStream()).Returns(MockStreamFromString(validJson));
            //storageFileMock.SetupGet(file => file.OpenReadAsync()).Returns();



            //storageFileMock.SetupGet(file => file.OpenReadAsync()).Returns();


        }
    }
}
