using FluentAssertions;
using MediaPlayer.Managers;
using MediaPlayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod]
        public void DeserializeValidFileTest()
        {
            var validJson = "{\"ScreenId\":\"55-444-888\",\"CalledURL\":\"www.fakeUrl.com\",\"SecurityKey\":\"securityKey\"," +
                            "\"DefaultClipURL\":\"www.fakeUrl2.com\",\"CronUpdateTime\":\"5\",\"ImagesDisplayTime\":\"50\"}";
            var settings = new Settings()
            {
                CronUpdateTime = 5,
                ImagesDisplayTime = 50,
                ScreenId = "55-444-888",
                CalledURL = "www.fakeUrl.com",
                SecurityKey = "securityKey",
                DefaultClipURL = "www.fakeUrl2.com",
            };
            var returnedResult = _manager.DeserializeFile(validJson);

            returnedResult.ShouldBeEquivalentTo(settings);
            Assert.AreEqual(returnedResult.AreNonNumericFieldsValid(), true);
            Assert.AreEqual(returnedResult.AreNumericFieldsValid(), true);
        }

        [TestMethod]
        public void DeserializeInvalidValuesFileTest()
        {
            var invalidJson = "{\"ScreenId\":\"55-444-888\",\"CalledURL\":\"\",\"SecurityKey\":\"securityKey\"," +
                              "\"DefaultClipURL\":\"www.fakeUrl2.com\",\"CronUpdateTime\":\"0\",\"ImagesDisplayTime\":\"50\"}";
            var settings = new Settings()
            {
                CronUpdateTime = 0,
                ImagesDisplayTime = 50,
                ScreenId = "55-444-888",
                CalledURL = "",
                SecurityKey = "securityKey",
                DefaultClipURL = "www.fakeUrl2.com",
            };
            var returnedResult = _manager.DeserializeFile(invalidJson);

            returnedResult.ShouldBeEquivalentTo(settings);
            Assert.AreEqual(returnedResult.AreNonNumericFieldsValid(), false);
            Assert.AreEqual(returnedResult.AreNumericFieldsValid(), false);
        }

        [TestMethod]
        public void DeserializeInvalidFileTest()
        {
            var invalidJson = "{reenId\":\"55-444-888\",\"CalledURL\":\"\",\"SecurityKey\":\"securityKey\"," +
                              "\"DefaultClipURL\":\"www.fakeUrl2.com\",\"CronUpdateTime\":\"0\",\"ImagesDisplayTime\":\"50\"}";
           
            var returnedResult = _manager.DeserializeFile(invalidJson);

            returnedResult.ShouldBeEquivalentTo(new Settings());
            Assert.AreEqual(returnedResult.AreNonNumericFieldsValid(), false);
            Assert.AreEqual(returnedResult.AreNumericFieldsValid(), false);
        }





    }
}
